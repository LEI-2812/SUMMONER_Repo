using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Summoner.EditModeTests
{
    public class SummonDataRegressionTests
    {
        private const double ExpectedMultiple = 5;

        [Test]
        public void PlayerSummons_KeepCurrentValuesBeforeDataMigration()
        {
            FieldInfo multipleField = FieldGet(TypeGet("Summon"), "multiple");
            double originMultiple = (double)multipleField.GetValue(null);
            multipleField.SetValue(null, ExpectedMultiple);

            try
            {
                AssertSummon(
                    "Cat", "Cat", "Low", "Cat", 1250, 150, 200,
                    "ClosestEnemyAttackStrategy", "None", 150, 1,
                    "ClosestEnemyAttackStrategy", "None", 200, 1);

                AssertSummon(
                    "Rabbit", "Rabbit", "Medium", "Rabbit", 1500, 185, 0,
                    "ClosestEnemyAttackStrategy", "None", 185, 1,
                    "TargetedAttackStrategy", "Heal", 0.3, 3);

                AssertSummon(
                    "Snake", "Snake", "Medium", "Snake", 1500, 200, 0,
                    "ClosestEnemyAttackStrategy", "None", 40, 1,
                    "AttackAllEnemiesStrategy", "Poison", 0.1, 3);

                AssertSummon(
                    "Wolf", "Wolf", "High", "Wolf", 1750, 250, 150,
                    "ClosestEnemyAttackStrategy", "None", 250, 1,
                    "AttackAllEnemiesStrategy", "None", 150, 2);

                AssertSummon(
                    "Eagle", "Eagle", "High", "Eagle", 1750, 225, 150,
                    "ClosestEnemyAttackStrategy", "None", 225, 1,
                    "TargetedAttackStrategy", "None", 150, 2);

                AssertSummon(
                    "Fox", "Fox", "Low", "Fox", 1250, 175, 0,
                    "ClosestEnemyAttackStrategy", "None", 175, 0,
                    "TargetedAttackStrategy", "Upgrade", 0.3, 3);
            }
            finally
            {
                multipleField.SetValue(null, originMultiple);
            }
        }

        [Test]
        public void SummonAttackData_CreatesCurrentAttackStrategies()
        {
            AssertAttackDataCreatesStrategy(
                AttackDataCreate("ClosestEnemy", "None", 150, 1),
                "ClosestEnemyAttackStrategy", "None", 150, 1);

            AssertAttackDataCreatesStrategy(
                AttackDataCreate("Targeted", "Heal", 0.3, 3),
                "TargetedAttackStrategy", "Heal", 0.3, 3);

            AssertAttackDataCreatesStrategy(
                AttackDataCreate("AllEnemies", "Poison", 0.1, 3, 2),
                "AttackAllEnemiesStrategy", "Poison", 0.1, 3);
        }

        [Test]
        public void Cat_UsesAssignedSummonData()
        {
            GameObject testObject = new GameObject("Cat");
            ScriptableObject data = CatCurrentDataCreate();

            try
            {
                Component summon = testObject.AddComponent(TypeGet("Cat"));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "summonInitialize");

                Assert.AreEqual("Cat", Invoke(summon, "getSummonName"));
                Assert.AreEqual("Low", Invoke(summon, "getSummonRank").ToString());
                Assert.AreEqual("Cat", Invoke(summon, "getSummonType").ToString());
                Assert.AreEqual(1250, Invoke(summon, "getMaxHP"));
                Assert.AreEqual(150, Invoke(summon, "getAttackPower"));
                Assert.AreEqual(200, Invoke(summon, "getHeavyAttackPower"));
                AssertAttackStrategy(
                    Invoke(summon, "getAttackStrategy"),
                    "ClosestEnemyAttackStrategy",
                    "None",
                    150,
                    1);

                Array specialAttacks = (Array)Invoke(summon, "getSpecialAttackStrategy");
                Assert.NotNull(specialAttacks);
                Assert.AreEqual(1, specialAttacks.Length);
                AssertAttackStrategy(
                    specialAttacks.GetValue(0),
                    "ClosestEnemyAttackStrategy",
                    "None",
                    200,
                    1);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
                UnityEngine.Object.DestroyImmediate(data);
            }
        }

        [Test]
        public void PlayerSummonPrefabs_ReferenceAssignedSummonDataAssets()
        {
            AssertSummonDataReference("Cat", "Cat", "a4d05cde29384af28a8a5b5e4eb02f4c");
            AssertSummonDataReference("Rabbit", "Rabbit", "b14c9d31b71f4ab0a8e6b3d2c5f90111");
            AssertSummonDataReference("Snake", "Snake", "c25d0e42c82f4bc1b9f7c4e3d6a01222");
            AssertSummonDataReference("Wolf", "Wolf", "d36e1f53d93f4cd2caf8d5f4e7b02333");
            AssertSummonDataReference("Eagle", "Eagle", "e47f2064ea4f4de3db09e605f8c03444");
            AssertSummonDataReference("Fox", "Fox", "f5803175fb5f4ef4ec1af71609d04555");
        }

        private void AssertSummon(
            string summonTypeName,
            string expectedName,
            string expectedRank,
            string expectedSummonType,
            double expectedMaxHp,
            double expectedAttackPower,
            double expectedHeavyAttackPower,
            string expectedNormalAttackTypeName,
            string expectedNormalAttackStatusType,
            double expectedNormalAttackDamage,
            int expectedNormalAttackCooltime,
            string expectedSpecialAttackTypeName,
            string expectedSpecialAttackStatusType,
            double expectedSpecialAttackDamage,
            int expectedSpecialAttackCooltime)
        {
            GameObject testObject = new GameObject(summonTypeName);

            try
            {
                Component summon = testObject.AddComponent(TypeGet(summonTypeName));
                Invoke(summon, "summonInitialize");

                Assert.AreEqual(expectedName, Invoke(summon, "getSummonName"));
                Assert.AreEqual(expectedRank, Invoke(summon, "getSummonRank").ToString());
                Assert.AreEqual(expectedSummonType, Invoke(summon, "getSummonType").ToString());
                Assert.AreEqual(expectedMaxHp, Invoke(summon, "getMaxHP"));
                Assert.AreEqual(expectedAttackPower, Invoke(summon, "getAttackPower"));
                Assert.AreEqual(expectedHeavyAttackPower, Invoke(summon, "getHeavyAttackPower"));

                AssertAttackStrategy(
                    Invoke(summon, "getAttackStrategy"),
                    expectedNormalAttackTypeName,
                    expectedNormalAttackStatusType,
                    expectedNormalAttackDamage,
                    expectedNormalAttackCooltime);

                Array specialAttacks = (Array)Invoke(summon, "getSpecialAttackStrategy");
                Assert.NotNull(specialAttacks);
                Assert.AreEqual(1, specialAttacks.Length);
                AssertAttackStrategy(
                    specialAttacks.GetValue(0),
                    expectedSpecialAttackTypeName,
                    expectedSpecialAttackStatusType,
                    expectedSpecialAttackDamage,
                    expectedSpecialAttackCooltime);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
        }

        private void AssertAttackStrategy(
            object attackStrategy,
            string expectedTypeName,
            string expectedStatusType,
            double expectedDamage,
            int expectedCooltime)
        {
            Assert.NotNull(attackStrategy);
            Assert.IsInstanceOf(TypeGet(expectedTypeName), attackStrategy);
            Assert.AreEqual(expectedStatusType, Invoke(attackStrategy, "getStatusType").ToString());
            Assert.AreEqual(expectedDamage, Invoke(attackStrategy, "getSpecialDamage"));
            Assert.AreEqual(expectedCooltime, Invoke(attackStrategy, "getCooltime"));
        }

        private void AssertAttackDataCreatesStrategy(
            object attackData,
            string expectedTypeName,
            string expectedStatusType,
            double expectedDamage,
            int expectedCooltime)
        {
            AssertAttackStrategy(
                Invoke(attackData, "AttackStrategyCreate"),
                expectedTypeName,
                expectedStatusType,
                expectedDamage,
                expectedCooltime);
        }

        private ScriptableObject CatCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "Cat");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Low"));
            FieldSet(data, "summonType", EnumValue("SummonType", "Cat"));
            FieldSet(data, "maxHp", 1250d);
            FieldSet(data, "attackPower", 150d);
            FieldSet(data, "heavyAttackPower", 200d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 150, 1));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("ClosestEnemy", "None", 200, 1), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private void AssertSummonDataReference(string summonName, string prefabName, string guid)
        {
            string assetText = File.ReadAllText($"Assets/Script/Summons/Data/{summonName}SummonData.asset");
            string metaText = File.ReadAllText($"Assets/Script/Summons/Data/{summonName}SummonData.asset.meta");
            string prefabText = File.ReadAllText($"Assets/Prefabs/SummonPrefab/{prefabName}.prefab");

            StringAssert.Contains($"m_Name: {summonName}SummonData", assetText);
            StringAssert.Contains($"guid: {guid}", metaText);
            StringAssert.Contains(
                $"summonData: {{fileID: 11400000, guid: {guid}, type: 2}}",
                prefabText);
        }

        private object AttackDataCreate(
            string strategyType,
            string statusType,
            double damage,
            int cooltime,
            int statusTime = 0)
        {
            return Activator.CreateInstance(
                TypeGet("SummonAttackData"),
                EnumValue("SummonAttackStrategyType", strategyType),
                EnumValue("StatusType", statusType),
                damage,
                cooltime,
                statusTime);
        }

        private object EnumValue(string typeName, string valueName)
        {
            return Enum.Parse(TypeGet(typeName), valueName);
        }

        private object Invoke(object target, string methodName)
        {
            return target.GetType().GetMethod(methodName).Invoke(target, null);
        }

        private void FieldSet(object target, string fieldName, object value)
        {
            FieldGet(target.GetType(), fieldName).SetValue(target, value);
        }

        private FieldInfo FieldGet(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            Assert.Fail(fieldName + " field was not found.");
            return null;
        }

        private Type TypeGet(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail(typeName + " type was not found.");
            return null;
        }
    }
}
