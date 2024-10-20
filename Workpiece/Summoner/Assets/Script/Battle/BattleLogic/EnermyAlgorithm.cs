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


    // 알고리즘 순서대로 실행
    public List<AttackPrediction> HandleReactPrediction(Summon attackingEnermySummon, List<AttackPrediction> playerAttackPredictionsList)
    {
        if (playerAttackPredictionsList.Count == 0)
        {
            // 리스트가 비어있으면 일반 공격으로 처리
            handleReactNormalAttack(attackingEnermySummon, plateController.getClosestPlayerPlateIndex()); // 기본 타겟 플레이트 인덱스 사용
        }
        else
        {
            for (int i = 0; i < playerAttackPredictionsList.Count; i++)
            {
                AttackPrediction playerPrediction = playerAttackPredictionsList[i];

                AttackProbability preAttackProbability = playerPrediction.getAttackProbability(); // 확률
                Summon preAttackingPlayerSummon = playerPrediction.getAttackSummon(); // 예측한 플레이어의 공격 소환수
                int preSummonPlateIndex = playerPrediction.getAttackSummonPlateIndex(); // 예측한 플레이어 공격 소환수의 플레이트 판넬
                IAttackStrategy preAttackStrategy = playerPrediction.getAttackStrategy(); // 예측한 공격방식
                List<Plate> targetPlate = playerPrediction.getTargetPlate(); // 예측한 타겟의 플레이트 (플레이어 혹은 적)
                int targetPlateIndex = playerPrediction.getTargetPlateIndex(); // 공격 타겟 플레이트 번호

                if (canReactSpecialAttack(preAttackProbability)) // 특수공격으로 공격할지
                {
                    handleReactSpecialAttack(attackingEnermySummon, preAttackStrategy, preSummonPlateIndex, targetPlate, targetPlateIndex);
                }
                else // 일반공격으로 공격
                {
                    handleReactNormalAttack(attackingEnermySummon, targetPlateIndex);
                }

                // 대응한 공격은 리스트에서 제거
                playerAttackPredictionsList.RemoveAt(i);
                i--; // 인덱스 보정
            }
        }

        return playerAttackPredictionsList; // 변경된 리스트 반환
    }



    //특수공격에 대한 대응
    private void handleReactSpecialAttack(Summon attacker, IAttackStrategy preAttackStrategy, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        if(preAttackStrategy is AttackAllEnemiesStrategy allAttackStrategy) //예측한 공격타입이 전체공격인지 검사
        {
            if(allAttackStrategy.getStatusType() == StatusType.Poison)
            {
                reactSpecialFromPoison(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
        }
        else if(preAttackStrategy is TargetedAttackStrategy targetAttackStrategy) //예측한 공격타입이 타겟공격인지 검사
        {
            if (targetAttackStrategy.getStatusType() == StatusType.None)
            {
                reactSpecialFromTargetNone(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
            else if (targetAttackStrategy.getStatusType() == StatusType.None)
            {
                reactSpecialFromUpgrade(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
            else if (targetAttackStrategy.getStatusType() == StatusType.Heal)
            {
                reactSpecialFromHeal(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
        }
        else
        {
            Debug.Log("예측공격이 잘못받아와졌음");
        }
    }



    private void reactSpecialFromPoison(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // 사용 가능한 스킬들 가져오기
        bool specialAttackExecuted = false; // 특수 공격 실행 여부를 추적

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 힐 특수 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //타겟공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 저격 공격을 실행했습니다.");
                    break;
                }
            }

            // 특수 공격을 실행하지 않은 경우, 일반 공격 실행
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            }
        }
    }

    private void reactSpecialFromTargetNone(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // 사용 가능한 스킬들 가져오기
        bool specialAttackExecuted = false; // 특수 공격 실행 여부를 추적

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 스턴 특수 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 쉴드 특수 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 힐 특수 공격을 실행했습니다.");
                    break;
                }
            }

            // 특수 공격을 실행하지 않은 경우, 일반 공격 실행
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다."); 
            }
        }
    }
    private void reactSpecialFromAllNone(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // 사용 가능한 스킬들 가져오기
        bool specialAttackExecuted = false; // 특수 공격 실행 여부를 추적

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 스턴 특수 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 쉴드 특수 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 힐 특수 스킬을 실행했습니다.");
                    break;
                }
            }

            // 특수 공격을 실행하지 않은 경우, 일반 공격 실행
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            }
        }
    }
    private void reactSpecialFromHeal(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // 사용 가능한 스킬들 가져오기
        bool specialAttackExecuted = false; // 특수 공격 실행 여부를 추적

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //타겟공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 저격 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //전체공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 전체 공격을 실행했습니다.");
                    break;
                }
            }

            // 특수 공격을 실행하지 않은 경우, 일반 공격 실행
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            }
        }
    }
    private void reactSpecialFromUpgrade(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // 사용 가능한 스킬들 가져오기
        bool specialAttackExecuted = false; // 특수 공격 실행 여부를 추적

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Curse) //저주공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 저주 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //저격공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}가 특수 저격 공격을 실행했습니다.");
                    break;
                }
            }

            // 특수 공격을 실행하지 않은 경우, 일반 공격 실행
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker,targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            }
        }
    }


    private void handleReactNormalAttack(Summon attacker, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // 사용 가능한 스킬들 가져오기

        if (attackStrategy == null)
        {
            attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlatesIndex(attacker)); //일반공격수행
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            return;
        }
        if (plateController.getPlayerPlates().Count >= 2) //소환수가 2마리 이상 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Upgrade) //강화 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 강화 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Burn) //화상 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 화상 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //화상 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 전체 공격을 실행했습니다.");
                    break;
                }
            }
        }
        else if (hasPlayerSummonOverMediumRank(plateController.getPlayerPlates())) //소환수 등급이 중급 이상인 몹이 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Curse) //저주 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 저주 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Stun) //혼란 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 혼란 공격을 실행했습니다.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield) //쉴드 인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 쉴드 스킬을 실행했습니다.");
                    break;
                }
            }
        }
        else if (get30PercentDifferentHP(attacker,plateController.getPlayerPlates()) != -1) //소환수중 다른 소환수와 체력이 30%이상 차이 나는 몹이 존재하는가?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.LifeDrain) //저주 공격인지 검사
                {
                    battleController.SpecialAttackLogic(attacker, get30PercentDifferentHP(attacker, plateController.getPlayerPlates()), i);
                    Debug.Log($"{attacker.getSummonName()}가 특수 저주 공격을 실행했습니다.");
                    break;
                }
            }
        }
        else
        {
            attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlateIndex()); //일반공격수행
            Debug.Log($"{attacker.getSummonName()}가 일반 공격을 실행했습니다.");
            return;
        }
    }







    private bool hasPlayerSummonOverMediumRank(List<Plate> plates)
    {
        foreach (Plate plate in plates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null && (summon.getSummonRank() == SummonRank.Medium || summon.getSummonRank() == SummonRank.High || summon.getSummonRank() == SummonRank.Boss))
            {
                return true; // 중급 이상인 소환수가 존재하면 true 반환
            }
        }
        return false; // 중급 이상인 소환수가 없으면 false 반환
    }



    private int get30PercentDifferentHP(Summon attacker, List<Plate> enermyPlates)
    {
        double attackerHealthRatio = (double)attacker.getNowHP() / attacker.getMaxHP();

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
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
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

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
    private List<Plate> GetApplyStatusEnermyPlates()
    {
        List<Plate> applyEnermyPlates = new List<Plate>(); // 새 리스트

        foreach (Plate plate in plateController.getEnermyPlates()) // enermyPlates를 하나씩 가져온다
        {
            Summon originalSummon = plate.getCurrentSummon(); // 해당 플레이트의 소환수를 가져와서
            if (originalSummon != null)
            {
                // 기존 몬스터를 가져와 상태를 조정 후 새 플레이트 리스트에 추가
                Summon applySummon = ApplyEnermyStatus(originalSummon);
                plate.setCurrentSummon(applySummon); //null이여도 넣어줌. 죽었을땐 null이므로 공격대상이 되지 않게
                applyEnermyPlates.Add(plate);
            }
        }

        return applyEnermyPlates;
    }

    // 2.(1) 몬스터 상태에 따라 수치 조정
    private Summon ApplyEnermyStatus(Summon enermySummon)
    {
        // 독성: 최대 체력의 10% 데미지 적용
        if (enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.1); // 10% 체력 감소
            if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
        }

        // 화상: 최대 체력의 20% 데미지 적용
        if (enermySummon.getAllStatusTypes().Contains(StatusType.Burn))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% 체력 감소
            if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
        }

        // 흡혈: 최대 체력의 20% 데미지 적용
        if (enermySummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% 체력 감소
            if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
        }

        return enermySummon; // 상태 적용된 몬스터 반환
    }
}
