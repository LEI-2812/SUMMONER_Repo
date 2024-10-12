using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class Player : Character
{
    [Header("����UI")]
    [SerializeField] private List<RawImage> manaList;
    [Header("�����ؽ���")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;
    private int mana;
    private int usedMana;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private TurnController turnController;
    [SerializeField] private BattleController battleController;

     BattleAlert battleAlert;

    private int selectedPlateIndex = -1;


    private bool hasSummonedThisTurn;

    private void Start()
    {
        battleAlert = GetComponent<BattleAlert>();
        ResetPlayerSetting();
    }

    public override void startTurn()
    {
        base.startTurn();
        Debug.Log($"{gameObject.name} �� ����: {mana}");
        hasSummonedThisTurn = false;
    }

    public void OnSummonBtnClick()
    {
        if (hasSummonedThisTurn)
        {
            Debug.Log("�� �Ͽ����� �̹� ��ȯ�� �߽��ϴ�. ���� �Ͽ� ��ȯ�� �� �ֽ��ϴ�.");
            return;
        }

        if (mana > 0)
        {
            for (int i = 0; i < summonController.getPlayerPlate().Count; i++)
            {
                if (!summonController.getPlayerPlate()[i].isInSummon)
                {
                    Debug.Log(i + "��° �÷���Ʈ�� ��ȯ ����");
                    summonController.StartSummon(i, false);
                    mana -= 1;
                    hasSummonedThisTurn = true;
                    UpdateManaUI();
                    return;
                }
            }
            Debug.Log("��� �÷���Ʈ�� ��ȯ���� �ֽ��ϴ�.");
        }
        else
        {
            takeAction();
            Debug.Log("������ �����Ͽ� ��ȯ �Ұ���");
        }
    }

    //�÷��̾�� ��ư Ŭ���� ���ؼ��� �����Ḧ ��Ų��.
    public void PlayerTurnOverBtn() //��ư�� ���� �޼ҵ�
    {
        // �÷��̾� ���� ���� �� ���� ����
        if (turnController.currentTurn == TurnController.Turn.PlayerTurn)
        {
            Debug.Log("�÷��̾� �� ����");
            turnController.EndTurn();
        }
        else
        {
            Debug.Log("�÷��̾� ���� �ƴմϴ�.");
        }
    }

    public void OnReSummonBtnClick() //���ȯ ��ư Ŭ��
    {
        if (mana >= usedMana) {
            if (summonController.StartResummon())
            { //���ȯ ����
              //���� ����
                mana -= usedMana;
                usedMana += 1;
                UpdateManaUI();
            }
        }
        else
        {
            Debug.Log("���ȯ�� �ʿ��� ������ ���ڶ��ϴ�.");
        }
    }


    public void OnAttackBtnClick()
    {
        Summon attackSummon = battleController.attackStart(); //������ ��ȯ���� �޾ƿ´�.
        if (attackSummon != null)
        {
            // �Ϲ� ���� ����
            attackSummon.normalAttack(battleController.getEnermyPlate(),selectedPlateIndex);
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

        if (battleController.IsEnermyPlateClear())
        {
            Debug.Log("�¸�!");
            battleAlert.clearAlert();
        }
    }

    public void OnSpecialAttackBtnClick()
    {
        Summon attackSummon = battleController.attackStart(); // ������ ��ȯ���� ������

        if (attackSummon != null)
        {
            // ��ų�� ��Ÿ�� ������ Ȯ��
            if (attackSummon.IsSkillOnCooldown("SpecialAttack"))
            {
                Debug.Log("Ư�� ��ų�� ��Ÿ�� ���Դϴ�. ����� �� �����ϴ�.");
                return;
            }

            // Ư�� ���� ����
            attackSummon.SpecialAttack(battleController.getEnermyPlate(), selectedPlateIndex);
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

        if (battleController.IsEnermyPlateClear())
        {
            Debug.Log("�¸�!");
            battleAlert.clearAlert();
        }
    }



    public void SetHasSummonedThisTurn(bool value) //�̹��Ͽ� ��ȯ�ߴ��� ����
    {
        hasSummonedThisTurn = value;
    }

    public bool HasSummonedThisTurn()
    {
        return hasSummonedThisTurn;
    }

    private void ResetPlayerSetting()
    {
        mana = 10;
        usedMana = 1;
        UpdateManaUI();
    }

    public void UpdateManaUI()
    {
        for (int i = 0; i < manaList.Count; i++)
        {
            manaList[i].texture = (i < mana) ? haveTexture : notHaveTexture;
        }
    }


    public void setSelectedPlateIndex(int sel)
    {
        this.selectedPlateIndex = sel;
    }
}
