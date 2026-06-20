using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class StageEnemyPlacementStaticRegressionTests
    {
        [Test]
        public void StageEnemyPlacementData_UsesExplicitStageAndEnemySlots()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/4_Data/StageEnemyPlacementData.cs");

            StringAssert.Contains("[CreateAssetMenu(menuName = \"Summoner/Stage Enemy Placement Data\")]", text);
            StringAssert.Contains("public class StageEnemyPlacementData : ScriptableObject", text);
            StringAssert.Contains("private List<StageEnemyPlacement> stageEnemyPlacements", text);
            StringAssert.Contains("public List<EnemyPlacementSlot> GetEnemyPlacementSlots(int stage)", text);
            StringAssert.Contains("public class StageEnemyPlacement", text);
            StringAssert.Contains("private int stage", text);
            StringAssert.Contains("private List<EnemyPlacementSlot> enemyPlacementSlots", text);
            StringAssert.Contains("public bool StageMatches(int stage)", text);
            StringAssert.Contains("public class EnemyPlacementSlot", text);
            StringAssert.Contains("private int plateIndex", text);
            StringAssert.Contains("private Summon enemySummonPrefab", text);
        }

        [Test]
        public void BattleEnemyPlacementController_AppliesDataThroughEnemyPlates()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/0_Start/BattleEnemyPlacementController.cs");

            StringAssert.Contains("[RequireComponent(typeof(BattleStageContext))]", text);
            StringAssert.Contains("private BattleStageContext stageContext", text);
            StringAssert.Contains("private PlateController plateController", text);
            StringAssert.Contains("private StageEnemyPlacementData stageEnemyPlacementData", text);
            StringAssert.Contains("public void EnemyPlacementApply()", text);
            StringAssert.Contains("plateController.GetEnermyPlates()", text);
            StringAssert.Contains("stageEnemyPlacementData.GetEnemyPlacementSlots(stageContext.CurrentStage)", text);
            StringAssert.Contains("targetPlate.SummonPlaceOnPlate(enemySummonPrefab)", text);
            Assert.IsFalse(text.Contains("private void Start()"));
        }

        [Test]
        public void BattleStartController_StartsBattleFlowOnceBeforeTurnFlow()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/0_Start/BattleStartController.cs");

            StringAssert.Contains("[RequireComponent(typeof(BattleEnemyPlacementController))]", text);
            StringAssert.Contains("public class BattleStartController : MonoBehaviour", text);
            StringAssert.Contains("private BattleEnemyPlacementController enemyPlacementController", text);
            StringAssert.Contains("private TurnController turnController", text);
            StringAssert.Contains("private bool hasStartedBattle", text);
            StringAssert.Contains("private void StartBattleFlow()", text);
            StringAssert.Contains("if (hasStartedBattle)", text);
            StringAssert.Contains("enemyPlacementController.EnemyPlacementApply();", text);
            StringAssert.Contains("turnController.StartBattleTurnFlow();", text);
            Assert.IsFalse(text.Contains("public void BattleStartInitialize()"));

            Assert.Less(
                text.IndexOf("enemyPlacementController.EnemyPlacementApply();"),
                text.IndexOf("turnController.StartBattleTurnFlow();"));
        }

        [Test]
        public void TurnController_ExposesOnlyBattleTurnFlowEntryPointsInsideAssembly()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/1_Turn/TurnController.cs");

            StringAssert.Contains("internal void StartBattleTurnFlow()", text);
            StringAssert.Contains("internal void EndCurrentTurn()", text);
            StringAssert.Contains("StartCurrentTurn();", text);
            StringAssert.Contains("EndCurrentTurn();", text);
            Assert.IsFalse(text.Contains("public void TurnStartInitialize()"));
            Assert.IsFalse(text.Contains("public void StartTurn()"));
            Assert.IsFalse(text.Contains("public void EndTurn()"));
            Assert.IsFalse(text.Contains("void Start()"));
        }

        [Test]
        public void BattleStageContext_UsesSceneDefaultStageBeforeSaveFallback()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/3_Result/BattleStageContext.cs");

            StringAssert.Contains("private int defaultStage = 1", text);
            StringAssert.Contains("CurrentStage = CurrentStageResolve()", text);
            StringAssert.Contains("if (defaultStage > 0)", text);
            StringAssert.Contains("return defaultStage", text);
            StringAssert.Contains("GameSaveController.GetGameSaveOrDefault().playingStage", text);
        }

        [Test]
        public void BattleEnemyPlacementController_GuardsMissingReferencesAndInvalidSlots()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/0_Start/BattleEnemyPlacementController.cs");

            StringAssert.Contains("stageContext == null || plateController == null || stageEnemyPlacementData == null", text);
            StringAssert.Contains("enemyPlates == null || enemyPlates.Count == 0", text);
            StringAssert.Contains("enemyPlacementSlot == null", text);
            StringAssert.Contains("plateIndex < 0 || plateIndex >= enemyPlates.Count", text);
            StringAssert.Contains("enemySummonPrefab == null", text);
            StringAssert.Contains("targetPlate == null || EnemyPlateHasSummon(targetPlate)", text);
            StringAssert.Contains("targetPlate.GetComponentInChildren<Summon>(true) != null", text);
            StringAssert.Contains("GetComponent<BattleStageContext>()", text);
            StringAssert.Contains("Debug.LogError(\"BattleEnemyPlacementController needs PlateController. Assign it in the Inspector.\");", text);
            Assert.IsFalse(text.Contains("FindObjectOfType<PlateController>()"), "Battle enemy placement should use explicit Inspector reference for PlateController.");
        }

        [Test]
        public void StageEnemyPlacementDataAsset_ContainsStageOneSlimePlacement()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/4_Data/StageEnemyPlacementData.asset");

            StringAssert.Contains("m_Name: StageEnemyPlacementData", text);
            StringAssert.Contains("stageEnemyPlacements:", text);
            StringAssert.Contains("- stage: 1", text);
            StringAssert.Contains("plateIndex: 0", text);
            StringAssert.Contains("plateIndex: 1", text);
            StringAssert.Contains("guid: 8fd706c78e2922046af9ec2e55c582ca", text);
        }

        [Test]
        public void StageEnemyPlacementDataAsset_ContainsExistingFightSceneEnemyPlacements()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/4_Data/StageEnemyPlacementData.asset");

            StringAssert.Contains("- stage: 2", text);
            StringAssert.Contains("guid: bf542230506a74c42b2451b8d3bc631f", text);
            StringAssert.Contains("- stage: 3", text);
            StringAssert.Contains("guid: 8fd297f85265bc04894ce80f6e81ac6c", text);
            StringAssert.Contains("guid: 5dd30040c5ae722429071dc4dc0dbca2", text);
            StringAssert.Contains("- stage: 4", text);
            StringAssert.Contains("guid: 90097dbc557a1854191dac4f46b78a46", text);
            StringAssert.Contains("guid: daceb49291e617948a072a85699e6272", text);
            StringAssert.Contains("- stage: 5", text);
            StringAssert.Contains("guid: 0742304be4be2ec42acdc85ae66b8eea", text);
            StringAssert.Contains("- stage: 6", text);
            StringAssert.Contains("guid: e40edda8095726c4085f00ac81879052", text);
            StringAssert.Contains("guid: d3bcd2f130775e243a6678d60797559b", text);
            StringAssert.Contains("- stage: 7", text);
            StringAssert.Contains("guid: 46576d69373393243a279ff2ab892158", text);
        }

        [Test]
        public void StageEnemyPlacementData_HasInspectorLabelsForReadablePlacementEditing()
        {
            string dataText = File.ReadAllText("Assets/Script/5_Battle/0_Flow/4_Data/StageEnemyPlacementData.cs");
            string drawerText = File.ReadAllText("Assets/Script/5_Battle/0_Flow/4_Data/Editor/StageEnemyPlacementPropertyDrawer.cs");

            StringAssert.Contains("[Header(\"Stage enemy placements\")]", dataText);
            StringAssert.Contains("[Tooltip(\"Enemy summons placed on enemy plates when each battle stage starts.\")]", dataText);
            StringAssert.Contains("[Min(1)]", dataText);
            StringAssert.Contains("[Min(0)]", dataText);
            StringAssert.Contains("[CustomPropertyDrawer(typeof(StageEnemyPlacement))]", drawerText);
            StringAssert.Contains("Stage \" + stage.intValue + \" enemy placements", drawerText);
            StringAssert.Contains("[CustomPropertyDrawer(typeof(EnemyPlacementSlot))]", drawerText);
            StringAssert.Contains("\"Plate \" + plateIndex.intValue + \" - \" + enemyName", drawerText);
        }

        [Test]
        public void FightSceneOne_ReferencesStageEnemyPlacementControllerAndData()
        {
            string text = File.ReadAllText("Assets/Screen/FightScene/Fight Screen_1Stage.unity");

            StringAssert.Contains("m_Name: 0_BattleFlow_Runtime", text);
            StringAssert.Contains("m_Name: 2_Stage_Runtime", text);
            StringAssert.Contains("m_Father: {fileID: 1052698416}", text);
            StringAssert.Contains("guid: 806dc46760cf61141ba36ec49f9746c5", text);
            StringAssert.Contains("guid: ab85d6e26531f4440a364f369671693f", text);
            StringAssert.Contains("m_GameObject: {fileID: 2140000011}", text);
            StringAssert.Contains("defaultStage: 1", text);
            StringAssert.Contains("stageContext: {fileID: 893681187}", text);
            StringAssert.Contains("plateController: {fileID: 893681172}", text);
            StringAssert.Contains("stageEnemyPlacementData: {fileID: 11400000, guid: b45f72d8c743c6247a1f8bd9d6601ee5, type: 2}", text);
            StringAssert.Contains("guid: 2b0a46764b2f4a84aa2e29cf8bde7a93", text);
            StringAssert.Contains("enemyPlacementController: {fileID: 893681188}", text);
            StringAssert.Contains("turnController: {fileID: 893681170}", text);
        }

        [Test]
        public void FightScenes_DoNotKeepDirectEnemySummonOverride()
        {
            for (int stage = 1; stage <= 7; stage++)
            {
                string text = File.ReadAllText($"Assets/Screen/FightScene/Fight Screen_{stage}Stage.unity");

                Assert.IsFalse(text.Contains("propertyPath: summon\n"), "Fight scene " + stage + " should use StageEnemyPlacementData instead of scene summon override.");
                Assert.IsFalse(text.Contains("propertyPath: summon\r\n"), "Fight scene " + stage + " should use StageEnemyPlacementData instead of scene summon override.");
            }
        }
    }
}
