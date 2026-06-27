using System.Collections.Generic;

// 역할: IAttackTargetSelector의 책임을 정의한다.
interface IAttackTargetSelector
{
    List<Summon> SelectTargets(Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex);
}

// 역할: IAttackTargetSelector의 책임을 정의한다.
class ClosestEnemyAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null)
        {
            return targets;
        }

        for (int i = 0; i < targetPlates.Count; i++)
        {
            if (targetPlates[i] == null)
            {
                continue;
            }

            Summon enemySummon = targetPlates[i].GetCurrentSummon();
            if (enemySummon != null)
            {
                targets.Add(enemySummon);
                return targets;
            }
        }

        return targets;
    }
}

// 역할: IAttackTargetSelector의 책임을 정의한다.
class SelectedPlateAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null || selectedPlateIndex < 0 || selectedPlateIndex >= targetPlates.Count)
        {
            return targets;
        }

        BattleBoardInputController targetPlate = targetPlates[selectedPlateIndex];
        if (targetPlate == null)
        {
            return targets;
        }

        Summon target = targetPlate.GetCurrentSummon();
        if (target != null)
        {
            targets.Add(target);
        }

        return targets;
    }
}

// 역할: IAttackTargetSelector의 책임을 정의한다.
class AllEnemiesAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null)
        {
            return targets;
        }

        foreach (BattleBoardInputController plate in targetPlates)
        {
            if (plate == null)
            {
                continue;
            }

            Summon target = plate.GetCurrentSummon();
            if (target != null)
            {
                targets.Add(target);
            }
        }

        return targets;
    }
}

// 역할: IAttackTargetSelector의 책임을 정의한다.
class SelfAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (attacker != null)
        {
            targets.Add(attacker);
        }

        return targets;
    }
}
