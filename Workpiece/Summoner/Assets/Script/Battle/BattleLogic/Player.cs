using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;

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
    [SerializeField] private PlateController plateController;

     BattleAlert battleAlert;

    private int selectedPlateIndex = -1;

    private bool hasSummonedThisTurn;

    private void Start()
    {
        battleAlert = GetComponent<BattleAlert>();
        ResetPlayerSetting();
    }

    private void Update()
    {
        if(battleController.IsPlayerPlateClear() && mana <= 0)
        {
            battleAlert.failAlert();
        }
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
            for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
            {
                if (!plateController.getPlayerPlates()[i].isInSummon)
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
            attackSummon.normalAttack(plateController.getEnermyPlates(),selectedPlateIndex);
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
            IAttackStrategy attackStrategy = attackSummon.getSpecialAttackStrategy();
            //���Ⱑ ���ڵ�
            // TargetedAttackStrategy�� ����ϴ��� Ȯ��
            if (attackStrategy is TargetedAttackStrategy targetedAttack)
            {
                StatusType attackStatusType = targetedAttack.getTargetAttackStatusType();
                if (attackStatusType == StatusType.Heal) //Ÿ���߿� ���ϰ��
                {
                    Debug.Log("TargetedAttackStrategy�� Heal�� ����մϴ�. �Ʊ��� �÷���Ʈ�� �����ϼ���.");
                    // �Ʊ��� �÷���Ʈ�� �����ϴ� �ڷ�ƾ ����
                    StartCoroutine(WaitForPlayerPlateSelection(attackSummon));
                }
                else
                {
                    Debug.Log("TargetedAttackStrategy�� ����մϴ�. ���� �÷���Ʈ�� �����ϼ���.");
                    // ���� �÷���Ʈ�� �����ϴ� �ڷ�ƾ ����
                    StartCoroutine(WaitForEnermyPlateSelection(attackSummon));
                }
            }
            else
            {
                // TargetedAttackStrategy�� �ƴ� ��� �ٷ� ���� ����
                battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, true); // true: �÷��̾� ����
            }
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

    private IEnumerator WaitForEnermyPlateSelection(Summon attackSummon)
    {
        battleController.setIsAttaking(true); // ���� ����
        summonController.OnDarkBackground(true); // ��� ��Ӱ� ó��
        plateController.DownTransparencyForWhoPlate(true); //�Ʊ� ��ȯ�� ����ȭ
        selectedPlateIndex = -1; //������ �÷���Ʈ �ʱ�ȭ
        // ���� �÷���Ʈ ������ ��ٸ�
        Debug.Log("���� �÷���Ʈ�� �����ϴ� ���Դϴ�...");
        // ���õ� �÷���Ʈ�� ���� ������ ��ٸ�
        while (selectedPlateIndex < 0)
        {
            if (battleController.getIsAttaking())
            {

                if (selectedPlateIndex >= 0)
                {
                    Debug.Log($"���� �÷���Ʈ {selectedPlateIndex}�� ���õǾ����ϴ�.");
                    break; // while ���� Ż��
                }
            }

            yield return null; // �� ������ ���
        }

        // ���� �÷���Ʈ�� ���õ� �� ���� ����
        if (selectedPlateIndex >= 0)
        {
            Debug.Log($"������ �غ� ���Դϴ�. ���õ� �÷���Ʈ �ε���: {selectedPlateIndex}");
            battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, true); // true: �÷��̾� ����
            summonController.OnDarkBackground(false); // ���� �� ��� ����
            selectedPlateIndex = -1; //�����ߴ� �÷���Ʈ �ǵ�����
        }
        else
        {
            Debug.LogError("������ ���� �÷���Ʈ �ε����� ��ȿ���� �ʽ��ϴ�.");
        }

    }

    //�Ʊ� �÷���Ʈ ����
    private IEnumerator WaitForPlayerPlateSelection(Summon attackSummon)
    {
        battleController.setIsAttaking(true); // ���� ����
        battleController.setIsHeal(true);
        summonController.OnDarkBackground(true); // ��� ��Ӱ� ó��
        plateController.DownTransparencyForWhoPlate(false); //�� ��ȯ�� ����ȭ
        selectedPlateIndex = -1; //������ �÷���Ʈ �ʱ�ȭ
        // �Ʊ��� �÷���Ʈ ������ ��ٸ�
        Debug.Log("�Ʊ��� �÷���Ʈ�� �����ϴ� ���Դϴ�...");
        // ���õ� �÷���Ʈ�� ���� ������ ��ٸ�
        while (selectedPlateIndex < 0)
        {
            if (battleController.getIsAttaking())
            {

                if (selectedPlateIndex >= 0)
                {
                    Debug.Log($"�Ʊ��� �÷���Ʈ {selectedPlateIndex}�� ���õǾ����ϴ�.");
                    break; // while ���� Ż��
                }
            }

            yield return null; // �� ������ ���
        }

        // ���� �÷���Ʈ�� ���õ� �� ���� ����
        if (selectedPlateIndex >= 0)
        {
            Debug.Log($"���� �غ� ���Դϴ�. ���õ� �÷���Ʈ �ε���: {selectedPlateIndex}");
            battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, true); // true: �÷��̾� ����
            summonController.OnDarkBackground(false); // ���� �� ��� ����
            selectedPlateIndex = -1; //�����ߴ� �÷���Ʈ �ǵ�����
        }
        else
        {
            Debug.LogError("�Ʊ��� �÷���Ʈ �ε����� ��ȿ���� �ʽ��ϴ�.");
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
