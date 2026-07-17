using System.Collections.Generic;
using UnityEngine;

public class StartBattleUseCase
{
    public void Execute(
        BattleStageData battleStageData,
        IReadOnlyList<BattleBoardInputController> enemyPlates,
        StageEnemyPlacementData stageEnemyPlacementData)
    {
        if (battleStageData == null)
        {
            Debug.LogWarning("전투 스테이지 데이터가 없어 배수를 적용할 수 없습니다.");
            return;
        }

        Summon.StatMultiplierSet(battleStageData.GetSummonStatMultiplier());

        if (enemyPlates == null || stageEnemyPlacementData == null)
        {
            Debug.LogWarning("적 배치 데이터가 없습니다. 스테이지 적 배치 없이 전투를 시작합니다.");
            return;
        }

        if (enemyPlates.Count == 0)
        {
            Debug.LogWarning("적 플레이트가 비어 있습니다. 스테이지 적 배치 없이 전투를 시작합니다.");
            return;
        }

        PlaceStageEnemies(
            battleStageData.CurrentStage,
            stageEnemyPlacementData,
            enemyPlates);
    }

    private void PlaceStageEnemies(
        int currentStage,
        StageEnemyPlacementData stageEnemyPlacementData,
        IReadOnlyList<BattleBoardInputController> enemyPlates)
    {
        foreach (EnemyPlacementSlot enemyPlacementSlot in stageEnemyPlacementData.GetEnemyPlacementSlots(currentStage))
        {
            ApplyEnemyPlacementSlot(enemyPlacementSlot, enemyPlates);
        }
    }

    private void ApplyEnemyPlacementSlot(EnemyPlacementSlot enemyPlacementSlot, IReadOnlyList<BattleBoardInputController> enemyPlates)
    {
        if (enemyPlacementSlot == null)
        {
            return;
        }

        int plateIndex = enemyPlacementSlot.GetPlateIndex();
        if (plateIndex < 0 || plateIndex >= enemyPlates.Count)
        {
            Debug.LogWarning("유효하지 않은 적 플레이트 인덱스: " + plateIndex);
            return;
        }

        Summon enemySummonPrefab = enemyPlacementSlot.GetEnemySummonPrefab();
        if (enemySummonPrefab == null)
        {
            Debug.LogWarning("적 소환수 프리팹이 없습니다.");
            return;
        }

        BattleBoardInputController targetPlate = enemyPlates[plateIndex];
        if (targetPlate == null || EnemyPlateHasSummon(targetPlate))
        {
            return;
        }

        targetPlate.SummonPlaceOnPlate(enemySummonPrefab);
        ApplyEnemyStageMultiplier(targetPlate);
    }

    private void ApplyEnemyStageMultiplier(BattleBoardInputController targetPlate)
    {
        Summon placedEnemySummon = targetPlate.GetCurrentSummon();
        if (placedEnemySummon == null)
        {
            return;
        }

        placedEnemySummon.ApplyStageMultiplier(Summon.GetStatMultiplier());
    }

    private bool EnemyPlateHasSummon(BattleBoardInputController targetPlate)
    {
        return targetPlate.GetCurrentSummon() != null
            || targetPlate.GetComponentInChildren<Summon>(true) != null;
    }
}
