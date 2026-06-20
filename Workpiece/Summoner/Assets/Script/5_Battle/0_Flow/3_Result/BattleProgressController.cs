using UnityEngine;

[RequireComponent(typeof(BattleStageContext))]
// 역할: 전투 결과 이후 재도전, 다음 스테이지, 스테이지 선택 이동을 처리한다.
public class BattleProgressController : MonoBehaviour
{
    [SerializeField] private BattleStageContext stageContext;

    private StageFlowController stageFlowController;

    private void Awake()
    {
        EnsureReferences();
    }

    public void CompleteClearResult(bool shouldRetry)
    {
        EnsureReferences();

        if (stageFlowController == null)
        {
            Debug.LogError("Cannot handle clear result because StageFlowController is missing.");
            return;
        }

        int stageNum = stageContext.CurrentStage;
        if (shouldRetry)
        {
            stageFlowController.SendFight(stageNum);
            return;
        }

        if (GameSaveController.instance == null)
        {
            Debug.LogError("Cannot save clear result because GameSaveController is missing.");
            return;
        }

        if (stageNum < 7)
        {
            GameSaveController.instance.SaveClearedStage(stageNum + 1);
        }

        stageFlowController.SendNextStageAfterBattle(stageNum);
    }

    public void CompleteFailResult(bool shouldRetry)
    {
        EnsureReferences();

        if (stageFlowController == null)
        {
            Debug.LogError("Cannot handle fail result because StageFlowController is missing.");
            return;
        }

        int stageNum = stageContext.CurrentStage;
        if (shouldRetry)
        {
            stageFlowController.SendFight(stageNum);
            return;
        }

        stageFlowController.SendStageSelect();
    }

    private void EnsureReferences()
    {
        if (stageContext == null)
        {
            stageContext = GetComponent<BattleStageContext>();
        }

        if (stageContext == null)
        {
            stageContext = gameObject.AddComponent<BattleStageContext>();
        }

        if (stageFlowController == null)
        {
            stageFlowController = StageFlowController.GetOrCreate();
        }
    }
}
