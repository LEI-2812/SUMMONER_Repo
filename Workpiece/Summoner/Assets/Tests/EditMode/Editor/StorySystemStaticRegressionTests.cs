using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class StorySystemStaticRegressionTests
    {
        private const string ScriptRoot = "Assets/Script";
        private const string ScreenRoot = "Assets/Screen";
        private const string FadePanelViewPath = "Assets/Script/Story/View/FadePanelView.cs";
        private const string FadePanelViewMetaPath = "Assets/Script/Story/View/FadePanelView.cs.meta";
        private const string MainSceneButtonViewPath = "Assets/Script/Story/View/MainSceneButtonView.cs";
        private const string MainSceneButtonViewMetaPath = "Assets/Script/Story/View/MainSceneButtonView.cs.meta";
        private const string StorySkipViewPath = "Assets/Script/Story/View/StorySkipView.cs";
        private const string StorySkipViewMetaPath = "Assets/Script/Story/View/StorySkipView.cs.meta";
        private const string InteractionControllerPath = "Assets/Script/Story/Dialogue/InteractionController.cs";
        private const string DialogueParserPath = "Assets/Script/Story/Dialogue/DialogueParser.cs";
        private const string DialogueCsvParsePath = "Assets/Script/Story/Dialogue/DialogueCsvParse.cs";
        private const string DialogueDatabaseLoadPath = "Assets/Script/Story/Dialogue/DialogueDatabaseLoad.cs";
        private const string StoryScenarioControllerBasePath = "Assets/Script/Story/Scenario/StoryScenarioControllerBase.cs";
        private const string Stage1ControllerPath = "Assets/Script/Story/Scenario/Stage1_Controller.cs";
        private const string Stage2ControllerPath = "Assets/Script/Story/Scenario/Stage2_Controller.cs";
        private const string Stage3ControllerPath = "Assets/Script/Story/Scenario/Stage3_Controller.cs";
        private const string Stage5ControllerPath = "Assets/Script/Story/Scenario/Stage5_Controller.cs";
        private const string Stage7ControllerPath = "Assets/Script/Story/Scenario/Stage7_Controller.cs";
        private const string FadePanelViewGuid = "d1c8ed078a786a44298790ea5ccdf7b0";
        private const string MainSceneButtonViewGuid = "69dbc41dfe9e18347834b153125c4571";
        private const string StorySkipViewGuid = "fce73e9dab229134d95d4cd0cddb67ab";

        [Test]
        public void StoryViewOldTypeNames_DoNotReturnToScripts()
        {
            string[] oldTypeNames =
            {
                "class StoryChangeImage",
                "class FadeController",
                "class ClicktoMain",
                "class Story : MonoBehaviour"
            };

            string[] matches = Directory.GetFiles(ScriptRoot, "*.cs", SearchOption.AllDirectories)
                .Where(path => oldTypeNames.Any(oldTypeName => File.ReadAllText(path).Contains(oldTypeName)))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsEmpty(matches, "Story View files were renamed. Do not reintroduce old View type names:\n" + string.Join("\n", matches));
        }

        [Test]
        public void FadePanelView_KeepsOriginalSceneGuid()
        {
            string metaText = File.ReadAllText(FadePanelViewMetaPath);

            StringAssert.Contains(
                "guid: " + FadePanelViewGuid,
                metaText,
                "FadePanelView must keep the original FadeController guid so scene component links remain assigned.");
        }

        [Test]
        public void FadePanelView_GuidIsStillUsedByStoryScenes()
        {
            string[] sceneMatches = Directory.GetFiles(ScreenRoot, "*.unity", SearchOption.AllDirectories)
                .Where(path => File.ReadAllText(path).Contains(FadePanelViewGuid))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsNotEmpty(sceneMatches, "Story scenes should still reference the FadePanelView script guid.");
        }

        [Test]
        public void MainSceneButtonView_KeepsOriginalThankSceneGuid()
        {
            string metaText = File.ReadAllText(MainSceneButtonViewMetaPath);

            StringAssert.Contains(
                "guid: " + MainSceneButtonViewGuid,
                metaText,
                "MainSceneButtonView must keep the original ClicktoMain guid so Thank Screen component links remain assigned.");

            string[] sceneMatches = Directory.GetFiles(ScreenRoot, "*.unity", SearchOption.AllDirectories)
                .Where(path => File.ReadAllText(path).Contains(MainSceneButtonViewGuid))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsNotEmpty(sceneMatches, "Thank Screen should still reference the MainSceneButtonView script guid.");
        }

        [Test]
        public void StorySkipView_KeepsOriginalStorySceneGuid()
        {
            string metaText = File.ReadAllText(StorySkipViewMetaPath);

            StringAssert.Contains(
                "guid: " + StorySkipViewGuid,
                metaText,
                "StorySkipView must keep the original Story guid so Story Scene component links remain assigned.");

            string[] sceneMatches = Directory.GetFiles(ScreenRoot, "*.unity", SearchOption.AllDirectories)
                .Where(path => File.ReadAllText(path).Contains(StorySkipViewGuid))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsNotEmpty(sceneMatches, "Story scenes should still reference the StorySkipView script guid.");
        }

        [Test]
        public void StorySkipView_UnityEventsUseCurrentTypeName()
        {
            string[] oldTypeNameMatches = Directory.GetFiles(ScreenRoot, "*.unity", SearchOption.AllDirectories)
                .Where(path => File.ReadAllText(path).Contains("m_TargetAssemblyTypeName: Story, Assembly-CSharp"))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsEmpty(
                oldTypeNameMatches,
                "StorySkipView UnityEvent type names must use the current class name:\n" + string.Join("\n", oldTypeNameMatches));
        }

        [Test]
        public void InteractionController_PreservesFadeControllerSerializedField()
        {
            string text = File.ReadAllText(InteractionControllerPath);

            StringAssert.Contains(
                "[FormerlySerializedAs(\"fadeController\")]",
                text,
                "InteractionController renamed fadeController to fadePanelView and must preserve existing scene serialization.");
            StringAssert.Contains("private FadePanelView fadePanelView;", text);
            StringAssert.Contains("fadePanelView.RegisterCallback", text);
            StringAssert.Contains("fadePanelView.FadeOut", text);
        }

        [Test]
        public void FadePanelView_ClassNameMatchesFileName()
        {
            string text = File.ReadAllText(FadePanelViewPath);

            Assert.IsTrue(
                Regex.IsMatch(text, @"\bpublic\s+class\s+FadePanelView\s*:\s*MonoBehaviour\b"),
                "FadePanelView.cs must declare public class FadePanelView : MonoBehaviour.");
        }

        [Test]
        public void MainSceneButtonView_ClassNameMatchesFileName()
        {
            string text = File.ReadAllText(MainSceneButtonViewPath);

            Assert.IsTrue(
                Regex.IsMatch(text, @"\bpublic\s+class\s+MainSceneButtonView\s*:\s*MonoBehaviour\b"),
                "MainSceneButtonView.cs must declare public class MainSceneButtonView : MonoBehaviour.");
        }

        [Test]
        public void StorySkipView_ClassNameMatchesFileName()
        {
            string text = File.ReadAllText(StorySkipViewPath);

            Assert.IsTrue(
                Regex.IsMatch(text, @"\bpublic\s+class\s+StorySkipView\s*:\s*MonoBehaviour\b"),
                "StorySkipView.cs must declare public class StorySkipView : MonoBehaviour.");
        }

        [Test]
        public void StorySceneMove_ReturnsExistingSceneNames()
        {
            string text = File.ReadAllText("Assets/Script/Story/Progress/StorySceneMove.cs");

            StringAssert.Contains("case 0:", text);
            StringAssert.Contains("return \"Stage Select Screen\";", text);
            StringAssert.Contains("case 8:", text);
            StringAssert.Contains("return \"Thank Screen\";", text);
            StringAssert.Contains("return \"Fight Screen_\" + storyNumber + \"Stage\";", text);
        }

        [Test]
        public void InteractionController_DelegatesStoryEndSceneMove()
        {
            string text = File.ReadAllText(InteractionControllerPath);

            StringAssert.Contains("StorySceneMove.LoadNextScene(storyStage.getStoryNum())", text);
            Assert.IsFalse(
                text.Contains("Fight Screen_\"+storyStage.getStoryNum()+\"Stage"),
                "InteractionController should delegate next scene name decisions to StorySceneMove.");
        }

        [Test]
        public void DialogueLineShow_BuildsExistingCombinedLineFormat()
        {
            string text = File.ReadAllText("Assets/Script/Story/View/DialogueLineShow.cs");

            StringAssert.Contains("if (i % 2 == 0)", text);
            StringAssert.Contains("combinedDialogue = \"\";", text);
            StringAssert.Contains("combinedDialogue += dialogue.context[i];", text);
            StringAssert.Contains("combinedDialogue += \"\\n\";", text);
        }

        [Test]
        public void InteractionController_DelegatesDialogueTextOutput()
        {
            string text = File.ReadAllText(InteractionControllerPath);

            StringAssert.Contains("DialogueLineShow.Show(characterName, dialogueContext, currentDialogue, dialogueLineIndex)", text);
        }

        [Test]
        public void StoryProgressAdvance_ReturnsDialogueLinesInExistingOrder()
        {
            string text = File.ReadAllText("Assets/Script/Story/Progress/StoryProgressAdvance.cs");

            StringAssert.Contains("public bool TryGetNextLine(out Dialogue dialogue, out int dialogueLineIndex)", text);
            StringAssert.Contains("CurrentDialogueLineIndex >= dialogues[CurrentDialogueIndex].context.Length", text);
            StringAssert.Contains("CurrentDialogueLineIndex = 0;", text);
            StringAssert.Contains("CurrentDialogueIndex++;", text);
            StringAssert.Contains("dialogue = dialogues[CurrentDialogueIndex];", text);
            StringAssert.Contains("dialogueLineIndex = CurrentDialogueLineIndex;", text);
            StringAssert.Contains("CurrentDialogueLineIndex++;", text);
        }

        [Test]
        public void InteractionController_DelegatesDialogueProgress()
        {
            string text = File.ReadAllText(InteractionControllerPath);

            StringAssert.Contains("private readonly StoryProgressAdvance storyProgressAdvance", text);
            StringAssert.Contains("storyProgressAdvance.Start(currentDialogues)", text);
            StringAssert.Contains("storyProgressAdvance.TryGetNextLine", text);
        }

        [Test]
        public void DialogueParser_DelegatesLoadAndCsvParse()
        {
            string text = File.ReadAllText(DialogueParserPath);

            StringAssert.Contains("DialogueDatabaseLoad.LoadText(CSV_FileName)", text);
            StringAssert.Contains("DialogueCsvParse.Parse", text);
            Assert.IsFalse(text.Contains("Resources.Load<TextAsset>"), "DialogueParser should delegate CSV loading to DialogueDatabaseLoad.");
            Assert.IsFalse(text.Contains("Regex.Replace"), "DialogueParser should delegate CSV parsing to DialogueCsvParse.");
        }

        [Test]
        public void DialogueCsvParse_KeepsExistingRowParsingRules()
        {
            string text = File.ReadAllText(DialogueCsvParsePath);

            StringAssert.Contains("csvText.Split(new char[] { '\\n' })", text);
            StringAssert.Contains("data[i].Split(new char[] { ',' }, 3)", text);
            StringAssert.Contains("Regex.Replace(context.Trim(), \"^\\\\s*\\\"|\\\"\\\\s*$\", \"\")", text);
            StringAssert.Contains("currentDialogue.context = contextList.ToArray()", text);
        }

        [Test]
        public void DialogueDatabaseLoad_UsesResourcesTextAsset()
        {
            string text = File.ReadAllText(DialogueDatabaseLoadPath);

            StringAssert.Contains("Resources.Load<TextAsset>(csvFileName)", text);
            StringAssert.Contains("return csvData.text;", text);
        }

        [Test]
        public void StoryScenarioControllerBase_OwnsCommonInputFlow()
        {
            string text = File.ReadAllText(StoryScenarioControllerBasePath);

            StringAssert.Contains("public abstract class StoryScenarioControllerBase : MonoBehaviour, ScenarioBase, IPointerClickHandler", text);
            StringAssert.Contains("if (Input.GetKeyDown(KeyCode.Space) && !IsOnlyMouseEnabled())", text);
            StringAssert.Contains("PlayerPrefs.GetInt(\"IsOnlyMouse\", 0) == 1", text);
            StringAssert.Contains("public void OnPointerClick(PointerEventData eventData)", text);
            StringAssert.Contains("public void OnClickDialogue()", text);
            StringAssert.Contains("interactionController.ShowNextLine();", text);
            StringAssert.Contains("PlayScenarioStep(scenarioFlowCount);", text);
        }

        [Test]
        public void Stage1Controller_DelegatesCommonInputFlowToBase()
        {
            string text = File.ReadAllText(Stage1ControllerPath);

            StringAssert.Contains("public class Stage1_Controller : StoryScenarioControllerBase", text);
            StringAssert.Contains("protected override void PlayScenarioStep(int scenarioStep)", text);
            Assert.IsFalse(text.Contains("Input.GetKeyDown(KeyCode.Space)"), "Stage1 input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("public void OnPointerClick"), "Stage1 pointer input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("private bool checkCSVDialogueID"), "Stage1 duplicate dialogue checks should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("scenarioFlowCount"), "Stage1 should use the scenarioStep parameter from StoryScenarioControllerBase.");
        }

        [Test]
        public void Stage2Controller_DelegatesCommonInputFlowToBase()
        {
            string text = File.ReadAllText(Stage2ControllerPath);

            StringAssert.Contains("public class Stage2_Controller : StoryScenarioControllerBase", text);
            StringAssert.Contains("protected override void PlayScenarioStep(int scenarioStep)", text);
            Assert.IsFalse(text.Contains("Input.GetKeyDown(KeyCode.Space)"), "Stage2 input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("public void OnPointerClick"), "Stage2 pointer input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("private bool checkCSVDialogueID"), "Stage2 duplicate dialogue checks should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("scenarioFlowCount"), "Stage2 should use the scenarioStep parameter from StoryScenarioControllerBase.");
        }

        [Test]
        public void Stage3Controller_DelegatesCommonInputFlowToBase()
        {
            string text = File.ReadAllText(Stage3ControllerPath);

            StringAssert.Contains("public class Stage3_Controller : StoryScenarioControllerBase", text);
            StringAssert.Contains("protected override void PlayScenarioStep(int scenarioStep)", text);
            Assert.IsFalse(text.Contains("Input.GetKeyDown(KeyCode.Space)"), "Stage3 input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("public void OnPointerClick"), "Stage3 pointer input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("private bool checkCSVDialogueID"), "Stage3 duplicate dialogue checks should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("scenarioFlowCount"), "Stage3 should use the scenarioStep parameter from StoryScenarioControllerBase.");
        }

        [Test]
        public void Stage5Controller_DelegatesCommonInputFlowToBase()
        {
            string text = File.ReadAllText(Stage5ControllerPath);

            StringAssert.Contains("public class Stage5_Controller : StoryScenarioControllerBase", text);
            StringAssert.Contains("protected override void PlayScenarioStep(int scenarioStep)", text);
            Assert.IsFalse(text.Contains("Input.GetKeyDown(KeyCode.Space)"), "Stage5 input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("public void OnPointerClick"), "Stage5 pointer input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("private bool checkCSVDialogueID"), "Stage5 duplicate dialogue checks should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("scenarioFlowCount"), "Stage5 should use the scenarioStep parameter from StoryScenarioControllerBase.");
        }

        [Test]
        public void Stage7Controller_DelegatesCommonInputFlowToBase()
        {
            string text = File.ReadAllText(Stage7ControllerPath);

            StringAssert.Contains("public class Stage7_Controller : StoryScenarioControllerBase", text);
            StringAssert.Contains("protected override void PlayScenarioStep(int scenarioStep)", text);
            Assert.IsFalse(text.Contains("Input.GetKeyDown(KeyCode.Space)"), "Stage7 input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("public void OnPointerClick"), "Stage7 pointer input should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("private bool checkCSVDialogueID"), "Stage7 duplicate dialogue checks should be handled by StoryScenarioControllerBase.");
            Assert.IsFalse(text.Contains("scenarioFlowCount"), "Stage7 should use the scenarioStep parameter from StoryScenarioControllerBase.");
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }
    }
}
