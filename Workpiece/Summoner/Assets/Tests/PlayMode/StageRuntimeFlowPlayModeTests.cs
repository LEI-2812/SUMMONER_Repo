using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Summoner.PlayModeTests
{
    public class StageRuntimeFlowPlayModeTests
    {
        private const string StartSceneName = "Start Screen";
        private const string StageSelectSceneName = "Stage Select Screen";
        private const string SavedStageKey = "savedStage";
        private const string PlayingStageKey = "playingStage";

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayerPrefs.DeleteKey(SavedStageKey);
            PlayerPrefs.DeleteKey(PlayingStageKey);
            PlayerPrefs.Save();
            ResetSingleton("GameSaveController", "instance");
            ResetSingleton("StageFlowController", "instance");
            yield return null;
        }

        [UnityTest]
        public IEnumerator StartScreen_LoadsSaveAndStageFlowControllers()
        {
            ResetSaveData();

            yield return LoadScene(StartSceneName);

            Assert.IsNotNull(FindMonoBehaviour("StartScreenView"), "StartScreenView must exist.");
            Assert.IsNotNull(FindMonoBehaviour("GameSaveController"), "GameSaveController must exist.");
            Assert.IsNotNull(FindMonoBehaviour("StageFlowController"), "StageFlowController must exist.");
        }

        [UnityTest]
        public IEnumerator StartScreen_SettingButton_OpensHudSettingPanel()
        {
            ResetSaveData();

            yield return LoadScene(StartSceneName);
            yield return WaitUntilSceneLoadedAdditive("HUD");

            MonoBehaviour startScreenView = FindMonoBehaviour("StartScreenView");
            Button settingButton = GetField<Button>(startScreenView, "settingBtn");
            Assert.IsNotNull(settingButton, "StartScreenView.settingBtn must be assigned.");

            settingButton.onClick.Invoke();
            yield return null;

            MonoBehaviour settingPanelView = FindMonoBehaviour("SettingPanelView");
            GameObject settingPanel = GetField<GameObject>(settingPanelView, "settingPanel");
            Assert.IsTrue(settingPanel.activeSelf, "Start screen setting button must open the HUD setting panel.");
        }

        [UnityTest]
        public IEnumerator PrologueMenuView_DoesNotPersistIntoStoryScene()
        {
            ResetSaveData();

            yield return LoadScene("Prologue Screen");
            Assert.IsNotNull(FindMonoBehaviour("MenuView"), "Prologue Screen must provide its scene menu view.");

            SceneManager.LoadScene("Story Screen_1Stage");
            yield return WaitUntilSceneLoaded("Story Screen_1Stage");
            yield return WaitUntilSceneLoadedAdditive("HUD");
            yield return null;

            Assert.AreEqual(0, FindMonoBehaviours("MenuView").Length, "Story scenes must not keep the old MenuView ESC handler from Prologue.");
            Assert.IsNotNull(FindMonoBehaviour("MenuHandler"), "Story scenes must use HUD MenuHandler for ESC menu input.");
        }

        [UnityTest]
        public IEnumerator StageSelectScreen_LoadsRequiredStageControllers()
        {
            ResetSaveData();
            SaveStage(3);

            yield return LoadScene(StartSceneName);
            yield return WaitUntilSceneLoadedAdditive("HUD");
            AssertNoMissingScriptsInLoadedObjects();

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendStageSelect");
            yield return WaitUntilSceneLoaded(StageSelectSceneName);
            AssertNoMissingScriptsInLoadedObjects();

            Assert.IsNotNull(FindMonoBehaviour("StageSelectView"), "StageSelectView must exist.");
            Assert.IsNotNull(GetSingletonInstance("GameSaveController", "instance"), "GameSaveController must stay alive after scene load.");
            Assert.IsNotNull(GetSingletonInstance("StageFlowController", "instance"), "StageFlowController must stay alive after scene load.");
        }

        [UnityTest]
        public IEnumerator StartGameFlow_ContinueSavedGame_LoadsStageSelectWithoutStageController()
        {
            ResetSaveData();
            SaveStage(3);

            yield return LoadScene(StartSceneName);

            Assert.AreEqual(true, InvokeStatic("StartGameFlow", "TryContinueSavedGame"));

            yield return WaitUntilSceneLoaded(StageSelectSceneName);

            Assert.IsNotNull(FindMonoBehaviour("StageController"), "Stage Select Screen must provide StageController after continue.");
        }

        [UnityTest]
        public IEnumerator StartGameFlow_NewGame_LoadsPrologueWithoutStageController()
        {
            ResetSaveData();
            SaveStage(5);

            yield return LoadScene(StartSceneName);

            Assert.AreEqual(true, InvokeStatic("StartGameFlow", "TryStartNewGame"));

            yield return WaitUntilSceneLoaded("Prologue Screen");

            object gameSaveController = GetSingletonInstance("GameSaveController", "instance");
            object saveData = Invoke(gameSaveController, "GetGameSave");
            Assert.AreEqual(1, GetIntField(saveData, "savedStage"));
            Assert.AreEqual(1, GetIntField(saveData, "playingStage"));
        }

        [UnityTest]
        public IEnumerator StageSelectView_StageLoader_SavesPlayingStage()
        {
            ResetSaveData();
            SaveStage(3);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendStageSelect");
            yield return WaitUntilSceneLoaded(StageSelectSceneName);

            MonoBehaviour stageSelectView = FindMonoBehaviour("StageSelectView");
            Invoke(stageSelectView, "StageLoader", 2);
            yield return null;

            object gameSaveController = GetSingletonInstance("GameSaveController", "instance");
            object saveData = Invoke(gameSaveController, "GetGameSave");

            Assert.AreEqual(2, GetIntField(saveData, "playingStage"));
        }

        [UnityTest]
        public IEnumerator StageFlowController_SendStageSelect_LoadsStageSelectScene()
        {
            ResetSaveData();

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendStageSelect");

            yield return WaitUntilSceneLoaded(StageSelectSceneName);

            Assert.AreEqual(StageSelectSceneName, SceneManager.GetActiveScene().name);
        }

        [UnityTest]
        public IEnumerator GameSaveController_SaveClearedStage_UpdatesSavedAndPlayingStage()
        {
            ResetSaveData();

            yield return LoadScene(StartSceneName);

            object gameSaveController = GetSingletonInstance("GameSaveController", "instance");
            Invoke(gameSaveController, "StartNewGame");
            Invoke(gameSaveController, "SaveClearedStage", 4);

            object saveData = Invoke(gameSaveController, "GetGameSave");

            Assert.AreEqual(4, GetIntField(saveData, "savedStage"));
            Assert.AreEqual(4, GetIntField(saveData, "playingStage"));
        }

        [UnityTest]
        public IEnumerator BattleResultAlertView_FightScene_HasRequiredRuntimeControllers()
        {
            ResetSaveData();
            SaveStage(1);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 1);
            yield return WaitUntilSceneLoaded("Fight Screen_1Stage");
            AssertNoMissingScriptsInLoadedObjects();

            Assert.IsNotNull(FindMonoBehaviour("BattleResultAlertView"), "BattleResultAlertView must exist.");
            Assert.IsNotNull(GetSingletonInstance("GameSaveController", "instance"), "GameSaveController must stay alive after scene load.");
            Assert.IsNotNull(GetSingletonInstance("StageFlowController", "instance"), "StageFlowController must stay alive after scene load.");
        }

        [UnityTest]
        public IEnumerator FightScene_ResultAlerts_StartHidden()
        {
            ResetSaveData();
            SaveStage(1);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 1);
            yield return WaitUntilSceneLoaded("Fight Screen_1Stage");
            yield return null;

            MonoBehaviour alertView = FindMonoBehaviour("BattleResultAlertView");
            GameObject clearAlert = GetField<GameObject>(alertView, "alertClear");
            GameObject failAlert = GetField<GameObject>(alertView, "alertFail");

            Assert.IsFalse(clearAlert.activeSelf, "Clear result alert must not block the fight scene at startup.");
            Assert.IsFalse(failAlert.activeSelf, "Fail result alert must not block the fight scene at startup.");
        }

        [UnityTest]
        public IEnumerator FightScene_NormalSummon_ShowsThreeOptionPanels()
        {
            ResetSaveData();
            SaveStage(1);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 1);
            yield return WaitUntilSceneLoaded("Fight Screen_1Stage");
            yield return null;

            MonoBehaviour summonController = FindMonoBehaviour("SummonController");
            Invoke(summonController, "StartSummon", 0, false);
            yield return null;

            GameObject redrawPanel = GetField<GameObject>(summonController, "redrawPanel");
            IList redrawOptionPanels = GetField<IList>(summonController, "redrawOptionPanels");

            Assert.IsTrue(redrawPanel.activeSelf, "Normal summon must show the shared three-option panel.");
            AssertThreeOptionPanel(redrawPanel, redrawOptionPanels, "Normal summon");
        }

        [UnityTest]
        public IEnumerator FightScene_RedrawSummon_ShowsThreeOptionPanels()
        {
            ResetSaveData();
            SaveStage(1);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 1);
            yield return WaitUntilSceneLoaded("Fight Screen_1Stage");
            yield return null;

            MonoBehaviour summonController = FindMonoBehaviour("SummonController");
            Invoke(summonController, "StartSummon", 0, true);
            yield return null;

            GameObject redrawPanel = GetField<GameObject>(summonController, "redrawPanel");
            IList redrawOptionPanels = GetField<IList>(summonController, "redrawOptionPanels");

            Assert.IsTrue(redrawPanel.activeSelf, "Redraw summon must show the three-option panel.");
            AssertThreeOptionPanel(redrawPanel, redrawOptionPanels, "Redraw summon");
        }

        [UnityTest]
        public IEnumerator FightScene_StageTwo_NormalSummonShowsVisibleOptionPanels()
        {
            ResetSaveData();
            SaveStage(2);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 2);
            yield return WaitUntilSceneLoaded("Fight Screen_2Stage");
            yield return null;

            AssertNoMissingScriptsInLoadedObjects();

            GameObject summonPicker = GameObject.Find("UI_60_SummonPicker");
            Assert.IsNotNull(summonPicker, "Fight Screen_2Stage must have a visible summon picker root.");
            Assert.AreEqual(Vector3.one, summonPicker.transform.localScale, "Fight Screen_2Stage summon picker root must not be hidden by zero scale.");

            MonoBehaviour summonController = FindMonoBehaviour("SummonController");
            Invoke(summonController, "StartSummon", 0, false);
            yield return null;

            GameObject redrawPanel = GetField<GameObject>(summonController, "redrawPanel");
            IList redrawOptionPanels = GetField<IList>(summonController, "redrawOptionPanels");

            Assert.IsTrue(redrawPanel.activeSelf, "Stage 2 normal summon must show the three-option panel.");
            AssertThreeOptionPanel(redrawPanel, redrawOptionPanels, "Stage 2 normal summon");
        }

        [UnityTest]
        public IEnumerator FightScene_StageTwo_PlayerAndEnemyTurnsCanExchange()
        {
            ResetSaveData();
            SaveStage(2);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 2);
            yield return WaitUntilSceneLoaded("Fight Screen_2Stage");
            yield return null;

            MonoBehaviour player = FindMonoBehaviour("PlayerController");
            MonoBehaviour turnController = FindMonoBehaviour("TurnController");

            AssertTurnState(turnController, "PlayerTurn", 1);

            Invoke(player, "PlayerTurnOverBtn");
            yield return null;

            AssertTurnState(turnController, "PlayerTurn", 2);
        }

        [UnityTest]
        public IEnumerator FightScene_StageFive_AppliesCurrentMultiplierToPlacedEnemies()
        {
            ResetSaveData();
            SaveStage(5);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFightStage", 5);
            yield return WaitUntilSceneLoaded("Fight Screen_5Stage");
            yield return null;

            MonoBehaviour plateController = FindMonoBehaviour("PlateController");
            object enemyPlatesObject = Invoke(plateController, "GetEnermyPlates");
            var enemyPlates = ((IEnumerable)enemyPlatesObject).Cast<object>().ToArray();

            object firstEnemySummon = enemyPlates
                .Select(plate => Invoke(plate, "GetCurrentSummon"))
                .FirstOrDefault(summon => summon != null);

            Assert.IsNotNull(firstEnemySummon, "Fight Screen_5Stage must place at least one enemy summon.");
            Assert.AreEqual(1300d, (double)Invoke(firstEnemySummon, "GetMaxHP"), 0.01d, "Stage 5 Skeleton max HP must use the current x2 fight multiplier.");
            Assert.AreEqual(1300d, (double)Invoke(firstEnemySummon, "GetNowHP"), 0.01d, "Stage 5 Skeleton current HP must match scaled max HP.");
            Assert.AreEqual(300d, (double)Invoke(firstEnemySummon, "GetAttackPower"), 0.01d, "Stage 5 Skeleton attack power must use the current x2 fight multiplier.");
            Assert.AreEqual(340d, (double)Invoke(firstEnemySummon, "GetHeavyAttackPower"), 0.01d, "Stage 5 Skeleton heavy attack power must use the current x2 fight multiplier.");
        }

        [UnityTest]
        public IEnumerator FightScene_PlayerAndEnemyTurns_ExchangeTwoOrThreeTimes()
        {
            ResetSaveData();
            SaveStage(1);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendFight", 1);
            yield return WaitUntilSceneLoaded("Fight Screen_1Stage");
            yield return null;

            MonoBehaviour player = FindMonoBehaviour("PlayerController");
            MonoBehaviour turnController = FindMonoBehaviour("TurnController");

            AssertTurnState(turnController, "PlayerTurn", 1);

            int clearTurn = (int)Invoke(turnController, "GetClearTurn");
            int lastExpectedTurnCount = Mathf.Min(4, clearTurn);
            Assert.GreaterOrEqual(lastExpectedTurnCount, 3, "Fight Screen_1Stage must allow at least two turn exchanges before fail.");

            for (int expectedTurnCount = 2; expectedTurnCount <= lastExpectedTurnCount; expectedTurnCount++)
            {
                Invoke(player, "PlayerTurnOverBtn");
                yield return null;

                AssertTurnState(turnController, "PlayerTurn", expectedTurnCount);
            }
        }

        [UnityTest]
        public IEnumerator StageSelectScreen_EnablesOnlyUnlockedStageButtons()
        {
            for (int savedStage = 1; savedStage <= 7; savedStage++)
            {
                ResetSaveData();
                SaveStage(savedStage);

                yield return LoadScene(StartSceneName);

                MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
                Invoke(stageFlowController, "SendStageSelect");
                yield return WaitUntilSceneLoaded(StageSelectSceneName);
                yield return null;

                MonoBehaviour stageSelectView = FindMonoBehaviour("StageSelectView");
                Button[] stageButtons = GetButtonArray(stageSelectView, "buttons");

                Assert.GreaterOrEqual(stageButtons.Length, 7, "Stage Select Screen must have at least seven stage buttons.");

                for (int buttonIndex = 0; buttonIndex < 7; buttonIndex++)
                {
                    bool shouldBeUnlocked = buttonIndex < savedStage;
                    Assert.AreEqual(
                        shouldBeUnlocked,
                        stageButtons[buttonIndex].interactable,
                        "savedStage=" + savedStage + " must unlock stages 1 through " + savedStage + " only.");
                }
            }
        }

        [UnityTest]
        public IEnumerator StageFlowController_SendNextStageAfterBattle_LoadsExpectedNextScene()
        {
            ResetSaveData();
            SaveStage(1);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");

            Invoke(stageFlowController, "SendNextStageAfterBattle", 1);
            yield return WaitUntilSceneLoaded("Story Screen_2Stage");

            Invoke(stageFlowController, "SendNextStageAfterBattle", 3);
            yield return WaitUntilSceneLoaded("Fight Screen_4Stage");

            Invoke(stageFlowController, "SendNextStageAfterBattle", 5);
            yield return WaitUntilSceneLoaded("Fight Screen_6Stage");

            Invoke(stageFlowController, "SendNextStageAfterBattle", 7);
            yield return WaitUntilSceneLoaded("Epilogue Screen");
        }

        private static IEnumerator LoadScene(string sceneName)
        {
            ResetSingleton("GameSaveController", "instance");
            ResetSingleton("StageFlowController", "instance");

            SceneManager.LoadScene(sceneName);
            yield return WaitUntilSceneLoaded(sceneName);
            yield return null;
        }

        private static IEnumerator WaitUntilSceneLoaded(string sceneName)
        {
            float timeoutAt = Time.realtimeSinceStartup + 5f;
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                if (Time.realtimeSinceStartup > timeoutAt)
                {
                    Assert.Fail("Scene load timed out: " + sceneName);
                }

                yield return null;
            }
        }

        private static IEnumerator WaitUntilSceneLoadedAdditive(string sceneName)
        {
            float timeoutAt = Time.realtimeSinceStartup + 5f;
            while (!IsSceneLoaded(sceneName))
            {
                if (Time.realtimeSinceStartup > timeoutAt)
                {
                    Assert.Fail("Additive scene load timed out: " + sceneName);
                }

                yield return null;
            }
        }

        private static bool IsSceneLoaded(string sceneName)
        {
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ResetSaveData()
        {
            PlayerPrefs.DeleteKey(SavedStageKey);
            PlayerPrefs.DeleteKey(PlayingStageKey);
            PlayerPrefs.Save();
        }

        private static void SaveStage(int stage)
        {
            PlayerPrefs.SetInt(SavedStageKey, stage);
            PlayerPrefs.SetInt(PlayingStageKey, stage);
            PlayerPrefs.Save();
        }

        private static MonoBehaviour FindMonoBehaviour(string typeName)
        {
            Type type = FindType(typeName);
            return Object.FindObjectOfType(type) as MonoBehaviour;
        }

        private static Object[] FindMonoBehaviours(string typeName)
        {
            Type type = FindType(typeName);
            return Object.FindObjectsOfType(type);
        }

        private static object GetSingletonInstance(string typeName, string fieldName)
        {
            Type type = FindType(typeName);
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(fieldInfo, typeName + "." + fieldName + " field was not found.");

            object instance = fieldInfo.GetValue(null);
            Assert.IsNotNull(instance, typeName + "." + fieldName + " is null.");
            return instance;
        }

        private static Type FindType(string typeName)
        {
            Type type = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType(typeName))
                .FirstOrDefault(foundType => foundType != null);

            Assert.IsNotNull(type, typeName + " type was not found.");
            return type;
        }

        private static object Invoke(object target, string methodName, params object[] parameters)
        {
            Type[] parameterTypes = parameters.Select(parameter => parameter.GetType()).ToArray();
            MethodInfo methodInfo = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, parameterTypes, null);
            Assert.IsNotNull(methodInfo, target.GetType().Name + "." + methodName + " method was not found.");
            return methodInfo.Invoke(target, parameters);
        }

        private static object InvokeStatic(string typeName, string methodName)
        {
            Type type = FindType(typeName);
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null);
            Assert.IsNotNull(methodInfo, typeName + "." + methodName + " method was not found.");
            return methodInfo.Invoke(null, null);
        }

        private static int GetIntField(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(fieldInfo, target.GetType().Name + "." + fieldName + " field was not found.");
            return (int)fieldInfo.GetValue(target);
        }

        private static void AssertTurnState(object turnController, string expectedTurn, int expectedTurnCount)
        {
            Assert.AreEqual(expectedTurn, Invoke(turnController, "GetCurrentTurn").ToString());
            Assert.AreEqual(expectedTurnCount, (int)Invoke(turnController, "GetTurnCount"));
        }

        private static void AssertThreeOptionPanel(GameObject redrawPanel, IList redrawOptionPanels, string owner)
        {
            Assert.GreaterOrEqual(redrawOptionPanels.Count, 3, owner + " must have at least three option panel views available.");

            GridLayoutGroup gridLayout = redrawPanel.GetComponent<GridLayoutGroup>();
            Assert.IsNotNull(gridLayout, owner + " three-option panel must use GridLayoutGroup.");
            Assert.AreEqual(new Vector2(360f, 520f), gridLayout.cellSize, owner + " three-option panel cell size must fit one row.");
            Assert.AreEqual(GridLayoutGroup.Constraint.FixedColumnCount, gridLayout.constraint, owner + " three-option panel must use fixed columns.");
            Assert.AreEqual(3, gridLayout.constraintCount, owner + " three-option panel must keep all options on one row.");
        }

        private static Button[] GetButtonArray(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, target.GetType().Name + "." + fieldName + " field was not found.");

            Button[] buttons = fieldInfo.GetValue(target) as Button[];
            Assert.IsNotNull(buttons, target.GetType().Name + "." + fieldName + " must be a Button array.");
            return buttons;
        }

        private static T GetField<T>(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, target.GetType().Name + "." + fieldName + " field was not found.");

            object value = fieldInfo.GetValue(target);
            Assert.IsInstanceOf<T>(value, target.GetType().Name + "." + fieldName + " must be a " + typeof(T).Name + ".");
            return (T)value;
        }

        private static void AssertNoMissingScriptsInLoadedObjects()
        {
            string[] missingScriptObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(HasMissingMonoBehaviour)
                .Select(GetMissingScriptDescription)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsEmpty(
                missingScriptObjects,
                "Loaded runtime objects must not have missing scripts:\n" + string.Join("\n", missingScriptObjects));
        }

        private static bool HasMissingMonoBehaviour(GameObject gameObject)
        {
            if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject) > 0)
            {
                return true;
            }

            return gameObject.GetComponents<MonoBehaviour>().Any(component => component == null);
        }

        private static string GetMissingScriptDescription(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents<Component>();
            string[] componentNames = components
                .Select((component, index) => index + ":" + (component == null ? "<missing>" : component.GetType().Name))
                .ToArray();

            return GetObjectPath(gameObject) + " [" + string.Join(", ", componentNames) + "]";
        }

        private static string GetObjectPath(GameObject gameObject)
        {
            string path = gameObject.name;
            Transform parent = gameObject.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return gameObject.scene.name + "/" + path;
        }

        private static void ResetSingleton(string typeName, string fieldName)
        {
            Type type = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType(typeName))
                .FirstOrDefault(foundType => foundType != null);

            if (type == null)
            {
                return;
            }

            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
            Component instance = fieldInfo?.GetValue(null) as Component;

            if (instance != null)
            {
                Object.DestroyImmediate(instance.gameObject);
            }

            fieldInfo?.SetValue(null, null);
        }
    }
}
