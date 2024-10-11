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

    public Summon attackStart()
    {
        isAttacking = true; //���ݽ���
        return statePanel.getStatePanelSummon(); //����â�� �ִ� ��ȯ���� ��ȯ
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
