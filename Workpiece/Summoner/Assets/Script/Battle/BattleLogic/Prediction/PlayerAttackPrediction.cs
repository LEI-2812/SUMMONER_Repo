using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerAttackPrediction : MonoBehaviour
{
    private PlateController plateController;
    private List<IAttackPrediction> attackPredictions;

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
        attackPredictions = new List<IAttackPrediction>(GetComponents<IAttackPrediction>());
    }

    public List<AttackPrediction> getPlayerAttackPredictionList(List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        List<AttackPrediction> playerPrediction = new List<AttackPrediction>();

        foreach (Plate plate in playerPlates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                // 소환수의 특수 스킬이 사용 가능한지 확인
                IAttackStrategy[] availableSpecialAttacks = summon.getAvailableSpecialAttacks();
                bool hasUsableSpecialAttack = (availableSpecialAttacks != null && availableSpecialAttacks.Length > 0);
                int attackIndex = plateController.getClosestPlayerPlateIndex();
                if (attackIndex == -1)
                {
                    Debug.Log("공격할 인덱스가 없음");
                }
                int attackSummonPlateIndex = plateController.GetPlateIndex(plate); //자기 자신의 플레이트 번호

                // 특수 스킬이 없는 경우 일반 공격으로 예측
                if (!hasUsableSpecialAttack)
                {
                    Debug.Log($"{summon.getSummonName()}는 사용 가능한 특수 스킬이 없습니다. 일반 공격으로 예측합니다.");
                    AttackProbability attackProbability = new AttackProbability(100f, 0f); //일반공격을 100%

                    AttackPrediction attackPrediction = new AttackPrediction(
                        summon, //공격하는 소환수
                        plateController.GetPlateIndex(plate), //자기 자신의 플레이트 번호
                        summon.getAttackStrategy(), //일반공격
                        0, //특수공격 번호
                        enermyPlates, //타겟 플레이트
                        attackIndex, ///타겟 번호
                        attackProbability //확률
                        );

                    playerPrediction.Add(attackPrediction);
                    Debug.Log($"추가된 일반공격 Prediction: {GetPredictionDetails(attackPrediction)}");
                }
                else
                {
                    // 적절한 예측 클래스를 찾아서 공격 예측 수행
                    foreach (IAttackPrediction prediction in attackPredictions)
                    {
                        if (prediction.getPreSummonType() == summon.getSummonType())
                        {
                            AttackPrediction result = prediction.getAttackPrediction(summon, attackSummonPlateIndex, playerPlates, enermyPlates);
                            if (result != null)
                            {
                                playerPrediction.Add(result);
                                Debug.Log($"추가된 Prediction: {GetPredictionDetails(result)}");
                            }
                        }
                    }
                }
            }
        }

        return playerPrediction;
    }


    private string GetPredictionDetails(AttackPrediction prediction)
    {
        return $"아군 공격 소환수: {prediction.getAttackSummon().getSummonName()}, " +
               $"아군 공격 종류: {prediction.getAttackStrategy().GetType().Name}, " +
               $"아군 타겟 플레이트번호: {prediction.getTargetPlateIndex()}, " +
               $"일반공격 확률: {prediction.getAttackProbability().normalAttackProbability}%, " +
               $"특수공격 확률: {prediction.getAttackProbability().specialAttackProbability}%";
    }
}
