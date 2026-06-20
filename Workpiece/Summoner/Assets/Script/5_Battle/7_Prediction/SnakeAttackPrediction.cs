using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 뱀 소환수의 공격 선택 가능성을 예측한다.
public class SnakeAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType GetPreSummonType()
    {
        return SummonType.Snake;
    }

    // 역할: 뱀이 독 공격을 쓸 상황인지, 일반 공격이 더 나은 상황인지 예측한다.
    public AttackPrediction GetAttackPrediction(Summon snake, int snakePlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = GetClosestEnermyIndex(enermyPlates);

        if (IsEnermyAlreadyPoisoned(enermyPlates) || !CanUseSpecialAttack(snake))
        {
            attackProbability = new AttackProbability(100f, 0f);
            return new AttackPrediction(snake, snakePlateIndex, snake.GetAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability); //일반공격으로
        }

        if (IsEnermyCountOverTwo(enermyPlates)) //적이 2마리 이상인가?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "뱀 적이 2마리 이상");
            if (AllEnermyHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "뱀 적의 체력이 모두 50%이상");
            }
            else
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "뱀 적의 체력이 모두 50%가 아님");
            }
            if (HasMonsterWithMoreThan3Attacks(enermyPlates)) //몬스터 중 공격의 개수가 3개 이상인 몹이 존재하는가?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "뱀 적의 몬스터중 공격이 3개 이상인 몹이 있는가");
            }
        }
        else //1마리 일때
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "뱀 적이 1마리 뿐");
            if (AllEnermyHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "뱀 적의 체력이 50% 이상");
            }
            if (GetIndexOfNormalAttackCanKill(snake, enermyPlates) != -1) //일반 공격 시 몬스터를 물리칠 수 있는가?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "뱀 일반 공격시 처치가능");
            }
        }

        return new AttackPrediction(snake, snakePlateIndex, snake.GetSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
    }



    // 역할: 적 중 이미 독 상태인 소환수가 있는지 확인한다.
    public bool IsEnermyAlreadyPoisoned(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.GetCurrentSummon();
            if (enermySummon != null && enermySummon.GetAllStatusTypes().Contains(StatusType.Poison))
            {
                return true;
            }
        }
        return false;
    }


    // 역할: 뱀이 지금 사용할 수 있는 특수 공격을 가지고 있는지 확인한다.
    public bool CanUseSpecialAttack(Summon snake)
    {
        var availableSpecialAttacks = snake.GetAvailableSpecialAttacks();
        return availableSpecialAttacks.Length > 0;
    }



    // 역할: 살아있는 적들의 체력이 모두 절반 이상인지 확인한다.
    public bool AllEnermyHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.GetCurrentSummon();
            if (enermySummon != null)
            {
                double healthRatio = enermySummon.GetNowHP() / enermySummon.GetMaxHP();
                if (healthRatio < 0.5)
                {
                    return false; // 하나라도 50% 이하이면 false 반환
                }
            }
        }
        return true;
    }



    // 역할: 적 중 특수 공격을 여러 개 가진 소환수가 있는지 확인한다.
    public bool HasMonsterWithMoreThan3Attacks(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.GetCurrentSummon();
            if (enermySummon != null && enermySummon.GetSpecialAttackCount() >= 3)
            {
                return true;
            }
        }
        return false;
    }


    // 역할: 살아있는 적이 2마리 이상인지 확인한다.
    public bool IsEnermyCountOverTwo(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                count++;
                if (count >= 2) return true;
            }
        }
        return false;
    }

    // 역할: 살아있는 적이 하나라도 있는지 확인한다.
    public bool IsEnermyCountOnlyOne(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            if (plate.GetCurrentSummon() != null)
            {
                return true;
            }
        }
        return false;
    }


    // 역할: 가장 가까운 적을 일반 공격 한 번으로 처치할 수 있으면 그 적 위치를 돌려준다.
    public int GetIndexOfNormalAttackCanKill(Summon snake, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = GetClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].GetCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && snake.GetAttackPower() >= closestEnermySummon.GetNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
            }
        }

        return -1; // 공격 가능한 적이 없으면 -1 반환
    }


    // 역할: 적 플레이트를 앞에서부터 확인해 가장 먼저 만나는 적 위치를 찾는다.
    public int GetClosestEnermyIndex(List<Plate> enermyPlates)
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null)
            {
                return i; // 가장 가까운(첫 번째로 발견된) 적 소환수의 인덱스 반환
            }
        }
        return -1; // 적 소환수가 없으면 -1 반환
    }

    // 역할: 판단 이유에 따라 일반 공격 또는 특수 공격 확률을 한쪽으로 조금 이동시킨다.
    private AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack, string reason)
    {
        if (isNormalAttack)
        {
            // 일반 공격 확률을 증가시키고, 특수 공격 확률을 그만큼 감소
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
            Debug.Log($"일반 공격 확률이 {AttackChange}% 증가하였습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        else
        {
            // 특수 공격 확률을 증가시키고, 일반 공격 확률을 그만큼 감소
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
            Debug.Log($"특수 공격 확률이 {AttackChange}% 증가하였습니다. 이유: {reason}. 현재 확률: 일반 {currentProbabilities.normalAttackProbability}%, 특수 {currentProbabilities.specialAttackProbability}%");
        }
        return currentProbabilities;
    }
}
