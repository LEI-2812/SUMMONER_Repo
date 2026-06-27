using System.Collections.Generic;

// 역할: ClosestEnemyAttackStrategy의 책임을 정의한다.
public class ClosestEnemyAttackStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        return new ClosestEnemyAttackTargetSelector().SelectTargets(attacker, targetPlates, selectedPlateIndex);
    }
}
