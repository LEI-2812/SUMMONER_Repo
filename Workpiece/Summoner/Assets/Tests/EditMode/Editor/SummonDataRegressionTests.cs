using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Summoner.EditModeTests
{
    public class SummonDataRegressionTests
    {
        private const double ExpectedMultiple = 5;

        private struct ExpectedSummon
        {
            public readonly string TypeName;
            public readonly string Name;
            public readonly string Rank;
            public readonly string SummonType;
            public readonly double MaxHp;
            public readonly double AttackPower;
            public readonly double HeavyAttackPower;
            public readonly ExpectedAttack NormalAttack;
            public readonly ExpectedAttack[] SpecialAttacks;

            public ExpectedSummon(
                string typeName,
                string name,
                string rank,
                string summonType,
                double maxHp,
                double attackPower,
                double heavyAttackPower,
                ExpectedAttack normalAttack,
                ExpectedAttack[] specialAttacks)
            {
                TypeName = typeName;
                Name = name;
                Rank = rank;
                SummonType = summonType;
                MaxHp = maxHp;
                AttackPower = attackPower;
                HeavyAttackPower = heavyAttackPower;
                NormalAttack = normalAttack;
                SpecialAttacks = specialAttacks;
            }
        }

        private struct ExpectedAttack
        {
            public readonly string StrategyTypeName;
            public readonly string StatusType;
            public readonly double Damage;
            public readonly int Cooltime;

            public ExpectedAttack(
                string strategyTypeName,
                string statusType,
                double damage,
                int cooltime)
            {
                StrategyTypeName = strategyTypeName;
                StatusType = statusType;
                Damage = damage;
                Cooltime = cooltime;
            }
        }

        [Test]
        public void PlayerSummons_KeepCurrentValuesBeforeDataMigration()
        {
            FieldInfo multipleField = FieldGet(TypeGet("Summon"), "multiple");
            double originMultiple = (double)multipleField.GetValue(null);
            multipleField.SetValue(null, ExpectedMultiple);

            try
            {
                AssertSummon(SummonExpectedCreate(
                    "Cat", "Low", 1250, 150, 200,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 150, 1),
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 1)));

                AssertSummon(SummonExpectedCreate(
                    "Rabbit", "Medium", 1500, 185, 0,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 185, 1),
                    AttackExpectedCreate("TargetedAttackStrategy", "Heal", 0.3, 3)));

                AssertSummon(SummonExpectedCreate(
                    "Snake", "Medium", 1500, 200, 0,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 40, 1),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "Poison", 0.1, 3)));

                AssertSummon(SummonExpectedCreate(
                    "Wolf", "High", 1750, 250, 150,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 250, 1),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 150, 2)));

                AssertSummon(SummonExpectedCreate(
                    "Eagle", "High", 1750, 225, 150,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 225, 1),
                    AttackExpectedCreate("TargetedAttackStrategy", "None", 150, 2)));

                AssertSummon(SummonExpectedCreate(
                    "Fox", "Low", 1250, 175, 0,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 175, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "Upgrade", 0.3, 3)));
            }
            finally
            {
                multipleField.SetValue(null, originMultiple);
            }
        }

        [Test]
        public void EnemySummons_KeepCurrentValuesBeforeDataMigration()
        {
            AssertSummon(SummonExpectedCreate(
                "Slime", "Normal", 200, 25, 40,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 25, 1),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 50, 2)),
                "Awake");

            AssertSummon(SummonExpectedCreate(
                "Skeleton", "Normal", 650, 150, 170,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 150, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 160, 0)),
                "Awake");

            AssertSummon(SummonExpectedCreate(
                "LowDevil", "Normal", 700, 180, 220,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 180, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "Curse", 0.2, 4),
                AttackExpectedCreate("TargetedAttackStrategy", "OnceInvincibility", 0, 2)),
                "Awake");

            AssertSummon(SummonExpectedCreate(
                "HighDevil", "Special", 1000, 200, 250,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 140, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 230, 0)),
                "Awake");

            AssertSummon(SummonExpectedCreate(
                "KingSlime", "Special", 250, 50, 65,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 50, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 35, 1),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)),
                "Awake");

            AssertSummon(SummonExpectedCreate(
                "WaterSpirit", "Normal", 350, 70, 120,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 70, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)),
                "Awake");

            AssertSummon(SummonExpectedCreate(
                "GrassSpirit", "Normal", 350, 80, 110,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 80, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.1, 3)),
                "Awake");
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

                Invoke(summon, "SummonInitialize");

                Assert.AreEqual("Cat", Invoke(summon, "GetSummonName"));
                Assert.AreEqual("Low", Invoke(summon, "GetSummonRank").ToString());
                Assert.AreEqual("Cat", Invoke(summon, "GetSummonType").ToString());
                Assert.AreEqual(1250, Invoke(summon, "GetMaxHP"));
                Assert.AreEqual(150, Invoke(summon, "GetAttackPower"));
                Assert.AreEqual(200, Invoke(summon, "GetHeavyAttackPower"));
                AssertAttackStrategy(
                    Invoke(summon, "GetAttackStrategy"),
                    "ClosestEnemyAttackStrategy",
                    "None",
                    150,
                    1);

                Array specialAttacks = (Array)Invoke(summon, "GetSpecialAttackStrategy");
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
        public void EnemySummons_UseAssignedSummonData()
        {
            AssertAssignedEnemySummonData(
                "Slime",
                SlimeCurrentDataCreate(),
                SummonExpectedCreate(
                    "Slime", "Normal", 200, 25, 40,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 25, 1),
                    AttackExpectedCreate("TargetedAttackStrategy", "Shield", 50, 2)));

            AssertAssignedEnemySummonData(
                "Skeleton",
                SkeletonCurrentDataCreate(),
                SummonExpectedCreate(
                    "Skeleton", "Normal", 650, 150, 170,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 150, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "None", 160, 0)));

            AssertAssignedEnemySummonData(
                "LowDevil",
                LowDevilCurrentDataCreate(),
                SummonExpectedCreate(
                    "LowDevil", "Normal", 700, 180, 220,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 180, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "Curse", 0.2, 4),
                    AttackExpectedCreate("TargetedAttackStrategy", "OnceInvincibility", 0, 2)));

            AssertAssignedEnemySummonData(
                "HighDevil",
                HighDevilCurrentDataCreate(),
                SummonExpectedCreate(
                    "HighDevil", "Special", 1000, 200, 250,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 140, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "None", 230, 0)));

            AssertAssignedEnemySummonData(
                "KingSlime",
                KingSlimeCurrentDataCreate(),
                SummonExpectedCreate(
                    "KingSlime", "Special", 250, 50, 65,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 50, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 35, 1),
                    AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)));

            AssertAssignedEnemySummonData(
                "WaterSpirit",
                WaterSpiritCurrentDataCreate(),
                SummonExpectedCreate(
                    "WaterSpirit", "Normal", 350, 70, 120,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 70, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)));

            AssertAssignedEnemySummonData(
                "GrassSpirit",
                GrassSpiritCurrentDataCreate(),
                SummonExpectedCreate(
                    "GrassSpirit", "Normal", 350, 80, 110,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 80, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.1, 3)));
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

        [Test]
        public void EnemySummonPrefabs_ReferenceAssignedSummonDataAssets()
        {
            AssertSummonDataReference("Slime", "Slime", "61b0a8e5a6404b91a89f1f5f7e9a1001");
            AssertSummonDataReference("Skeleton", "Skeleton", "72c1b9f6b7514ca2b90f2a608fab2112");
            AssertSummonDataReference("LowDevil", "LowDevil", "83d2caf7c8624db3a10f3b719abc3223");
            AssertSummonDataReference("HighDevil", "HighDevil", "94e3db08d9734ec4b21f4c82abcd4334");
            AssertSummonDataReference("KingSlime", "KingSlime", "a5f4ec19ea844fd5b32f5d93abcd5445");
        }

        [Test]
        public void EnemySummonDataAssets_KeepCurrentValues()
        {
            AssertSummonDataAsset(SummonExpectedCreate(
                "Slime", "Normal", 200, 25, 40,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 25, 1),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 50, 2)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "Skeleton", "Normal", 650, 150, 170,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 150, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 160, 0)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "LowDevil", "Normal", 700, 180, 220,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 180, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "Curse", 0.2, 4),
                AttackExpectedCreate("TargetedAttackStrategy", "OnceInvincibility", 0, 2)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "HighDevil", "Special", 1000, 200, 250,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 140, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 230, 0)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "KingSlime", "Special", 250, 50, 65,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 50, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 35, 1),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "WaterSpirit", "Normal", 350, 70, 120,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 70, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "GrassSpirit", "Normal", 350, 80, 110,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 80, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.1, 3)));
        }

        private ExpectedSummon SummonExpectedCreate(
            string typeName,
            string rank,
            double maxHp,
            double attackPower,
            double heavyAttackPower,
            ExpectedAttack normalAttack,
            params ExpectedAttack[] specialAttacks)
        {
            return new ExpectedSummon(
                typeName,
                typeName,
                rank,
                typeName,
                maxHp,
                attackPower,
                heavyAttackPower,
                normalAttack,
                specialAttacks);
        }

        private ExpectedAttack AttackExpectedCreate(
            string strategyTypeName,
            string statusType,
            double damage,
            int cooltime)
        {
            return new ExpectedAttack(
                strategyTypeName,
                statusType,
                damage,
                cooltime);
        }

        private void AssertSummon(
            ExpectedSummon expected,
            string initializeMethodName = "SummonInitialize")
        {
            if (expected.SpecialAttacks.Length == 1)
            {
                AssertSummon(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.SummonType,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.Cooltime,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].Cooltime,
                    initializeMethodName);
                return;
            }

            if (expected.SpecialAttacks.Length == 2)
            {
                AssertSummonWithTwoSpecialAttacks(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.SummonType,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.Cooltime,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].Cooltime,
                    expected.SpecialAttacks[1].StrategyTypeName,
                    expected.SpecialAttacks[1].StatusType,
                    expected.SpecialAttacks[1].Damage,
                    expected.SpecialAttacks[1].Cooltime,
                    initializeMethodName);
                return;
            }

            Assert.Fail(expected.TypeName + " expected special attack count is not supported.");
        }

        private void AssertAssignedEnemySummonData(
            string summonTypeName,
            ScriptableObject data,
            ExpectedSummon expected)
        {
            if (expected.SpecialAttacks.Length == 1)
            {
                AssertAssignedEnemySummonData(
                    summonTypeName,
                    data,
                    expected.Name,
                    expected.Rank,
                    expected.SummonType,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.Cooltime,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].Cooltime);
                return;
            }

            if (expected.SpecialAttacks.Length == 2)
            {
                AssertAssignedEnemySummonDataWithTwoSpecialAttacks(
                    summonTypeName,
                    data,
                    expected.Name,
                    expected.Rank,
                    expected.SummonType,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.Cooltime,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].Cooltime,
                    expected.SpecialAttacks[1].StrategyTypeName,
                    expected.SpecialAttacks[1].StatusType,
                    expected.SpecialAttacks[1].Damage,
                    expected.SpecialAttacks[1].Cooltime);
                return;
            }

            Assert.Fail(expected.TypeName + " expected special attack count is not supported.");
        }

        private void AssertSummonDataAsset(ExpectedSummon expected)
        {
            if (expected.SpecialAttacks.Length == 1)
            {
                AssertSummonDataAsset(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.SummonType,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.Cooltime,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].Cooltime);
                return;
            }

            if (expected.SpecialAttacks.Length == 2)
            {
                AssertSummonDataAssetWithTwoSpecialAttacks(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.SummonType,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.Cooltime,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].Cooltime,
                    expected.SpecialAttacks[1].StrategyTypeName,
                    expected.SpecialAttacks[1].StatusType,
                    expected.SpecialAttacks[1].Damage,
                    expected.SpecialAttacks[1].Cooltime);
                return;
            }

            Assert.Fail(expected.TypeName + " expected special attack count is not supported.");
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
            int expectedSpecialAttackCooltime,
            string initializeMethodName = "SummonInitialize")
        {
            GameObject testObject = new GameObject(summonTypeName);

            try
            {
                Component summon = testObject.AddComponent(TypeGet(summonTypeName));
                Invoke(summon, initializeMethodName);

                Assert.AreEqual(expectedName, Invoke(summon, "GetSummonName"));
                Assert.AreEqual(expectedRank, Invoke(summon, "GetSummonRank").ToString());
                Assert.AreEqual(expectedSummonType, Invoke(summon, "GetSummonType").ToString());
                Assert.AreEqual(expectedMaxHp, Invoke(summon, "GetMaxHP"));
                Assert.AreEqual(expectedAttackPower, Invoke(summon, "GetAttackPower"));
                Assert.AreEqual(expectedHeavyAttackPower, Invoke(summon, "GetHeavyAttackPower"));

                AssertAttackStrategy(
                    Invoke(summon, "GetAttackStrategy"),
                    expectedNormalAttackTypeName,
                    expectedNormalAttackStatusType,
                    expectedNormalAttackDamage,
                    expectedNormalAttackCooltime);

                Array specialAttacks = (Array)Invoke(summon, "GetSpecialAttackStrategy");
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

        private void AssertSummonWithTwoSpecialAttacks(
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
            string expectedFirstSpecialAttackTypeName,
            string expectedFirstSpecialAttackStatusType,
            double expectedFirstSpecialAttackDamage,
            int expectedFirstSpecialAttackCooltime,
            string expectedSecondSpecialAttackTypeName,
            string expectedSecondSpecialAttackStatusType,
            double expectedSecondSpecialAttackDamage,
            int expectedSecondSpecialAttackCooltime,
            string initializeMethodName)
        {
            GameObject testObject = new GameObject(summonTypeName);

            try
            {
                Component summon = testObject.AddComponent(TypeGet(summonTypeName));
                Invoke(summon, initializeMethodName);

                AssertSummonValuesWithTwoSpecialAttacks(
                    summon,
                    expectedName,
                    expectedRank,
                    expectedSummonType,
                    expectedMaxHp,
                    expectedAttackPower,
                    expectedHeavyAttackPower,
                    expectedNormalAttackTypeName,
                    expectedNormalAttackStatusType,
                    expectedNormalAttackDamage,
                    expectedNormalAttackCooltime,
                    expectedFirstSpecialAttackTypeName,
                    expectedFirstSpecialAttackStatusType,
                    expectedFirstSpecialAttackDamage,
                    expectedFirstSpecialAttackCooltime,
                    expectedSecondSpecialAttackTypeName,
                    expectedSecondSpecialAttackStatusType,
                    expectedSecondSpecialAttackDamage,
                    expectedSecondSpecialAttackCooltime);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
        }

        private void AssertAssignedEnemySummonData(
            string summonTypeName,
            ScriptableObject data,
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
            testObject.SetActive(false);

            try
            {
                Component summon = testObject.AddComponent(TypeGet(summonTypeName));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "Awake");

                AssertSummonValues(
                    summon,
                    expectedName,
                    expectedRank,
                    expectedSummonType,
                    expectedMaxHp,
                    expectedAttackPower,
                    expectedHeavyAttackPower,
                    expectedNormalAttackTypeName,
                    expectedNormalAttackStatusType,
                    expectedNormalAttackDamage,
                    expectedNormalAttackCooltime,
                    expectedSpecialAttackTypeName,
                    expectedSpecialAttackStatusType,
                    expectedSpecialAttackDamage,
                    expectedSpecialAttackCooltime);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
                UnityEngine.Object.DestroyImmediate(data);
            }
        }

        private void AssertAssignedEnemySummonDataWithTwoSpecialAttacks(
            string summonTypeName,
            ScriptableObject data,
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
            string expectedFirstSpecialAttackTypeName,
            string expectedFirstSpecialAttackStatusType,
            double expectedFirstSpecialAttackDamage,
            int expectedFirstSpecialAttackCooltime,
            string expectedSecondSpecialAttackTypeName,
            string expectedSecondSpecialAttackStatusType,
            double expectedSecondSpecialAttackDamage,
            int expectedSecondSpecialAttackCooltime)
        {
            GameObject testObject = new GameObject(summonTypeName);
            testObject.SetActive(false);

            try
            {
                Component summon = testObject.AddComponent(TypeGet(summonTypeName));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "Awake");

                AssertSummonValuesWithTwoSpecialAttacks(
                    summon,
                    expectedName,
                    expectedRank,
                    expectedSummonType,
                    expectedMaxHp,
                    expectedAttackPower,
                    expectedHeavyAttackPower,
                    expectedNormalAttackTypeName,
                    expectedNormalAttackStatusType,
                    expectedNormalAttackDamage,
                    expectedNormalAttackCooltime,
                    expectedFirstSpecialAttackTypeName,
                    expectedFirstSpecialAttackStatusType,
                    expectedFirstSpecialAttackDamage,
                    expectedFirstSpecialAttackCooltime,
                    expectedSecondSpecialAttackTypeName,
                    expectedSecondSpecialAttackStatusType,
                    expectedSecondSpecialAttackDamage,
                    expectedSecondSpecialAttackCooltime);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
                UnityEngine.Object.DestroyImmediate(data);
            }
        }

        private void AssertSummonValues(
            Component summon,
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
            Assert.AreEqual(expectedName, Invoke(summon, "GetSummonName"));
            Assert.AreEqual(expectedRank, Invoke(summon, "GetSummonRank").ToString());
            Assert.AreEqual(expectedSummonType, Invoke(summon, "GetSummonType").ToString());
            Assert.AreEqual(expectedMaxHp, Invoke(summon, "GetMaxHP"));
            Assert.AreEqual(expectedAttackPower, Invoke(summon, "GetAttackPower"));
            Assert.AreEqual(expectedHeavyAttackPower, Invoke(summon, "GetHeavyAttackPower"));

            AssertAttackStrategy(
                Invoke(summon, "GetAttackStrategy"),
                expectedNormalAttackTypeName,
                expectedNormalAttackStatusType,
                expectedNormalAttackDamage,
                expectedNormalAttackCooltime);

            Array specialAttacks = (Array)Invoke(summon, "GetSpecialAttackStrategy");
            Assert.NotNull(specialAttacks);
            Assert.AreEqual(1, specialAttacks.Length);
            AssertAttackStrategy(
                specialAttacks.GetValue(0),
                expectedSpecialAttackTypeName,
                expectedSpecialAttackStatusType,
                expectedSpecialAttackDamage,
                expectedSpecialAttackCooltime);
        }

        private void AssertSummonValuesWithTwoSpecialAttacks(
            Component summon,
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
            string expectedFirstSpecialAttackTypeName,
            string expectedFirstSpecialAttackStatusType,
            double expectedFirstSpecialAttackDamage,
            int expectedFirstSpecialAttackCooltime,
            string expectedSecondSpecialAttackTypeName,
            string expectedSecondSpecialAttackStatusType,
            double expectedSecondSpecialAttackDamage,
            int expectedSecondSpecialAttackCooltime)
        {
            Assert.AreEqual(expectedName, Invoke(summon, "GetSummonName"));
            Assert.AreEqual(expectedRank, Invoke(summon, "GetSummonRank").ToString());
            Assert.AreEqual(expectedSummonType, Invoke(summon, "GetSummonType").ToString());
            Assert.AreEqual(expectedMaxHp, Invoke(summon, "GetMaxHP"));
            Assert.AreEqual(expectedAttackPower, Invoke(summon, "GetAttackPower"));
            Assert.AreEqual(expectedHeavyAttackPower, Invoke(summon, "GetHeavyAttackPower"));

            AssertAttackStrategy(
                Invoke(summon, "GetAttackStrategy"),
                expectedNormalAttackTypeName,
                expectedNormalAttackStatusType,
                expectedNormalAttackDamage,
                expectedNormalAttackCooltime);

            Array specialAttacks = (Array)Invoke(summon, "GetSpecialAttackStrategy");
            Assert.NotNull(specialAttacks);
            Assert.AreEqual(2, specialAttacks.Length);
            AssertAttackStrategy(
                specialAttacks.GetValue(0),
                expectedFirstSpecialAttackTypeName,
                expectedFirstSpecialAttackStatusType,
                expectedFirstSpecialAttackDamage,
                expectedFirstSpecialAttackCooltime);
            AssertAttackStrategy(
                specialAttacks.GetValue(1),
                expectedSecondSpecialAttackTypeName,
                expectedSecondSpecialAttackStatusType,
                expectedSecondSpecialAttackDamage,
                expectedSecondSpecialAttackCooltime);
        }

        private void AssertSummonDataAsset(
            string summonName,
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
            UnityEngine.Object data = AssetDatabase.LoadAssetAtPath(
                $"Assets/Script/Summons/Data/{summonName}SummonData.asset",
                TypeGet("SummonData"));

            Assert.NotNull(data);
            Assert.AreEqual(expectedName, Invoke(data, "SummonNameGet"));
            Assert.AreEqual(expectedRank, Invoke(data, "SummonRankGet").ToString());
            Assert.AreEqual(expectedSummonType, Invoke(data, "SummonTypeGet").ToString());
            Assert.AreEqual(expectedMaxHp, Invoke(data, "MaxHpGet"));
            Assert.AreEqual(expectedAttackPower, Invoke(data, "AttackPowerGet"));
            Assert.AreEqual(expectedHeavyAttackPower, Invoke(data, "HeavyAttackPowerGet"));

            AssertAttackStrategy(
                Invoke(data, "NormalAttackStrategyCreate"),
                expectedNormalAttackTypeName,
                expectedNormalAttackStatusType,
                expectedNormalAttackDamage,
                expectedNormalAttackCooltime);

            Array specialAttacks = (Array)Invoke(data, "SpecialAttackStrategiesCreate");
            Assert.NotNull(specialAttacks);
            Assert.AreEqual(1, specialAttacks.Length);
            AssertAttackStrategy(
                specialAttacks.GetValue(0),
                expectedSpecialAttackTypeName,
                expectedSpecialAttackStatusType,
                expectedSpecialAttackDamage,
                expectedSpecialAttackCooltime);
        }

        private void AssertSummonDataAssetWithTwoSpecialAttacks(
            string summonName,
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
            string expectedFirstSpecialAttackTypeName,
            string expectedFirstSpecialAttackStatusType,
            double expectedFirstSpecialAttackDamage,
            int expectedFirstSpecialAttackCooltime,
            string expectedSecondSpecialAttackTypeName,
            string expectedSecondSpecialAttackStatusType,
            double expectedSecondSpecialAttackDamage,
            int expectedSecondSpecialAttackCooltime)
        {
            UnityEngine.Object data = AssetDatabase.LoadAssetAtPath(
                $"Assets/Script/Summons/Data/{summonName}SummonData.asset",
                TypeGet("SummonData"));

            Assert.NotNull(data);
            Assert.AreEqual(expectedName, Invoke(data, "SummonNameGet"));
            Assert.AreEqual(expectedRank, Invoke(data, "SummonRankGet").ToString());
            Assert.AreEqual(expectedSummonType, Invoke(data, "SummonTypeGet").ToString());
            Assert.AreEqual(expectedMaxHp, Invoke(data, "MaxHpGet"));
            Assert.AreEqual(expectedAttackPower, Invoke(data, "AttackPowerGet"));
            Assert.AreEqual(expectedHeavyAttackPower, Invoke(data, "HeavyAttackPowerGet"));

            AssertAttackStrategy(
                Invoke(data, "NormalAttackStrategyCreate"),
                expectedNormalAttackTypeName,
                expectedNormalAttackStatusType,
                expectedNormalAttackDamage,
                expectedNormalAttackCooltime);

            Array specialAttacks = (Array)Invoke(data, "SpecialAttackStrategiesCreate");
            Assert.NotNull(specialAttacks);
            Assert.AreEqual(2, specialAttacks.Length);
            AssertAttackStrategy(
                specialAttacks.GetValue(0),
                expectedFirstSpecialAttackTypeName,
                expectedFirstSpecialAttackStatusType,
                expectedFirstSpecialAttackDamage,
                expectedFirstSpecialAttackCooltime);
            AssertAttackStrategy(
                specialAttacks.GetValue(1),
                expectedSecondSpecialAttackTypeName,
                expectedSecondSpecialAttackStatusType,
                expectedSecondSpecialAttackDamage,
                expectedSecondSpecialAttackCooltime);
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
            Assert.AreEqual(expectedStatusType, Invoke(attackStrategy, "GetStatusType").ToString());
            Assert.AreEqual(expectedDamage, Invoke(attackStrategy, "GetSpecialDamage"));
            Assert.AreEqual(expectedCooltime, Invoke(attackStrategy, "GetCooltime"));
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

        private ScriptableObject SlimeCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "Slime");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "summonType", EnumValue("SummonType", "Slime"));
            FieldSet(data, "maxHp", 200d);
            FieldSet(data, "attackPower", 25d);
            FieldSet(data, "heavyAttackPower", 40d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 25, 1));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Shield", 50, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject SkeletonCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "Skeleton");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "summonType", EnumValue("SummonType", "Skeleton"));
            FieldSet(data, "maxHp", 650d);
            FieldSet(data, "attackPower", 150d);
            FieldSet(data, "heavyAttackPower", 170d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 150, 0));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "None", 160, 0), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject LowDevilCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "LowDevil");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "summonType", EnumValue("SummonType", "LowDevil"));
            FieldSet(data, "maxHp", 700d);
            FieldSet(data, "attackPower", 180d);
            FieldSet(data, "heavyAttackPower", 220d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 180, 0));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 2);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Curse", 0.2, 4, 1), 0);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "OnceInvincibility", 0, 2), 1);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject HighDevilCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "HighDevil");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Special"));
            FieldSet(data, "summonType", EnumValue("SummonType", "HighDevil"));
            FieldSet(data, "maxHp", 1000d);
            FieldSet(data, "attackPower", 200d);
            FieldSet(data, "heavyAttackPower", 250d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 200, 0));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 2);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 140, 0), 0);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "None", 230, 0), 1);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject KingSlimeCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "KingSlime");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Special"));
            FieldSet(data, "summonType", EnumValue("SummonType", "KingSlime"));
            FieldSet(data, "maxHp", 250d);
            FieldSet(data, "attackPower", 50d);
            FieldSet(data, "heavyAttackPower", 65d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 50, 0));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 2);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 35, 1), 0);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Shield", 80, 2), 1);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject WaterSpiritCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "WaterSpirit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "summonType", EnumValue("SummonType", "WaterSpirit"));
            FieldSet(data, "maxHp", 350d);
            FieldSet(data, "attackPower", 70d);
            FieldSet(data, "heavyAttackPower", 120d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 70, 0));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Shield", 80, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject GrassSpiritCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(TypeGet("SummonData"));
            FieldSet(data, "summonName", "GrassSpirit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "summonType", EnumValue("SummonType", "GrassSpirit"));
            FieldSet(data, "maxHp", 350d);
            FieldSet(data, "attackPower", 80d);
            FieldSet(data, "heavyAttackPower", 110d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 80, 0));

            Array specialAttacks = Array.CreateInstance(TypeGet("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "Heal", 0.1, 3), 0);
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
            return target.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Invoke(target, null);
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
