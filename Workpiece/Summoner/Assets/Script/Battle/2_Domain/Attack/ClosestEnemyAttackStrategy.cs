using System.Collections.Generic;

// 역할: ClosestEnemyAttackStrategy의 책임을 정의한다.
public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
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
