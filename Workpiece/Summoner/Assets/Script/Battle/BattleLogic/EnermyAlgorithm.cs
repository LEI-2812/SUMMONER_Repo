using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
            handleReactNormalAttack(attackingEnermySummon, attackingEnermyPlateIndex, plateController.getClosestPlayerPlateIndex()); // 기본 타겟 플레이트 인덱스 사용
            Debug.Log("리스트가 비어서 일반공격 대응");
        }
        else
        {
            Debug.Log("대응공격 중...");
            int indexToRemove = canReactWithSpecialAttack(attackingEnermySummon, attackingEnermyPlateIndex, playerAttackPredictionsList);
            if (indexToRemove != -1) //특수공격으로 대응이 가능할경우
            {
                // 특수공격으로 대응한 경우 해당 항목을 리스트에서 제거
                playerAttackPredictionsList.RemoveAt(indexToRemove);
                Debug.Log("특수공격 대응 완료, 리스트에서 항목 제거");
            }
            else
            {
                // 특수공격이 불가능한 경우 일반공격으로 대응
                handleReactNormalAttack(attackingEnermySummon, attackingEnermyPlateIndex, plateController.getClosestPlayerPlateIndex());
                Debug.Log("특수공격에 대한 대응공격이 없거나 확률이 걸렸습니다. 일반공격으로 대응");
            }
        }

        return playerAttackPredictionsList; // 변경된 리스트 반환
    }


    // 특수공격으로 대응이 가능한지 검사하는 메소드
    private int canReactWithSpecialAttack(Summon attacker, int attackingEnermyPlateIndex, List<AttackPrediction> playerAttackPredictionsList)
    {
        int indexToRemove = -1;
        for (int i = 0; i < playerAttackPredictionsList.Count; i++)
        {
            AttackPrediction playerPrediction = playerAttackPredictionsList[i];
            AttackProbability preAttackProbability = playerPrediction.getAttackProbability(); // 확률


            if (canReactSpecialAttack(preAttackProbability)) // 특수공격으로 공격할지
            {
                bool specialAttackExecuted = handleReactSpecialAttack(attacker, attackingEnermyPlateIndex, playerPrediction);
                if (specialAttackExecuted)
                {
                    return i; // 특수 공격 성공 시 해당 인덱스를 반환
                }
            }
        }
        return indexToRemove; // 특수 공격에 성공하지 못한 경우
    }



    //특수공격에 대한 대응
    private bool handleReactSpecialAttack(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        if(playerPrediction.getAttackStrategy() is AttackAllEnemiesStrategy allAttackStrategy) //예측한 공격타입이 전체공격인지 검사
        {
            if(allAttackStrategy.getStatusType() == StatusType.Poison)
            {
               return reactSpecialFromPoison(attacker , attackingEnermyPlateIndex,playerPrediction); //독성에 대한 대응
            }
            else if(allAttackStrategy.getStatusType() == StatusType.None)
            {
                return reactSpecialFromAllNone(attacker, attackingEnermyPlateIndex, playerPrediction); //전체공격 대응
            }
        }
        else if(playerPrediction.getAttackStrategy() is TargetedAttackStrategy targetAttackStrategy) //예측한 공격타입이 타겟공격인지 검사
        {
            if (targetAttackStrategy.getStatusType() == StatusType.None)
            {
                return reactSpecialFromTargetNone(attacker, attackingEnermyPlateIndex, playerPrediction); //저격공격 대응

            }
            else if (targetAttackStrategy.getStatusType() == StatusType.Upgrade)
            {
                return reactSpecialFromUpgrade(attacker, attackingEnermyPlateIndex, playerPrediction);

            }
            else if (targetAttackStrategy.getStatusType() == StatusType.Heal)
            {
               return reactSpecialFromHeal(attacker, attackingEnermyPlateIndex, playerPrediction);

            }
        }
        else if (playerPrediction.getAttackStrategy() is ClosestEnemyAttackStrategy)//예측한 공격이 근접공격일경우 (24.10.20 기준 cat뿐)
        { //가독성을 위해 else 말고 elseif사용
            return false; //고양이는 특수공격 대응에 없으므로
        }
        else
        {
            Debug.Log("적 소환수의 특수공격 대응에서 공격이 잘못 들어옴");
            return false;
        }


        return true;
    }



    private bool reactSpecialFromPoison(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getTargetPlateIndex());
            Debug.Log($"{attacker.getSummonName()}가 스킬이 없어서 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Heal) //타겟플레이트가 적이 되어야함
                {
                    int targetPlateIndex = plateController.getLowestHealthEnermyPlateIndex();
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 힐 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //저격공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i); //저격공격
                    Debug.Log($"{attacker.getSummonName()}가 특수 저격 공격을 실행했습니다.");
                    return true;
                }
            }

        }
        return false;
    }

    private bool reactSpecialFromTargetNone(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getAttackSummonPlateIndex());
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.getSummonName()}가 스턴 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 쉴드 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 힐 특수 공격을 실행했습니다.");
                    return true;
                }
            }
        }
        return false;
    }

    //전체 공격 대응
    private bool reactSpecialFromAllNone(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex,playerPrediction.getAttackSummonPlateIndex()); //전체공격을 사용한 적에게 일반공격대응
            Debug.Log($"{attacker.getSummonName()}가 일반 대응했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i); //공격한 대상에게 스턴

                    Debug.Log($"{attacker.getSummonName()}가 스턴 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield) //가장 체력이 낮은 적에게 쉴드
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i); //자기 자신에게 쉴드
                    Debug.Log($"{attacker.getSummonName()}가 쉴드 특수 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i); //자기자신에게 힐
                    Debug.Log($"{attacker.getSummonName()}가 힐 특수 스킬을 실행했습니다.");
                    return true;
                }
            }

           }
        return false;
    }
    private bool reactSpecialFromHeal(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getAttackSummonPlateIndex());
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //타겟공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 저격 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //전체공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 전체 공격을 실행했습니다.");
                    return true;
                }
            }

        }
        return false;
    }
    private bool reactSpecialFromUpgrade(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getAttackSummonPlateIndex());
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Curse) //저주공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    //specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 저주 공격을 실행했습니다.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //저격공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                   // specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 저격 공격을 실행했습니다.");
                    return true;
                }
            }

        }
        return true;
    }


    private void handleReactNormalAttack(Summon attacker, int attackingEnermyPlateIndex, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //해당 소환수의 스킬 가져오기
        float randomValue;
        if (attackStrategy == null)
        {
            randomValue = UnityEngine.Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값
            if (randomValue < 30f) //강공격
            {
                Debug.Log($"{attacker.name} 의 강공격");
                double originPower = attacker.getAttackPower();
                attacker.setAttackPower(attacker.getHeavyAttackPower()); //공격력을 강공격력으로 전환
                attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlatesIndex(attacker)); //일반공격수행
                attacker.setAttackPower(originPower); //원래 공격력으로 되돌리기
            }
            else //일반 공격력으로 공격
            {
                attacker.normalAttack(plateController.getPlayerPlates(), targetPlateIndex); //일반공격 수행
            }
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            return;
        }

        if (plateController.getPlayerSummonCount() >= 2) //소환수가 2마리 이상 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Upgrade) //강화 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 강화 공격을 실행했습니다.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Burn) //화상 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 화상 공격을 실행했습니다.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //화상 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 전체 공격을 실행했습니다.");
                    return;
                }
            }
        }
        if (hasPlayerSummonOverMediumRank(plateController.getPlayerPlates())) //소환수 등급이 중급 이상인 몹이 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Curse)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 저주 공격을 실행했습니다.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 혼란 공격을 실행했습니다.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i); //자기 자신에게 쉴드
                    Debug.Log($"{attacker.getSummonName()}가 특수 쉴드 스킬을 실행했습니다.");
                    return;
                }
            }
        }
        if (get30PercentDifferentHP(attacker,plateController.getPlayerPlates()) != -1) //소환수중 다른 소환수와 체력이 30%이상 차이 나는 몹이 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //쿨타임이면 다음 특수스킬 검사
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.LifeDrain) //저주 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, get30PercentDifferentHP(attacker, plateController.getPlayerPlates()), i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 저주 공격을 실행했습니다.");
                    return;
                }
            }
        }


        randomValue = UnityEngine.Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값
        if (randomValue < 30f) //강공격
        {
            Debug.Log($"{attacker.name} 의 강공격");
            double originPower = attacker.getAttackPower();
            attacker.setAttackPower(attacker.getHeavyAttackPower()); //공격력을 강공격력으로 전환
            attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlatesIndex(attacker)); //일반공격수행
            attacker.setAttackPower(originPower); //원래 공격력으로 되돌리기
        }
        else //일반 공격력으로 공격
        {
            attacker.normalAttack(plateController.getPlayerPlates(), targetPlateIndex); //일반공격 수행
        }
        Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");

    }


    private bool hasPlayerSummonOverMediumRank(List<Plate> plates)
    {
        foreach (Plate plate in plates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null && (summon.getSummonRank() == SummonRank.Medium || summon.getSummonRank() == SummonRank.High ))
            {
                return true; // 중급 이상인 소환수가 존재하면 true 반환
            }
        }
        return false; // 중급 이상인 소환수가 없으면 false 반환
    }



    private int get30PercentDifferentHP(Summon attacker, List<Plate> targetPlates)
    {
        double attackerHealthRatio = (double)attacker.getNowHP() / attacker.getMaxHP();

        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon enermySummon = targetPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double enermyHealthRatio = (double)enermySummon.getNowHP() / enermySummon.getMaxHP();

                // 체력 차이가 30% 이상인 경우 인덱스 반환
                if (Math.Abs(attackerHealthRatio - enermyHealthRatio) >= 0.3)
                {
                    return i;
                }
            }
        }

        return -1; // 조건을 만족하는 소환수가 없으면 -1 반환
    }


    private bool canReactSpecialAttack(AttackProbability attackProbability)
    {
        // 0에서 100 사이의 랜덤 값을 생성
        float randomValue = UnityEngine.Random.Range(0f, 100f);

        // 특수 공격 확률이 랜덤 값보다 크면 true 반환 (특수 공격 선택)
        return randomValue < attackProbability.specialAttackProbability;
    }


    //플레이어의 예측공격을 리스트 생성로직

    public List<AttackPrediction> getPlayerAttackPredictionsList()
    {
        // 1. 소환수의 상태 체크
        List<Plate> playerPlates = CheckPlayerPlateState(); // 현재 playerPlates들

        // 2. 몬스터의 상태 체크 (새 리스트에 상태 조정된 enermyPlates 추가)
        List<Plate> applyEnermyPlates = getApplyStatusEnermyPlates();

        //3. 소환수의 예측공격 리스트를 받아온다.
        playerAttackPredictionsList = playerAttackPrediction.getPlayerAttackPredictionList(playerPlates, applyEnermyPlates);

        return playerAttackPredictionsList;
    }


    // 1. 현재 playerPlate들을 공격이나 데미지 적용이 안되게 새 리스트로 가져온다.
    public List<Plate> CheckPlayerPlateState()
    {
        List<Plate> playerPlateStates = new List<Plate>();
        List<Plate> playerPlates = plateController.getPlayerPlates();

        for (int i = 0; i < playerPlates.Count; i++) // 인덱스를 이용해 순회
        {
            Plate plate = playerPlates[i];
            Summon summon = plate.getCurrentSummon();

            if (summon != null)
            {
                Debug.Log($"{summon.getSummonName()}이 리스트로 들어감");
                // 기존 플레이트를 새 리스트에 추가 (상태만 관리)
                playerPlateStates.Add(plate);
            }
        }

        return playerPlateStates;
    }
    // 2. 적 몬스터의 상태를 조정하여 새로운 플레이트 리스트 반환
    private List<Plate> getApplyStatusEnermyPlates()
    {
        List<Plate> applyEnermyPlates = new List<Plate>();

        foreach (Plate plate in plateController.getEnermyPlates())
        {
            Summon originSummon = plate.getCurrentSummon();
            if (originSummon != null)
            {
                // Summon 객체만 복제하고 상태 효과를 적용
                Summon clonedSummon = originSummon.Clone();
                Summon adjustedSummon = ApplyEnermyStatus(clonedSummon);

                // 원본 Plate에 임시로 복제된 Summon 설정
                plate.setCurrentSummon(adjustedSummon);
                applyEnermyPlates.Add(plate);

                // 원본 Summon으로 복원
                plate.setCurrentSummon(originSummon);
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
        if (clonedSummon.getAllStatusTypes().Contains(StatusType.Poison))
        {
            clonedSummon.setNowHP(clonedSummon.getNowHP() - clonedSummon.getMaxHP() * 0.1);
            if (clonedSummon.getNowHP() <= 0) return null;
        }

        if (clonedSummon.getAllStatusTypes().Contains(StatusType.Burn))
        {
            clonedSummon.setNowHP(clonedSummon.getNowHP() - clonedSummon.getMaxHP() * 0.2);
            if (clonedSummon.getNowHP() <= 0) return null;
        }

        if (clonedSummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
        {
            clonedSummon.setNowHP(clonedSummon.getNowHP() - clonedSummon.getMaxHP() * 0.2);
            if (clonedSummon.getNowHP() <= 0) return null;
        }

        return clonedSummon; // 상태가 적용된 복제본 Summon 반환
    }


    //// 2. 적 몬스터의 상태를 조정하여 새로운 플레이트 리스트 반환
    //private List<Plate> getApplyStatusEnermyPlates()
    //{
    //    List<Plate> applyEnermyPlates = new List<Plate>(); // 새 리스트

    //    foreach (Plate plate in plateController.getEnermyPlates()) // enermyPlates를 하나씩 가져온다
    //    {
    //        Summon originSummon = plate.getCurrentSummon(); // 해당 플레이트의 소환수를 가져와서
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
    //    if (enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
    //    {
    //        enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.1); // 10% 체력 감소
    //        if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
    //    }

    //    // 화상: 최대 체력의 20% 데미지 적용
    //    if (enermySummon.getAllStatusTypes().Contains(StatusType.Burn))
    //    {
    //        enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% 체력 감소
    //        if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
    //    }

    //    // 흡혈: 최대 체력의 20% 데미지 적용
    //    if (enermySummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
    //    {
    //        enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% 체력 감소
    //        if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
    //    }

    //    return enermySummon; // 상태 적용된 몬스터 반환
    //}



    public PlateController getPlateController()
    {
        return this.plateController;
    }
}
