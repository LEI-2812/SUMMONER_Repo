// 역할: storyNumber에 맞는 다음 씬 이동을 처리한다.
public class StorySceneUseCase
{
    public string GetNextSceneName(int storyNumber)
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

    public void LoadNextScene(int storyNumber)
    {
        if (storyNumber == 0)
        {
            GameSceneUseCase.LoadStageSelect();
            return;
        }

        if (storyNumber == 8)
        {
            GameSceneUseCase.LoadThank();
            return;
        }

        GameSceneUseCase.LoadFight(storyNumber);
    }

    public void LoadFightScene(int stageNumber)
    {
        GameSceneUseCase.LoadFight(stageNumber);
    }
}
