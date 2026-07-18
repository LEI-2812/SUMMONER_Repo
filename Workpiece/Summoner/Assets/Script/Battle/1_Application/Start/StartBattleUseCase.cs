using System.Collections.Generic;

public readonly struct EnemyPlacementResult
{
    public EnemyPlacementResult(int plateIndex, Summon enemySummonPrefab)
    {
        PlateIndex = plateIndex;
        EnemySummonPrefab = enemySummonPrefab;
    }

    public int PlateIndex { get; }
    public Summon EnemySummonPrefab { get; }
}

public class StartBattleResult
{
    public StartBattleResult(
        double summonStatMultiplier,
        IReadOnlyList<EnemyPlacementResult> enemyPlacements,
        IReadOnlyList<string> warnings)
    {
        SummonStatMultiplier = summonStatMultiplier;
        EnemyPlacements = enemyPlacements;
        Warnings = warnings;
    }

    public double SummonStatMultiplier { get; }
    public IReadOnlyList<EnemyPlacementResult> EnemyPlacements { get; }
    public IReadOnlyList<string> Warnings { get; }
}

public class StartBattleUseCase
{
    public StartBattleResult Execute(
        BattleStageData battleStageData,
        StageEnemyPlacementData stageEnemyPlacementData,
        int enemyPlateCount)
    {
        var placements = new List<EnemyPlacementResult>();
        var warnings = new List<string>();

        if (battleStageData == null)
        {
            warnings.Add("전투 스테이지 데이터가 없어 배수를 적용할 수 없습니다.");
            return new StartBattleResult(1.0, placements, warnings);
        }

        double statMultiplier = battleStageData.GetSummonStatMultiplier();
        if (stageEnemyPlacementData == null)
        {
            warnings.Add("적 배치 데이터가 없습니다. 스테이지 적 배치 없이 전투를 시작합니다.");
            return new StartBattleResult(statMultiplier, placements, warnings);
        }

        if (enemyPlateCount <= 0)
        {
            warnings.Add("적 플레이트가 비어 있습니다. 스테이지 적 배치 없이 전투를 시작합니다.");
            return new StartBattleResult(statMultiplier, placements, warnings);
        }

        foreach (EnemyPlacementSlot slot in stageEnemyPlacementData.GetEnemyPlacementSlots(
                     battleStageData.CurrentStage))
        {
            AddEnemyPlacement(slot, enemyPlateCount, placements, warnings);
        }

        return new StartBattleResult(statMultiplier, placements, warnings);
    }

    private static void AddEnemyPlacement(
        EnemyPlacementSlot slot,
        int enemyPlateCount,
        List<EnemyPlacementResult> placements,
        List<string> warnings)
    {
        if (slot == null)
        {
            return;
        }

        int plateIndex = slot.GetPlateIndex();
        if (plateIndex < 0 || plateIndex >= enemyPlateCount)
        {
            warnings.Add("유효하지 않은 적 플레이트 인덱스: " + plateIndex);
            return;
        }

        Summon enemySummonPrefab = slot.GetEnemySummonPrefab();
        if (enemySummonPrefab == null)
        {
            warnings.Add("적 소환수 프리팹이 없습니다.");
            return;
        }

        placements.Add(new EnemyPlacementResult(plateIndex, enemySummonPrefab));
    }
}
