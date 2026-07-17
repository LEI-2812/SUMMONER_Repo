using UnityEngine;

public class CompleteBattleResultUseCase
{
    private const int FinalStageNumber = 7;

    private readonly BattleStageData battleStageData;
    private readonly GameSaveUseCase gameSaveUseCase = new GameSaveUseCase();
    private readonly StageTransitionUseCase stageTransitionUseCase;

    public CompleteBattleResultUseCase(
        BattleStageData battleStageData,
        StageTransitionUseCase stageTransitionUseCase)
    {
        this.battleStageData = battleStageData;
        this.stageTransitionUseCase = stageTransitionUseCase;
    }

    public void ExecuteClear(bool shouldRetry)
    {
        if (!CanHandleResult())
        {
            return;
        }

        int stageNum = GetCurrentStage();
        if (shouldRetry)
        {
            stageTransitionUseCase.SendFightStage(stageNum);
            return;
        }

        if (!SaveClearedStage(stageNum))
        {
            return;
        }

        stageTransitionUseCase.SendNextStageAfterBattle(stageNum);
    }

    public void ExecuteFail(bool shouldRetry)
    {
        if (!CanHandleResult())
        {
            return;
        }

        int stageNum = GetCurrentStage();
        if (shouldRetry)
        {
            stageTransitionUseCase.SendFightStage(stageNum);
            return;
        }

        stageTransitionUseCase.SendStageSelect();
    }

    private bool CanHandleResult()
    {
        if (battleStageData == null)
        {
            Debug.LogError("BattleStageData가 없어 전투 결과를 처리할 수 없습니다.");
            return false;
        }

        if (battleStageData.CurrentStage <= 0 && gameSaveUseCase == null)
        {
            Debug.LogError("GameSaveUseCase가 없어 전투 스테이지를 확인할 수 없습니다.");
            return false;
        }

        if (stageTransitionUseCase == null)
        {
            Debug.LogError("StageTransitionUseCase가 없어 전투 결과를 처리할 수 없습니다.");
            return false;
        }

        return true;
    }

    private int GetCurrentStage()
    {
        if (battleStageData.CurrentStage > 0)
        {
            return battleStageData.CurrentStage;
        }

        return gameSaveUseCase.GetGameSave().playingStage;
    }

    private bool SaveClearedStage(int clearedStageNumber)
    {
        if (gameSaveUseCase == null)
        {
            Debug.LogError("GameSaveUseCase가 없어 클리어 결과를 저장할 수 없습니다.");
            return false;
        }

        if (clearedStageNumber < FinalStageNumber)
        {
            gameSaveUseCase.SaveClearedStage(clearedStageNumber + 1);
        }

        return true;
    }
}
