using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("�Ʊ� �� �� �÷���Ʈ")]
    [SerializeField] private List<Plate> playerPlates; //�÷��̾� �÷���Ʈ
    [SerializeField] private List<Plate> enermyPlates; //�� �÷���Ʈ

    [SerializeField] private StatePanel statePanel;

    private bool isAttacking = false; //���������� �Ǻ�

    [Header("��ȯ�� ��Ʈ�ѷ�")]
    [SerializeField] private SummonController summonController;

    public Summon attackStart()
    {
        isAttacking = true; //���ݽ���
        return statePanel.getStatePanelSummon(); //����â�� �ִ� ��ȯ���� ��ȯ
    }

    // Ư�� ���� ó�� �޼���
    public void SpecialAttack(Summon attackSummon, int selectedPlateIndex)
    {
        if (attackSummon != null)
        {
            // ��ų�� ��Ÿ�� ������ Ȯ��
            if (attackSummon.IsSkillOnCooldown("SpecialAttack"))
            {
                Debug.Log("Ư�� ��ų�� ��Ÿ�� ���Դϴ�. ����� �� �����ϴ�.");
                return;
            }

            // TargetedAttackStrategy�� ����ϴ��� Ȯ��
            if (attackSummon.getSpecialAttackStrategy() is TargetedAttackStrategy)
            {
                Debug.Log("TargetedAttackStrategy�� ����մϴ�. ���� �÷���Ʈ�� �����ϼ���.");

                // ����ڰ� ������ �÷���Ʈ �ε����� �����ɴϴ� (���� ��� ��)
                Plate selectedEnemyPlate = summonController.SelectPlateAndResummon(); // ���� �÷���Ʈ�� �����ϴ� ���� �ʿ�
                if (selectedEnemyPlate != null)
                {
                    selectedPlateIndex = getEnermyPlate().IndexOf(selectedEnemyPlate); // ������ �÷���Ʈ�� �ε��� ����
                    Debug.Log($"�÷���Ʈ {selectedPlateIndex} ���õ�.");
                }
                else
                {
                    Debug.Log("�÷���Ʈ�� ���õ��� �ʾҽ��ϴ�.");
                    return; // ���õ� �÷���Ʈ�� ������ ������ �ߴ�
                }
            }

            // Ư�� ���� ����
            attackSummon.SpecialAttack(getEnermyPlate(), selectedPlateIndex);
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

    }






    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking= isAttacking;
    }

    public List<Plate> getEnermyPlate()
    {
        return enermyPlates;
    }
















    //�� �÷���Ʈ�� ��ȯ���� �����ϴ���
    public bool IsEnermyPlateClear()
    {
        foreach(Plate plate in enermyPlates) //�÷���Ʈ�� ��ȯ
        {
            Summon summon = plate.getSummon(); //�÷���Ʈ���� ��ȯ���� �����´�
            if(summon != null) //���� ��ȯ���� �ϳ��� �ִٸ� true�� ��ȯ
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPlayerPlateClear()
    {
        foreach (Plate plate in playerPlates) //�÷���Ʈ�� ��ȯ
        {
            Summon summon = plate.getSummon(); //�÷���Ʈ���� ��ȯ���� �����´�
            if (summon != null) //���� ��ȯ���� �ϳ��� �ִٸ� true�� ��ȯ
            {
                return false;
            }
        }

        return true;
    }

    // �÷��̾��� �÷���Ʈ�� �ִ� ��� ��ȯ������ ��ȯ�ϴ� �޼ҵ�
    public List<Summon> getPlayerSummons()
    {
        List<Summon> playerSummons = new List<Summon>();
        foreach (Plate plate in playerPlates)
        {
            Summon summon = plate.getSummon();
            if (summon != null)
            {
                playerSummons.Add(summon);
            }
        }
        return playerSummons;
    }

    // ���� �÷���Ʈ�� �ִ� ��� ��ȯ������ ��ȯ�ϴ� �޼ҵ�
    public List<Summon> getEnermySummons()
    {
        List<Summon> enermySummons = new List<Summon>();
        foreach (Plate plate in enermyPlates)
        {
            Summon summon = plate.getSummon();
            if (summon != null)
            {
                enermySummons.Add(summon);
            }
        }
        return enermySummons;
    }

}
