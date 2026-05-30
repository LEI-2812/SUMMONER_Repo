using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Summoner.PlayModeTests
{
    public class GameSaveProgressReadTests
    {
        private const string SavedStageKey = "savedStage";
        private const string PlayingStageKey = "playingStage";

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayerPrefs.DeleteKey(SavedStageKey);
            PlayerPrefs.DeleteKey(PlayingStageKey);
            PlayerPrefs.Save();
            ResetSingleton("GameSaveController", "instance");
            DestroyObjectByName("Stage Text");
            DestroyObjectByName("StageTextView");
            DestroyObjectByName("StageController");
            yield return null;
        }

        [Test]
        public void GetGameSaveOrDefault_LoadsPlayerPrefs_WhenControllerIsMissing()
        {
            ResetSaveData();
            SaveProgress(4, 2);
            ResetSingleton("GameSaveController", "instance");

            object saveData = InvokeStatic("GameSaveController", "GetGameSaveOrDefault");

            Assert.AreEqual(4, GetIntField(saveData, "savedStage"));
            Assert.AreEqual(2, GetIntField(saveData, "playingStage"));
        }

        [Test]
        public void GetGameSaveOrDefault_UsesControllerInstance_WhenControllerExists()
        {
            ResetSaveData();
            SaveProgress(2, 2);
            MonoBehaviour controller = CreateMonoBehaviour("GameSaveController");

            Invoke(controller, "SavePlayingStage", 5);
            object saveData = InvokeStatic("GameSaveController", "GetGameSaveOrDefault");

            Assert.AreEqual(2, GetIntField(saveData, "savedStage"));
            Assert.AreEqual(5, GetIntField(saveData, "playingStage"));
        }

        [Test]
        public void StageTextView_DisplaysClampedSavedStage_WithoutWritingPlayerPrefs()
        {
            ResetSaveData();
            SaveProgress(9, 9);

            GameObject textObject = new GameObject("Stage Text");
            Text text = textObject.AddComponent<Text>();

            MonoBehaviour stageTextView = CreateMonoBehaviour("StageTextView");
            SetField(stageTextView, "stageText", text);
            Invoke(stageTextView, "Start");

            Assert.That(text.text, Does.Contain("[7 스테이지"));
            Assert.AreEqual(9, PlayerPrefs.GetInt(SavedStageKey));
        }

        [Test]
        public void StageController_Awake_ReadsSavedStageThroughGameSaveControllerFallback()
        {
            ResetSaveData();
            SaveProgress(6, 3);
            ResetSingleton("GameSaveController", "instance");

            MonoBehaviour stageController = CreateMonoBehaviour("StageController");
            Invoke(stageController, "Awake");

            Assert.AreEqual(6, GetIntField(stageController, "stageNum"));
        }

        private static void ResetSaveData()
        {
            PlayerPrefs.DeleteKey(SavedStageKey);
            PlayerPrefs.DeleteKey(PlayingStageKey);
            PlayerPrefs.Save();
        }

        private static void SaveProgress(int savedStage, int playingStage)
        {
            PlayerPrefs.SetInt(SavedStageKey, savedStage);
            PlayerPrefs.SetInt(PlayingStageKey, playingStage);
            PlayerPrefs.Save();
        }

        private static MonoBehaviour CreateMonoBehaviour(string typeName)
        {
            Type type = FindType(typeName);
            GameObject gameObject = new GameObject(typeName);
            return gameObject.AddComponent(type) as MonoBehaviour;
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

        private static object InvokeStatic(string typeName, string methodName)
        {
            Type type = FindType(typeName);
            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
            Assert.IsNotNull(methodInfo, typeName + "." + methodName + " method was not found.");
            return methodInfo.Invoke(null, null);
        }

        private static object Invoke(object target, string methodName, params object[] parameters)
        {
            Type[] parameterTypes = parameters.Select(parameter => parameter.GetType()).ToArray();
            MethodInfo methodInfo = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                parameterTypes,
                null);
            Assert.IsNotNull(methodInfo, target.GetType().Name + "." + methodName + " method was not found.");
            return methodInfo.Invoke(target, parameters);
        }

        private static int GetIntField(object target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(fieldInfo, target.GetType().Name + "." + fieldName + " field was not found.");
            return (int)fieldInfo.GetValue(target);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, target.GetType().Name + "." + fieldName + " field was not found.");
            fieldInfo.SetValue(target, value);
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

        private static void DestroyObjectByName(string objectName)
        {
            GameObject gameObject = GameObject.Find(objectName);
            if (gameObject != null)
            {
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}
