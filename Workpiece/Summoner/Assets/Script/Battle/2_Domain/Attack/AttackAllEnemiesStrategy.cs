using System.Collections.Generic;

// 역할: AttackAllEnemiesStrategy의 책임을 정의한다.
public class AttackAllEnemiesStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        return new AllEnemiesAttackTargetSelector().SelectTargets(attacker, targetPlates, selectedPlateIndex);
    }
}
