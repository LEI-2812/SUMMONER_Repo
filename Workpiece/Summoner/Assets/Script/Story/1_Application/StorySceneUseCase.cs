// 역할: storyNumber에 맞는 다음 씬 이동을 처리한다.
public class StorySceneUseCase
{
    private readonly StageTransitionUseCase stageTransitionUseCase = new StageTransitionUseCase();

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

        stageTransitionUseCase.SendFightStage(storyNumber);
    }
}
