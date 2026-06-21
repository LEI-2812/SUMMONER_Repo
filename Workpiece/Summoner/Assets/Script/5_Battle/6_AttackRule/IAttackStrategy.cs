using System.Collections.Generic;

// 역할: 공격 전략이 실행, 대상 방향, 피해/상태 값, 쿨타임 정보를 제공하기 위한 계약을 정의한다.
public interface IAttackStrategy
{
    // 역할: 공격자와 대상 플레이트 정보를 받아 실제 공격 효과를 실행한다.
    void Attack(Summon attacker, IReadOnlyList<Plate> targetPlates, int selectedPlateIndex);

    // 역할: 공격이 적용할 상태 타입을 알려준다.
    StatusType GetStatusType();

    // 역할: 이 공격이 아군 플레이트를 대상으로 삼는지 알려준다.
    bool TargetsOwnPlates();

    // 역할: 공격 피해량, 회복 비율, 상태 효과 수치처럼 전략이 가진 효과 값을 알려준다.
    double GetSpecialDamage();

    // 역할: 공격 사용 후 적용할 기본 쿨타임을 알려준다.
    int GetCooltime();

    // 역할: 현재 남은 쿨타임을 알려준다.
    int GetCurrentCooldown();

    // 역할: 공격 사용 직후 쿨타임을 적용한다.
    void ApplyCooldown();

    // 역할: 턴 종료 시 남은 쿨타임을 줄인다.
    void ReduceCooldown();
}
