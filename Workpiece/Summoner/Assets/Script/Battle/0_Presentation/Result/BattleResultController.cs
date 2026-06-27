using UnityEngine;

[RequireComponent(typeof(BattleResultAlertView))]
[RequireComponent(typeof(BattleRuntimeData))]
// 역할: BattleResultController의 책임을 정의한다.
public class BattleResultController : MonoBehaviour
{
    [SerializeField] private BattleResultAlertView alertView;
    [SerializeField] private BattleRuntimeData battleRuntimeData;

    private readonly BattleResultStateMachine resultStateMachine = new BattleResultStateMachine();
    private StageTransitionController stageTransitionController;

    private void Awake()
    {
        Ensure참조();
    }

    public bool TryStartClearResult(bool isEnemyPlateClear, int clearTurn, int currentTurn)
    {
        if (!resultStateMachine.TryStartClear(isEnemyPlateClear, clearTurn, currentTurn))
        {
            return false;
        }

        Debug.Log("전투 클리어.");
        Ensure참조();
        alertView.ShowClearResultAlert(CompleteClearResult);
        return true;
    }

    public void TryStartFailResult(int clearTurn, int currentTurn)
    {
        if (!resultStateMachine.TryStartFail(clearTurn, currentTurn))
        {
            return;
        }

        Debug.Log("전투 실패.");
        Ensure참조();
        alertView.ShowFailResultAlert(CompleteFailResult);
    }

    private void CompleteClearResult(bool shouldRetry)
    {
        Ensure참조();
        GetCompleteBattleResultUseCase().ExecuteClear(shouldRetry);
    }

    private void CompleteFailResult(bool shouldRetry)
    {
        Ensure참조();
        GetCompleteBattleResultUseCase().ExecuteFail(shouldRetry);
    }

    private CompleteBattleResultUseCase GetCompleteBattleResultUseCase()
    {
        return new CompleteBattleResultUseCase(
            battleRuntimeData != null ? battleRuntimeData.StageData : null,
            stageTransitionController);
    }

    private void Ensure참조()
    {
        if (alertView == null)
        {
            alertView = GetComponent<BattleResultAlertView>();
        }

        if (alertView == null)
        {
            Debug.LogError("BattleResultController에 BattleResultAlertView가 필요합니다.");
        }

        if (battleRuntimeData == null)
        {
            battleRuntimeData = GetComponent<BattleRuntimeData>();
        }

        if (battleRuntimeData == null)
        {
            Debug.LogError("BattleResultController에 BattleRuntimeData가 필요합니다.");
        }

        if (stageTransitionController == null)
        {
            stageTransitionController = StageTransitionController.GetOrCreate();
        }
    }
}
