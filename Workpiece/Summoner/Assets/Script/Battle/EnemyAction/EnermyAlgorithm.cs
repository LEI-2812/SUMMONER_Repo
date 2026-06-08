using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackProbability
{
    public float normalAttackProbability;
    public float specialAttackProbability;

    // 생성자를 추가하여 초기화할 수 있도록 함
    public AttackProbability(float normalProb, float specialProb)
    {
        normalAttackProbability = normalProb;
        specialAttackProbability = specialProb;
    }
}


public class EnermyAlgorithm : MonoBehaviour
{
    [SerializeField] private PlateController plateController;
    [SerializeField] private PlayerAttackPrediction playerAttackPrediction;
    [SerializeField] private BattleController battleController;

    private List<AttackPrediction> playerAttackPredictionsList;


    //알고리즘 순서대로 실행
    public List<AttackPrediction> HandleReactPrediction(Summon attackingEnermySummon, int attackingEnermyPlateIndex, List<AttackPrediction> playerAttackPredictionsList)
    {

        if (playerAttackPredictionsList.Count == 0)
        {
            // 리스트가 비어있으면 일반 공격으로 처리
            HandleReactNormalAttack(attackingEnermySummon, attackingEnermyPlateIndex, plateController.GetClosestPlayerPlateIndex()); // 기본 타겟 플레이트 인덱스 사용
            Debug.Log("리스트가 비어서 일반공격 대응");
        }
        else
        {
            Debug.Log("대응공격 중...");
            int indexToRemove = CanReactWithSpecialAttack(attackingEnermySummon, attackingEnermyPlateIndex, playerAttackPredictionsList);
            if (indexToRemove != -1) //특수공격으로 대응이 가능할경우
            {
                // 특수공격으로 대응한 경우 해당 항목을 리스트에서 제거
                playerAttackPredictionsList.RemoveAt(indexToRemove);
                Debug.Log("특수공격 대응 완료, 리스트에서 항목 제거");
            }
            else
            {
                // 특수공격이 불가능한 경우 일반공격으로 대응
                HandleReactNormalAttack(attackingEnermySummon, attackingEnermyPlateIndex, plateController.GetClosestPlayerPlateIndex());
                Debug.Log("특수공격에 대한 대응공격이 없거나 확률이 걸렸습니다. 일반공격으로 대응");
            }
        }

        return playerAttackPredictionsList; // 변경된 리스트 반환
    }


    // 특수공격으로 대응이 가능한지 검사하는 메소드
    private int CanReactWithSpecialAttack(Summon attacker, int attackingEnermyPlateIndex, List<AttackPrediction> playerAttackPredictionsList)
    {
        int indexToRemove = -1;
        for (int i = 0; i < playerAttackPredictionsList.Count; i++)
        {
            AttackPrediction playerPrediction = playerAttackPredictionsList[i];
            AttackProbability preAttackProbability = playerPrediction.GetAttackProbability(); // 확률


            if (CanReactSpecialAttack(preAttackProbability)) // 특수공격으로 공격할지
            {
                bool specialAttackExecuted = HandleReactSpecialAttack(attacker, attackingEnermyPlateIndex, playerPrediction);
                if (specialAttackExecuted)
                {
                    return i; // 특수 공격 성공 시 해당 인덱스를 반환
                }
            }
        }
        return indexToRemove; // 특수 공격에 성공하지 못한 경우
    }



    //특수공격에 대한 대응
    private bool HandleReactSpecialAttack(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        if(playerPrediction.GetAttackStrategy() is AttackAllEnemiesStrategy allAttackStrategy) //예측한 공격타입이 전체공격인지 검사
        {
            if(allAttackStrategy.GetStatusType() == StatusType.Poison)
            {
               return ReactSpecialFromPoison(attacker , attackingEnermyPlateIndex,playerPrediction); //독성에 대한 대응
            }
            else if(allAttackStrategy.GetStatusType() == StatusType.None)
            {
                return ReactSpecialFromAllNone(attacker, attackingEnermyPlateIndex, playerPrediction); //전체공격 대응
            }
        }
        else if(playerPrediction.GetAttackStrategy() is TargetedAttackStrategy targetAttackStrategy) //예측한 공격타입이 타겟공격인지 검사
        {
            if (targetAttackStrategy.GetStatusType() == StatusType.None)
            {
                return ReactSpecialFromTargetNone(attacker, attackingEnermyPlateIndex, playerPrediction); //저격공격 대응

            }
            else if (targetAttackStrategy.GetStatusType() == StatusType.Upgrade)
            {
                return ReactSpecialFromUpgrade(attacker, attackingEnermyPlateIndex, playerPrediction);

            }
            else if (targetAttackStrategy.GetStatusType() == StatusType.Heal)
            {
               return ReactSpecialFromHeal(attacker, attackingEnermyPlateIndex, playerPrediction);

            }
        }
        else if (playerPrediction.GetAttackStrategy() is ClosestEnemyAttackStrategy)//예측한 공격이 근접공격일경우 (24.10.20 기준 cat뿐)
        { //가독성을 위해 else 말고 elseif사용
            return false; //고양이는 특수공격 대응에 없으므로
        }
        else
        {
            Debug.Log("적 소환수의 특수공격 대응에서 공격이 잘못 들어옴");
            return false;
        }


        Debug.Log("적 소환수의 특수공격 대응 조건에 맞는 분기가 없습니다.");
        return false;
    }



    private bool ReactSpecialFromPoison(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.GetSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            HandleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.GetTargetPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 스킬이 없어서 일반 공격을 실행했습니다.");
            return true;
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].GetStatusType() == StatusType.Heal) //타겟플레이트가 적이 되어야함
                {
                    int targetPlateIndex = plateController.GetLowestHealthEnermyPlateIndex();
                    battleController.SpecialAttackExecute(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.GetSummonName()}가 힐 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //저격공격인지 검사
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i); //저격공격
                    Debug.Log($"{attacker.GetSummonName()}가 특수 저격 공격을 실행했습니다.");
                    return true;
                }
            }

        }
        return false;
    }

    private bool ReactSpecialFromTargetNone(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.GetSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            HandleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return true;
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].GetStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.GetSummonName()}가 스턴 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackExecute(attacker, attackingEnermyPlateIndex, i);
                    Debug.Log($"{attacker.GetSummonName()}가 쉴드 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackExecute(attacker, attackingEnermyPlateIndex, i);
                    Debug.Log($"{attacker.GetSummonName()}가 힐 특수 공격을 실행했습니다.");
                    return true;
                }
            }
        }
        return false;
    }

    //전체 공격 대응
    private bool ReactSpecialFromAllNone(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.GetSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            HandleReactNormalAttack(attacker, attackingEnermyPlateIndex,playerPrediction.GetAttackSummonPlateIndex()); //전체공격을 사용한 적에게 일반공격대응
            Debug.Log($"{attacker.GetSummonName()}가 일반 대응했습니다.");
            return true;
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].GetStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i); //공격한 대상에게 스턴

                    Debug.Log($"{attacker.GetSummonName()}가 스턴 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.Shield) //가장 체력이 낮은 적에게 쉴드
                {
                    battleController.SpecialAttackExecute(attacker, attackingEnermyPlateIndex, i); //자기 자신에게 쉴드
                    Debug.Log($"{attacker.GetSummonName()}가 쉴드 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackExecute(attacker, attackingEnermyPlateIndex, i); //자기자신에게 힐
                    Debug.Log($"{attacker.GetSummonName()}가 힐 특수 스킬을 실행했습니다.");
                    return true;
                }
            }

           }
        return false;
    }
    private bool ReactSpecialFromHeal(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.GetSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            HandleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return true;
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].GetStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //타겟공격인지 검사
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.GetSummonName()}가 특수 저격 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //전체공격인지 검사
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.GetSummonName()}가 특수 전체 공격을 실행했습니다.");
                    return true;
                }
            }

        }
        return false;
    }
    private bool ReactSpecialFromUpgrade(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.GetSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            HandleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return true;
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].GetStatusType() == StatusType.Curse) //저주공격인지 검사
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i);
                    //specialAttackExecuted = true;
                    Debug.Log($"{attacker.GetSummonName()}가 특수 저주 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //저격공격인지 검사
                {
                    battleController.SpecialAttackExecute(attacker, playerPrediction.GetAttackSummonPlateIndex(), i);
                   // specialAttackExecuted = true;
                    Debug.Log($"{attacker.GetSummonName()}가 특수 저격 공격을 실행했습니다.");
                    return true;
                }
            }

        }
        return false;
    }


    private void HandleReactNormalAttack(Summon attacker, int attackingEnermyPlateIndex, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.GetSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기
        float randomValue;
        if (attackStrategy == null)
        {
            randomValue = UnityEngine.Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값
            if (randomValue < 30f) //강공격
            {
                Debug.Log($"{attacker.name} 의 강공격");
                double originPower = attacker.GetAttackPower();
                attacker.SetAttackPower(attacker.GetHeavyAttackPower()); //공격력을 강공격력으로 전환
                attacker.NormalAttack(plateController.GetPlayerPlates(), plateController.GetClosestPlayerPlateIndexExcept(attacker)); //일반공격수행
                attacker.SetAttackPower(originPower); //원래 공격력으로 되돌리기
            }
            else //일반 공격력으로 공격
            {
                attacker.NormalAttack(plateController.GetPlayerPlates(), targetPlateIndex); //일반공격 수행
            }
            Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");
            return;
        }

        if (plateController.GetPlayerSummonCount() >= 2) //소환수가 2마리 이상 존재하는가?
        {
            int index = Get30PercentDifferentHP(plateController.GetPlayerPlates());
            if (index != -1) //소환수들중 체력차이가 30%이상 차이나는 소환수가 있는가? 있을경우 체력이 높은쪽의 인덱스 반환
            {
                Debug.Log("소환수2마리 이상중 30%이상인 인덱스: " + index);
                for (int i = 0; i < attackStrategy.Length; i++)
                {
                    if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                    {
                        continue;
                    }
                    if (attackStrategy[i].GetStatusType() == StatusType.LifeDrain) //흡혈 공격인지 검사
                    {
                        battleController.SpecialAttackExecute(attacker, index, i);
                        Debug.Log($"{attacker.GetSummonName()}가 특수 흡혈 공격을 실행했습니다.");
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < attackStrategy.Length; i++)
                {
                    if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                    {
                        continue;
                    }
                    if (attackStrategy[i].GetStatusType() == StatusType.Upgrade) //강화 공격인지 검사
                    {
                        battleController.SpecialAttackExecute(attacker, targetPlateIndex, i);
                        Debug.Log($"{attacker.GetSummonName()}가 특수 강화 공격을 실행했습니다.");
                        return;
                    }
                    else if (attackStrategy[i].GetStatusType() == StatusType.Burn) //화상 공격인지 검사
                    {
                        battleController.SpecialAttackExecute(attacker, targetPlateIndex, i);
                        Debug.Log($"{attacker.GetSummonName()}가 특수 화상 공격을 실행했습니다.");
                        return;
                    }
                    else if (attackStrategy[i].GetStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //전체 공격인지 검사
                    {
                        battleController.SpecialAttackExecute(attacker, targetPlateIndex, i);
                        Debug.Log($"{attacker.GetSummonName()}가 특수 전체 공격을 실행했습니다.");
                        return;
                    }
                }
            }
        }
        if (HasPlayerSummonOverMediumRank(plateController.GetPlayerPlates())) //소환수 등급이 중급 이상인 몹이 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.IsSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].GetStatusType() == StatusType.Curse)
                {
                    battleController.SpecialAttackExecute(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.GetSummonName()}가 특수 저주 공격을 실행했습니다.");
                    return;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackExecute(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.GetSummonName()}가 특수 혼란 공격을 실행했습니다.");
                    return;
                }
                else if (attackStrategy[i].GetStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackExecute(attacker, attackingEnermyPlateIndex, i); //자기 자신에게 쉴드
                    Debug.Log($"{attacker.GetSummonName()}가 특수 쉴드 스킬을 실행했습니다.");
                    return;
                }
            }
        }
        


        randomValue = UnityEngine.Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값
        if (randomValue < 30f) //강공격
        {
            Debug.Log($"{attacker.name} 의 강공격");
            double originPower = attacker.GetAttackPower();
            attacker.SetAttackPower(attacker.GetHeavyAttackPower()); //공격력을 강공격력으로 전환
            attacker.NormalAttack(plateController.GetPlayerPlates(), plateController.GetClosestPlayerPlateIndexExcept(attacker)); //일반공격수행
            attacker.SetAttackPower(originPower); //원래 공격력으로 되돌리기
        }
        else //일반 공격력으로 공격
        {
            attacker.NormalAttack(plateController.GetPlayerPlates(), targetPlateIndex); //일반공격 수행
        }
        Debug.Log($"{attacker.GetSummonName()}가 일반 공격을 실행했습니다.");

    }


    private bool HasPlayerSummonOverMediumRank(List<Plate> plates)
    {
        foreach (Plate plate in plates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon != null && (summon.GetSummonRank() == SummonRank.Medium || summon.GetSummonRank() == SummonRank.High ))
            {
                return true; // 중급 이상인 소환수가 존재하면 true 반환
            }
        }
        return false; // 중급 이상인 소환수가 없으면 false 반환
    }

    private int Get30PercentDifferentHP(List<Plate> playerplates)
    {
        if (playerplates.Count < 2) return -1;

        int highestHealthIndex = -1;
        double highestHealth = double.MinValue;

        // 1. 가장 현재 체력이 높은 소환수 찾기
        for (int i = 0; i < playerplates.Count; i++)
        {
            Summon currentSummon = playerplates[i].GetCurrentSummon();
            if (currentSummon == null) continue;

            if (currentSummon.GetNowHP() > highestHealth)
            {
                highestHealth = currentSummon.GetNowHP();
                highestHealthIndex = i;
            }
        }

        // 가장 높은 체력의 소환수를 찾지 못한 경우
        if (highestHealthIndex == -1) return -1;

        // 2. 나머지 소환수들 사이에서 가장 높은 체력보다 30% 낮은 소환수 찾기
        for (int i = 0; i < playerplates.Count; i++)
        {
            if (i == highestHealthIndex) continue; // 가장 높은 체력의 소환수는 비교에서 제외

            Summon compareSummon = playerplates[i].GetCurrentSummon();
            if (compareSummon == null) continue;

            // 체력 비교: 가장 높은 소환수의 체력보다 30% 낮은지 체크
            if (compareSummon.GetNowHP() <= highestHealth * 0.7)
            {
                // 조건을 만족하면 가장 높은 체력의 소환수 인덱스를 반환
                return highestHealthIndex;
            }
        }

        return -1; // 조건을 만족하는 소환수가 없으면 -1 반환
    }

    private int GetLowestMonsterIndex(List<Plate> enermyPlates)
    {
        int lowestIndex=-1;

        for(int i = 0; i<enermyPlates.Count; i++)
        {

        }

        return lowestIndex;
    }

    //private int Get30PercentDifferentHP(Summon attacker, List<Plate> targetPlates)
    //{
    //    double attackerHealthRatio = (double)attacker.GetNowHP() / attacker.GetMaxHP();

    //    for (int i = 0; i < targetPlates.Count; i++)
    //    {
    //        Summon enermySummon = targetPlates[i].GetCurrentSummon();
    //        if (enermySummon != null)
    //        {
    //            double enermyHealthRatio = (double)enermySummon.GetNowHP() / enermySummon.GetMaxHP();

    //            // 체력 차이가 30% 이상인 경우 인덱스 반환
    //            if (Math.Abs(attackerHealthRatio - enermyHealthRatio) >= 0.3)
    //            {
    //                return i;
    //            }
    //        }
    //    }

    //    return -1; // 조건을 만족하는 소환수가 없으면 -1 반환
    //}


    private bool CanReactSpecialAttack(AttackProbability attackProbability)
    {
        // 0에서 100 사이의 랜덤 값을 생성
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if(randomValue < attackProbability.specialAttackProbability)
        {
            Debug.Log("특수공격확률 당첨");
        }

        // 특수 공격 확률이 랜덤 값보다 크면 true 반환 (특수 공격 선택)
        return randomValue < attackProbability.specialAttackProbability;
    }


    //플레이어의 예측공격을 리스트 생성로직

    public List<AttackPrediction> GetPlayerAttackPredictionsList()
    {
        // 1. 소환수의 상태 체크
        List<Plate> playerPlates = CheckPlayerPlateState(); // 현재 playerPlates들

        // 2. 몬스터의 상태 체크 (새 리스트에 상태 조정된 enermyPlates 추가)
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

        //3. 소환수의 예측공격 리스트를 받아온다.
        playerAttackPredictionsList = playerAttackPrediction.GetPlayerAttackPredictionList(playerPlates, applyEnermyPlates);

        return playerAttackPredictionsList;
    }


    // 1. 현재 playerPlate들을 공격이나 데미지 적용이 안되게 새 리스트로 가져온다.
    public List<Plate> CheckPlayerPlateState()
    {
        List<Plate> playerPlateStates = new List<Plate>();
        List<Plate> playerPlates = plateController.GetPlayerPlates();

        for (int i = 0; i < playerPlates.Count; i++) // 인덱스를 이용해 순회
        {
            Plate plate = playerPlates[i];
            Summon summon = plate.GetCurrentSummon();

            if (summon != null)
            {
                Debug.Log($"{summon.GetSummonName()}이 리스트로 들어감");
                // 기존 플레이트를 새 리스트에 추가 (상태만 관리)
                playerPlateStates.Add(plate);
            }
        }

        return playerPlateStates;
    }
    // 2. 적 몬스터의 상태를 조정하여 새로운 플레이트 리스트 반환
    private List<Plate> GetApplyStatusEnermyPlates()
    {
        List<Plate> applyEnermyPlates = new List<Plate>();

        foreach (Plate plate in plateController.GetEnermyPlates())
        {
            Summon originSummon = plate.GetCurrentSummon();
            if (originSummon != null)
            {
                // Summon 객체만 복제하고 상태 효과를 적용
                Summon clonedSummon = originSummon.Clone();
                Summon adjustedSummon = ApplyEnermyStatus(clonedSummon);

                // 원본 Plate에 임시로 복제된 Summon 설정
                plate.SetCurrentSummon(adjustedSummon);
                applyEnermyPlates.Add(plate);

                // 원본 Summon으로 복원
                plate.SetCurrentSummon(originSummon);
            }
            else
            {
                applyEnermyPlates.Add(plate); // 소환수가 없으면 그대로 추가
            }
        }

        return applyEnermyPlates;
    }

    // 복제된 Summon에 상태 효과 적용
    private Summon ApplyEnermyStatus(Summon clonedSummon)
    {
        if (clonedSummon.GetAllStatusTypes().Contains(StatusType.Poison))
        {
            clonedSummon.SetNowHP(clonedSummon.GetNowHP() - clonedSummon.GetMaxHP() * 0.1);
            if (clonedSummon.GetNowHP() <= 0) return null;
        }

        if (clonedSummon.GetAllStatusTypes().Contains(StatusType.Burn))
        {
            clonedSummon.SetNowHP(clonedSummon.GetNowHP() - clonedSummon.GetMaxHP() * 0.2);
            if (clonedSummon.GetNowHP() <= 0) return null;
        }

        if (clonedSummon.GetAllStatusTypes().Contains(StatusType.LifeDrain))
        {
            clonedSummon.SetNowHP(clonedSummon.GetNowHP() - clonedSummon.GetMaxHP() * 0.2);
            if (clonedSummon.GetNowHP() <= 0) return null;
        }

        return clonedSummon; // 상태가 적용된 복제본 Summon 반환
    }


    //// 2. 적 몬스터의 상태를 조정하여 새로운 플레이트 리스트 반환
    //private List<Plate> GetApplyStatusEnermyPlates()
    //{
    //    List<Plate> applyEnermyPlates = new List<Plate>(); // 새 리스트

    //    foreach (Plate plate in plateController.GetEnermyPlates()) // enermyPlates를 하나씩 가져온다
    //    {
    //        Summon originSummon = plate.GetCurrentSummon(); // 해당 플레이트의 소환수를 가져와서
    //        if (originSummon != null)
    //        {
    //            Summon copySummon = originSummon;
    //            // 기존 몬스터를 가져와 상태를 조정 후 새 플레이트 리스트에 추가
    //            Summon applySummon = ApplyEnermyStatus(copySummon);
    //            plate.setCurrentSummon(applySummon); //null이여도 넣어줌. 죽었을땐 null이므로 공격대상이 되지 않게
    //            applyEnermyPlates.Add(plate);
    //        }
    //    }

    //    return applyEnermyPlates;
    //}

    //// 2.(1) 몬스터 상태에 따라 수치 조정
    //private Summon ApplyEnermyStatus(Summon enermySummon)
    //{
    //    // 독성: 최대 체력의 10% 데미지 적용
    //    if (enermySummon.GetAllStatusTypes().Contains(StatusType.Poison))
    //    {
    //        enermySummon.SetNowHP(enermySummon.GetNowHP() - enermySummon.GetMaxHP() * 0.1); // 10% 체력 감소
    //        if (enermySummon.GetNowHP() <= 0) return null; // 체력이 0 이하라면 제외
    //    }

    //    // 화상: 최대 체력의 20% 데미지 적용
    //    if (enermySummon.GetAllStatusTypes().Contains(StatusType.Burn))
    //    {
    //        enermySummon.SetNowHP(enermySummon.GetNowHP() - enermySummon.GetMaxHP() * 0.2); // 20% 체력 감소
    //        if (enermySummon.GetNowHP() <= 0) return null; // 체력이 0 이하라면 제외
    //    }

    //    // 흡혈: 최대 체력의 20% 데미지 적용
    //    if (enermySummon.GetAllStatusTypes().Contains(StatusType.LifeDrain))
    //    {
    //        enermySummon.SetNowHP(enermySummon.GetNowHP() - enermySummon.GetMaxHP() * 0.2); // 20% 체력 감소
    //        if (enermySummon.GetNowHP() <= 0) return null; // 체력이 0 이하라면 제외
    //    }

    //    return enermySummon; // 상태 적용된 몬스터 반환
    //}



    public PlateController GetPlateController()
    {
        return this.plateController;
    }
}
