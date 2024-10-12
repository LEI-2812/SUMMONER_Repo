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
    private bool isHeal = false;

    [Header("(�ܺ� ������Ʈ)��Ʈ�ѷ�")]
    [SerializeField] private SummonController summonController;
    private PlateController plateController;

    //���ݴ��� �÷���Ʈ
    private Plate AttackedPlate;

    void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    public Summon attackStart()
    {
        return statePanel.getStatePanelSummon(); //����â�� �ִ� ��ȯ���� ��ȯ
    }

    // Ư�� ���� ó�� �޼���
    public void SpecialAttackLogic(Summon attackSummon, int selectedPlateIndex, bool isPlayer = false)
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
                Debug.Log("TargetedAttackStrategy�� ����Ͽ� ������ �����մϴ�.");

                if (isPlayer) // �÷��̾ ȣ���ϴ� ���
                {
                    if (selectedPlateIndex >= 0 && selectedPlateIndex < plateController.getEnermyPlates().Count)
                    {
                        if (isHeal)
                        {
                            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex); // �Ʊ��� �÷���Ʈ�� �ε��� ����
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
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex); // ���� �÷���Ʈ�� �ε��� ����
            }
            else //���� ����
            {
                attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex); // �÷��̾��� �÷���Ʈ�� �ε��� ����
            }

            Debug.Log("Ư�� ������ ���������� ����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

        isHeal = false;
        isAttacking = false; // ���� ����
    }



    public void selectAttackPlate(Plate plate)
    {
        for (int i = 0; i < plateController.getEnermyPlates().Count; i++)
        {
            if (plateController.getEnermyPlates()[i] == plate)
            {
                AttackedPlate = plateController.getEnermyPlates()[i]; // ���õ� �÷���Ʈ ����
                Debug.Log($"������ �÷���Ʈ {i} ���õ�.");
                return;
            }
        }

        Debug.Log("���� �÷���Ʈ�� ���õ��� �ʾҽ��ϴ�.");

    }


    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking= isAttacking;
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


    public Plate getAttackedPlate()
    {
        return AttackedPlate;
    }

    // SummonController�� PlateController ���� �޼��� �߰�
    public PlateController GetPlateController()
    {
        return plateController; // �̹� SummonController���� PlateController�� �����ϰ� �ִٰ� ����
    }

    public void setIsHeal(bool heal)
    {
        this.isHeal = heal;
    }
    public bool getIsHeal()
    {
        return isHeal;
    }
}
