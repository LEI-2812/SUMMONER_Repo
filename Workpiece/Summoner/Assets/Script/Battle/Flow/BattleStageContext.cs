using UnityEngine;

public class BattleStageContext : MonoBehaviour
{
    public int CurrentStage { get; private set; }

    private void Awake()
    {
        CurrentStage = GameSaveController.GetGameSaveOrDefault().playingStage;
    }
}
