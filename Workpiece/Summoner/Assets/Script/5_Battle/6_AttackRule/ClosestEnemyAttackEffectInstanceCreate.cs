using UnityEngine;

// 역할: 상태 타입에 맞는 근접 공격 효과 객체를 생성한다.
public static class ClosestEnemyAttackEffectInstanceCreate
{
    public static IAttackEffect Create(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.None:
                return new ClosestEnemyNormalAttackEffect();

            default:
                return new EmptyAttackEffect();
        }
    }

    // 역할: 가장 가까운 대상에게 현재 공격력 기반 피해를 적용한다.
    private class ClosestEnemyNormalAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
            Debug.Log($"{attacker.GetSummonName()}이(가) {target.GetSummonName()}을(를) 공격합니다.");
            // 버프/저주/강공격 전환이 반영된 현재 공격력을 사용한다.
            target.TakeDamage(attacker.GetAttackPower());
        }
    }

    // 역할: 해당 상태 타입에서 실제 효과가 없을 때 사용하는 빈 공격 효과다.
    private class EmptyAttackEffect : IAttackEffect
    {
        public bool BenefitEffectCheck() => false;

        public void AttackEffectApply(Summon attacker, Summon target, int specialAttackArrayIndex)
        {
        }
    }
}
