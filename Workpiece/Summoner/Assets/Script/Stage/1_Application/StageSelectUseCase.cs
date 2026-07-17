// 역할: Stage Select 화면에서 필요한 저장 조회와 스테이지 선택 흐름을 처리한다.
public readonly struct StageDisplayData
{
    private const int LastStage = 7;

    public StageDisplayData(int stageNumber)
    {
        StageNumber = UnityEngine.Mathf.Clamp(stageNumber, 1, LastStage);
        StageName = GetStageName(StageNumber);
    }

    public int StageNumber { get; }
    public string StageName { get; }

    private static string GetStageName(int stage)
    {
        switch (stage)
        {
            case 1:
                return "광활한 브리뉴 평원 동쪽";
            case 2:
                return "광활한 브리뉴 평원 서쪽";
            case 3:
                return "정령 숲지대의 왼편";
            case 4:
                return "정령 숲지대의 오른편";
            case 5:
                return "중간계의 오염지대";
            case 6:
                return "화염구의 무덤";
            case 7:
                return "다크 드래곤의 둥지";
            default:
                return string.Empty;
        }
    }
}

public class StageSelectUseCase
{
    private readonly GameSaveUseCase gameSaveUseCase = new GameSaveUseCase();
    private readonly StageTransitionUseCase stageTransitionUseCase = new StageTransitionUseCase();

    public int GetUnlockedStage()
    {
        return gameSaveUseCase.GetGameSave().savedStage;
    }

    public bool SelectStage(int stage)
    {
        if (!stageTransitionUseCase.CanSendStage(stage))
        {
            return false;
        }

        gameSaveUseCase.SavePlayingStage(stage);
        return stageTransitionUseCase.SendStage(stage);
    }
}
