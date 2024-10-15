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
    public void SpecialAttackLogic(Summon attackSummon, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer = false)
    {
        if (attackSummon == null)
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
            return;
        }

        // Ư�� ���� �迭�� ������ Ȯ���Ͽ� �ε����� ��ȿ���� ����
        if (!IsValidSpecialAttackIndex(attackSummon, selectSpecialAttackIndex))
        {
            Debug.LogError("��ȿ���� ���� Ư�� ���� �ε����Դϴ�. �ε���: " + selectSpecialAttackIndex);
            return;
        }

        // Ư�� ������ �迭 �ε����� ������
        IAttackStrategy attackStrategy = attackSummon.getSpecialAttackStrategy()[selectSpecialAttackIndex];

        // ���� Ÿ�Ժ��� ���� ����
        if (attackStrategy is TargetedAttackStrategy targetedAttack)
        {
            HandleTargetedAttack(attackSummon, targetedAttack, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else if (attackStrategy is AttackAllEnemiesStrategy attackAll)
        {
            HandleAttackAll(attackSummon, attackAll, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else if (attackStrategy is ClosestEnemyAttackStrategy closestAttack)
        {
            HandleClosestEnemyAttack(attackSummon, closestAttack, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else
        {
            Debug.LogWarning("�� �� ���� ���� �����Դϴ�.");
        }

        ResetBattleSummonAndAttackInfo();
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int selectSpecialAttackIndex)
    {
        return selectSpecialAttackIndex >= 0 && selectSpecialAttackIndex < attackSummon.getSpecialAttackStrategy().Length;
    }

    //Ÿ������ ����
    private void HandleTargetedAttack(Summon attackSummon, TargetedAttackStrategy targetedAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {
        StatusType attackStatusType = targetedAttack.getStatusType();

        if (isPlayer) //�÷��̾�
        {
            if (!IsValidPlateIndex(selectedPlateIndex, plateController.getEnermyPlates().Count))
            {
                Debug.Log("��ȿ�� ���� �÷���Ʈ �ε����� ���õ��� �ʾҽ��ϴ�.");
                return;
            }

            if (attackStatusType == StatusType.Heal || attackStatusType == StatusType.Upgrade || attackStatusType == StatusType.Shield)
            {
                attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex, selectSpecialAttackIndex); // �Ʊ� �÷���Ʈ�� �̷ο� ȿ��
                Debug.Log($"�÷��̾ ������ �Ʊ��� �÷���Ʈ {selectedPlateIndex}�� �̷ο� ȿ�� ����Դϴ�.");
            }
            else
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // �� �÷���Ʈ�� ����
                Debug.Log($"�÷��̾ ������ ���� �÷���Ʈ {selectedPlateIndex}�� ���� ����Դϴ�.");
            }
        }
        else //��
        {
            if (!IsValidPlateIndex(selectedPlateIndex, plateController.getPlayerPlates().Count))
            {
                Debug.Log("��ȿ�� �÷��̾��� �÷���Ʈ �ε����� ���õ��� �ʾҽ��ϴ�.");
                return;
            }

            if (attackStatusType == StatusType.Heal || attackStatusType == StatusType.Upgrade || attackStatusType == StatusType.Shield)
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // �� �÷���Ʈ�� �̷ο� ȿ��
                Debug.Log($"���� ������ ���� �÷���Ʈ {selectedPlateIndex}�� �̷ο� ȿ�� ����Դϴ�.");
            }
            else
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // �� �÷���Ʈ�� ����
                Debug.Log($"���� ������ �÷��̾��� �÷���Ʈ {selectedPlateIndex}�� ���� ����Դϴ�.");
            }

        }
    }


    //��ü���� ����
    private void HandleAttackAll(Summon attackSummon, AttackAllEnemiesStrategy AllusAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {

        if (isPlayer)
        {
            attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // ���� �÷���Ʈ�� ����
            Debug.Log("�Ʊ��� Ư�� ��ü ������ ���������� ����Ǿ����ϴ�.");
        }
        else
        {
            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex, selectSpecialAttackIndex); // ���� �÷��̾� �÷���Ʈ�� ����
            Debug.Log("���� Ư�� ��ü ������ ���������� ����Ǿ����ϴ�.");
        }
    }

    //�������� ����
    private void HandleClosestEnemyAttack(Summon attackSummon, ClosestEnemyAttackStrategy closestAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {

        if (isPlayer)
        {
            attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // ���� �÷���Ʈ�� ����
            Debug.Log("�Ʊ��� Ư�� ���� ������ ���������� ����Ǿ����ϴ�.");
        }
        else
        {
            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex, selectSpecialAttackIndex); // ���� �÷��̾� �÷���Ʈ�� ����
            Debug.Log("���� Ư�� ���� ������ ���������� ����Ǿ����ϴ�.");
        }
    }


    //��ȿ�� �÷���Ʈ���� �˻�
    private bool IsValidPlateIndex(int selectedPlateIndex, int plateCount)
    {
        return selectedPlateIndex >= 0 && selectedPlateIndex < plateCount;
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
