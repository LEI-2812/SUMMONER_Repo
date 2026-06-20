using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Summoner/Stage Enemy Placement Data")]
// 역할: 스테이지별 적 배치 목록을 Unity 에셋 데이터로 보관한다.
public class StageEnemyPlacementData : ScriptableObject
{
    [Header("Stage enemy placements")]
    [Tooltip("Enemy summons placed on enemy plates when each battle stage starts.")]
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
// 역할: 한 스테이지에 배치될 적 슬롯 목록을 보관한다.
public class StageEnemyPlacement
{
    [Min(1)]
    [Tooltip("Battle stage number that uses this enemy placement.")]
    [SerializeField] private int stage;

    [Tooltip("Enemy summon prefab per enemy plate index.")]
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
// 역할: 특정 적 소환수 프리팹을 어느 적 플레이트에 놓을지 보관한다.
public class EnemyPlacementSlot
{
    [Min(0)]
    [Tooltip("Index in PlateController enemy plate list.")]
    [SerializeField] private int plateIndex;

    [Tooltip("Enemy summon prefab to place on this plate.")]
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
