using UnityEngine;

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
