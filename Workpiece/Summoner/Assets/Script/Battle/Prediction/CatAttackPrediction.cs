using System.Collections.Generic;
using UnityEngine;

public class CatAttackPrediction : MonoBehaviour, IAttackPrediction
{
    public SummonType GetPreSummonType()
    {
        return SummonType.Cat;
    }

    // 고양이 예측 공격
    public AttackPrediction GetAttackPrediction(Summon cat, int catPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = GetClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.GetSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        // 일반 공격으로 처치 가능하면 일반 공격 확률 10% 증가
        if (GetIndexOfNormalAttackCanKill(cat, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "고양이가 일반 공격으로 처치 가능");
            attackIndex = GetIndexOfNormalAttackCanKill(cat, enermyPlates); // 일반 공격으로 처치 가능한 인덱스 받기
        }
        else
        {
            // 특수 공격으로 처치 가능하면 특수 공격 확률 증가
            if (GetIndexOfSpecialCanKill(cat, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "고양이가 특수 공격으로 처치 가능");
                attackIndex = GetIndexOfSpecialCanKill(cat, enermyPlates); // 특수 공격으로 처치 가능한 인덱스 받기
            }
            else
            {
                // 더 강한 공격의 확률 5% 증가
                if (GetMostDamageAttack(cat) == AttackType.NormalAttack)
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "고양이 일반 공격이 더 많은 대미지를 입힘");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "고양이 특수 공격이 더 많은 대미지를 입힘");
                }
                attackIndex = GetClosestEnermyIndex(enermyPlates); // 적 플레이트 중 가장 가까이 있는 인덱스 받기
            }
        }

        attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.GetSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }

    public AttackType GetMostDamageAttack(Summon attackingSummon)
    {
        // 사용 가능한 특수 공격 목록 가져오기
        IAttackStrategy[] availableSpecialAttacks = attackingSummon.GetAvailableSpecialAttacks();

        // 각 특수 공격 확인
        foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
        {
            // 특수 공격이 null이면 일반 공격 반환
            if (availableSpecialAttacks == null)
            {
                return AttackType.NormalAttack;
            }

            // 일반 공격력이 더 세면 일반 공격 반환. 고양이는 단일 타깃 공격이기 때문에 공격력만 비교
            if (attackingSummon.GetAttackPower() > specialAttack.GetSpecialDamage())
                return AttackType.NormalAttack;
            else
                return AttackType.SpecialAttack;
        }

        return AttackType.NormalAttack;
    }

    // 일반 공격으로 적을 물리칠 수 있는 가장 가까운 인덱스를 반환하는 메서드
    public int GetIndexOfNormalAttackCanKill(Summon cat, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = GetClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].GetCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && cat.GetAttackPower() >= closestEnermySummon.GetNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
            }
        }

        return -1; // 공격 가능한 적이 없으면 -1 반환
    }

    // 특수 공격으로 죽일 수 있는 인덱스 반환
    public int GetIndexOfSpecialCanKill(Summon cat, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null && cat.GetHeavyAttackPower() >= enermySummon.GetNowHP())
            {
                // 특수 공격으로 적의 체력을 0 이하로 만들 수 있으면 해당 인덱스 반환
                return i;
            }
        }
        return -1; // 공격 가능한 적이 없으면 -1 반환
    }

    public int GetClosestEnermyIndex(List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null)
            {
                return i; // 가장 가까운, 처음 발견한 적 소환수의 인덱스 반환
            }
        }
        return -1; // 적 소환수가 없으면 -1 반환
    }

    // 확률 값을 설정하고 조정하여 반환하는 메서드
    private AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack, string reason)
    {
        if (isNormalAttack)
        {
            // 일반 공격 확률을 증가시키고 특수 공격 확률을 그만큼 감소
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
            Debug.Log($"일반 공격 확률을 {AttackChange}% 증가시켰습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        else
        {
            // 특수 공격 확률을 증가시키고 일반 공격 확률을 그만큼 감소
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
            Debug.Log($"특수 공격 확률을 {AttackChange}% 증가시켰습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        return currentProbabilities;
    }
}
