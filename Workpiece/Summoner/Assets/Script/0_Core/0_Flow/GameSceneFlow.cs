using UnityEngine.SceneManagement;

// 역할: 프로젝트 전체 씬 이동의 실제 실행 지점을 한 곳으로 모은다.
public static class GameSceneFlow
{
    public static void LoadStartScreen()
    {
        SceneManager.LoadScene("Start Screen");
    }

    public static void LoadHudAdditive()
    {
        SceneManager.LoadScene("Screen/HUD", LoadSceneMode.Additive);
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
