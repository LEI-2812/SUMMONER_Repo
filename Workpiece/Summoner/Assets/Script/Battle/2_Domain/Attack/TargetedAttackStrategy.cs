using System.Collections.Generic;

// 역할: TargetedAttackStrategy의 책임을 정의한다.
public class TargetedAttackStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        if (attackData.StatusType == StatusType.Shield || attackData.StatusType == StatusType.OnceInvincibility)
        {
            List<Summon> selfTarget = new List<Summon>();

            if (attacker != null)
            {
                selfTarget.Add(attacker);
            }

            return selfTarget;
        }

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
