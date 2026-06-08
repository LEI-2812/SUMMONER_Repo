using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Summoner/Stage Enemy Placement Data")]
public class StageEnemyPlacementData : ScriptableObject
{
    [SerializeField] private List<StageEnemyPlacement> stageEnemyPlacements = new List<StageEnemyPlacement>();

    public List<EnemyPlacementSlot> GetEnemyPlacementSlots(int stage)
    {
        foreach (StageEnemyPlacement stageEnemyPlacement in stageEnemyPlacements)
        {
            if (stageEnemyPlacement != null && stageEnemyPlacement.StageMatches(stage))
            {
                return stageEnemyPlacement.GetEnemyPlacementSlots();
            }
        }

        return new List<EnemyPlacementSlot>();
    }
}

[Serializable]
public class StageEnemyPlacement
{
    [SerializeField] private int stage;
    [SerializeField] private List<EnemyPlacementSlot> enemyPlacementSlots = new List<EnemyPlacementSlot>();

    public bool StageMatches(int stage)
    {
        return this.stage == stage;
    }

    public List<EnemyPlacementSlot> GetEnemyPlacementSlots()
    {
        return enemyPlacementSlots ?? new List<EnemyPlacementSlot>();
    }
}

[Serializable]
public class EnemyPlacementSlot
{
    [SerializeField] private int plateIndex;
    [SerializeField] private Summon enemySummonPrefab;

    public int GetPlateIndex()
    {
        return plateIndex;
    }

    public Summon GetEnemySummonPrefab()
    {
        return enemySummonPrefab;
    }
}
