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
            public readonly double MaxHp;
            public readonly double AttackPower;
            public readonly double HeavyAttackPower;
            public readonly ExpectedAttack NormalAttack;
            public readonly ExpectedAttack[] SpecialAttacks;

            public ExpectedSummon(
                string typeName,
                string name,
                string rank,
                double maxHp,
                double attackPower,
                double heavyAttackPower,
                ExpectedAttack normalAttack,
                ExpectedAttack[] specialAttacks)
            {
                TypeName = typeName;
                Name = name;
                Rank = rank;
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
            public readonly int CooldownDuration;

            public ExpectedAttack(
                string strategyTypeName,
                string statusType,
                double damage,
                int cooldownDuration)
            {
                StrategyTypeName = strategyTypeName;
                StatusType = statusType;
                Damage = damage;
                CooldownDuration = cooldownDuration;
            }
        }

        [Test]
        public void PlayerSummons_KeepCurrentValuesBeforeDataMigration()
        {
            FieldInfo multipleField = GetFieldInfo(GetTypeByName("Summon"), "multiple");
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
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 1),
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
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 50, 2)));

            AssertSummon(SummonExpectedCreate(
                "Skeleton", "Normal", 650, 150, 170,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 150, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 160, 0)));

            AssertSummon(SummonExpectedCreate(
                "LowDevil", "Normal", 700, 180, 220,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 180, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "Curse", 0.2, 4),
                AttackExpectedCreate("TargetedAttackStrategy", "OnceInvincibility", 0, 2)));

            AssertSummon(SummonExpectedCreate(
                "HighDevil", "Special", 1000, 200, 250,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 140, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 230, 0)));

            AssertSummon(SummonExpectedCreate(
                "KingSlime", "Special", 250, 50, 65,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 50, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 35, 1),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)));

            AssertSummon(SummonExpectedCreate(
                "WaterSpirit", "Normal", 350, 70, 120,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 70, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "Shield", 80, 2)));

            AssertSummon(SummonExpectedCreate(
                "GrassSpirit", "Normal", 350, 80, 110,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 80, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.1, 3)));

            AssertSummon(SummonExpectedCreate(
                "FireSpirit", "Normal", 350, 60, 130,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 60, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Upgrade", 0.1, 3)));

            AssertSummon(SummonExpectedCreate(
                "QueenSpirit", "Special", 400, 100, 140,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 100, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 70, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.2, 3),
                AttackExpectedCreate("TargetedAttackStrategy", "Stun", 0, 3)));

            AssertSummon(SummonExpectedCreate(
                "DarkDragon", "Boss", 3000, 400, 500,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 400, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 370, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Burn", 0.2, 5),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 450, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "LifeDrain", 0.2, 4)));
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
        public void SummonAttackData_ProvidesGetPrefixGetterNames()
        {
            object attackData = AttackDataCreate("AllEnemies", "Poison", 0.1, 3, 2);

            Assert.AreEqual(EnumValue("SummonAttackStrategyType", "AllEnemies"), Invoke(attackData, "GetStrategyType"));
            Assert.AreEqual(EnumValue("StatusType", "Poison"), Invoke(attackData, "GetStatusType"));
            Assert.AreEqual(0.1, Invoke(attackData, "GetDamage"));
            Assert.AreEqual(3, Invoke(attackData, "GetCooltime"));
            Assert.AreEqual(2, Invoke(attackData, "GetStatusTime"));
        }

        [Test]
        public void SummonAttackData_DoesNotKeepDuplicateGetterNames()
        {
            object attackData = AttackDataCreate("AllEnemies", "Poison", 0.1, 3, 2);

            AssertMethodMissing(attackData, "StrategyTypeGet");
            AssertMethodMissing(attackData, "StatusTypeGet");
            AssertMethodMissing(attackData, "DamageGet");
            AssertMethodMissing(attackData, "CooltimeGet");
            AssertMethodMissing(attackData, "StatusTimeGet");
        }

        [Test]
        public void SummonAttackData_ProvidesCreatePrefixStrategyFactoryName()
        {
            object attackData = AttackDataCreate("AllEnemies", "Poison", 0.1, 3, 2);

            Assert.IsNotNull(Invoke(attackData, "CreateAttackStrategy"));
            AssertMethodMissing(attackData, "AttackStrategyCreate");
        }

        [Test]
        public void PlayerSummons_RemoveRedundantDamageAndDeathOverrides()
        {
            string[] summonNames = { "Rabbit", "Snake", "Wolf", "Eagle", "Fox" };

            foreach (string summonName in summonNames)
            {
                string text = File.ReadAllText($"Assets/Script/6_Summons/{summonName}.cs");

                Assert.IsFalse(text.Contains("public override void Die()"), $"{summonName} should inherit Die without a pass-through override.");
                Assert.IsFalse(text.Contains("public override void TakeDamage(double damage)"), $"{summonName} should inherit TakeDamage without a pass-through override.");
            }
        }

        [Test]
        public void EnemyAndSpiritSummons_RemoveRedundantDamageAndDeathOverrides()
        {
            string[] summonNames =
            {
                "Slime", "Skeleton", "LowDevil", "HighDevil", "KingSlime",
                "WaterSpirit", "GrassSpirit", "FireSpirit", "QueenSpirit", "DarkDragon"
            };

            foreach (string summonName in summonNames)
            {
                string text = File.ReadAllText($"Assets/Script/6_Summons/{summonName}.cs");

                Assert.IsFalse(text.Contains("public override void Die()"), $"{summonName} should inherit Die without a pass-through override.");
                Assert.IsFalse(text.Contains("public override void TakeDamage(double damage)"), $"{summonName} should inherit TakeDamage without a pass-through override.");
            }
        }

        [Test]
        public void PlayerDrawSummons_DoNotInitializeThroughSummonController()
        {
            string controllerText = File.ReadAllText("Assets/Script/5_Battle/5_SummonPick/SummonController.cs");

            Assert.IsFalse(controllerText.Contains("DrawSummonsInitialize();"));
            Assert.IsFalse(controllerText.Contains("private void DrawSummonsInitialize()"));
            Assert.IsFalse(controllerText.Contains("summon.SummonInitialize();"));
        }

        [Test]
        public void PlayerDrawSummons_SelectRankFromSummonData()
        {
            string summonText = File.ReadAllText("Assets/Script/6_Summons/Summon.cs");
            string drawServiceText = File.ReadAllText("Assets/Script/5_Battle/5_SummonPick/0_Draw/SummonDrawService.cs");

            Assert.IsTrue(summonText.Contains("public SummonRank GetDrawRank()"));
            Assert.IsTrue(summonText.Contains("return summonData == null ? summonRank : summonData.GetSummonRank();"));
            Assert.IsTrue(drawServiceText.Contains("summon.GetDrawRank() == rank"));
            Assert.IsFalse(drawServiceText.Contains("summon.GetSummonRank() == rank"));
        }

        [Test]
        public void SummonData_ProvidesGetPrefixGetterNames()
        {
            ScriptableObject data = CatCurrentDataCreate();

            Assert.AreEqual("Cat", Invoke(data, "GetSummonName"));
            Assert.AreEqual(EnumValue("SummonRank", "Low"), Invoke(data, "GetSummonRank"));
            Assert.AreEqual(1250d, Invoke(data, "GetMaxHp"));
            Assert.AreEqual(150d, Invoke(data, "GetAttackPower"));
            Assert.AreEqual(200d, Invoke(data, "GetHeavyAttackPower"));
            Assert.IsNotNull(Invoke(data, "GetNormalAttack"));
            Assert.IsNotNull(Invoke(data, "GetSpecialAttacks"));
        }

        [Test]
        public void SummonData_DoesNotKeepDuplicateGetterNames()
        {
            ScriptableObject data = CatCurrentDataCreate();

            AssertMethodMissing(data, "SummonNameGet");
            AssertMethodMissing(data, "SummonRankGet");
            AssertMethodMissing(data, "MaxHpGet");
            AssertMethodMissing(data, "AttackPowerGet");
            AssertMethodMissing(data, "HeavyAttackPowerGet");
            AssertMethodMissing(data, "NormalAttackGet");
            AssertMethodMissing(data, "SpecialAttacksGet");
        }

        [Test]
        public void SummonData_ProvidesCreatePrefixStrategyFactoryNames()
        {
            ScriptableObject data = CatCurrentDataCreate();

            Assert.IsNotNull(Invoke(data, "CreateNormalAttackStrategy"));
            Assert.IsNotNull(Invoke(data, "CreateSpecialAttackStrategies"));
            AssertMethodMissing(data, "NormalAttackStrategyCreate");
            AssertMethodMissing(data, "SpecialAttackStrategiesCreate");
        }

        [Test]
        public void PlayerSummons_DoNotInitializeFromStart()
        {
            string[] summonNames = { "Cat", "Rabbit", "Snake", "Wolf", "Eagle", "Fox" };

            foreach (string summonName in summonNames)
            {
                string text = File.ReadAllText($"Assets/Script/6_Summons/{summonName}.cs");

                Assert.IsFalse(text.Contains("void Start()"), $"{summonName} should not own draw-list initialization through Start.");
                Assert.IsFalse(text.Contains("SummonInitialize();\r\n    }"), $"{summonName} should leave draw-list initialization to SummonController.");
            }
        }

        [Test]
        public void EnemySummons_DoNotKeepHealthLogOnlyStart()
        {
            string[] summonNames = { "LowDevil", "HighDevil" };

            foreach (string summonName in summonNames)
            {
                string text = File.ReadAllText($"Assets/Script/6_Summons/{summonName}.cs");

                Assert.IsFalse(text.Contains("private void Start()"), $"{summonName} should not keep a Start method only for health logging.");
                Assert.IsFalse(text.Contains("Debug.Log(\"남은 체력: \" + nowHP);"), $"{summonName} should not log health from Start.");
            }
        }
        [Test]
        public void SummonFallbackHelpers_UseSetPrefixNames()
        {
            string summonText = File.ReadAllText("Assets/Script/6_Summons/Summon.cs");
            string[] summonNames =
            {
                "Cat", "Rabbit", "Snake", "Wolf", "Eagle", "Fox",
                "Slime", "Skeleton", "LowDevil", "HighDevil", "KingSlime",
                "WaterSpirit", "GrassSpirit", "FireSpirit", "QueenSpirit", "DarkDragon"
            };

            StringAssert.Contains("protected void SetFallbackStatus(", summonText);
            StringAssert.Contains("protected void SetAttackStrategies(", summonText);
            StringAssert.Contains("protected bool TryApplyAssignedSummonData()", summonText);
            Assert.IsFalse(summonText.Contains("FallbackStatusSet("));
            Assert.IsFalse(summonText.Contains("AttackStrategiesSet("));
            Assert.IsFalse(summonText.Contains("SummonDataApply("));
            Assert.IsFalse(summonText.Contains("GetSummonData()"));

            foreach (string summonName in summonNames)
            {
                string text = File.ReadAllText($"Assets/Script/6_Summons/{summonName}.cs");

                StringAssert.Contains("SetFallbackStatus(", text);
                StringAssert.Contains("SetAttackStrategies(", text);
                StringAssert.Contains("ApplyFallbackData()", text);
                Assert.IsFalse(text.Contains("TryApplyAssignedSummonData()"), summonName);
                Assert.IsFalse(text.Contains("FallbackStatusSet("), summonName);
                Assert.IsFalse(text.Contains("AttackStrategiesSet("), summonName);
                Assert.IsFalse(text.Contains("SummonDataApply(GetSummonData())"), summonName);
            }
        }


        [Test]
        public void PlayerSummons_UseAssignedSummonData_CoversAllPlayerTypes()
        {
            string testText = File.ReadAllText("Assets/Tests/EditMode/Editor/SummonDataRegressionTests.cs");
            string[] summonNames = { "Cat", "Rabbit", "Snake", "Wolf", "Eagle", "Fox" };

            foreach (string summonName in summonNames)
            {
                Assert.IsTrue(testText.Contains($"{summonName}CurrentDataCreate()"), $"{summonName} needs assigned data test data.");
            }
        }

        [Test]
        public void Cat_UsesAssignedSummonData()
        {
            GameObject testObject = new GameObject("Cat");
            ScriptableObject data = CatCurrentDataCreate();

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName("Cat"));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "SummonInitialize");

                Assert.AreEqual("Cat", Invoke(summon, "GetSummonName"));
                Assert.AreEqual("Low", Invoke(summon, "GetSummonRank").ToString());
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
        public void PlayerSummons_UseAssignedSummonData()
        {
            AssertAssignedPlayerSummonData(
                "Cat",
                CatCurrentDataCreate(),
                SummonExpectedCreate(
                    "Cat", "Low", 1250, 150, 200,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 150, 1),
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 200, 1)));

            AssertAssignedPlayerSummonData(
                "Rabbit",
                RabbitCurrentDataCreate(),
                SummonExpectedCreate(
                    "Rabbit", "Medium", 1500, 185, 0,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 185, 1),
                    AttackExpectedCreate("TargetedAttackStrategy", "Heal", 0.3, 3)));

            AssertAssignedPlayerSummonData(
                "Snake",
                SnakeCurrentDataCreate(),
                SummonExpectedCreate(
                    "Snake", "Medium", 1500, 200, 0,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 40, 1),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "Poison", 0.1, 3)));

            AssertAssignedPlayerSummonData(
                "Wolf",
                WolfCurrentDataCreate(),
                SummonExpectedCreate(
                    "Wolf", "High", 1750, 250, 150,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 250, 1),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 150, 2)));

            AssertAssignedPlayerSummonData(
                "Eagle",
                EagleCurrentDataCreate(),
                SummonExpectedCreate(
                    "Eagle", "High", 1750, 225, 150,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 225, 1),
                    AttackExpectedCreate("TargetedAttackStrategy", "None", 150, 2)));

            AssertAssignedPlayerSummonData(
                "Fox",
                FoxCurrentDataCreate(),
                SummonExpectedCreate(
                    "Fox", "Low", 1250, 175, 0,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 175, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "Upgrade", 0.3, 3)));
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

            AssertAssignedEnemySummonData(
                "FireSpirit",
                FireSpiritCurrentDataCreate(),
                SummonExpectedCreate(
                    "FireSpirit", "Normal", 350, 60, 130,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 60, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "Upgrade", 0.1, 3)));

            AssertAssignedEnemySummonData(
                "QueenSpirit",
                QueenSpiritCurrentDataCreate(),
                SummonExpectedCreate(
                    "QueenSpirit", "Special", 400, 100, 140,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 100, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 70, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.2, 3),
                    AttackExpectedCreate("TargetedAttackStrategy", "Stun", 0, 3)));

            AssertAssignedEnemySummonData(
                "DarkDragon",
                DarkDragonCurrentDataCreate(),
                SummonExpectedCreate(
                    "DarkDragon", "Boss", 3000, 400, 500,
                    AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 400, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 370, 0),
                    AttackExpectedCreate("AttackAllEnemiesStrategy", "Burn", 0.2, 5),
                    AttackExpectedCreate("TargetedAttackStrategy", "None", 450, 0),
                    AttackExpectedCreate("TargetedAttackStrategy", "LifeDrain", 0.2, 4)));
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
            AssertSummonDataReference("FireSpirit", "FireSpirit", "d8c7bf4c1d1742e8e65f8fc6abcd8778");
            AssertSummonDataReference("QueenSpirit", "QueenSpirit", "e9d8c05d2e2843f9f76a9fd7abcd9889");
            AssertSummonDataReference("DarkDragon", "DarkDragon", "fae9d16e3f3944a8a87b0fe8abcd0990");
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

            AssertSummonDataAsset(SummonExpectedCreate(
                "FireSpirit", "Normal", 350, 60, 130,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 60, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Upgrade", 0.1, 3)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "QueenSpirit", "Special", 400, 100, 140,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 100, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 70, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Heal", 0.2, 3),
                AttackExpectedCreate("TargetedAttackStrategy", "Stun", 0, 3)));

            AssertSummonDataAsset(SummonExpectedCreate(
                "DarkDragon", "Boss", 3000, 400, 500,
                AttackExpectedCreate("ClosestEnemyAttackStrategy", "None", 400, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "None", 370, 0),
                AttackExpectedCreate("AttackAllEnemiesStrategy", "Burn", 0.2, 5),
                AttackExpectedCreate("TargetedAttackStrategy", "None", 450, 0),
                AttackExpectedCreate("TargetedAttackStrategy", "LifeDrain", 0.2, 4)));
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
            int cooldownDuration)
        {
            return new ExpectedAttack(
                strategyTypeName,
                statusType,
                damage,
                cooldownDuration);
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
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.CooldownDuration,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].CooldownDuration,
                    initializeMethodName);
                return;
            }

            if (expected.SpecialAttacks.Length == 2)
            {
                AssertSummonWithTwoSpecialAttacks(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.CooldownDuration,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].CooldownDuration,
                    expected.SpecialAttacks[1].StrategyTypeName,
                    expected.SpecialAttacks[1].StatusType,
                    expected.SpecialAttacks[1].Damage,
                    expected.SpecialAttacks[1].CooldownDuration,
                    initializeMethodName);
                return;
            }

            if (expected.SpecialAttacks.Length >= 3)
            {
                AssertSummonWithExpectedAttacks(expected.TypeName, expected, initializeMethodName);
                return;
            }

            Assert.Fail(expected.TypeName + " expected special attack count is not supported.");
        }

        private void AssertAssignedEnemySummonData(
            string summonClassName,
            ScriptableObject data,
            ExpectedSummon expected)
        {
            if (expected.SpecialAttacks.Length == 1)
            {
                AssertAssignedEnemySummonData(
                    summonClassName,
                    data,
                    expected.Name,
                    expected.Rank,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.CooldownDuration,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].CooldownDuration);
                return;
            }

            if (expected.SpecialAttacks.Length == 2)
            {
                AssertAssignedEnemySummonDataWithTwoSpecialAttacks(
                    summonClassName,
                    data,
                    expected.Name,
                    expected.Rank,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.CooldownDuration,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].CooldownDuration,
                    expected.SpecialAttacks[1].StrategyTypeName,
                    expected.SpecialAttacks[1].StatusType,
                    expected.SpecialAttacks[1].Damage,
                    expected.SpecialAttacks[1].CooldownDuration);
                return;
            }

            if (expected.SpecialAttacks.Length >= 3)
            {
                AssertAssignedEnemySummonDataWithExpectedAttacks(summonClassName, data, expected);
                return;
            }

            Assert.Fail(expected.TypeName + " expected special attack count is not supported.");
        }

        private void AssertAssignedPlayerSummonData(
            string summonClassName,
            ScriptableObject data,
            ExpectedSummon expected)
        {
            GameObject testObject = new GameObject(summonClassName);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "SummonInitialize");

                AssertSummonValues(summon, expected);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
                UnityEngine.Object.DestroyImmediate(data);
            }
        }

        private void AssertSummonDataAsset(ExpectedSummon expected)
        {
            if (expected.SpecialAttacks.Length == 1)
            {
                AssertSummonDataAsset(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.CooldownDuration,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].CooldownDuration);
                return;
            }

            if (expected.SpecialAttacks.Length == 2)
            {
                AssertSummonDataAssetWithTwoSpecialAttacks(
                    expected.TypeName,
                    expected.Name,
                    expected.Rank,
                    expected.MaxHp,
                    expected.AttackPower,
                    expected.HeavyAttackPower,
                    expected.NormalAttack.StrategyTypeName,
                    expected.NormalAttack.StatusType,
                    expected.NormalAttack.Damage,
                    expected.NormalAttack.CooldownDuration,
                    expected.SpecialAttacks[0].StrategyTypeName,
                    expected.SpecialAttacks[0].StatusType,
                    expected.SpecialAttacks[0].Damage,
                    expected.SpecialAttacks[0].CooldownDuration,
                    expected.SpecialAttacks[1].StrategyTypeName,
                    expected.SpecialAttacks[1].StatusType,
                    expected.SpecialAttacks[1].Damage,
                    expected.SpecialAttacks[1].CooldownDuration);
                return;
            }

            if (expected.SpecialAttacks.Length >= 3)
            {
                AssertSummonDataAssetWithExpectedAttacks(expected);
                return;
            }

            Assert.Fail(expected.TypeName + " expected special attack count is not supported.");
        }

        private void AssertSummon(
            string summonClassName,
            string expectedName,
            string expectedRank,
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
            GameObject testObject = new GameObject(summonClassName);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                Invoke(summon, initializeMethodName);

                Assert.AreEqual(expectedName, Invoke(summon, "GetSummonName"));
                Assert.AreEqual(expectedRank, Invoke(summon, "GetSummonRank").ToString());
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
            string summonClassName,
            string expectedName,
            string expectedRank,
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
            GameObject testObject = new GameObject(summonClassName);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                Invoke(summon, initializeMethodName);

                AssertSummonValuesWithTwoSpecialAttacks(
                    summon,
                    expectedName,
                    expectedRank,
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
            string summonClassName,
            ScriptableObject data,
            string expectedName,
            string expectedRank,
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
            GameObject testObject = new GameObject(summonClassName);
            testObject.SetActive(false);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "SummonInitialize");

                AssertSummonValues(
                    summon,
                    expectedName,
                    expectedRank,
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
            string summonClassName,
            ScriptableObject data,
            string expectedName,
            string expectedRank,
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
            GameObject testObject = new GameObject(summonClassName);
            testObject.SetActive(false);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "SummonInitialize");

                AssertSummonValuesWithTwoSpecialAttacks(
                    summon,
                    expectedName,
                    expectedRank,
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
                $"Assets/Script/6_Summons/Data/{summonName}SummonData.asset",
                GetTypeByName("SummonData"));

            Assert.NotNull(data);
            Assert.AreEqual(expectedName, Invoke(data, "GetSummonName"));
            Assert.AreEqual(expectedRank, Invoke(data, "GetSummonRank").ToString());
            Assert.AreEqual(expectedMaxHp, Invoke(data, "GetMaxHp"));
            Assert.AreEqual(expectedAttackPower, Invoke(data, "GetAttackPower"));
            Assert.AreEqual(expectedHeavyAttackPower, Invoke(data, "GetHeavyAttackPower"));

            AssertAttackStrategy(
                Invoke(data, "CreateNormalAttackStrategy"),
                expectedNormalAttackTypeName,
                expectedNormalAttackStatusType,
                expectedNormalAttackDamage,
                expectedNormalAttackCooltime);

            Array specialAttacks = (Array)Invoke(data, "CreateSpecialAttackStrategies");
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
                $"Assets/Script/6_Summons/Data/{summonName}SummonData.asset",
                GetTypeByName("SummonData"));

            Assert.NotNull(data);
            Assert.AreEqual(expectedName, Invoke(data, "GetSummonName"));
            Assert.AreEqual(expectedRank, Invoke(data, "GetSummonRank").ToString());
            Assert.AreEqual(expectedMaxHp, Invoke(data, "GetMaxHp"));
            Assert.AreEqual(expectedAttackPower, Invoke(data, "GetAttackPower"));
            Assert.AreEqual(expectedHeavyAttackPower, Invoke(data, "GetHeavyAttackPower"));

            AssertAttackStrategy(
                Invoke(data, "CreateNormalAttackStrategy"),
                expectedNormalAttackTypeName,
                expectedNormalAttackStatusType,
                expectedNormalAttackDamage,
                expectedNormalAttackCooltime);

            Array specialAttacks = (Array)Invoke(data, "CreateSpecialAttackStrategies");
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

        private void AssertSummonWithExpectedAttacks(
            string summonClassName,
            ExpectedSummon expected,
            string initializeMethodName)
        {
            GameObject testObject = new GameObject(summonClassName);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                Invoke(summon, initializeMethodName);

                AssertSummonValues(summon, expected);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
        }

        private void AssertAssignedEnemySummonDataWithExpectedAttacks(
            string summonClassName,
            ScriptableObject data,
            ExpectedSummon expected)
        {
            GameObject testObject = new GameObject(summonClassName);
            testObject.SetActive(false);

            try
            {
                Component summon = testObject.AddComponent(GetTypeByName(summonClassName));
                FieldSet(summon, "summonData", data);

                Invoke(summon, "SummonInitialize");

                AssertSummonValues(summon, expected);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
                UnityEngine.Object.DestroyImmediate(data);
            }
        }

        private void AssertSummonDataAssetWithExpectedAttacks(ExpectedSummon expected)
        {
            UnityEngine.Object data = AssetDatabase.LoadAssetAtPath(
                $"Assets/Script/6_Summons/Data/{expected.TypeName}SummonData.asset",
                GetTypeByName("SummonData"));

            Assert.NotNull(data);
            Assert.AreEqual(expected.Name, Invoke(data, "GetSummonName"));
            Assert.AreEqual(expected.Rank, Invoke(data, "GetSummonRank").ToString());
            Assert.AreEqual(expected.MaxHp, Invoke(data, "GetMaxHp"));
            Assert.AreEqual(expected.AttackPower, Invoke(data, "GetAttackPower"));
            Assert.AreEqual(expected.HeavyAttackPower, Invoke(data, "GetHeavyAttackPower"));

            AssertAttackStrategy(
                Invoke(data, "CreateNormalAttackStrategy"),
                expected.NormalAttack.StrategyTypeName,
                expected.NormalAttack.StatusType,
                expected.NormalAttack.Damage,
                expected.NormalAttack.CooldownDuration);

            Array specialAttacks = (Array)Invoke(data, "CreateSpecialAttackStrategies");
            AssertSpecialAttacks(specialAttacks, expected.SpecialAttacks);
        }

        private void AssertSummonValues(Component summon, ExpectedSummon expected)
        {
            Assert.AreEqual(expected.Name, Invoke(summon, "GetSummonName"));
            Assert.AreEqual(expected.Rank, Invoke(summon, "GetSummonRank").ToString());
            Assert.AreEqual(expected.MaxHp, Invoke(summon, "GetMaxHP"));
            Assert.AreEqual(expected.AttackPower, Invoke(summon, "GetAttackPower"));
            Assert.AreEqual(expected.HeavyAttackPower, Invoke(summon, "GetHeavyAttackPower"));

            AssertAttackStrategy(
                Invoke(summon, "GetAttackStrategy"),
                expected.NormalAttack.StrategyTypeName,
                expected.NormalAttack.StatusType,
                expected.NormalAttack.Damage,
                expected.NormalAttack.CooldownDuration);

            Array specialAttacks = (Array)Invoke(summon, "GetSpecialAttackStrategy");
            AssertSpecialAttacks(specialAttacks, expected.SpecialAttacks);
        }

        private void AssertSpecialAttacks(Array actualSpecialAttacks, ExpectedAttack[] expectedSpecialAttacks)
        {
            Assert.NotNull(actualSpecialAttacks);
            Assert.AreEqual(expectedSpecialAttacks.Length, actualSpecialAttacks.Length);

            for (int i = 0; i < expectedSpecialAttacks.Length; i++)
            {
                ExpectedAttack expectedAttack = expectedSpecialAttacks[i];
                AssertAttackStrategy(
                    actualSpecialAttacks.GetValue(i),
                    expectedAttack.StrategyTypeName,
                    expectedAttack.StatusType,
                    expectedAttack.Damage,
                    expectedAttack.CooldownDuration);
            }
        }

        private void AssertAttackStrategy(
            object attackStrategy,
            string expectedTypeName,
            string expectedStatusType,
            double expectedDamage,
            int expectedCooldownDuration)
        {
            Assert.NotNull(attackStrategy);
            Assert.IsInstanceOf(GetTypeByName(expectedTypeName), attackStrategy);
            Assert.AreEqual(expectedStatusType, Invoke(attackStrategy, "GetStatusType").ToString());
            Assert.AreEqual(expectedDamage, Invoke(attackStrategy, "GetSpecialDamage"));
            Assert.AreEqual(expectedCooldownDuration, Invoke(attackStrategy, "GetCooltime"));
        }

        private void AssertAttackDataCreatesStrategy(
            object attackData,
            string expectedTypeName,
            string expectedStatusType,
            double expectedDamage,
            int expectedCooldownDuration)
        {
            AssertAttackStrategy(
                Invoke(attackData, "CreateAttackStrategy"),
                expectedTypeName,
                expectedStatusType,
                expectedDamage,
                expectedCooldownDuration);
        }

        private ScriptableObject CatCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Cat");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Low"));
            FieldSet(data, "maxHp", 1250d);
            FieldSet(data, "attackPower", 150d);
            FieldSet(data, "heavyAttackPower", 200d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 150, 1));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("ClosestEnemy", "None", 200, 1), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject RabbitCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Rabbit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Medium"));
            FieldSet(data, "maxHp", 1500d);
            FieldSet(data, "attackPower", 185d);
            FieldSet(data, "heavyAttackPower", 0d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 185, 1));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Heal", 0.3, 3), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject SnakeCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Snake");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Medium"));
            FieldSet(data, "maxHp", 1500d);
            FieldSet(data, "attackPower", 200d);
            FieldSet(data, "heavyAttackPower", 0d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 40, 1));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "Poison", 0.1, 3, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject WolfCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Wolf");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "High"));
            FieldSet(data, "maxHp", 1750d);
            FieldSet(data, "attackPower", 250d);
            FieldSet(data, "heavyAttackPower", 150d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 250, 1));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 150, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject EagleCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Eagle");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "High"));
            FieldSet(data, "maxHp", 1750d);
            FieldSet(data, "attackPower", 225d);
            FieldSet(data, "heavyAttackPower", 150d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 225, 1));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "None", 150, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject FoxCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Fox");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Low"));
            FieldSet(data, "maxHp", 1250d);
            FieldSet(data, "attackPower", 175d);
            FieldSet(data, "heavyAttackPower", 0d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 175, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Upgrade", 0.3, 3, 1), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject SlimeCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Slime");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "maxHp", 200d);
            FieldSet(data, "attackPower", 25d);
            FieldSet(data, "heavyAttackPower", 40d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 25, 1));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Shield", 50, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject SkeletonCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "Skeleton");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "maxHp", 650d);
            FieldSet(data, "attackPower", 150d);
            FieldSet(data, "heavyAttackPower", 170d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 150, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "None", 160, 0), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject LowDevilCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "LowDevil");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "maxHp", 700d);
            FieldSet(data, "attackPower", 180d);
            FieldSet(data, "heavyAttackPower", 220d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 180, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 2);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Curse", 0.2, 4, 1), 0);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "OnceInvincibility", 0, 2), 1);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject HighDevilCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "HighDevil");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Special"));
            FieldSet(data, "maxHp", 1000d);
            FieldSet(data, "attackPower", 200d);
            FieldSet(data, "heavyAttackPower", 250d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 200, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 2);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 140, 0), 0);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "None", 230, 0), 1);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject KingSlimeCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "KingSlime");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Special"));
            FieldSet(data, "maxHp", 250d);
            FieldSet(data, "attackPower", 50d);
            FieldSet(data, "heavyAttackPower", 65d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 50, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 2);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 35, 1), 0);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Shield", 80, 2), 1);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject WaterSpiritCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "WaterSpirit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "maxHp", 350d);
            FieldSet(data, "attackPower", 70d);
            FieldSet(data, "heavyAttackPower", 120d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 70, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Shield", 80, 2), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject GrassSpiritCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "GrassSpirit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "maxHp", 350d);
            FieldSet(data, "attackPower", 80d);
            FieldSet(data, "heavyAttackPower", 110d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 80, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "Heal", 0.1, 3), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject FireSpiritCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "FireSpirit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Normal"));
            FieldSet(data, "maxHp", 350d);
            FieldSet(data, "attackPower", 60d);
            FieldSet(data, "heavyAttackPower", 130d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 60, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 1);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "Upgrade", 0.1, 3, 1), 0);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject QueenSpiritCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "QueenSpirit");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Special"));
            FieldSet(data, "maxHp", 400d);
            FieldSet(data, "attackPower", 100d);
            FieldSet(data, "heavyAttackPower", 140d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 100, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 3);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 70, 0), 0);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "Heal", 0.2, 3), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "Stun", 0, 3, 1), 2);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private ScriptableObject DarkDragonCurrentDataCreate()
        {
            ScriptableObject data = ScriptableObject.CreateInstance(GetTypeByName("SummonData"));
            FieldSet(data, "summonName", "DarkDragon");
            FieldSet(data, "summonRank", EnumValue("SummonRank", "Boss"));
            FieldSet(data, "maxHp", 3000d);
            FieldSet(data, "attackPower", 400d);
            FieldSet(data, "heavyAttackPower", 500d);
            FieldSet(data, "normalAttack", AttackDataCreate("ClosestEnemy", "None", 400, 0));

            Array specialAttacks = Array.CreateInstance(GetTypeByName("SummonAttackData"), 4);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "None", 370, 0), 0);
            specialAttacks.SetValue(AttackDataCreate("AllEnemies", "Burn", 0.2, 5, 2), 1);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "None", 450, 0), 2);
            specialAttacks.SetValue(AttackDataCreate("Targeted", "LifeDrain", 0.2, 4, 2), 3);
            FieldSet(data, "specialAttacks", specialAttacks);
            return data;
        }

        private void AssertSummonDataReference(string summonName, string prefabName, string guid)
        {
            string assetText = File.ReadAllText($"Assets/Script/6_Summons/Data/{summonName}SummonData.asset");
            string metaText = File.ReadAllText($"Assets/Script/6_Summons/Data/{summonName}SummonData.asset.meta");
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
            int cooldownDuration,
            int statusTime = 0)
        {
            return Activator.CreateInstance(
                GetTypeByName("SummonAttackData"),
                EnumValue("SummonAttackStrategyType", strategyType),
                EnumValue("StatusType", statusType),
                damage,
                cooldownDuration,
                statusTime);
        }

        private object EnumValue(string typeName, string valueName)
        {
            return Enum.Parse(GetTypeByName(typeName), valueName);
        }

        private object Invoke(object target, string methodName)
        {
            return target.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Invoke(target, null);
        }

        private void AssertMethodMissing(object target, string methodName)
        {
            MethodInfo method = target.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.IsNull(method, methodName + " should not remain as a duplicate getter name.");
        }

        private void FieldSet(object target, string fieldName, object value)
        {
            GetFieldInfo(target.GetType(), fieldName).SetValue(target, value);
        }

        private FieldInfo GetFieldInfo(Type type, string fieldName)
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

        private Type GetTypeByName(string typeName)
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
