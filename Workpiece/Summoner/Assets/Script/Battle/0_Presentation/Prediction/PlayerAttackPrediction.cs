using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: PlayerAttackPrediction의 책임을 정의한다.
public class PlayerAttackPrediction : MonoBehaviour
{
    private List<IAttackPrediction> attackPredictions;

    private void Awake()
    {
        attackPredictions = new List<IAttackPrediction>(GetComponents<IAttackPrediction>());
    }

    public List<AttackPredictionData> GetPlayerAttackPredictionList(IReadOnlyList<IPlateState> playerPlates, IReadOnlyList<IPlateState> enemyPlates)
    {
        List<AttackPredictionData> playerPrediction = new List<AttackPredictionData>();

        foreach (IPlateState plate in playerPlates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon != null)
            {
                AttackData[] availableSpecialAttacks = summon.GetAvailableSpecialAttacks();
                bool hasUsableSpecialAttack = (availableSpecialAttacks != null && availableSpecialAttacks.Length > 0);
                int attackIndex = GetClosestEnemyPlateIndexExcept(enemyPlates, summon);
                if (attackIndex == -1)
                {
                    Debug.Log("공격 대상 인덱스가 없습니다.");
                }
                int attackSummonPlateIndex = plate.GetPlateIndex();

                if (!hasUsableSpecialAttack)
                {
                    Debug.Log($"{summon.GetSummonName()}은 사용할 수 있는 특수 스킬이 없습니다. 일반 공격으로 예측합니다.");
                    AttackProbabilityData AttackProbabilityData = new AttackProbabilityData(100f, 0f);

                    AttackPredictionData AttackPredictionData = new AttackPredictionData(
                        summon,
                        attackSummonPlateIndex,
                        summon.GetAttackStrategy(),
                        0,
                        enemyPlates,
                        attackIndex,
                        AttackProbabilityData
                        );

                    playerPrediction.Add(AttackPredictionData);
                    Debug.Log($"일반 공격 예측 추가: {GetPredictionDetails(AttackPredictionData)}");
                }
                else
                {
                    foreach (IAttackPrediction prediction in attackPredictions)
                    {
                        if (prediction.CanPredict(summon))
                        {
                            AttackPredictionData result = prediction.GetAttackPrediction(summon, attackSummonPlateIndex, playerPlates, enemyPlates);
                            if (result != null)
                            {
                                playerPrediction.Add(result);
                                Debug.Log($"공격 예측 추가: {GetPredictionDetails(result)}");
                            }
                        }
                    }
                }
            }
        }

        return playerPrediction;
    }


    private string GetPredictionDetails(AttackPredictionData prediction)
    {
        return $"아군 공격 소환수: {prediction.GetAttackSummon().GetSummonName()}, " +
               $"아군 공격 종류: {prediction.GetAttackStrategy().GetType().Name}, " +
               $"아군 대상 플레이트 번호: {prediction.GetTargetPlateIndex()}, " +
               $"일반 공격 확률: {prediction.GetAttackProbability().normalAttackProbability}%, " +
               $"특수 공격 확률: {prediction.GetAttackProbability().specialAttackProbability}%";
    }

    private int GetClosestEnemyPlateIndexExcept(IReadOnlyList<IPlateState> enemyPlates, Summon excludedSummon)
    {
        for (int i = 0; i < enemyPlates.Count; i++)
        {
            Summon summon = enemyPlates[i].GetCurrentSummon();
            if (summon != null && summon != excludedSummon)
            {
                return i;
            }
        }

        return -1;
    }
}
