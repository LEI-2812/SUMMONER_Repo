using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;
using TMPro;

public class Player : Character
{
    [Header("����UI")]
    [SerializeField] private List<RawImage> manaList;

    [Header("�����ؽ���")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;

    private int mana;
    private int usedMana;

    [Header("��ư UI")]
    [SerializeField] private Button summonButton;
     private TextMeshProUGUI summonButtonText;
    [SerializeField] private Button reSummonButton;
     private TextMeshProUGUI reSummonButtonText;

    [Header("����â �г�")]
    [SerializeField] private Image statePanel;

    [Header("ȿ����")]
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource failSound;

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
        summonButtonText = summonButton.GetComponentInChildren<TextMeshProUGUI>();
        reSummonButtonText = reSummonButton.GetComponentInChildren<TextMeshProUGUI>();
        battleAlert = GetComponent<BattleAlert>();
        ResetPlayerSetting();
    }

    private void Update()
    {
        if (plateController.IsEnermyPlateClear())
        {
            Debug.Log("�¸�!");
            battleAlert.clearAlert();
        }
        if (plateController.IsPlayerPlateClear() && mana <= 0)
        {
            battleAlert.failAlert();
        }
        //if(���߿� ���� Ŭ���� �� > turnController.turnCount){  �¸� ���� ���߿� �ٲ�� ��
        //    battleAlert.failAlert();
        //}
        if (mana < usedMana)
        {
            reSummonButton.image.color = new Color32(174, 174, 174, 255);
            reSummonButtonText.color = new Color32(209, 209, 209, 255);
        }
        else
        {
            reSummonButton.image.color = new Color32(249, 247, 196, 255);
            reSummonButtonText.color = new Color32(249, 247, 196, 255);
        }
    }

    public void startTurn()
    {
        Debug.Log("�÷��̾� �� ����");
        Debug.Log($"{gameObject.name} �� ����: {mana}");
        hasSummonedThisTurn = false;
        UpdateManaUI();
    }

    public void OnSummonBtnClick()
    {
        if (hasSummonedThisTurn)
        {
            failSound.Play();
            Debug.Log("�� �Ͽ����� �̹� ��ȯ�� �߽��ϴ�. ���� �Ͽ� ��ȯ�� �� �ֽ��ϴ�.");
            return;
        }

        if (mana > 0)
        {
            for (int i = 0; i < plateController.getPlayerPlates().Count; i++)
            {
                if (!plateController.getPlayerPlates()[i].getIsInSummon())
                {
                    Debug.Log(i + "��° �÷���Ʈ�� ��ȯ ����");
                    summonController.StartSummon(i, false);
                    mana -= 1;
                    hasSummonedThisTurn = true;
                    UpdateManaUI();
                    clickSound.Play();
                    summonButton.image.color = new Color32(137, 125, 115, 255); // ȸ��(#897D73)
                    summonButtonText.color = new Color32(159, 159, 159, 255);  // ȸ��(#9F9F9F)
                    return;
                }
            }
            Debug.Log("��� �÷���Ʈ�� ��ȯ���� �ֽ��ϴ�.");
        }
        else
        {
            failSound.Play(); // ȿ���� ���
            Debug.Log("������ �����Ͽ� ��ȯ �Ұ���");
        }
    }

    //�÷��̾�� ��ư Ŭ���� ���ؼ��� �����Ḧ ��Ų��.
    public void PlayerTurnOverBtn() //��ư�� ���� �޼ҵ�
    {
        // �÷��̾� ���� ���� �� ���� ����
        if (turnController.getCurrentTurn() == TurnController.Turn.PlayerTurn)
        {
            Debug.Log("�÷��̾� �� ����");
            turnController.EndTurn();
            clickSound.Play();
        }
        else
        {
            failSound.Play();
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
                clickSound.Play();
            }           
        }
        else
        {
            failSound.Play();
            Debug.Log("���ȯ�� �ʿ��� ������ ���ڶ��ϴ�.");
        }
    }


    public void OnAttackBtnClick() //�Ϲݰ���
    {
        Summon attackSummon = battleController.attackStart(0); //������ ��ȯ���� �޾ƿ´�.
        if (!attackSummon.getIsAttack()) {
            Debug.Log("������ �� �����ϴ�. ");
            failSound.Play();
            return; 
        }

        if (attackSummon != null)
        {
            // �Ϲ� ���� ����(�÷���Ʈ, ������ �ε���)
            attackSummon.normalAttack(plateController.getEnermyPlates() ,selectedPlateIndex);
            clickSound.Play();
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

        if (plateController.IsEnermyPlateClear())
        {
            Debug.Log("�¸�!");
            battleAlert.clearAlert();
        }
        plateController.CompactEnermyPlates();
        statePanel.gameObject.SetActive(false);
    }

    public void OnSpecialAttackBtnClick() //Ư������
    {
        Summon attackSummon = battleController.attackStart(0); // ������ ��ȯ���� ������
        if (!attackSummon.getIsAttack())
        {
            Debug.Log("������ �� �����ϴ�. ");
            failSound.Play();
            return;
        }

        if (attackSummon != null)
        {

            IAttackStrategy attackStrategy = attackSummon.getSpecialAttackStrategy()[0];

            // ��ų�� ��Ÿ�� ������ Ȯ��
            if (attackStrategy.getCurrentCooldown() > 0)
            {
                Debug.Log("Ư�� ��ų�� ��Ÿ�� ���Դϴ�. ����� �� �����ϴ�.");
                failSound.Play();
                return;
            }
            
            // TargetedAttackStrategy�� ����ϴ��� Ȯ��
            if (attackStrategy is TargetedAttackStrategy targetedAttack)
            {
                StatusType attackStatusType = targetedAttack.getStatusType();
                clickSound.Play();
                if (attackStatusType == StatusType.Heal || attackStatusType == StatusType.Upgrade || attackStatusType == StatusType.Shield) //Ÿ���߿� ���ϰ��
                {
                    Debug.Log("TargetedAttackStrategy�� Heal�� ����մϴ�. �Ʊ��� �÷���Ʈ�� �����ϼ���.");
                    // �Ʊ��� �÷���Ʈ�� �����ϴ� �ڷ�ƾ ����
                    StartCoroutine(WaitForPlayerPlateSelection(attackSummon, battleController.getNowSpecialAttackInfo().getAttackInfoIndex()));
                }
                else
                {
                    Debug.Log("TargetedAttackStrategy�� ����մϴ�. ���� �÷���Ʈ�� �����ϼ���.");
                    // ���� �÷���Ʈ�� �����ϴ� �ڷ�ƾ ����
                    StartCoroutine(WaitForEnermyPlateSelection(attackSummon, battleController.getNowSpecialAttackInfo().getAttackInfoIndex()));
                }
            }
            else
            {
                // TargetedAttackStrategy�� �ƴ� ��� �ٷ� ���� ����
                //������ ��ȯ��, ������ �÷���Ʈ �ε���, Ư����ų �迭�ε���, �÷��̾� ����
                battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, 0,true);
                clickSound.Play();
            }
        }
        else
        {
            Debug.Log("���õ� plate�� ��ȯ���� �����ϴ�.");
        }

        plateController.CompactEnermyPlates();
        statePanel.gameObject.SetActive(false);
    }

    private IEnumerator WaitForEnermyPlateSelection(Summon attackSummon, int SpecialAttackArrayIndex)
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
            battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, SpecialAttackArrayIndex, true); // true: �÷��̾� ����
            summonController.OnDarkBackground(false); // ���� �� ��� ����
            selectedPlateIndex = -1; //�����ߴ� �÷���Ʈ �ǵ�����
        }
        else
        {
            Debug.LogError("������ ���� �÷���Ʈ �ε����� ��ȿ���� �ʽ��ϴ�.");
        }

    }

    //�Ʊ� �÷���Ʈ ����
    private IEnumerator WaitForPlayerPlateSelection(Summon attackSummon, int SpecialAttackArrayIndex)
    {
        battleController.setIsAttaking(true); // ���� ����
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

        // �Ʊ��� �÷���Ʈ�� ���õ� �� ���� ����
        if (selectedPlateIndex >= 0)
        {
            Debug.Log($"�Ʊ����� ������ �غ����Դϴ�. ���õ� �÷���Ʈ �ε���: {selectedPlateIndex}");
            battleController.SpecialAttackLogic(attackSummon, selectedPlateIndex, SpecialAttackArrayIndex, true); // true: �÷��̾� ����
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

        // ��ȯ ��ư ���� �ʱ�ȭ
        if (mana > 0 && !hasSummonedThisTurn)   //  ��ȯ�� ������ �����ְ� �̹� �� ��ȯ�� ���� �ʾҴٸ�
        {
            summonButton.image.color = new Color32(227, 138, 64, 255);
            summonButtonText.color = new Color32(233, 197, 135, 255);
        }
    }

    public void AddMana()
    {
        mana += 1;
        if (mana > 10) {
            mana = 10;
        }
        UpdateManaUI();
    }

    public void setSelectedPlateIndex(int sel)
    {
        this.selectedPlateIndex = sel;
    }

    public PlateController getPlateController()
    {
        return plateController;
    }
}
