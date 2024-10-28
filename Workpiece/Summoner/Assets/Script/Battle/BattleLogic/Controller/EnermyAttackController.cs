using System.Collections.Generic;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    private PlateController plateController;
    private EnermyAlgorithm enermyAlgorithm;

    private enum AttackType{ NormalAttack, SpecialAttack}; //Ư����ų�� ������� �Ϲݰ����� ��������� ���� Enum


    private void Start()
    {
        enermyAlgorithm = GetComponent<EnermyAlgorithm>();
        plateController = enermyAlgorithm.getPlateController();
    }


    public void EnermyAttackStart(List<AttackPrediction> playerAttackPredictionsList)
    {
        List<Plate> enermyPlate = plateController.getEnermyPlates();

        for (int index = 0; index < plateController.getEnermySummonCount(); index++) //���� ���������� �����غ�
        {
            Summon attackingSummon = enermyPlate[index].getCurrentSummon(); //�÷���Ʈ�� ��ȯ���� ���ʷ� �����ͼ�
            // ��ȯ���� ���� �������� Ȯ��
            if (attackingSummon.IsStun())
            {
                continue; // ���� ���¸� ���� ��ȯ���� �Ѿ
            }

            // ��ȯ���� ���� ���� �˻�
            if(HandleStatusAndReactPrediction(attackingSummon, enermyPlate, index))
            {
                return;
            }
                                    
            //�´��� ����
            playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, index, playerAttackPredictionsList); //�ּ� 1�� ����
            for (int seq = 0; seq < 2; seq++)
            {
                if (continuesAttackByRank(attackingSummon))
                {
                    Debug.Log("���Ӱ��� �ߵ�");
                    playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, index, playerAttackPredictionsList);
                }
                else
                {
                    break; //���Ӱ��� for�� ����
                }
            }
        }
    }

    //ȭ��, ����, ������ ���ؼ��� ����ų�� ������� �����
    private bool HandleStatusAndReactPrediction(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex)
    {
        List<StatusType> statusList = attackingSummon.getAllStatusTypes();
        // ���� ���°� �ִ��� �˻�
        foreach (StatusType statusType in statusList)
        {
            if (statusType == StatusType.Burn || statusType == StatusType.LifeDrain || statusType == StatusType.Poison)
            {
                if(useHealIfAvailable(attackingSummon, enermyPlate, enermyPlateIndex)) //����ų�� ��� �ߴ°�
                    return true;
            }
        }

        return false; // ����� ����Ʈ ��ȯ
    }

    //�� ����� �����ϴٸ� �� ���
    private bool useHealIfAvailable(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex)
    {
        IAttackStrategy[] specialAttackStrategies = attackingSummon.getSpecialAttackStrategy(); //��ų���� �����´�.

        for (int i = 0; i < specialAttackStrategies.Length; i++)
        {
            // ��ų�� �� �� ��ų�� �ִ� ��� �ڱ� �ڽſ��� ���
            if (specialAttackStrategies[i].getStatusType() == StatusType.Heal && specialAttackStrategies[i].getCurrentCooldown()<=0) //���̿����ϰ� ��Ÿ���� 0 �Ʒ������Ѵ�.
            {
                attackingSummon.SpecialAttack(enermyPlate, enermyPlateIndex, i); // �ڱ� �ڽſ��� �� ���
                return true; // ���� ��������� ���� Ż��
            }
        }
        return false;
    }


    //��޺� ���Ӱ��� ���ɿ���
    private bool continuesAttackByRank(Summon summon)
    {
        float randomValue = Random.Range(0f, 100f); // 0���� 100 ������ ������ ��

        if(summon.getSummonRank() == SummonRank.Normal) //�븻����� ���Ӱ��� X
        {
            return false;
        }
        else if(summon.getSummonRank() == SummonRank.Special) //Ư���� 20%
        {
            if (randomValue <= 20) //20%�� ���Ӱ���
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (summon.getSummonRank() == SummonRank.Boss) //���� 30%
        {
            if (randomValue <= 30) //30%�� ���Ӱ���
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public PlateController getPlateController()
    {
        return enermyAlgorithm.getPlateController();
    }


    public EnermyAlgorithm getEnermyAlgorithmController()
    {
        return this.enermyAlgorithm;
    }
}
