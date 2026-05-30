using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Summoner.PlayModeTests
{
    public class GameSaveContinueTests
    {
        private const string StartSceneName = "Start Screen";
        private const string SavedStageKey = "savedStage";
        private const string PlayingStageKey = "playingStage";

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayerPrefs.DeleteKey(SavedStageKey);
            PlayerPrefs.DeleteKey(PlayingStageKey);
            PlayerPrefs.Save();
            ResetGameSaveControllerSingleton();
            yield return null;
        }

        [UnityTest]
        public IEnumerator ContinueButton_Hides_WhenSaveMissing()
        {
            ResetSaveData();

            yield return LoadStartScene();

            Button loadButton = GetLoadButton();

            Assert.IsFalse(loadButton.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator ContinueButton_Shows_WhenSavedStageExists()
        {
            ResetSaveData();
            SaveStage(3);

            yield return LoadStartScene();

            Button loadButton = GetLoadButton();

            Assert.IsTrue(loadButton.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator GameSaveController_LoadsSavedStage_WhenSavedStageExists()
        {
            ResetSaveData();
            SaveStage(3);

            yield return LoadStartScene();

            object gameSaveController = GetGameSaveController();
            object saveData = Invoke(gameSaveController, "GetGameSave");
            int savedStage = GetIntField(saveData, "savedStage");

            Assert.AreEqual(3, savedStage);
        }

        [UnityTest]
        public IEnumerator GameSaveController_DoesNotTreatZeroStageAsSave()
        {
            ResetSaveData();
            PlayerPrefs.SetInt(SavedStageKey, 0);
            PlayerPrefs.Save();

            yield return LoadStartScene();

            object gameSaveController = GetGameSaveController();
            bool hasGameSave = (bool)Invoke(gameSaveController, "HasGameSave");

            Assert.IsFalse(hasGameSave);
        }

        private static IEnumerator LoadStartScene()
        {
            ResetGameSaveControllerSingleton();
            SceneManager.LoadScene(StartSceneName);
            yield return null;
            yield return null;
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

        private static Button GetLoadButton()
        {
            MonoBehaviour startScreenEvent = FindMonoBehaviour("StartScreenView");
            FieldInfo loadButtonField = startScreenEvent.GetType().GetField("loadButton", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(loadButtonField, "StartScreenView.loadButton field was not found.");

            Button loadButton = loadButtonField.GetValue(startScreenEvent) as Button;
            Assert.IsNotNull(loadButton, "StartScreenView.loadButton is not assigned.");
            return loadButton;
        }

        private static object GetGameSaveController()
        {
            Type controllerType = FindType("GameSaveController");
            FieldInfo instanceField = controllerType.GetField("instance", BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(instanceField, "GameSaveController.instance field was not found.");

            object instance = instanceField.GetValue(null);
            Assert.IsNotNull(instance, "GameSaveController.instance is null.");
            return instance;
        }

        private static MonoBehaviour FindMonoBehaviour(string typeName)
        {
            Type type = FindType(typeName);
            MonoBehaviour component = Object.FindObjectOfType(type) as MonoBehaviour;
            Assert.IsNotNull(component, typeName + " component was not found in the scene.");
            return component;
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

        private static object Invoke(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(method, target.GetType().Name + "." + methodName + " method was not found.");
            return method.Invoke(target, null);
        }

        private static int GetIntField(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(field, target.GetType().Name + "." + fieldName + " field was not found.");
            return (int)field.GetValue(target);
        }

        private static void ResetGameSaveControllerSingleton()
        {
            Type controllerType = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType("GameSaveController"))
                .FirstOrDefault(foundType => foundType != null);

            if (controllerType == null)
            {
                return;
            }

            FieldInfo instanceField = controllerType.GetField("instance", BindingFlags.Static | BindingFlags.Public);
            Component instance = instanceField?.GetValue(null) as Component;

            if (instance != null)
            {
                Object.DestroyImmediate(instance.gameObject);
            }

            instanceField?.SetValue(null, null);
        }
    }
}
