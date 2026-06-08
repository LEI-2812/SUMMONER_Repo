using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class StageEnemyPlacementStaticRegressionTests
    {
        [Test]
        public void StageEnemyPlacementData_UsesExplicitStageAndEnemySlots()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/StageEnemyPlacementData.cs");

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
            string text = File.ReadAllText("Assets/Script/Battle/Flow/BattleEnemyPlacementController.cs");

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
        public void BattleStartController_ControlsEnemyPlacementBeforeTurnStart()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/BattleStartController.cs");

            StringAssert.Contains("[RequireComponent(typeof(BattleEnemyPlacementController))]", text);
            StringAssert.Contains("public class BattleStartController : MonoBehaviour", text);
            StringAssert.Contains("private BattleEnemyPlacementController enemyPlacementController", text);
            StringAssert.Contains("private TurnController turnController", text);
            StringAssert.Contains("public void BattleStartInitialize()", text);
            StringAssert.Contains("enemyPlacementController.EnemyPlacementApply();", text);
            StringAssert.Contains("turnController.TurnStartInitialize();", text);

            Assert.Less(
                text.IndexOf("enemyPlacementController.EnemyPlacementApply();"),
                text.IndexOf("turnController.TurnStartInitialize();"));
        }

        [Test]
        public void TurnController_ExposesTurnStartInitializeForBattleStartFlow()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/TurnController.cs");

            StringAssert.Contains("public void TurnStartInitialize()", text);
            StringAssert.Contains("StartTurn();", text);
            Assert.IsFalse(text.Contains("void Start()"));
        }

        [Test]
        public void BattleStageContext_UsesSceneDefaultStageBeforeSaveFallback()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/BattleStageContext.cs");

            StringAssert.Contains("private int defaultStage = 1", text);
            StringAssert.Contains("CurrentStage = CurrentStageResolve()", text);
            StringAssert.Contains("if (defaultStage > 0)", text);
            StringAssert.Contains("return defaultStage", text);
            StringAssert.Contains("GameSaveController.GetGameSaveOrDefault().playingStage", text);
        }

        [Test]
        public void BattleEnemyPlacementController_GuardsMissingReferencesAndInvalidSlots()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/BattleEnemyPlacementController.cs");

            StringAssert.Contains("stageContext == null || plateController == null || stageEnemyPlacementData == null", text);
            StringAssert.Contains("enemyPlates == null || enemyPlates.Count == 0", text);
            StringAssert.Contains("enemyPlacementSlot == null", text);
            StringAssert.Contains("plateIndex < 0 || plateIndex >= enemyPlates.Count", text);
            StringAssert.Contains("enemySummonPrefab == null", text);
            StringAssert.Contains("targetPlate == null || EnemyPlateHasSummon(targetPlate)", text);
            StringAssert.Contains("targetPlate.GetComponentInChildren<Summon>(true) != null", text);
            StringAssert.Contains("GetComponent<BattleStageContext>()", text);
            StringAssert.Contains("FindObjectOfType<PlateController>()", text);
        }

        [Test]
        public void StageEnemyPlacementDataAsset_ContainsStageOneSlimePlacement()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/Data/StageEnemyPlacementData.asset");

            StringAssert.Contains("m_Name: StageEnemyPlacementData", text);
            StringAssert.Contains("stageEnemyPlacements:", text);
            StringAssert.Contains("- stage: 1", text);
            StringAssert.Contains("plateIndex: 0", text);
            StringAssert.Contains("plateIndex: 1", text);
            StringAssert.Contains("guid: 8fd706c78e2922046af9ec2e55c582ca", text);
        }

        [Test]
        public void FightSceneOne_ReferencesStageEnemyPlacementControllerAndData()
        {
            string text = File.ReadAllText("Assets/Screen/FightScene/Fight Screen_1Stage.unity");

            StringAssert.Contains("m_Name: BattleRuntime", text);
            StringAssert.Contains("m_Father: {fileID: 1052698416}", text);
            StringAssert.Contains("guid: 806dc46760cf61141ba36ec49f9746c5", text);
            StringAssert.Contains("guid: ab85d6e26531f4440a364f369671693f", text);
            StringAssert.Contains("m_GameObject: {fileID: 2140000001}", text);
            StringAssert.Contains("defaultStage: 1", text);
            StringAssert.Contains("stageContext: {fileID: 893681187}", text);
            StringAssert.Contains("plateController: {fileID: 893681172}", text);
            StringAssert.Contains("stageEnemyPlacementData: {fileID: 11400000, guid: b45f72d8c743c6247a1f8bd9d6601ee5, type: 2}", text);
            StringAssert.Contains("guid: 2b0a46764b2f4a84aa2e29cf8bde7a93", text);
            StringAssert.Contains("enemyPlacementController: {fileID: 893681188}", text);
            StringAssert.Contains("turnController: {fileID: 893681170}", text);
        }

        [Test]
        public void FightSceneOne_DoesNotKeepDirectEnemySummonOverride()
        {
            string text = File.ReadAllText("Assets/Screen/FightScene/Fight Screen_1Stage.unity");
            string normalizedText = text.Replace("\r\n", "\n");
            string directSlimeOverride =
                "propertyPath: summon\n" +
                "      value: \n" +
                "      objectReference: {fileID: 1760420319589789094, guid: 8fd706c78e2922046af9ec2e55c582ca, type: 3}";

            Assert.IsFalse(normalizedText.Contains(directSlimeOverride));
        }
    }
}
