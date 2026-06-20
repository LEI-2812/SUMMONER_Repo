// 역할: 선택된 대상에게 피해나 상태 효과를 적용하는 공격 효과 계약을 정의한다.
public interface IAttackEffect
{
    bool BenefitEffectCheck();
    void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex);
}
