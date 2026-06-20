// 역할: 스토리 흐름에서 필요한 다음 씬 이동을 실행한다.
public static class StorySceneFlow
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
        if (storyNumber == 0)
        {
            GameSceneFlow.LoadStageSelect();
            return;
        }

        if (storyNumber == 8)
        {
            GameSceneFlow.LoadThank();
            return;
        }

        GameSceneFlow.LoadFight(storyNumber);
    }

    public static void LoadFightScene(int stageNumber)
    {
        GameSceneFlow.LoadFight(stageNumber);
    }

    public static void LoadCurrentPlayingFightScene()
    {
        LoadFightScene(GameSaveController.GetGameSaveOrDefault().playingStage);
    }
}
