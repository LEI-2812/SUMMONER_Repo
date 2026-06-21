using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleStageContext))]
// 역할: 현재 스테이지의 적 배치 데이터를 읽고 적 소환수를 적 플레이트에 배치한다.
public class BattleEnemyPlacementController : MonoBehaviour
{
    [SerializeField] private BattleStageContext stageContext;
    [SerializeField] private PlateController plateController;
    [SerializeField] private StageEnemyPlacementData stageEnemyPlacementData;

    private void Awake()
    {
        ReferencesEnsure();
    }

    public void EnemyPlacementApply()
    {
        ReferencesEnsure();

        if (stageContext == null || plateController == null || stageEnemyPlacementData == null)
        {
            Debug.LogWarning("적 배치에 필요한 참조가 없어 스테이지 적 배치를 건너뜁니다.");
            return;
        }

        IReadOnlyList<Plate> enemyPlates = plateController.GetEnermyPlates();
        if (enemyPlates == null || enemyPlates.Count == 0)
        {
            Debug.LogWarning("적 플레이트가 없어 스테이지 적 배치를 건너뜁니다.");
            return;
        }

        foreach (EnemyPlacementSlot enemyPlacementSlot in stageEnemyPlacementData.GetEnemyPlacementSlots(stageContext.CurrentStage))
        {
            EnemyPlacementSlotApply(enemyPlacementSlot, enemyPlates);
        }
    }

    private void EnemyPlacementSlotApply(EnemyPlacementSlot enemyPlacementSlot, IReadOnlyList<Plate> enemyPlates)
    {
        if (enemyPlacementSlot == null)
        {
            return;
        }

        int plateIndex = enemyPlacementSlot.GetPlateIndex();
        if (plateIndex < 0 || plateIndex >= enemyPlates.Count)
        {
            Debug.LogWarning("유효하지 않은 적 플레이트 인덱스입니다: " + plateIndex);
            return;
        }

        Summon enemySummonPrefab = enemyPlacementSlot.GetEnemySummonPrefab();
        if (enemySummonPrefab == null)
        {
            Debug.LogWarning("적 소환수 프리팹이 비어 있어 배치를 건너뜁니다.");
            return;
        }

        Plate targetPlate = enemyPlates[plateIndex];
        if (targetPlate == null || EnemyPlateHasSummon(targetPlate))
        {
            return;
        }

        targetPlate.SummonPlaceOnPlate(enemySummonPrefab);
        EnemyStageMultiplierApply(targetPlate);
    }

    private void EnemyStageMultiplierApply(Plate targetPlate)
    {
        Summon placedEnemySummon = targetPlate.GetCurrentSummon();
        if (placedEnemySummon == null)
        {
            return;
        }

        placedEnemySummon.ApplayMultiple(Summon.GetStatMultiplier());
    }

    private bool EnemyPlateHasSummon(Plate targetPlate)
    {
        return targetPlate.GetCurrentSummon() != null
            || targetPlate.GetComponentInChildren<Summon>(true) != null;
    }

    private void ReferencesEnsure()
    {
        if (stageContext == null)
        {
            stageContext = GetComponent<BattleStageContext>();
        }

        if (plateController == null)
        {
            Debug.LogError("BattleEnemyPlacementController needs PlateController. Assign it in the Inspector.");
        }
    }
}
