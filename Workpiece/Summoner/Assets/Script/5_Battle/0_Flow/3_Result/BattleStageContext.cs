using UnityEngine;

// 역할: 현재 전투가 어느 스테이지에서 시작됐는지 보관한다.
public class BattleStageContext : MonoBehaviour
{
    [SerializeField] private int defaultStage = 1;

    public int CurrentStage { get; private set; }

    private void Awake()
    {
        CurrentStage = CurrentStageResolve();
    }

    private int CurrentStageResolve()
    {
        if (defaultStage > 0)
        {
            return defaultStage;
        }

        return GameSaveController.GetGameSaveOrDefault().playingStage;
    }
}
