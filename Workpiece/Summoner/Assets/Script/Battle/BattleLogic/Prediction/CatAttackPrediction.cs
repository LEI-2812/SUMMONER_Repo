using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Cat;
    }

    //고양이 예측공격
    public AttackPrediction getAttackPrediction(Summon cat, int catPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        // 일반 공격으로 처치가 가능하면 일반 공격 확률 10% 증가
        if (getIndexOfNormalAttackCanKill(cat, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "고양이 일반공격으로 처치 가능");
            attackIndex = getIndexOfNormalAttackCanKill(cat, enermyPlates); //일반 공격으로 처치가능한 인덱스 받기
        }
        else
        {
            // 특수 공격으로 처치가 가능하면 특수 공격 확률 증가
            if (getIndexOfSpecialCanKill(cat, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "고양이 특수공격으로 처치가능");
                attackIndex = getIndexOfSpecialCanKill(cat, enermyPlates); //특수 공격으로 처치가능한 인덱스 받기
            }
            else
            { //특수공격에 +5%
                if(getMostDamageAttack(cat) == AttackType.NormalAttack)
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true, "고양이 일반공격이 더 많은 데미지를 입힘");
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false, "고양이 특수공격이 더 많은 데미지를 입힘");
                }
                attackIndex = getClosestEnermyIndex(enermyPlates); //적 플레이트 중에서 가장 가까이 있는 인덱스 받기
            }
        }

        attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }

    public AttackType getMostDamageAttack(Summon attackingSummon)
    {

        // 사용 가능한 특수 공격 목록 가져오기
        IAttackStrategy[] availableSpecialAttacks = attackingSummon.getAvailableSpecialAttacks();

        // 각 특수 공격 확인
        foreach (IAttackStrategy specialAttack in availableSpecialAttacks)
        {
            //특수공격이 null이면 일반공격 반환
            if(availableSpecialAttacks == null)
            {
                return AttackType.NormalAttack;
            }

            //일반공격력이 더 쌔면 일반공격 반환 고양이는 단일타깃 공격이기때문에 공격력만 비교
            if (attackingSummon.getAttackPower() > specialAttack.getSpecialDamage())
                return AttackType.NormalAttack;
            else
                return AttackType.SpecialAttack;
            }

        return AttackType.NormalAttack;
    }


    // 일반 공격으로 적을 물리칠 수 있는 가장 가까운 인덱스를 반환하는 메소드
    public int getIndexOfNormalAttackCanKill(Summon cat, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && cat.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
            }
        }

        return -1; // 공격 가능한 적이 없으면 -1 반환
    }

    //특수스킬로 죽일 수 있는 인덱스 반환
    public int getIndexOfSpecialCanKill(Summon cat, List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null && cat.getHeavyAttackPower() >= enermySummon.getNowHP())
            {
                // 일반 공격으로 적의 체력을 0 이하로 만들 수 있으면 해당 인덱스 반환
                return i;
            }
        }
        return -1; // 공격 가능한 적이 없으면 -1 반환
    }


    public int getClosestEnermyIndex(List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                return i; // 가장 가까운(첫 번째로 발견된) 적 소환수의 인덱스 반환
            }
        }
        return -1; // 적 소환수가 없으면 -1 반환
    }

    // 확률 값을 설정하고 조정하여 반환하는 메소드
    private AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack, string reason)
    {
        if (isNormalAttack)
        {
            // 일반 공격 확률을 증가시키고, 특수 공격 확률을 그만큼 감소
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
            Debug.Log($"일반 공격 확률이 {AttackChange*100}% 증가하였습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        else
        {
            // 특수 공격 확률을 증가시키고, 일반 공격 확률을 그만큼 감소
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
            Debug.Log($"특수 공격 확률이 {AttackChange * 100}% 증가하였습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        return currentProbabilities;
    }

}
