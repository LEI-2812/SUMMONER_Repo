using UnityEngine;

public class CompleteBattleResultUseCase
{
    private const int FinalStageNumber = 7;

    private readonly BattleStageData battleStageData;
    private readonly GameSaveUseCase gameSaveUseCase;
    private readonly StageTransitionController stageTransitionController;

    public CompleteBattleResultUseCase(
        BattleStageData battleStageData,
        StageTransitionController stageTransitionController)
        : this(
            battleStageData,
            new GameSaveUseCase(new StageProgressSaveStore()),
            stageTransitionController)
    {
    }

    public CompleteBattleResultUseCase(
        BattleStageData battleStageData,
        GameSaveUseCase gameSaveUseCase,
        StageTransitionController stageTransitionController)
    {
        this.battleStageData = battleStageData;
        this.gameSaveUseCase = gameSaveUseCase;
        this.stageTransitionController = stageTransitionController;
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
            stageTransitionController.SendFight(stageNum);
            return;
        }

        if (!SaveClearedStage(stageNum))
        {
            return;
        }

        stageTransitionController.SendNextStageAfterBattle(stageNum);
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
            stageTransitionController.SendFight(stageNum);
            return;
        }

        stageTransitionController.SendStageSelect();
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

        if (stageTransitionController == null)
        {
            Debug.LogError("StageTransitionController가 없어 전투 결과를 처리할 수 없습니다.");
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
