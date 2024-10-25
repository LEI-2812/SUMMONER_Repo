using System.Collections.Generic;
using System.Net.NetworkInformation;
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

        for (int enermyPlateIndex = 0; enermyPlateIndex < plateController.getEnermySummonCount(); enermyPlateIndex++) //���� ���������� �����غ�
        {
            Summon attackingSummon = enermyPlate[enermyPlateIndex].getCurrentSummon(); //�÷���Ʈ�� ��ȯ���� ���ʷ� �����ͼ�
            // ��ȯ���� ���� �������� Ȯ��
            if (IsSummonStunned(attackingSummon))
            {
                continue; // ���� ���¸� ���� ��ȯ���� �Ѿ
            }

            // ��ȯ���� ���� ���� �˻�
            if(HandleStatusAndReactPrediction(attackingSummon, enermyPlate, enermyPlateIndex))
            {
                return;
            }
                                    
            //�´��� ����
            playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, enermyPlateIndex, playerAttackPredictionsList); //�ּ� 1�� ����
            for (int seq = 0; seq < 2; seq++)
            {
                if (continuesAttackByRank(attackingSummon))
                {
                    Debug.Log("���Ӱ��� �ߵ�");
                    playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, enermyPlateIndex, playerAttackPredictionsList);
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
                useHealIfAvailable(attackingSummon, enermyPlate, enermyPlateIndex);
                return true;
            }
        }

        return false; // ����� ����Ʈ ��ȯ
    }

   
    private bool IsSummonStunned(Summon summon)
    {
        List<StatusType> statusList = summon.getAllStatusTypes();
        return statusList.Contains(StatusType.Stun);
    }
    private void useHealIfAvailable(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex)
    {
        IAttackStrategy[] specialAttackStrategies = attackingSummon.getSpecialAttackStrategy(); // ��밡���� ��ų���� ������

        for (int i = 0; i < specialAttackStrategies.Length; i++)
        {
            // ��ų�� �� �� ��ų�� �ִ� ��� �ڱ� �ڽſ��� ���
            if (specialAttackStrategies[i].getStatusType() == StatusType.Heal)
            {
                attackingSummon.SpecialAttack(enermyPlate, enermyPlateIndex, i); // �ڱ� �ڽſ��� �� ���
                return; // ���� ��������� ���� Ż��
            }
        }
    }

    //private void EnerymyAttackLogic(Summon attackingSummon, Summon target, int targetIndex)
    //{
    //    if (!attackingSummon.IsCooltime()) //��Ÿ������ ��ų�� �������
    //    {
    //        AttackType selectedAttakType = SelectAttackType(); //�Ϲݰ��ݰ� Ư�������� �������� �޾ƿ�
    //        if (selectedAttakType == AttackType.SpecialAttack)
    //        {
    //            //��Ÿ���� ���� Ư����ų�� ����ϰ� �Ѵ�.
    //            List<int> availableSpecialAttacks = attackingSummon.getAvailableSpecialAttack();  // ��Ÿ���� ���� Ư�� ��ų ����� ������


    //            int selectSpecialAttackIndex = getRandomAvilableSpecialAttackIndex(availableSpecialAttacks); //������ Ư����ų ��ȣ�� ������
    //            int selectedPlateIndex = plateController.getClosestPlayerPlatesIndex(attackingSummon); //�ӽ÷� ���� ������� �����ϰ� ��. ���߿� �����ʿ�
    //            algorithm.ExecuteEnermyAlgorithm(attackingSummon, targetIndex); //�˰��� ����

    //            battleController.SpecialAttackLogic(attackingSummon, selectedPlateIndex, selectSpecialAttackIndex); //Ư����ų ���
    //        }
    //        else
    //        {
    //            enermyNormalAttackLogic(attackingSummon); //��Ÿ
    //        }
    //    }
    //    else //��ų���� ��Ÿ���̿��� ��Ÿ�� ����
    //    {
    //        enermyNormalAttackLogic(attackingSummon);
    //    }
    //}   




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
