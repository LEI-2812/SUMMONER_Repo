using UnityEngine.SceneManagement;

// 역할: 프로젝트 전체 씬 이동의 실제 실행 지점을 한 곳으로 모은다.
public static class GameSceneFlow
{
    private const string HudSceneName = "HUD";

    public static void LoadStartScreen()
    {
        SceneManager.LoadScene("Start Screen");
    }

    public static void LoadHudAdditive()
    {
        if (IsHudLoaded())
        {
            return;
        }

        SceneManager.LoadScene(HudSceneName, LoadSceneMode.Additive);
    }

    public static bool IsHudLoaded()
    {
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            Scene scene = SceneManager.GetSceneAt(sceneIndex);
            if (scene.name == HudSceneName && scene.isLoaded)
            {
                return true;
            }
        }

        return false;
    }

    public static void LoadPrologue()
    {
        SceneManager.LoadScene("Prologue Screen");
    }

    public static void LoadStageSelect()
    {
        SceneManager.LoadScene("Stage Select Screen");
    }

    public static void LoadStory(int stage)
    {
        SceneManager.LoadScene("Story Screen_" + stage + "Stage");
    }

    public static void LoadFight(int stage)
    {
        SceneManager.LoadScene("Fight Screen_" + stage + "Stage");
    }

    public static void LoadEpilogue()
    {
        SceneManager.LoadScene("Epilogue Screen");
    }

    public static void LoadThank()
    {
        SceneManager.LoadScene("Thank Screen");
    }
}
