using System.Collections.Generic;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("��Ʈ�ѷ�")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;
    private enum AttackType{ NormalAttack, SpecialAttack};
    
    public void EnermyAttackLogic(Summon attakingSummon)
    {
        if (attakingSummon == null)
        {
            Debug.LogError("������ ��ȯ���� �����ϴ�.");
            return;
        }

        if (!attakingSummon.IsCooltime()) //��Ÿ������ ��ų�� �������
        {
            AttackType selectedAttakType = SelectAttackType(); //�Ϲݰ��ݰ� Ư�������� �������� �޾ƿ�
            if(selectedAttakType == AttackType.SpecialAttack)
            {
                //��Ÿ���� ���� Ư����ų�� ����ϰ� �Ѵ�. (2�� �̻��̸� ��������)
                // ��Ÿ���� ���� Ư�� ��ų ����� ������
                List<int> availableSpecialAttacks = new List<int>();
            }
            else
            {
                normalAttackLogic(attakingSummon); //��Ÿ
            }
        }
        else //��ų���� ��Ÿ���̿��� ��Ÿ�� ����
        {
            normalAttackLogic(attakingSummon);
        }





    }

    private void normalAttackLogic(Summon attackingSummon)
    {
        int selectAttackIndex = plateController.getClosestPlayerPlatesIndex(); //�÷��̾� �÷���Ʈ���� ���� ����� ��ȯ���� �ε����� �޾ƿ´�.
        if (selectAttackIndex < 0)
        {
            Debug.Log("������ ��ȯ���� �����ϴ�."); return;
        }

        attackingSummon.normalAttack(plateController.getPlayerPlates(), selectAttackIndex); //�Ϲݰ��� ����
    }




    //50% 50% �Ϲݰ��ݰ� Ư�������� �޾ƿ´�.
    private AttackType SelectAttackType()
    {
        float randomValue = Random.Range(0f, 100f); // 0���� 100 ������ ������ ��

        if (randomValue <= 50)
        {
            return AttackType.NormalAttack;
        }
        else // High ��� (15%)
        {
            return AttackType.SpecialAttack;
        }
    }



}
