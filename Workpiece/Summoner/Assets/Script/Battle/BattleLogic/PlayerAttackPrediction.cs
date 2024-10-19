using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPrediction : MonoBehaviour
{
    private PlateController plateController;

    public AttackProbability catAttackProbability(List<Plate> playerSummons, List<Plate> enermySummons, AttackProbability attackProbability)
    {
        AttackProbability recalculatedProbability = attackProbability; // 기본 50% 50%

        // 1. 일반 공격 시 몬스터를 물리칠 수 있는지 여부를 판단
        if (CanDefeatWithNormalAttack(playerSummons, enermySummons))
        {
            recalculatedProbability.normalAttackProbability += 0.1f; // 일반 공격 확률 +10%
        }

        // 2. 각 소환수의 특수 공격 전략을 순회하여 조건을 체크
        foreach (Plate plate in playerSummons)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                IAttackStrategy[] specialAttackStrategies = summon.getAvailableSpecialAttacks(); //사용가능한 스킬들을 반환

                foreach (IAttackStrategy specialAttack in specialAttackStrategies)
                {
                    if (CanDefeatWithSpecialAttack(specialAttack, enermySummons))
                    {
                        recalculatedProbability.specialAttackProbability += 0.1f; // 특수 공격 확률 +10%
                    }
                }
            }
        }

        // 3. 피해를 많이 줄 수 있는 공격에 추가적인 확률을 부여
        if (recalculatedProbability.normalAttackProbability > recalculatedProbability.specialAttackProbability)
        {
            recalculatedProbability.normalAttackProbability += 0.05f; // 일반 공격 확률 +5%
        }
        else
        {
            recalculatedProbability.specialAttackProbability += 0.05f; // 특수 공격 확률 +5%
        }

        return recalculatedProbability;
    }

    // 일반 공격으로 적을 물리칠 수 있는지 여부를 판단하는 메서드
    private bool CanDefeatWithNormalAttack(List<Plate> playerSummons, List<Plate> enermySummons)
    {
        // 일반 공격으로 적 소환수를 물리칠 수 있는지 확인하는 로직 추가
        // 예시 로직: 가장 가까운 적 소환수를 타겟으로
        foreach (Plate enermyPlate in enermySummons)
        {
            Summon enermySummon = enermyPlate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() < 50) // 임의의 조건 예시
            {
                return true;
            }
        }
        return false;
    }

    // 특정 특수 공격 전략으로 적을 물리칠 수 있는지 여부를 판단하는 메서드
    private bool CanDefeatWithSpecialAttack(IAttackStrategy specialAttack, List<Plate> enermySummons)
    {
        // 특수 공격으로 적 소환수를 물리칠 수 있는지 확인하는 로직 추가
        foreach (Plate enermyPlate in enermySummons)
        {
            Summon enermySummon = enermyPlate.getCurrentSummon();
            //if (enermySummon != null && specialAttack.getCalculateDamage() > enermySummon.getNowHP()) // 특수 공격이 적 체력보다 큰지 확인
            //{
            //    return true;
            //}
        }
        return false;
    }
}
