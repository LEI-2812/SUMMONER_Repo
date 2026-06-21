using System.Collections.Generic;

// 역할: 공격 전략이 대상 소환수를 고르기 위해 지켜야 하는 계약을 정의한다.
interface IAttackTargetSelector
{
    List<Summon> SelectTargets(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex);
}

// 역할: 가까운 순서로 전달된 플레이트 목록에서 첫 점유 소환수를 공격 대상으로 선택한다.
class ClosestEnemyAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
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

// 역할: 사용자가 선택한 플레이트의 소환수를 공격 대상으로 선택한다.
class SelectedPlateAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null || selectedPlateIndex < 0 || selectedPlateIndex >= targetPlates.Count)
        {
            return targets;
        }

        Plate targetPlate = targetPlates[selectedPlateIndex];
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

// 역할: 대상 플레이트 목록에 있는 모든 소환수를 공격 대상으로 선택한다.
class AllEnemiesAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null)
        {
            return targets;
        }

        foreach (Plate plate in targetPlates)
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

// 역할: 공격자 자신을 이로운 효과의 대상으로 선택한다.
class SelfAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (attacker != null)
        {
            targets.Add(attacker);
        }

        return targets;
    }
}
