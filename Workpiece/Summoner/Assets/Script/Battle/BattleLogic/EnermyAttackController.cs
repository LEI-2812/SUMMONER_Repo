using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("��Ʈ�ѷ�")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;

    double normalAttackValue = 50f; double specialAttackValue = 50f;
    private enum AttackType{ NormalAttack, SpecialAttack}; //Ư����ų�� ������� �Ϲݰ����� ��������� ���� Enum
    
    public void EnermyAttackStart(Summon attackingSummon)
    {
        if (attackingSummon == null)
        {
            Debug.LogError("������ ��ȯ���� �����ϴ�.");
            return;
        }

        for (int i = 0; i < plateController.getPlayerPlates().Count; i++) //�÷��̾� �÷���Ʈ�� ���������� ����
        {
            Plate targetPlate = plateController.getPlayerPlates()[i];
            Summon target = targetPlate.getCurrentSummon();

            for (int ii = 0; ii < 2; ii++) //���Ӱ��� ���ɼ�
            {
                EnerymyAttackLogic(attackingSummon, target); //�ּ�1���� ����

                if (continuesAttackByRank(attackingSummon)) //�Ű������� ���� ��ȯ���� ��޿����� ���Ӱ����� �����ϸ� ���� ���� �ִ� 3�� ����
                {
                    Debug.Log("���Ӱ��� �ߵ�!");
                    continue; //��ӽ���
                }
                else //���Ӱ��ݰ��ɼ��� false�� �ٷ� ����
                {
                    return;
                }
            }
        }
    }

    private void EnerymyAttackLogic(Summon attackingSummon, Summon target)
    {
        if (!attackingSummon.IsCooltime()) //��Ÿ������ ��ų�� �������
        {
            AttackType selectedAttakType = SelectAttackType(); //�Ϲݰ��ݰ� Ư�������� �������� �޾ƿ�
            if (selectedAttakType == AttackType.SpecialAttack)
            {
                //��Ÿ���� ���� Ư����ų�� ����ϰ� �Ѵ�.
                List<int> availableSpecialAttacks = attackingSummon.getAvailableSpecialAttack();  // ��Ÿ���� ���� Ư�� ��ų ����� ������
                int selectSpecialAttackIndex = getRandomAvilableSpecialAttackIndex(availableSpecialAttacks); //������ Ư����ų ��ȣ�� ������

                int selectedPlateIndex = plateController.getClosestPlayerPlatesIndex(attackingSummon); //�ӽ÷� ���� ������� �����ϰ� ��. ���߿� �����ʿ�

                battleController.SpecialAttackLogic(attackingSummon, selectedPlateIndex, selectSpecialAttackIndex); //Ư����ų ���
            }
            else
            {
                enermyNormalAttackLogic(attackingSummon); //��Ÿ
            }
        }
        else //��ų���� ��Ÿ���̿��� ��Ÿ�� ����
        {
            enermyNormalAttackLogic(attackingSummon);
        }
    }


    //�Ϲ� ����
    private void enermyNormalAttackLogic(Summon attackingSummon)
    {
        int selectAttackIndex = plateController.getClosestPlayerPlatesIndex(attackingSummon); //�÷��̾� �÷���Ʈ���� ���� ����� ��ȯ���� �ε����� �޾ƿ´�.
        if (selectAttackIndex < 0)
        {
            Debug.Log("������ ��ȯ���� �����ϴ�."); return;
        }

        float randomValue = Random.Range(0f, 100f); // 0���� 100 ������ ������ ��
        if (randomValue < 30f) //������
        {
            Debug.Log($"{attackingSummon.name} �� ������");
            attackingSummon.setAttackPower(attackingSummon.getHeavyAttakPower()); //���ݷ��� �����ݷ����� ��ȯ
            attackingSummon.normalAttack(plateController.getPlayerPlates(), selectAttackIndex); //�Ϲݰ��� ����
            attackingSummon.setAttackPower(attackingSummon.getAttackPower()); //���� ���ݷ����� �ǵ�����
        }
        else //�Ϲ� ���ݷ����� ����
        {
            attackingSummon.normalAttack(plateController.getPlayerPlates(), selectAttackIndex); //�Ϲݰ��� ����
        }
    }


    
    //��밡���� ������ Ư����ų �ε��� �ޱ� ---- �� �������� ȥ������ �������� �ٲ��� �����ʿ�
    private int getRandomAvilableSpecialAttackIndex(List<int> availableSpecialAttacks)
    {
        if (availableSpecialAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpecialAttacks.Count); //��밡���� Ư����ų���� �������� �޴´�.
            int selectedSpecialAttackIndex = availableSpecialAttacks[randomIndex]; //Ư����ų ���� �ε���
            return selectedSpecialAttackIndex;
        }
        return -1;
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

    //50% 50% �Ϲݰ��ݰ� Ư�������� �޾ƿ´�.
    private AttackType SelectAttackType()
    {
        float randomValue = Random.Range(0f, 100f); // 0���� 100 ������ ������ ��

        if (randomValue <= 50)
        {
            return AttackType.NormalAttack;
        }
        else
        {
            return AttackType.SpecialAttack;
        }
    }

    public PlateController getPlateController()
    {
        return plateController;
    }





    public void setNormalAttackValue(float value)
    {
        normalAttackValue = value;
    }
    public void setSpecialAttackValue(float value)
    {
        specialAttackValue = value;
    }
}
