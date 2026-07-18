using System.Collections.Generic;

// 역할: IAttackStrategy의 책임을 정의한다.
public interface IAttackStrategy
{
    List<Summon> SelectTargets(AttackData attackData, AttackTargetInput input);
}
