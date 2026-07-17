using UnityEngine;

// 역할: ContinueGameUseCase의 책임을 정의한다.
public class ContinueGameUseCase
{
    private readonly GameSaveUseCase gameSaveUseCase = new GameSaveUseCase();

    public bool HasSavedGame()
    {
        return gameSaveUseCase.HasGameSave();
    }

    public bool TryGetSavedStage(out int savedStage)
    {
        savedStage = 0;

        if (!HasSavedGame())
        {
            return false;
        }

        savedStage = gameSaveUseCase.GetGameSave().savedStage;
        return true;
    }

    public bool TryGetSavedStageDisplayData(out StageDisplayData stageDisplayData)
    {
        stageDisplayData = default;

        if (!TryGetSavedStage(out int savedStage))
        {
            return false;
        }

        stageDisplayData = new StageDisplayData(savedStage);
        return true;
    }

    public bool TryContinueSavedGame()
    {
        if (!TryGetSavedStage(out int savedStage))
        {
            return false;
        }

        Debug.Log($"저장된 스테이지 번호: {savedStage}");
        GameSceneUseCase.LoadStageSelect();
        return true;
    }
}
