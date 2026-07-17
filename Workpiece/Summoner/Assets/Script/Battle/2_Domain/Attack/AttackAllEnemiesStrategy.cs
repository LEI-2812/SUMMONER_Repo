using System.Collections.Generic;

// 역할: AttackAllEnemiesStrategy의 책임을 정의한다.
public class AttackAllEnemiesStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
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
