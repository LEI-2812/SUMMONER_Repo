using UnityEngine;

[RequireComponent(typeof(BattleResultAlertView))]
// 역할: BattleResultController의 책임을 정의한다.
public class BattleResultController : MonoBehaviour
{
    [SerializeField] private BattleResultAlertView alertView;

    private readonly BattleResultStateMachine resultStateMachine = new BattleResultStateMachine();
    private readonly StageTransitionUseCase stageTransitionUseCase = new StageTransitionUseCase();
    private BattleStageData battleStageData;

    private void Awake()
    {
        EnsureAlertView();
    }

    public void ConnectBattleStage(BattleStageData battleStageData)
    {
        this.battleStageData = battleStageData;
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
            battleStageData,
            stageTransitionUseCase);
    }

    private void Ensure참조()
    {
        EnsureAlertView();

        if (battleStageData == null)
        {
            Debug.LogError("BattleResultController에 BattleStageData가 필요합니다.");
        }
    }

    private void EnsureAlertView()
    {
        if (alertView == null)
        {
            alertView = GetComponent<BattleResultAlertView>();
        }

        if (alertView == null)
        {
            Debug.LogError("BattleResultController에 BattleResultAlertView가 필요합니다.");
        }
    }
}
