using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class BattleResultFlowStaticRegressionTests
    {
        [Test]
        public void TurnController_DoesNotReferenceStageController()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/1_Turn/TurnController.cs");
            string turnPhaseActionsText = File.ReadAllText("Assets/Script/5_Battle/0_Flow/1_Turn/TurnPhaseActions.cs");

            Assert.IsFalse(text.Contains("StageController"), "TurnController should not depend on StageController in fight scenes.");
            StringAssert.Contains("BattleResultController", text);
            StringAssert.Contains("BattleResultController", turnPhaseActionsText);
            StringAssert.Contains("battleResultController.ClearResultTry(", turnPhaseActionsText);
            Assert.IsFalse(text.Contains("battleResultController.ClearResultTry("), "TurnController should delegate turn phase decisions instead of checking clear result directly.");
        }

        [Test]
        public void BattleResultAlertView_DoesNotSaveOrMoveScenes()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/9_View/BattleResultAlertView.cs");

            Assert.IsFalse(text.Contains("GameSaveController"), "BattleResultAlertView should only display alerts.");
            Assert.IsFalse(text.Contains("StageFlowController"), "BattleResultAlertView should not own scene flow.");
            Assert.IsFalse(text.Contains("SendNextStageAfterBattle"), "BattleResultAlertView should not move to the next stage.");
            Assert.IsFalse(text.Contains("SendStageSelect"), "BattleResultAlertView should not move to stage select.");
            Assert.IsFalse(text.Contains("SendFight"), "BattleResultAlertView should not retry scenes directly.");
            StringAssert.Contains("System.Action<bool>", text);
        }

        [Test]
        public void BattleResultController_DelegatesAlertResultToProgress()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/3_Result/BattleResultController.cs");

            StringAssert.Contains("BattleResultAlertView", text);
            StringAssert.Contains("BattleProgressController", text);
            StringAssert.Contains("ShowClearResultAlert(progressController.CompleteClearResult)", text);
            StringAssert.Contains("ShowFailResultAlert(progressController.CompleteFailResult)", text);
            StringAssert.Contains("resultStarted", text);
            Assert.IsFalse(text.Contains("AddComponent<BattleResultAlertView>()"), "BattleResultController should not create result alert view at runtime.");
            Assert.IsFalse(text.Contains("AddComponent<BattleProgressController>()"), "BattleResultController should not create progress controller at runtime.");
            StringAssert.Contains("BattleResultController needs BattleResultAlertView.", text);
            StringAssert.Contains("BattleResultController needs BattleProgressController.", text);
        }

        [Test]
        public void BattleProgressController_OwnsSaveAndStageFlow()
        {
            string text = File.ReadAllText("Assets/Script/5_Battle/0_Flow/3_Result/BattleProgressController.cs");

            StringAssert.Contains("BattleStageContext", text);
            StringAssert.Contains("GameSaveController.instance.SaveClearedStage(stageNum + 1)", text);
            StringAssert.Contains("stageFlowController.SendNextStageAfterBattle(stageNum)", text);
            StringAssert.Contains("stageFlowController.SendFight(stageNum)", text);
            StringAssert.Contains("stageFlowController.SendStageSelect()", text);
            Assert.IsFalse(text.Contains("AddComponent<BattleStageContext>()"), "BattleProgressController should not create stage context at runtime.");
            StringAssert.Contains("BattleProgressController needs BattleStageContext.", text);
        }

        [Test]
        public void Player_UsesBattleResultControllerForResults()
        {
            string playerText = File.ReadAllText("Assets/Script/5_Battle/1_Player/PlayerController.cs");
            string turnActionsText = File.ReadAllText("Assets/Script/5_Battle/1_Player/0_Turn/PlayerTurnActions.cs");

            StringAssert.Contains("BattleResultController", playerText);
            StringAssert.Contains("BattleResultController", turnActionsText);
            StringAssert.Contains("battleResultController.ClearResultTry(", turnActionsText);
            StringAssert.Contains("battleResultController.FailResultTry(", turnActionsText);
            Assert.IsFalse(playerText.Contains("BattleResultAlertView battleResultAlertView"), "Player should not call the alert view directly.");
            Assert.IsFalse(turnActionsText.Contains("BattleResultAlertView battleResultAlertView"), "Player turn action should not call the alert view directly.");
            Assert.IsFalse(turnActionsText.Contains("ShowClearResultAlert(stageNum)"), "Player turn action should not pass stage numbers to the alert view.");
            Assert.IsFalse(turnActionsText.Contains("ShowFailResultAlert(stageNum)"), "Player turn action should not pass stage numbers to the alert view.");
        }
    }
}
