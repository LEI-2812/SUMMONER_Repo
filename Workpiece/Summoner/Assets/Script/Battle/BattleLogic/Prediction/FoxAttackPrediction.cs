using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType getPreSummonType()
    {
        return SummonType.Fox;
    }

    //여우 예측공격
    public AttackPrediction getAttackPrediction(Summon fox, int foxPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = getClosestEnermyIndex(enermyPlates);
        List<Plate> targetPlate = enermyPlates;

        //소환수, 소환수의 플레이트 번호, 소환수의 특수공격첫번째, 특수공격배열 인덱스번호, 타겟플레이트, 타겟플레이트 변호, 확률
        AttackPrediction attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);


        if (getIndexOfSummonWithCurseStatus(playerPlates) != -1) //소환수중 저주상태에 걸려있는 몹이 있는가?
        {
            attackProbability = new AttackProbability(0f, 100f); //특수공격 100%
            attackIndex = getIndexOfSummonWithCurseStatus(playerPlates);
            attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (isTwoOrMoreEnemies(enermyPlates)) //적이 2마리 이상 존재하는가?
        {
            if (AllEnemiesHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
            {
                if (AllSummonsLowOrMediumRank(playerPlates)) //아군의 등급이 모두 하급과 중급인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "여우 아군 등급이 모두 하급과 중급");
                    targetPlate = playerPlates;
                }
            }
            else if (isAnyEnemyHealthDown30Percent(enermyPlates)) //적의 체력이 하나만 30% 아래인가
            {
                if (getIndexOfNormalAttackCanKill(fox, enermyPlates) != -1) //일반공격시 처치할 수 있는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "여우 일반공격시 처치가능");
                }
            }
        }
        else if (isOnlyOneEnemy(enermyPlates)) //적이 1마리 인가?
        {
            if (isAnyEnemyHealthOver70Percent(enermyPlates)) //적의 체력이 70% 이상인가?
            {
                if (AllSummonsLowOrMediumRank(playerPlates)) //아군의 등급이 모두 하급과 중급인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "여우 아군 등급이 모두 하급과 중급");
                    targetPlate = playerPlates;
                }
            }
            else if (getIndexOfNormalAttackCanKill(fox, enermyPlates) != -1) //일반공격시 몬스터를 물리칠 수 있는가?
            {
                if (getIndexOfNormalAttackCanKill(fox, enermyPlates) != -1) //일반공격시 처치할 수 있는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "여우 일반공격시 가까운적 처치 가능");
                }
            }
        }
        else
        {
            attackIndex = getIndexOfHighestAttackPower(playerPlates);
            attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }

        return attackPrediction;
    }

    // 소환수 중 저주 상태 이상에 걸려있는 몹이 존재하는가?
    public int getIndexOfSummonWithCurseStatus(List<Plate> playerPlates)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].getCurrentSummon();
            if (playerSummon != null && playerSummon.IsCursed())
            {
                return i; // 저주 상태에 걸린 소환수의 인덱스 반환
            }
        }
        return -1; // 저주 상태에 걸린 소환수가 없으면 -1 반환
    }

    // 적이 2마리 이상인가?
    public bool isTwoOrMoreEnemies(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) count++;
        }
        return count >= 2;
    }


    // 적이 1마리인가?
    public bool isOnlyOneEnemy(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.getCurrentSummon() != null) count++;
        }
        return count == 1;
    }

    // 적의 체력이 모두 50% 이상인가?
    public bool AllEnemiesHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.5)
            {
                return false;
            }
        }
        return true;
    }

    // 아군의 등급이 모두 하급과 중급인가?
    public bool AllSummonsLowOrMediumRank(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.getCurrentSummon();
            if (playerSummon != null && (playerSummon.getSummonRank() != SummonRank.Low && playerSummon.getSummonRank() != SummonRank.Medium))
            {
                return false;
            }
        }
        return true;
    }

    // 적의 체력이 하나만 30% 아래인가?
    public bool isAnyEnemyHealthDown30Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() < 0.3)
            {
                return true;
            }
        }
        return false;
    }

    // 적의 체력이 70% 이상인가?
    public bool isAnyEnemyHealthOver70Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.getCurrentSummon();
            if (enermySummon != null && enermySummon.getNowHP() / enermySummon.getMaxHP() > 0.7)
            {
                return true;
            }
        }
        return false;
    }

    // 가장 공격력이 높은 소환수의 플레이트 인덱스를 반환하는 메소드
    public int getIndexOfHighestAttackPower(List<Plate> playerPlates)
    {
        int highestAttackIndex = -1;
        double highestAttackPower = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].getCurrentSummon();
            if (summon != null)
            {
                double attackPower = summon.getAttackPower();
                if (attackPower > highestAttackPower)
                {
                    highestAttackPower = attackPower;
                    highestAttackIndex = i; // 가장 높은 공격력을 가진 소환수의 인덱스를 기록
                }
            }
        }

        return highestAttackIndex; // 가장 높은 공격력의 소환수 인덱스 반환
    }



    // 가장 가까운 적을 공격했을 때 물리칠 수 있는지 확인하는 메소드
    public int getIndexOfNormalAttackCanKill(Summon fox, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = getClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].getCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && fox.getAttackPower() >= closestEnermySummon.getNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
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
