using System.Collections.Generic;

// 역할: AttackAllEnemiesStrategy의 책임을 정의한다.
public class AttackAllEnemiesStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, AttackTargetInput input)
    {
        List<Summon> targets = new List<Summon>();
        IReadOnlyList<PlateData> targetPlates = input?.GetTargetPlates(attackData.TargetsOwnPlates());

        if (targetPlates == null)
        {
            return targets;
        }

        foreach (PlateData plate in targetPlates)
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
