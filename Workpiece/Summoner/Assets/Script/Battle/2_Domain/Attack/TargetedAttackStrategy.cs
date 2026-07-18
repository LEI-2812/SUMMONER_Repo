using System.Collections.Generic;

// 역할: TargetedAttackStrategy의 책임을 정의한다.
public class TargetedAttackStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, AttackTargetInput input)
    {
        if (attackData.StatusType == StatusType.Shield || attackData.StatusType == StatusType.OnceInvincibility)
        {
            List<Summon> selfTarget = new List<Summon>();

            if (input?.Attacker != null)
            {
                selfTarget.Add(input.Attacker);
            }

            return selfTarget;
        }

        List<Summon> targets = new List<Summon>();
        IReadOnlyList<PlateData> targetPlates = input?.GetTargetPlates(attackData.TargetsOwnPlates());
        int selectedPlateIndex = input == null ? -1 : input.SelectedPlateIndex;

        if (targetPlates == null || selectedPlateIndex < 0 || selectedPlateIndex >= targetPlates.Count)
        {
            return targets;
        }

        PlateData targetPlate = targetPlates[selectedPlateIndex];
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
