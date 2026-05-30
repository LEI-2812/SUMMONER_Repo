using UnityEngine.SceneManagement;

public static class StorySceneMove
{
    public static string GetNextSceneName(int storyNumber)
    {
        switch (storyNumber)
        {
            case 0:
                return "Stage Select Screen";
            case 8:
                return "Thank Screen";
            default:
                return "Fight Screen_" + storyNumber + "Stage";
        }
    }

    public static void LoadNextScene(int storyNumber)
    {
        SceneManager.LoadScene(GetNextSceneName(storyNumber));
    }
}
