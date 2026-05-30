using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Summoner.EditModeTests
{
    public class StageSceneConnectionEditModeTests
    {
        private const string StartScenePath = "Assets/Screen/Start Screen.unity";
        private const string StageSelectScenePath = "Assets/Screen/Stage Select Screen.unity";

        private static readonly string[] FightScenePaths =
        {
            "Assets/Screen/FightScene/Fight Screen_1Stage.unity",
            "Assets/Screen/FightScene/Fight Screen_2Stage.unity",
            "Assets/Screen/FightScene/Fight Screen_3Stage.unity",
            "Assets/Screen/FightScene/Fight Screen_4Stage.unity",
            "Assets/Screen/FightScene/Fight Screen_5Stage.unity",
            "Assets/Screen/FightScene/Fight Screen_6Stage.unity",
            "Assets/Screen/FightScene/Fight Screen_7Stage.unity"
        };

        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        [Test]
        public void StartScreen_HasRequiredUiConnections()
        {
            OpenScene(StartScenePath);

            AssertNoMissingScriptsInOpenScene();

            MonoBehaviour startScreenView = FindOneComponent("StartScreenView");
            AssertFieldAssigned(startScreenView, "newAlert");
            AssertFieldAssigned(startScreenView, "newAlertResult");
            AssertFieldAssigned(startScreenView, "loadAlert");
            AssertFieldAssigned(startScreenView, "loadAlertResult");
            AssertFieldAssigned(startScreenView, "loadButton");
            AssertFieldAssigned(startScreenView, "settingBtn");
            AssertFieldAssigned(startScreenView, "audioSource");

            List<MonoBehaviour> stageTextViews = FindComponents("StageTextView");
            Assert.GreaterOrEqual(stageTextViews.Count, 1, "Start Screen must have at least one StageTextView.");
            foreach (MonoBehaviour stageTextView in stageTextViews)
            {
                AssertFieldAssigned(stageTextView, "stageText");
            }

            AssertHasComponent("GameSaveController");
            AssertHasComponent("StageFlowController");
            AssertHasComponent("StageController");
        }

        [Test]
        public void StageSelectScreen_HasRequiredUiConnections()
        {
            OpenScene(StageSelectScenePath);

            AssertNoMissingScriptsInOpenScene();

            MonoBehaviour stageSelectView = FindOneComponent("StageSelectView");
            AssertButtonArrayAssigned(stageSelectView, "buttons");
            AssertFieldAssigned(stageSelectView, "audioSource");
        }

        [Test]
        public void FightScreens_HaveRequiredResultAlertConnections()
        {
            foreach (string fightScenePath in FightScenePaths)
            {
                OpenScene(fightScenePath);

                AssertNoMissingScriptsInOpenScene();

                List<MonoBehaviour> resultAlertViews = FindComponents("BattleResultAlertView");
                Assert.GreaterOrEqual(resultAlertViews.Count, 1, fightScenePath + " must have BattleResultAlertView.");

                foreach (MonoBehaviour resultAlertView in resultAlertViews)
                {
                    AssertFieldAssigned(resultAlertView, "alertClear");
                    AssertFieldAssigned(resultAlertView, "ClearResult");
                    AssertFieldAssigned(resultAlertView, "alertFail");
                    AssertFieldAssigned(resultAlertView, "FailResult");
                    AssertFieldAssigned(resultAlertView, "clearSound");
                    AssertFieldAssigned(resultAlertView, "failSound");
                }
            }
        }

        private static void OpenScene(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private static void AssertNoMissingScriptsInOpenScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                foreach (Transform transform in rootObject.GetComponentsInChildren<Transform>(true))
                {
                    int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                    Assert.AreEqual(0, missingScriptCount, transform.gameObject.name + " has missing script components.");
                }
            }
        }

        private static void AssertHasComponent(string typeName)
        {
            Assert.GreaterOrEqual(FindComponents(typeName).Count, 1, typeName + " must exist in " + SceneManager.GetActiveScene().name + ".");
        }

        private static MonoBehaviour FindOneComponent(string typeName)
        {
            List<MonoBehaviour> components = FindComponents(typeName);
            Assert.AreEqual(1, components.Count, SceneManager.GetActiveScene().name + " must have exactly one " + typeName + ".");
            return components[0];
        }

        private static List<MonoBehaviour> FindComponents(string typeName)
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.GetRootGameObjects()
                .SelectMany(rootObject => rootObject.GetComponentsInChildren<MonoBehaviour>(true))
                .Where(component => component != null && component.GetType().Name == typeName)
                .ToList();
        }

        private static void AssertFieldAssigned(MonoBehaviour component, string fieldName)
        {
            object fieldValue = GetFieldValue(component, fieldName);
            AssertUnityObjectAssigned(fieldValue, component.GetType().Name + "." + fieldName + " is not assigned.");
        }

        private static void AssertButtonArrayAssigned(MonoBehaviour component, string fieldName)
        {
            object fieldValue = GetFieldValue(component, fieldName);
            Button[] buttons = fieldValue as Button[];

            Assert.IsNotNull(buttons, component.GetType().Name + "." + fieldName + " must be a Button array.");
            Assert.Greater(buttons.Length, 0, component.GetType().Name + "." + fieldName + " must have at least one button.");

            for (int i = 0; i < buttons.Length; i++)
            {
                AssertUnityObjectAssigned(buttons[i], component.GetType().Name + "." + fieldName + "[" + i + "] is not assigned.");
            }
        }

        private static object GetFieldValue(MonoBehaviour component, string fieldName)
        {
            FieldInfo fieldInfo = component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.IsNotNull(fieldInfo, component.GetType().Name + "." + fieldName + " field was not found.");
            return fieldInfo.GetValue(component);
        }

        private static void AssertUnityObjectAssigned(object fieldValue, string message)
        {
            UnityEngine.Object unityObject = fieldValue as UnityEngine.Object;
            if (fieldValue is UnityEngine.Object)
            {
                Assert.IsNotNull(unityObject, message);
                return;
            }

            Assert.IsNotNull(fieldValue, message);
        }
    }
}
