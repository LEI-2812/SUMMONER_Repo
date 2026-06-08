using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
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
        public IEnumerator StageSelectScreen_LoadsRequiredStageControllers()
        {
            ResetSaveData();
            SaveStage(3);

            yield return LoadScene(StartSceneName);

            MonoBehaviour stageFlowController = FindMonoBehaviour("StageFlowController");
            Invoke(stageFlowController, "SendStageSelect");
            yield return WaitUntilSceneLoaded(StageSelectSceneName);

            Assert.IsNotNull(FindMonoBehaviour("StageSelectView"), "StageSelectView must exist.");
            Assert.IsNotNull(GetSingletonInstance("GameSaveController", "instance"), "GameSaveController must stay alive after scene load.");
            Assert.IsNotNull(GetSingletonInstance("StageFlowController", "instance"), "StageFlowController must stay alive after scene load.");
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

            Assert.IsNotNull(FindMonoBehaviour("BattleResultAlertView"), "BattleResultAlertView must exist.");
            Assert.IsNotNull(GetSingletonInstance("GameSaveController", "instance"), "GameSaveController must stay alive after scene load.");
            Assert.IsNotNull(GetSingletonInstance("StageFlowController", "instance"), "StageFlowController must stay alive after scene load.");
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

            MonoBehaviour player = FindMonoBehaviour("Player");
            MonoBehaviour turnController = FindMonoBehaviour("TurnController");

            AssertTurnState(turnController, "PlayerTurn", 1);

            int clearTurn = (int)Invoke(turnController, "GetClearTurn");
            int lastExpectedTurnCount = Mathf.Min(4, clearTurn);
            Assert.GreaterOrEqual(lastExpectedTurnCount, 3, "Fight Screen_1Stage must allow at least two turn exchanges before fail.");

            for (int expectedTurnCount = 2; expectedTurnCount <= lastExpectedTurnCount; expectedTurnCount++)
            {
                LogAssert.Expect(LogType.Log, "플레이어 턴 종료");
                LogAssert.Expect(LogType.Log, "적 턴 시작");
                LogAssert.Expect(LogType.Log, "리스트를 가져와서 적 대응시작");
                LogAssert.Expect(LogType.Log, "적 턴 종료");
                LogAssert.Expect(LogType.Log, "현재 턴: " + expectedTurnCount);
                LogAssert.Expect(LogType.Log, "플레이어 턴 시작");

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

        private static Button[] GetButtonArray(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, target.GetType().Name + "." + fieldName + " field was not found.");

            Button[] buttons = fieldInfo.GetValue(target) as Button[];
            Assert.IsNotNull(buttons, target.GetType().Name + "." + fieldName + " must be a Button array.");
            return buttons;
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
