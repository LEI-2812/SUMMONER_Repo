using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : MonoBehaviour
{

    [SerializeField] private StatePanel statePanel;

    private bool isAttacking = false; //���������� �Ǻ�

    [Header("(�ܺ� ������Ʈ)��Ʈ�ѷ�")]
    [SerializeField] private SummonController summonController;
    private PlateController plateController;

    Summon attakingSummon;
    SpecialAttackInfo SpecialAttackInfo;

    public Summon getAttakingSummon()
    {
        return attakingSummon;
    }

    void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    public Summon attackStart(int buttonIndex)
    {
        attakingSummon = statePanel.getStatePanelSummon();
        SpecialAttackInfo = new SpecialAttackInfo(attakingSummon.getSpecialAttackStrategy()[buttonIndex], buttonIndex);
        return attakingSummon; //����â�� �ִ� ��ȯ���� ��ȯ
    }

    // Ư�� ���� ó�� �޼���
    public void SpecialAttackLogic(Summon attackSummon, int selectSpecialAttackIndex, int selectedPlateIndex, bool isPlayer = false)
    {
        if (attackSummon != null)
        {
            // TargetedAttackStrategy�� ����ϴ��� Ȯ��
            if (attackSummon.getSpecialAttackStrategy() is TargetedAttackStrategy[] targetedAttack)
            {
                Debug.Log("TargetedAttackStrategy�� ����Ͽ� ������ �����մϴ�.");
                StatusType attackStatusType = targetedAttack[selectSpecialAttackIndex].getStatusType();
                if (isPlayer) // �÷��̾ ȣ���ϴ� ���
                {
                    if (selectedPlateIndex >= 0 && selectedPlateIndex < plateController.getEnermyPlates().Count)
                    {
                        if (attackStatusType == StatusType.Heal) //����� �˻�
                        {
                            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectSpecialAttackIndex, selectedPlateIndex); // �Ʊ��� �÷���Ʈ�� �ε��� ����
                            ResetBattleSummonAndAttackInfo();
                            return;
                        }
                        Debug.Log($"�÷��̾ ������ �Ʊ��� �÷���Ʈ {selectedPlateIndex}�� ���� ����Դϴ�.");
                    }
                    else
                    {
                        Debug.Log("��ȿ�� ���� �÷���Ʈ �ε����� ���õ��� �ʾҽ��ϴ�.");
                        return; // ���õ� �÷���Ʈ�� ������ ������ �ߴ�
                    }
                }
                else // ���� ȣ���ϴ� ���, �÷��̾��� �÷���Ʈ �ε����� ���
                {
                    if (selectedPlateIndex >= 0 && selectedPlateIndex < plateController.getPlayerPlates().Count)
                    {
                        Debug.Log($"���� �÷��̾��� �÷���Ʈ {selectedPlateIndex}�� �����մϴ�.");
                    }
                    else
                    {
                        Debug.Log("��ȿ�� �÷��̾��� �÷���Ʈ �ε����� ���õ��� �ʾҽ��ϴ�.");
                        return; // ���õ� �ε����� ��ȿ���� ������ ������ �ߴ�
                    }
                }
            }

            //Ÿ�� ������ �ƴҰ��
            // Ư�� ���� ���� (�÷��̾�� ���� �÷���Ʈ, ���� �÷��̾��� �÷���Ʈ)
            if (isPlayer) //�÷��̾� ����
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectSpecialAttackIndex, selectedPlateIndex); // ���� �÷���Ʈ�� �ε��� ����
            }
            else //���� ����
            {
                attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectSpecialAttackIndex, selectedPlateIndex); // �÷��̾��� �÷���Ʈ�� �ε��� ����
            }

            Debug.Log("Ư�� ������ ���������� ����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

        ResetBattleSummonAndAttackInfo();
    }


    public void ResetBattleSummonAndAttackInfo()
    {
        isAttacking = false;
        attakingSummon = null;
        SpecialAttackInfo = null;
        plateController.ResetAllPlateHighlight();
    }


    //�� �÷���Ʈ�� ��ȯ���� �����ϴ���
    public bool IsEnermyPlateClear()
    {
        foreach(Plate plate in plateController.getPlayerPlates()) //�÷���Ʈ�� ��ȯ
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
        foreach (Plate plate in plateController.getPlayerPlates()) //�÷���Ʈ�� ��ȯ
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
        foreach (Plate plate in plateController.getPlayerPlates())
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
        foreach (Plate plate in plateController.getEnermyPlates())
        {
            Summon summon = plate.getSummon();
            if (summon != null)
            {
                enermySummons.Add(summon);
            }
        }
        return enermySummons;
    }


    // SummonController�� PlateController ���� �޼��� �߰�
    public PlateController GetPlateController()
    {
        return plateController; // �̹� SummonController���� PlateController�� �����ϰ� �ִٰ� ����
    }

    public SpecialAttackInfo getNowSpecialAttackInfo()
    {
        return SpecialAttackInfo;
    }

    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }
}
