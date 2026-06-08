using UnityEngine;

[RequireComponent(typeof(BattleResultAlertView))]
[RequireComponent(typeof(BattleProgressController))]
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

    public void ClearResultTry(bool isEnemyPlateClear, int clearTurn, int currentTurn)
    {
        if (!isEnemyPlateClear || clearTurn < currentTurn)
        {
            return;
        }

        Debug.Log("승리!");
        Clear();
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
            alertView = gameObject.AddComponent<BattleResultAlertView>();
        }

        if (progressController == null)
        {
            progressController = GetComponent<BattleProgressController>();
        }

        if (progressController == null)
        {
            progressController = gameObject.AddComponent<BattleProgressController>();
        }
    }
}
