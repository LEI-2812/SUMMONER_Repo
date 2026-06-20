using UnityEngine;

[RequireComponent(typeof(BattleResultAlertView))]
[RequireComponent(typeof(BattleProgressController))]
// 역할: 전투 승리/패배 조건을 확인하고 결과 알림을 표시한다.
public class BattleResultController : MonoBehaviour
{
    [SerializeField] private BattleResultAlertView alertView;
    [SerializeField] private BattleProgressController progressController;

    private bool resultStarted;

    private void Awake()
    {
        EnsureReferences();
    }

    public void Clear()
    {
        if (resultStarted)
        {
            return;
        }

        resultStarted = true;
        EnsureReferences();
        alertView.ShowClearResultAlert(progressController.CompleteClearResult);
    }

    public bool ClearResultTry(bool isEnemyPlateClear, int clearTurn, int currentTurn)
    {
        if (!isEnemyPlateClear || clearTurn < currentTurn)
        {
            return false;
        }

        Debug.Log("승리!");
        Clear();
        return true;
    }

    public void FailResultTry(int clearTurn, int currentTurn)
    {
        if (clearTurn >= currentTurn)
        {
            return;
        }

        Debug.Log("패배!");
        Fail();
    }

    public void Fail()
    {
        if (resultStarted)
        {
            return;
        }

        resultStarted = true;
        EnsureReferences();
        alertView.ShowFailResultAlert(progressController.CompleteFailResult);
    }

    private void EnsureReferences()
    {
        if (alertView == null)
        {
            alertView = GetComponent<BattleResultAlertView>();
        }

        if (alertView == null)
        {
            Debug.LogError("BattleResultController needs BattleResultAlertView.");
        }

        if (progressController == null)
        {
            progressController = GetComponent<BattleProgressController>();
        }

        if (progressController == null)
        {
            Debug.LogError("BattleResultController needs BattleProgressController.");
        }
    }
}
