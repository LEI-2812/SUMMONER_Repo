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
                // ��ȯ���� Ư�� ��ų�� ��� �������� Ȯ��
                IAttackStrategy[] availableSpecialAttacks = summon.getAvailableSpecialAttacks();
                bool hasUsableSpecialAttack = (availableSpecialAttacks != null && availableSpecialAttacks.Length > 0);
                int attackIndex = plateController.getClosestPlayerPlateIndex();
                if (attackIndex == -1)
                {
                    Debug.Log("������ �ε����� ����");
                }
                int attackSummonPlateIndex = plateController.GetPlateIndex(plate); //�ڱ� �ڽ��� �÷���Ʈ ��ȣ

                // Ư�� ��ų�� ���� ��� �Ϲ� �������� ����
                if (!hasUsableSpecialAttack)
                {
                    Debug.Log($"{summon.getSummonName()}�� ��� ������ Ư�� ��ų�� �����ϴ�. �Ϲ� �������� �����մϴ�.");
                    AttackProbability attackProbability = new AttackProbability(100f, 0f); //�Ϲݰ����� 100%

                    AttackPrediction attackPrediction = new AttackPrediction(
                        summon, //�����ϴ� ��ȯ��
                        plateController.GetPlateIndex(plate), //�ڱ� �ڽ��� �÷���Ʈ ��ȣ
                        summon.getAttackStrategy(), //�Ϲݰ���
                        0, //Ư������ ��ȣ
                        enermyPlates, //Ÿ�� �÷���Ʈ
                        attackIndex, ///Ÿ�� ��ȣ
                        attackProbability //Ȯ��
                        );

                    playerPrediction.Add(attackPrediction);
                    Debug.Log($"�߰��� �Ϲݰ��� Prediction: {GetPredictionDetails(attackPrediction)}");
                }
                else
                {
                    // ������ ���� Ŭ������ ã�Ƽ� ���� ���� ����
                    foreach (IAttackPrediction prediction in attackPredictions)
                    {
                        if (prediction.getPreSummonType() == summon.getSummonType())
                        {
                            AttackPrediction result = prediction.getAttackPrediction(summon, attackSummonPlateIndex, playerPlates, enermyPlates);
                            if (result != null)
                            {
                                playerPrediction.Add(result);
                                Debug.Log($"�߰��� Prediction: {GetPredictionDetails(result)}");
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
        return $"�Ʊ� ���� ��ȯ��: {prediction.getAttackSummon().getSummonName()}, " +
               $"�Ʊ� ���� ����: {prediction.getAttackStrategy().GetType().Name}, " +
               $"�Ʊ� Ÿ�� �÷���Ʈ��ȣ: {prediction.getTargetPlateIndex()}, " +
               $"�Ϲݰ��� Ȯ��: {prediction.getAttackProbability().normalAttackProbability}%, " +
               $"Ư������ Ȯ��: {prediction.getAttackProbability().specialAttackProbability}%";
    }
}
