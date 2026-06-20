using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 여우 소환수의 공격 선택 가능성을 예측한다.
public class FoxAttackPrediction : MonoBehaviour, IAttackPrediction
{

    public SummonType GetPreSummonType()
    {
        return SummonType.Fox;
    }

    // 역할: 여우가 적을 공격할지 아군 저주 해제를 노릴지 판단해 다음 행동을 예측한다.
    public AttackPrediction GetAttackPrediction(Summon fox, int foxPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = GetClosestEnermyIndex(enermyPlates);
        List<Plate> targetPlate = enermyPlates;

        int cursedSummonIndex = GetIndexOfSummonWithCurseStatus(playerPlates);

        if (cursedSummonIndex != -1) //소환수중 저주상태에 걸려있는 몹이 있는가?
        {
            attackProbability = new AttackProbability(0f, 100f); //특수공격 100%
            return new AttackPrediction(fox, foxPlateIndex, fox.GetSpecialAttackStrategy()[0], 0, playerPlates, cursedSummonIndex, attackProbability);
        }

        else if (IsTwoOrMoreEnemies(enermyPlates)) //적이 2마리 이상 존재하는가?
        {
            if (AllEnemiesHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
            {
                if (AllSummonsLowOrMediumRank(playerPlates)) //아군의 등급이 모두 하급과 중급인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "여우 아군 등급이 모두 하급과 중급");
                    targetPlate = playerPlates;
                    attackIndex = foxPlateIndex;
                }
            }
            else
            {
                int under30Index = IsAnyEnemyHealthDown30Percent(enermyPlates);

                if (under30Index != -1) //적의 체력이 하나만 30% 아래인가
                {
                    int normalAttack30PerKillIndex = GetIndexOfNormalAttack30PerCanKill(fox, enermyPlates, under30Index);

                    if (normalAttack30PerKillIndex != -1) //일반공격시 처치할 수 있는가?
                    {
                        attackIndex = under30Index;
                        attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "여우 일반공격시 처치가능");
                    }
                }
            }
        }
        else if (IsOnlyOneEnemy(enermyPlates)) //적이 1마리 인가?
        {
            if (IsAnyEnemyHealthOver70Percent(enermyPlates)) //적의 체력이 70% 이상인가?
            {
                if (AllSummonsLowOrMediumRank(playerPlates)) //아군의 등급이 모두 하급과 중급인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false, "여우 적 1마리 아군 등급이 모두 하급과 중급");
                    targetPlate = playerPlates;
                    attackIndex = foxPlateIndex;
                }
            }
            else
            {
                int normalAttackKillIndex = GetIndexOfNormalAttackCanKill(fox, enermyPlates);

                if (normalAttackKillIndex != -1) //일반공격시 몬스터를 물리칠 수 있는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true, "여우 적 1마리 일반공격시 가까운적 처치 가능");
                }
            }
        }
        else
        {
            attackIndex = GetIndexOfHighestAttackPower(playerPlates);
        }

        return new AttackPrediction(fox, foxPlateIndex, fox.GetSpecialAttackStrategy()[0], 0, targetPlate, attackIndex, attackProbability);
    }

    // 역할: 아군 중 저주 상태인 소환수가 있으면 그 위치를 찾는다.
    public int GetIndexOfSummonWithCurseStatus(List<Plate> playerPlates)
    {
        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon playerSummon = playerPlates[i].GetCurrentSummon();
            if (playerSummon != null && playerSummon.IsCursed())
            {
                return i; // 저주 상태에 걸린 소환수의 인덱스 반환
            }
        }
        return -1; // 저주 상태에 걸린 소환수가 없으면 -1 반환
    }

    // 역할: 살아있는 적이 2마리 이상인지 확인한다.
    public bool IsTwoOrMoreEnemies(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.GetCurrentSummon() != null) count++;
        }
        return count >= 2;
    }


    // 역할: 살아있는 적이 정확히 1마리인지 확인한다.
    public bool IsOnlyOneEnemy(List<Plate> enermyPlates)
    {
        int count = 0;
        foreach (Plate plate in enermyPlates)
        {
            if (plate.GetCurrentSummon() != null) count++;
        }
        return count == 1;
    }

    // 역할: 살아있는 적들의 체력이 모두 절반 이상인지 확인한다.
    public bool AllEnemiesHealthOver50(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.GetCurrentSummon();
            if (enermySummon != null && enermySummon.GetNowHP() / enermySummon.GetMaxHP() < 0.5)
            {
                return false;
            }
        }
        return true;
    }

    // 역할: 아군에 상급 소환수가 없는지 확인한다.
    public bool AllSummonsLowOrMediumRank(List<Plate> playerPlates)
    {
        foreach (Plate plate in playerPlates)
        {
            Summon playerSummon = plate.GetCurrentSummon();
            if (playerSummon != null && playerSummon.GetSummonRank() == SummonRank.High)
            {
                return false;
            }
        }
        return true;
    }

    // 역할: 체력이 30% 아래인 적이 딱 한 명이면 그 위치를 돌려준다.
    public int IsAnyEnemyHealthDown30Percent(List<Plate> enermyPlates)
    {
        int index = -1;
        int count = 0;

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].GetCurrentSummon();
            if (enermySummon != null && enermySummon.GetNowHP() / enermySummon.GetMaxHP() < 0.3)
            {
                count++;
                index = i; // 30% 이하인 소환수의 인덱스를 기록

                if (count > 1) return -1; // 30% 이하인 적이 2개 이상일 경우 -1 반환
            }
        }

        return count == 1 ? index : -1; // 30% 이하인 적이 정확히 하나일 때 해당 인덱스 반환, 아니면 -1
    }

    // 역할: 체력이 70%를 넘는 적이 하나라도 있는지 확인한다.
    public bool IsAnyEnemyHealthOver70Percent(List<Plate> enermyPlates)
    {
        foreach (Plate plate in enermyPlates)
        {
            Summon enermySummon = plate.GetCurrentSummon();
            if (enermySummon != null && enermySummon.GetNowHP() / enermySummon.GetMaxHP() > 0.7)
            {
                return true;
            }
        }
        return false;
    }

    // 역할: 아군 중 공격력이 가장 높은 소환수의 위치를 찾는다.
    public int GetIndexOfHighestAttackPower(List<Plate> playerPlates)
    {
        int highestAttackIndex = -1;
        double highestAttackPower = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon summon = playerPlates[i].GetCurrentSummon();
            if (summon != null)
            {
                double attackPower = summon.GetAttackPower();
                if (attackPower > highestAttackPower)
                {
                    highestAttackPower = attackPower;
                    highestAttackIndex = i; // 가장 높은 공격력을 가진 소환수의 인덱스를 기록
                }
            }
        }

        return highestAttackIndex; // 가장 높은 공격력의 소환수 인덱스 반환
    }



    // 역할: 체력이 30% 아래인 적이 가장 가까운 적이고 일반 공격으로 처치 가능한지 확인한다.
    public int GetIndexOfNormalAttack30PerCanKill(Summon fox, List<Plate> enermyPlates, int under30Index)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = GetClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].GetCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && fox.GetAttackPower() >= closestEnermySummon.GetNowHP())
            {
                if(closestIndex == under30Index)
                    return under30Index; // 공격으로 물리칠 수 있으면 인덱스 반환
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
    // 역할: 가장 가까운 적을 일반 공격 한 번으로 처치할 수 있으면 그 적 위치를 돌려준다.
    public int GetIndexOfNormalAttackCanKill(Summon fox, List<Plate> enermyPlates)
    {
        // 가장 가까운 적의 인덱스를 가져옴
        int closestIndex = GetClosestEnermyIndex(enermyPlates);

        if (closestIndex != -1)
        {
            Summon closestEnermySummon = enermyPlates[closestIndex].GetCurrentSummon();
            // 가장 가까운 적의 소환수가 있고, 일반 공격으로 물리칠 수 있는지 확인
            if (closestEnermySummon != null && fox.GetAttackPower() >= closestEnermySummon.GetNowHP())
            {
                return closestIndex; // 공격으로 물리칠 수 있으면 인덱스 반환
            }
        }

        return -1; // 공격 가능한 적이 없으면 -1 반환
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
