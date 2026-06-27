using System.Collections.Generic;

// 역할: TargetedAttackStrategy의 책임을 정의한다.
public class TargetedAttackStrategy : IAttackStrategy
{
    public List<Summon> SelectTargets(AttackData attackData, Summon attacker, IReadOnlyList<BattleBoardInputController> targetPlates, int selectedPlateIndex)
    {
        if (attackData.StatusType == StatusType.Shield || attackData.StatusType == StatusType.OnceInvincibility)
        {
            return new SelfAttackTargetSelector().SelectTargets(attacker, targetPlates, selectedPlateIndex);
        }

        return new SelectedPlateAttackTargetSelector().SelectTargets(attacker, targetPlates, selectedPlateIndex);
    }
}
