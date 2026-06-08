using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class BattleResultFlowStaticRegressionTests
    {
        [Test]
        public void TurnController_DoesNotReferenceStageController()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/TurnController.cs");

            Assert.IsFalse(text.Contains("StageController"), "TurnController should not depend on StageController in fight scenes.");
            StringAssert.Contains("BattleResultController", text);
            StringAssert.Contains("battleResultController.ClearResultTry(", text);
        }

        [Test]
        public void BattleResultAlertView_DoesNotSaveOrMoveScenes()
        {
            string text = File.ReadAllText("Assets/Script/Battle/View/BattleResultAlertView.cs");

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
            string text = File.ReadAllText("Assets/Script/Battle/Flow/BattleResultController.cs");

            StringAssert.Contains("BattleResultAlertView", text);
            StringAssert.Contains("BattleProgressController", text);
            StringAssert.Contains("ShowClearResultAlert(progressController.CompleteClearResult)", text);
            StringAssert.Contains("ShowFailResultAlert(progressController.CompleteFailResult)", text);
            StringAssert.Contains("resultStarted", text);
        }

        [Test]
        public void BattleProgressController_OwnsSaveAndStageFlow()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Flow/BattleProgressController.cs");

            StringAssert.Contains("BattleStageContext", text);
            StringAssert.Contains("GameSaveController.instance.SaveClearedStage(stageNum + 1)", text);
            StringAssert.Contains("stageFlowController.SendNextStageAfterBattle(stageNum)", text);
            StringAssert.Contains("stageFlowController.SendFight(stageNum)", text);
            StringAssert.Contains("stageFlowController.SendStageSelect()", text);
        }

        [Test]
        public void Player_UsesBattleResultControllerForResults()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Unit/Player.cs");

            StringAssert.Contains("BattleResultController", text);
            StringAssert.Contains("battleResultController.ClearResultTry(", text);
            StringAssert.Contains("battleResultController.FailResultTry(", text);
            Assert.IsFalse(text.Contains("BattleResultAlertView battleResultAlertView"), "Player should not call the alert view directly.");
            Assert.IsFalse(text.Contains("ShowClearResultAlert(stageNum)"), "Player should not pass stage numbers to the alert view.");
            Assert.IsFalse(text.Contains("ShowFailResultAlert(stageNum)"), "Player should not pass stage numbers to the alert view.");
        }
    }
}
