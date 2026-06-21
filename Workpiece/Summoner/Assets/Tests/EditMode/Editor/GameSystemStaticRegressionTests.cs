using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace Summoner.EditModeTests
{
    public class GameSystemStaticRegressionTests
    {
        private const string ScriptRoot = "Assets/Script";
        private const string ScreenRoot = "Assets/Screen";
        private const string PlayerPrefsSaveStorePath = "Assets/Script/7_Save/PlayerPrefsSaveStore.cs";
        private const string GameSaveControllerPath = "Assets/Script/7_Save/GameSaveController.cs";
        private const string GameSaveDataPath = "Assets/Script/7_Save/GameSaveData.cs";
        private const string AudioSettingViewPath = "Assets/Script/8_Option/Audio/AudioSettingView.cs";
        private const string AudioSettingStorePath = "Assets/Script/8_Option/Audio/AudioSettingStore.cs";
        private const string GameplaySettingViewPath = "Assets/Script/8_Option/Gameplay/GameplaySettingView.cs";
        private const string GameplaySettingStorePath = "Assets/Script/8_Option/Gameplay/GameplaySettingStore.cs";
        private const string VideoSettingViewPath = "Assets/Script/8_Option/Video/VideoSettingView.cs";
        private const string VideoSettingStorePath = "Assets/Script/8_Option/Video/VideoSettingStore.cs";
        private const string StartScreenViewPath = "Assets/Script/1_StartScreen/StartScreenView.cs";

        private static readonly string[] KnownPlayerPrefsAccessFiles =
        {
            "Assets/Script/7_Save/PlayerPrefsSaveStore.cs",
            "Assets/Script/8_Option/Audio/AudioSettingStore.cs",
            "Assets/Script/8_Option/Gameplay/GameplaySettingStore.cs",
            "Assets/Script/8_Option/Video/VideoSettingStore.cs"
        };

        private static readonly string[] ProgressKeys =
        {
            "savedStage",
            "playingStage"
        };

        [Test]
        public void PlayerPrefsDirectAccess_StaysInsideKnownStorageFiles()
        {
            string[] filesWithPlayerPrefs = GetScriptFiles()
                .Where(path => File.ReadAllText(path).Contains("PlayerPrefs."))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            CollectionAssert.AreEquivalent(
                KnownPlayerPrefsAccessFiles,
                filesWithPlayerPrefs,
                "PlayerPrefs direct access changed. Move new game progress access into PlayerPrefsSaveStore, or update this test only when a known setting-storage exception is intentional.");
        }

        [Test]
        public void ProgressPlayerPrefsKeys_AreOnlyUsedByPlayerPrefsSaveStore()
        {
            List<string> violations = new List<string>();

            foreach (string path in GetScriptFiles())
            {
                string normalizedPath = NormalizePath(path);
                if (normalizedPath == PlayerPrefsSaveStorePath)
                {
                    continue;
                }

                string text = File.ReadAllText(path);
                foreach (string progressKey in ProgressKeys)
                {
                    if (ContainsPlayerPrefsAccessToKey(text, progressKey))
                    {
                        violations.Add(normalizedPath + " uses PlayerPrefs with " + progressKey);
                    }
                }
            }

            Assert.IsEmpty(
                violations,
                "Game progress PlayerPrefs keys must be accessed only through PlayerPrefsSaveStore:\n" + string.Join("\n", violations));
        }

        [Test]
        public void RemovedLoadStageMethod_DoesNotReturnToScriptsOrScenes()
        {
            string[] matches = GetFiles(ScriptRoot, "*.cs")
                .Concat(GetFiles(ScreenRoot, "*.unity"))
                .Where(path => Regex.IsMatch(File.ReadAllText(path), @"\b[Ll]oadStage\b"))
                .Select(NormalizePath)
                .OrderBy(path => path)
                .ToArray();

            Assert.IsEmpty(matches, "LoadStage was removed. Do not reintroduce it:\n" + string.Join("\n", matches));
        }

        [Test]
        public void StartScreenView_ReadsSavedStageOnlyOnceForContinue()
        {
            string startScreenViewText = File.ReadAllText(StartScreenViewPath);
            string startGameFlowText = File.ReadAllText("Assets/Script/0_Core/0_Flow/StartGameFlow.cs");
            int startScreenViewGetGameSaveCallCount = Regex.Matches(startScreenViewText, @"\.GetGameSave\s*\(").Count;
            int startGameFlowGetGameSaveCallCount = Regex.Matches(startGameFlowText, @"\.GetGameSave\s*\(").Count;

            Assert.AreEqual(
                0,
                startScreenViewGetGameSaveCallCount,
                "StartScreenView should delegate saved stage reads to StartGameFlow.");
            Assert.AreEqual(
                1,
                startGameFlowGetGameSaveCallCount,
                "StartGameFlow should read GetGameSave only once for the continue flow.");
        }

        [Test]
        public void StartScreenFlow_DoesNotDependOnStageController()
        {
            string startScreenViewText = File.ReadAllText(StartScreenViewPath);
            string startGameFlowText = File.ReadAllText("Assets/Script/0_Core/0_Flow/StartGameFlow.cs");

            Assert.IsFalse(startScreenViewText.Contains("FindObjectOfType<StageController>()"), "Start Screen must not search for StageController.");
            Assert.IsFalse(startScreenViewText.Contains("private StageController"), "StartScreenView should not keep a StageController reference.");
            Assert.IsFalse(startGameFlowText.Contains("TryStartNewGame(StageController"), "StartGameFlow should not require StageController for new game.");
            Assert.IsFalse(startGameFlowText.Contains("TryContinueSavedGame(StageController"), "StartGameFlow should not require StageController for continue.");
            Assert.IsFalse(startGameFlowText.Contains("checkStage"), "Start flow should not log the old checkStage error.");
        }

        [Test]
        public void GameSaveController_MovesToRootBeforeDontDestroyOnLoad()
        {
            string text = File.ReadAllText(GameSaveControllerPath);

            StringAssert.Contains("transform.SetParent(null);", text);
            Assert.Less(
                text.IndexOf("transform.SetParent(null);"),
                text.IndexOf("DontDestroyOnLoad(gameObject);"),
                "GameSaveController must become a root object before DontDestroyOnLoad.");
        }

        [Test]
        public void GameSaveController_UsesGameSaveDataFactoryForNewProgress()
        {
            string controllerText = File.ReadAllText(GameSaveControllerPath);
            string dataText = File.ReadAllText(GameSaveDataPath);

            StringAssert.Contains("public static GameSaveData CreateNewGameProgress()", dataText);
            StringAssert.Contains("currentSaveData = GameSaveData.CreateNewGameProgress();", controllerText);
            Assert.AreEqual(
                0,
                Regex.Matches(controllerText, @"new\s+GameSaveData\s*\{").Count,
                "GameSaveController should not duplicate new-game default progress values.");
        }

        [Test]
        public void AudioSettingView_GuardsZeroVolumeBeforeLog10()
        {
            string text = File.ReadAllText(AudioSettingViewPath);

            StringAssert.Contains("private const float MutedVolumeDb", text);
            StringAssert.Contains("VolumeToDecibel(adjustedBGMVolume)", text);
            StringAssert.Contains("VolumeToDecibel(adjustedSFXVolume)", text);
            StringAssert.Contains("if (volume <= 0f)", text);
            Assert.IsFalse(text.Contains("Mathf.Log10(adjustedBGMVolume)"));
            Assert.IsFalse(text.Contains("Mathf.Log10(adjustedSFXVolume)"));
        }

        [Test]
        public void AudioSettingView_DelegatesPlayerPrefsToStore()
        {
            string viewText = File.ReadAllText(AudioSettingViewPath);
            string storeText = File.ReadAllText(AudioSettingStorePath);

            StringAssert.Contains("private readonly AudioSettingStore audioSettingStore", viewText);
            StringAssert.Contains("audioSettingStore.LoadMasterVolume()", viewText);
            StringAssert.Contains("audioSettingStore.LoadBgmVolume()", viewText);
            StringAssert.Contains("audioSettingStore.LoadSfxVolume()", viewText);
            StringAssert.Contains("audioSettingStore.SaveMasterVolume(volume)", viewText);
            StringAssert.Contains("audioSettingStore.SaveBgmVolume(volume)", viewText);
            StringAssert.Contains("audioSettingStore.SaveSfxVolume(volume)", viewText);
            Assert.IsFalse(viewText.Contains("PlayerPrefs."), "AudioSettingView should not access PlayerPrefs directly.");

            StringAssert.Contains("private const string MasterVolumeKey = \"MasterVolume\"", storeText);
            StringAssert.Contains("private const string BgmVolumeKey = \"BGMVolume\"", storeText);
            StringAssert.Contains("private const string SfxVolumeKey = \"SFXVolume\"", storeText);
            StringAssert.Contains("LoadMasterVolume()", storeText);
            StringAssert.Contains("SaveMasterVolume(float volume)", storeText);
            StringAssert.Contains("LoadBgmVolume()", storeText);
            StringAssert.Contains("SaveBgmVolume(float volume)", storeText);
            StringAssert.Contains("LoadSfxVolume()", storeText);
            StringAssert.Contains("SaveSfxVolume(float volume)", storeText);
        }

        [Test]
        public void VideoSettingView_RepairsInvalidSavedIndexesBeforeToggleSetup()
        {
            string viewText = File.ReadAllText(VideoSettingViewPath);
            string storeText = File.ReadAllText(VideoSettingStorePath);

            StringAssert.Contains("videoSettingStore.LoadValidResolutionIndex(resolutionToggles.Count)", viewText);
            StringAssert.Contains("videoSettingStore.LoadValidScreenModeIndex(screenModeToggles.Count)", viewText);
            StringAssert.Contains("savedIndex >= 0 && savedIndex < itemCount", storeText);
            StringAssert.Contains("PlayerPrefs.SetInt(prefsKey, 0)", storeText);
            Assert.IsFalse(viewText.Contains("PlayerPrefs."), "VideoSettingView should not access PlayerPrefs directly.");
        }

        [TestCase("resolutionIndex", -1, 3)]
        [TestCase("resolutionIndex", 3, 3)]
        [TestCase("screenModeIndex", -1, 3)]
        [TestCase("screenModeIndex", 3, 3)]
        public void VideoSettingView_RepairsInvalidSavedIndexesInPlayerPrefs(string prefsKey, int savedIndex, int itemCount)
        {
            PlayerPrefs.SetInt(prefsKey, savedIndex);

            int validIndex = InvokeGetValidSavedIndex(prefsKey, itemCount);

            Assert.AreEqual(0, validIndex);
            Assert.AreEqual(0, PlayerPrefs.GetInt(prefsKey));
        }

        [TestCase("resolutionIndex", 1, 3)]
        [TestCase("screenModeIndex", 2, 3)]
        public void VideoSettingView_KeepsValidSavedIndexesInPlayerPrefs(string prefsKey, int savedIndex, int itemCount)
        {
            PlayerPrefs.SetInt(prefsKey, savedIndex);

            int validIndex = InvokeGetValidSavedIndex(prefsKey, itemCount);

            Assert.AreEqual(savedIndex, validIndex);
            Assert.AreEqual(savedIndex, PlayerPrefs.GetInt(prefsKey));
        }

        [Test]
        public void GameplaySettingStore_OwnsGameplayPlayerPrefsKeys()
        {
            string text = File.ReadAllText(GameplaySettingStorePath);

            StringAssert.Contains("private const string StorySkipKey = \"IsStorySkip\"", text);
            StringAssert.Contains("private const string OnlyMouseKey = \"IsOnlyMouse\"", text);
            StringAssert.Contains("LoadStorySkipEnabled()", text);
            StringAssert.Contains("SaveStorySkipEnabled(bool isEnabled)", text);
            StringAssert.Contains("LoadOnlyMouseEnabled()", text);
            StringAssert.Contains("SaveOnlyMouseEnabled(bool isEnabled)", text);
        }

        [Test]
        public void GameplaySettingFiles_StayInGameplayFolder()
        {
            string viewText = File.ReadAllText(GameplaySettingViewPath);
            string storeText = File.ReadAllText(GameplaySettingStorePath);

            StringAssert.Contains("public class GameplaySettingView : MonoBehaviour", viewText);
            StringAssert.Contains("private readonly GameplaySettingStore gameplaySettingStore", viewText);
            StringAssert.Contains("public class GameplaySettingStore", storeText);
            Assert.IsFalse(File.Exists("Assets/Script/8_Option/GameplaySettingView.cs"));
            Assert.IsFalse(File.Exists("Assets/Script/8_Option/GameplaySettingStore.cs"));
        }

        [Test]
        public void VideoSettingView_DelegatesPlayerPrefsToStore()
        {
            string viewText = File.ReadAllText(VideoSettingViewPath);
            string storeText = File.ReadAllText(VideoSettingStorePath);

            StringAssert.Contains("private readonly VideoSettingStore videoSettingStore", viewText);
            StringAssert.Contains("videoSettingStore.LoadValidResolutionIndex(resolutionToggles.Count)", viewText);
            StringAssert.Contains("videoSettingStore.LoadValidScreenModeIndex(screenModeToggles.Count)", viewText);
            StringAssert.Contains("videoSettingStore.SaveResolutionIndex(index)", viewText);
            StringAssert.Contains("videoSettingStore.SaveScreenModeIndex(index)", viewText);
            Assert.IsFalse(viewText.Contains("PlayerPrefs."), "VideoSettingView should not access PlayerPrefs directly.");

            StringAssert.Contains("private const string ResolutionIndexKey = \"resolutionIndex\"", storeText);
            StringAssert.Contains("private const string ScreenModeIndexKey = \"screenModeIndex\"", storeText);
            StringAssert.Contains("LoadValidResolutionIndex(int itemCount)", storeText);
            StringAssert.Contains("SaveResolutionIndex(int index)", storeText);
            StringAssert.Contains("LoadValidScreenModeIndex(int itemCount)", storeText);
            StringAssert.Contains("SaveScreenModeIndex(int index)", storeText);
        }

        private static bool ContainsPlayerPrefsAccessToKey(string text, string key)
        {
            return Regex.IsMatch(text, @"PlayerPrefs\.\w+\s*\(\s*""" + Regex.Escape(key) + @"""");
        }

        private static IEnumerable<string> GetScriptFiles()
        {
            return GetFiles(ScriptRoot, "*.cs");
        }

        private static IEnumerable<string> GetFiles(string root, string pattern)
        {
            if (!Directory.Exists(root))
            {
                return Enumerable.Empty<string>();
            }

            return Directory.GetFiles(root, pattern, SearchOption.AllDirectories);
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }

        private static int InvokeGetValidSavedIndex(string prefsKey, int itemCount)
        {
            Type videoSettingStoreType = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetType("VideoSettingStore"))
                .FirstOrDefault(type => type != null);

            Assert.IsNotNull(videoSettingStoreType, "VideoSettingStore type was not found.");

            MethodInfo methodInfo = videoSettingStoreType.GetMethod(
                "GetValidSavedIndex",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.IsNotNull(methodInfo, "VideoSettingStore.GetValidSavedIndex was not found.");
            return (int)methodInfo.Invoke(null, new object[] { prefsKey, itemCount });
        }
    }
}

