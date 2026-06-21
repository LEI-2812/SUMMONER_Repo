using System;
using System.Collections.Generic;
using System.IO;
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
        private const string HudScenePath = "Assets/Screen/HUD.unity";
        private const string ThankScenePath = "Assets/Screen/Thank Screen.unity";
        private const string MenuLoaderScriptPath = "Assets/Script/1_StartScreen/MenuLoader.cs";
        private const string FightSceneOnePath = "Assets/Screen/FightScene/Fight Screen_1Stage.unity";
        private const string PlayerPlatePrefabPath = "Assets/Prefabs/PlayerPlates.prefab";
        private const string EnemyPlatePrefabPath = "Assets/Prefabs/EnemyPlate.prefab";
        private const string PrefabFolderPath = "Assets/Prefabs";
        private const string SummonPrefabFolderPath = "Assets/Prefabs/SummonPrefab";

        private static readonly string[] FightScenePaths =
        {
            FightSceneOnePath,
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
        }

        [Test]
        public void StageSelectScreen_HasRequiredUiConnections()
        {
            OpenScene(StageSelectScenePath);

            AssertNoMissingScriptsInOpenScene();

            MonoBehaviour stageSelectView = FindOneComponent("StageSelectView");
            AssertButtonArrayAssigned(stageSelectView, "buttons");
            AssertFieldAssigned(stageSelectView, "audioSource");
            AssertHasComponent("StageController");
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

        [Test]
        public void FightScreens_HaveRequiredPlateRuntimeConnections()
        {
            foreach (string fightScenePath in FightScenePaths)
            {
                OpenScene(fightScenePath);

                AssertNoMissingScriptsInOpenScene();

                List<MonoBehaviour> plates = FindComponents("Plate");
                Assert.Greater(plates.Count, 0, fightScenePath + " must have Plate components.");

                foreach (MonoBehaviour plate in plates)
                {
                    AssertFieldAssigned(plate, "spawnTransform");
                    AssertFieldAssigned(plate, "statePanel");
                    AssertFieldAssigned(plate, "statePanelScript");
                }
            }
        }

        [Test]
        public void FightScreens_HaveRequiredBattleRuntimeControllerConnections()
        {
            foreach (string fightScenePath in FightScenePaths)
            {
                OpenScene(fightScenePath);

                AssertNoMissingScriptsInOpenScene();

                MonoBehaviour battleStartController = FindOneComponent("BattleStartController");
                AssertFieldAssigned(battleStartController, "enemyPlacementController");
                AssertFieldAssigned(battleStartController, "turnController");

                MonoBehaviour enemyPlacementController = FindOneComponent("BattleEnemyPlacementController");
                AssertFieldAssigned(enemyPlacementController, "stageContext");
                AssertFieldAssigned(enemyPlacementController, "plateController");
                AssertFieldAssigned(enemyPlacementController, "stageEnemyPlacementData");

                MonoBehaviour turnController = FindOneComponent("TurnController");
                AssertFieldAssigned(turnController, "player");
                AssertFieldAssigned(turnController, "enermy");
                AssertFieldAssigned(turnController, "plateController");
                AssertFieldAssigned(turnController, "battleResultController");
                AssertFieldAssigned(turnController, "turnCountText");
                AssertFieldAssigned(turnController, "turnClearText");

                MonoBehaviour playerController = FindOneComponent("PlayerController");
                AssertEnumerableFieldAssigned(playerController, "manaList");
                AssertFieldAssigned(playerController, "notHaveTexture");
                AssertFieldAssigned(playerController, "haveTexture");
                AssertFieldAssigned(playerController, "summonButton");
                AssertFieldAssigned(playerController, "reSummonButton");
                AssertFieldAssigned(playerController, "statePanel");
                AssertFieldAssigned(playerController, "clickSound");
                AssertFieldAssigned(playerController, "failSound");
                AssertFieldAssigned(playerController, "summonController");
                AssertFieldAssigned(playerController, "turnController");
                AssertFieldAssigned(playerController, "battleController");
                AssertFieldAssigned(playerController, "plateController");
                AssertFieldAssigned(playerController, "battleResultController");

                MonoBehaviour enemyAttackController = FindOneComponent("EnermyAttackController");
                AssertFieldAssigned(enemyAttackController, "plateController");
                AssertFieldAssigned(enemyAttackController, "playerAttackPrediction");
                AssertFieldAssigned(enemyAttackController, "battleController");

                MonoBehaviour plateController = FindOneComponent("PlateController");
                AssertEnumerableFieldAssigned(plateController, "playerPlates");
                AssertEnumerableFieldAssigned(plateController, "enermyPlates");

                MonoBehaviour battleResultController = FindOneComponent("BattleResultController");
                AssertFieldAssigned(battleResultController, "alertView");
                AssertFieldAssigned(battleResultController, "progressController");
            }
        }

        [Test]
        public void PlatePrefabs_HaveRequiredSpawnTransform()
        {
            AssertPlatePrefabSpawnTransformAssigned(PlayerPlatePrefabPath);
            AssertPlatePrefabSpawnTransformAssigned(EnemyPlatePrefabPath);
        }

        [Test]
        public void BuildScenesAndPrefabs_HaveNoMissingScripts()
        {
            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes.Where(scene => scene.enabled))
            {
                OpenScene(buildScene.path);
                AssertNoMissingScriptsInOpenScene();
            }

            foreach (string prefabGuid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolderPath }))
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    AssertNoMissingScriptsInGameObject(prefabRoot, prefabPath);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }

        [Test]
        public void SerializedUnityEvents_UseCurrentControllerAndMethodNames()
        {
            foreach (string fightScenePath in FightScenePaths)
            {
                string sceneText = File.ReadAllText(fightScenePath);
                Assert.IsFalse(
                    sceneText.Contains("m_TargetAssemblyTypeName: Player, Assembly-CSharp"),
                    fightScenePath + " must not keep stale Player UnityEvent target type names.");
            }

            string thankSceneText = File.ReadAllText(ThankScenePath);
            Assert.IsFalse(
                thankSceneText.Contains("m_MethodName: onClickSound"),
                ThankScenePath + " must call GameplaySettingView.OnClickSound with current method casing.");
            Assert.IsFalse(
                thankSceneText.Contains("m_MethodName: openOption"),
                ThankScenePath + " must call SettingPanelView.OpenOption with current method casing.");
        }

        [Test]
        public void BuildSceneButtons_HaveResolvableUnityEventTargets()
        {
            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes.Where(scene => scene.enabled))
            {
                OpenScene(buildScene.path);

                foreach (Button button in Resources.FindObjectsOfTypeAll<Button>().Where(button => button.gameObject.scene == SceneManager.GetActiveScene()))
                {
                    int eventCount = button.onClick.GetPersistentEventCount();
                    for (int eventIndex = 0; eventIndex < eventCount; eventIndex++)
                    {
                        UnityEngine.Object target = button.onClick.GetPersistentTarget(eventIndex);
                        string methodName = button.onClick.GetPersistentMethodName(eventIndex);

                        Assert.IsNotNull(
                            target,
                            buildScene.path + "/" + GetTransformPath(button.transform) + " has a missing UnityEvent target at index " + eventIndex + ".");
                        Assert.IsFalse(
                            string.IsNullOrEmpty(methodName),
                            buildScene.path + "/" + GetTransformPath(button.transform) + " has an empty UnityEvent method at index " + eventIndex + ".");
                    }
                }
            }
        }

        [Test]
        public void MenuLoaderScript_ResolvesRuntimeType()
        {
            MonoScript menuLoaderScript = AssetDatabase.LoadAssetAtPath<MonoScript>(MenuLoaderScriptPath);

            Assert.IsNotNull(menuLoaderScript, "MenuLoader MonoScript asset must exist.");
            Assert.IsNotNull(menuLoaderScript.GetClass(), "MenuLoader MonoScript must resolve to a runtime class.");
            Assert.AreEqual("MenuLoader", menuLoaderScript.GetClass().Name);
        }

        [Test]
        public void SummonPrefabs_HaveRequiredShieldImage()
        {
            foreach (string prefabGuid in AssetDatabase.FindAssets("t:Prefab", new[] { SummonPrefabFolderPath }))
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    MonoBehaviour summon = prefabRoot.GetComponentsInChildren<MonoBehaviour>(true)
                        .FirstOrDefault(component => component != null && TryGetFieldInfo(component.GetType(), "shieldImage") != null);
                    Assert.IsNotNull(summon, prefabPath + " must have a Summon-derived component with shieldImage.");
                    AssertFieldAssigned(summon, "shieldImage");
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }

        [Test]
        public void HudScene_HasOrganizedRootGroups()
        {
            OpenScene(HudScenePath);

            AssertNoMissingScriptsInOpenScene();
            Assert.AreEqual(0, FindComponents("StageController").Count, "HUD must not own stage selection flow.");
            AssertGameObjectPathExists("__UI");
            AssertGameObjectPathExists("__UI/MenuCanvas");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_00_Background/MenuBackgroundPanel");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_10_Menu/MenuPanel");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_20_Settings/Setting/SettingPanel");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_90_Alerts/AlertObject");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_90_Alerts/알림창");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_90_Alerts/알림창/Alert_clear");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_90_Alerts/알림창/Alert_fail");
            AssertGameObjectPathExists("__UI/MenuCanvas/UI_90_Alerts/알림창/Alert_Skip");
            AssertGameObjectPathExists("__Audio");
            AssertGameObjectPathExists("__Audio/MenuClickAudio");
            AssertGameObjectPathExists("__Audio/AlterClickAudio");
            AssertGameObjectPathExists("__Handlers");
            AssertGameObjectPathExists("__Handlers/MenuHandler");
            AssertGameObjectPathExists("__Handlers/ToMainAlertHandler");
            AssertGameObjectPathExists("__Handlers/SettingHandler");
            AssertGameObjectPathExists("__Handlers/ToQuitAlertHandler");
            AssertGameObjectPathExists("__Handlers/SkipAlertHandler");
        }

        [Test]
        public void FightSceneOne_HasOrganizedUiGroups()
        {
            OpenScene(FightSceneOnePath);

            AssertNoMissingScriptsInOpenScene();
            AssertGameObjectPathExists("0_Runtime/EventSystem");
            AssertGameObjectPathExists("0_Runtime/GameSaveController");
            AssertGameObjectPathExists("0_Runtime/0_BattleFlow_Runtime");
            AssertGameObjectPathExists("0_Runtime/0_BattleFlow_Runtime/0_Board_Runtime");
            AssertGameObjectPathExists("0_Runtime/0_BattleFlow_Runtime/1_Turn_Runtime");
            AssertGameObjectPathExists("0_Runtime/0_BattleFlow_Runtime/2_Stage_Runtime");
            AssertHasComponent("BattleStartController");
            AssertGameObjectPathExists("1_Camera/Main Camera");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_00_BackgroundLayer");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_10_BattleBoard/PlayerSide");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_10_BattleBoard/EnemySide");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_20_TurnStatus");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_20_TurnStatus/TurnTextUI");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_20_TurnStatus/CurrentTurnText");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_20_TurnStatus/ClearTurnText");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_30_PlayerCommands/DrawButton");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_30_PlayerCommands/RedrawButton");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_30_PlayerCommands/EndTurnButton");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_40_PlayerMana");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_50_SelectedUnitState");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_60_SummonPicker/RedrawPanel");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_90_ResultAlerts/Alert_clear");
            AssertGameObjectPathExists("2_UI/BattleCanvas/UI_90_ResultAlerts/Alert_fail");
        }

        [Test]
        public void FightSceneOne_UsesLaterStageBattleUiLayout()
        {
            OpenScene(FightSceneOnePath);

            AssertRectTransform(
                "2_UI/BattleCanvas/UI_20_TurnStatus",
                Vector3.one,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.one);
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_20_TurnStatus/TurnTextUI",
                Vector3.one,
                new Vector2(-720f, 493f),
                new Vector2(400f, 400f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_20_TurnStatus/CurrentTurnText",
                Vector3.one,
                new Vector2(-720f, 470f),
                new Vector2(350f, 75f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_20_TurnStatus/ClearTurnText",
                Vector3.one,
                new Vector2(-720f, 400f),
                new Vector2(300f, 75f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_30_PlayerCommands",
                Vector3.one,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.one);
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_30_PlayerCommands/DrawButton",
                Vector3.one,
                new Vector2(175f, 150f),
                new Vector2(250f, 250f),
                Vector2.zero,
                Vector2.zero);
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_30_PlayerCommands/RedrawButton",
                Vector3.one,
                new Vector2(420f, 230f),
                new Vector2(150f, 100f),
                Vector2.zero,
                Vector2.zero);
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_30_PlayerCommands/EndTurnButton",
                Vector3.one,
                new Vector2(600f, 230f),
                new Vector2(150f, 100f),
                Vector2.zero,
                Vector2.zero);
            AssertRectTransform(
                "2_UI/BattleCanvas/UI_90_ResultAlerts",
                Vector3.one,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.one);
        }

        [Test]
        public void FightScreens_KeepSummonPickerVisibleAndPlayerActionOverlayInactive()
        {
            foreach (string fightScenePath in FightScenePaths)
            {
                OpenScene(fightScenePath);

                GameObject summonPicker = FindGameObjectByPath("2_UI/BattleCanvas/UI_60_SummonPicker");
                Assert.IsNotNull(summonPicker, fightScenePath + " must have UI_60_SummonPicker.");

                RectTransform summonPickerRect = summonPicker.GetComponent<RectTransform>();
                Assert.IsNotNull(summonPickerRect, fightScenePath + " UI_60_SummonPicker must have RectTransform.");
                AssertVector3(Vector3.one, summonPickerRect.localScale, fightScenePath + " UI_60_SummonPicker must not be hidden by zero scale.");
                AssertVector2(new Vector2(1700.9507f, 1219.0833f), summonPickerRect.sizeDelta, fightScenePath + " UI_60_SummonPicker must keep the visible summon-pick layout.");
                AssertGameObjectPathMissing("2_UI/BattleCanvas/UI_60_SummonPicker/DrawPanel");
                AssertRectTransform(
                    "2_UI/BattleCanvas/UI_60_SummonPicker/RedrawPanel",
                    Vector3.one,
                    Vector2.zero,
                    new Vector2(-300f, -430f),
                    Vector2.zero,
                    Vector2.one);
                AssertRedrawPanelGridLayout(fightScenePath);
                AssertSummonOptionPanelSize(fightScenePath, "2_UI/BattleCanvas/UI_60_SummonPicker/RedrawPanel/RedrawOption_1");
                AssertSummonOptionPanelSize(fightScenePath, "2_UI/BattleCanvas/UI_60_SummonPicker/RedrawPanel/RedrawOption_2");
                AssertSummonOptionPanelSize(fightScenePath, "2_UI/BattleCanvas/UI_60_SummonPicker/RedrawPanel/RedrawOption_3");

                GameObject playerPlateActions = FindGameObjectByPath("2_UI/BattleCanvas/UI_10_BattleBoard/PlayerSide/PlayerPlateActions");
                Assert.IsNotNull(playerPlateActions, fightScenePath + " must have PlayerPlateActions.");
                Assert.IsFalse(playerPlateActions.activeSelf, fightScenePath + " PlayerPlateActions must start inactive.");

                GameObject enemySide = FindGameObjectByPath("2_UI/BattleCanvas/UI_10_BattleBoard/EnemySide");
                Assert.IsNotNull(enemySide, fightScenePath + " must have EnemySide.");
                Assert.IsTrue(enemySide.activeSelf, fightScenePath + " EnemySide must start active.");
            }
        }

        private static void AssertGameObjectPathMissing(string objectPath)
        {
            Assert.IsNull(FindGameObjectByPath(objectPath), objectPath + " must not exist.");
        }

        private static void AssertSummonOptionPanelSize(string fightScenePath, string objectPath)
        {
            GameObject optionPanel = FindGameObjectByPath(objectPath);
            Assert.IsNotNull(optionPanel, fightScenePath + " must have " + objectPath + ".");

            RectTransform optionPanelRect = optionPanel.GetComponent<RectTransform>();
            Assert.IsNotNull(optionPanelRect, fightScenePath + " " + objectPath + " must have RectTransform.");
            AssertVector2(new Vector2(360f, 520f), optionPanelRect.sizeDelta, fightScenePath + " " + objectPath + " must keep the compact summon option panel size.");
        }

        private static void AssertRedrawPanelGridLayout(string fightScenePath)
        {
            GameObject redrawPanel = FindGameObjectByPath("2_UI/BattleCanvas/UI_60_SummonPicker/RedrawPanel");
            Assert.IsNotNull(redrawPanel, fightScenePath + " must have RedrawPanel.");

            GridLayoutGroup gridLayout = redrawPanel.GetComponent<GridLayoutGroup>();
            Assert.IsNotNull(gridLayout, fightScenePath + " RedrawPanel must use GridLayoutGroup.");
            AssertVector2(new Vector2(360f, 520f), gridLayout.cellSize, fightScenePath + " RedrawPanel grid cell size must fit three summon options on one row.");
            Assert.AreEqual(GridLayoutGroup.Constraint.FixedColumnCount, gridLayout.constraint, fightScenePath + " RedrawPanel grid must keep three columns.");
            Assert.AreEqual(3, gridLayout.constraintCount, fightScenePath + " RedrawPanel grid must keep three summon options on one row.");
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
                AssertNoMissingScriptsInGameObject(rootObject, scene.path);
            }
        }

        private static void AssertNoMissingScriptsInGameObject(GameObject rootObject, string ownerPath)
        {
            foreach (Transform transform in rootObject.GetComponentsInChildren<Transform>(true))
            {
                int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
                Assert.AreEqual(0, missingScriptCount, ownerPath + "/" + GetTransformPath(transform) + " has missing script components.");
            }
        }

        private static string GetTransformPath(Transform transform)
        {
            List<string> pathParts = new List<string>();
            while (transform != null)
            {
                pathParts.Add(transform.name);
                transform = transform.parent;
            }

            pathParts.Reverse();
            return string.Join("/", pathParts);
        }

        private static void AssertHasComponent(string typeName)
        {
            Assert.GreaterOrEqual(FindComponents(typeName).Count, 1, typeName + " must exist in " + SceneManager.GetActiveScene().name + ".");
        }

        private static void AssertGameObjectPathExists(string objectPath)
        {
            Assert.IsNotNull(FindGameObjectByPath(objectPath), objectPath + " must exist in " + SceneManager.GetActiveScene().name + ".");
        }

        private static GameObject FindGameObjectByPath(string objectPath)
        {
            string[] pathParts = objectPath.Split('/');
            Scene scene = SceneManager.GetActiveScene();
            GameObject current = scene.GetRootGameObjects().FirstOrDefault(rootObject => rootObject.name == pathParts[0]);
            if (current == null)
            {
                return null;
            }

            for (int i = 1; i < pathParts.Length; i++)
            {
                Transform child = current.transform.Find(pathParts[i]);
                if (child == null)
                {
                    return null;
                }

                current = child.gameObject;
            }

            return current;
        }

        private static void AssertRectTransform(
            string objectPath,
            Vector3 expectedScale,
            Vector2 expectedPosition,
            Vector2 expectedSize,
            Vector2 expectedAnchorMin,
            Vector2 expectedAnchorMax)
        {
            GameObject target = FindGameObjectByPath(objectPath);
            Assert.IsNotNull(target, objectPath + " must exist.");

            RectTransform rectTransform = target.GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform, objectPath + " must have RectTransform.");

            AssertVector3(expectedScale, rectTransform.localScale, objectPath + " scale mismatch.");
            AssertVector2(expectedPosition, rectTransform.anchoredPosition, objectPath + " anchored position mismatch.");
            AssertVector2(expectedSize, rectTransform.sizeDelta, objectPath + " size mismatch.");
            AssertVector2(expectedAnchorMin, rectTransform.anchorMin, objectPath + " anchorMin mismatch.");
            AssertVector2(expectedAnchorMax, rectTransform.anchorMax, objectPath + " anchorMax mismatch.");
        }

        private static void AssertVector2(Vector2 expected, Vector2 actual, string message)
        {
            Assert.AreEqual(expected.x, actual.x, 0.01f, message + " x");
            Assert.AreEqual(expected.y, actual.y, 0.01f, message + " y");
        }

        private static void AssertVector3(Vector3 expected, Vector3 actual, string message)
        {
            Assert.AreEqual(expected.x, actual.x, 0.01f, message + " x");
            Assert.AreEqual(expected.y, actual.y, 0.01f, message + " y");
            Assert.AreEqual(expected.z, actual.z, 0.01f, message + " z");
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

        private static void AssertEnumerableFieldAssigned(MonoBehaviour component, string fieldName)
        {
            object fieldValue = GetFieldValue(component, fieldName);
            System.Collections.IEnumerable enumerable = fieldValue as System.Collections.IEnumerable;

            Assert.IsNotNull(enumerable, component.GetType().Name + "." + fieldName + " must be an enumerable field.");

            int index = 0;
            foreach (object item in enumerable)
            {
                AssertUnityObjectAssigned(item, component.GetType().Name + "." + fieldName + "[" + index + "] is not assigned.");
                index++;
            }

            Assert.Greater(index, 0, component.GetType().Name + "." + fieldName + " must have at least one item.");
        }

        private static void AssertPlatePrefabSpawnTransformAssigned(string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Assert.IsNotNull(prefab, prefabPath + " must exist.");

            MonoBehaviour plate = prefab.GetComponentsInChildren<MonoBehaviour>(true)
                .FirstOrDefault(component => component != null && component.GetType().Name == "Plate");
            Assert.IsNotNull(plate, prefabPath + " must have a Plate component.");
            AssertFieldAssigned(plate, "spawnTransform");
        }

        private static object GetFieldValue(MonoBehaviour component, string fieldName)
        {
            FieldInfo fieldInfo = TryGetFieldInfo(component.GetType(), fieldName);
            Assert.IsNotNull(fieldInfo, component.GetType().Name + "." + fieldName + " field was not found.");
            return fieldInfo.GetValue(component);
        }

        private static FieldInfo TryGetFieldInfo(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    return fieldInfo;
                }

                type = type.BaseType;
            }

            return null;
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
