using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class GameSystemStaticRegressionTests
    {
        private const string ScriptRoot = "Assets/Script";
        private const string ScreenRoot = "Assets/Screen";
        private const string PlayerPrefsSaveStorePath = "Assets/Script/Save/PlayerPrefsSaveStore.cs";
        private const string GameSaveControllerPath = "Assets/Script/Save/GameSaveController.cs";
        private const string AudioSettingViewPath = "Assets/Script/Option/AudioSettingView.cs";
        private const string VideoSettingViewPath = "Assets/Script/Option/VideoSettingView.cs";
        private const string StartScreenViewPath = "Assets/Script/Start Screen/StartScreenView.cs";

        private static readonly string[] KnownPlayerPrefsAccessFiles =
        {
            "Assets/Script/Save/PlayerPrefsSaveStore.cs",
            "Assets/Script/Option/AudioSettingView.cs",
            "Assets/Script/Option/GameplaySettingStore.cs",
            "Assets/Script/Option/VideoSettingView.cs"
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
            string text = File.ReadAllText(StartScreenViewPath);
            int getGameSaveCallCount = Regex.Matches(text, @"\.GetGameSave\s*\(").Count;

            Assert.AreEqual(
                1,
                getGameSaveCallCount,
                "StartScreenView should read GetGameSave only for StartSavedStage continue flow. Reuse local values instead of re-reading after StartNewGame.");
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
        public void VideoSettingView_RepairsInvalidSavedIndexesBeforeToggleSetup()
        {
            string text = File.ReadAllText(VideoSettingViewPath);

            StringAssert.Contains("GetValidSavedIndex(\"resolutionIndex\", resolutionToggles.Count)", text);
            StringAssert.Contains("GetValidSavedIndex(\"screenModeIndex\", screenModeToggles.Count)", text);
            StringAssert.Contains("savedIndex >= 0 && savedIndex < itemCount", text);
            StringAssert.Contains("PlayerPrefs.SetInt(prefsKey, 0)", text);
        }

        [Test]
        public void GameplaySettingStore_OwnsGameplayPlayerPrefsKeys()
        {
            string text = File.ReadAllText("Assets/Script/Option/GameplaySettingStore.cs");

            StringAssert.Contains("private const string StorySkipKey = \"IsStorySkip\"", text);
            StringAssert.Contains("private const string OnlyMouseKey = \"IsOnlyMouse\"", text);
            StringAssert.Contains("LoadStorySkipEnabled()", text);
            StringAssert.Contains("SaveStorySkipEnabled(bool isEnabled)", text);
            StringAssert.Contains("LoadOnlyMouseEnabled()", text);
            StringAssert.Contains("SaveOnlyMouseEnabled(bool isEnabled)", text);
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
    }
}
