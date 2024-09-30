using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Player : Character
{
    [Header("�÷��̾� �÷���Ʈ")]
    [SerializeField] private List<Plate> playerPlates; // �÷��̾ ����� �÷���Ʈ ���

    //����
    private int mana; // �÷��̾��� �⺻����
    private int usedMana; //���ȯ�� �ʿ��� ����
    [Header("����UI")]
    [SerializeField] private List<RawImage> manaList; //����UI
    [Header("�����ؽ���")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;


    //��Ʈ�ѷ���
    [Header("��Ʈ�ѷ�")]
    [SerializeField] private TurnController turnController; // TurnController ����
    [SerializeField] private SummonController summonController; //��ȯ��ư���� ��ȯ���� �������� ���� �ʿ�

    // ���������� ��ȯ�ߴ� �÷���Ʈ ��ȣ ����
    private int lastSummonedPlateIndex = -1; // ��ȯ�ߴ� �÷���Ʈ�� �ε����� ���� (-1�� �ʱⰪ)

    // �ϴ� ��ȯ ���� Ȯ��
    private bool hasSummonedThisTurn;

    private void Start()
    {
        ResetPlayerSetting();
    }

  
    //�Ͻ���
    public override void startTurn()
    {
        base.startTurn();
        Debug.Log($"{gameObject.name} �� ����: {mana}");
        hasSummonedThisTurn = false; // �� �� ��ȯ���� �ʱ�ȭ
    }


    //�÷��̾��� Ȱ�� ����
    public override void takeAction()
    {
        Debug.Log("�÷��̾� takeAction ����");
    }


    // ��ȯ���� �̾� �ش� �÷���Ʈ�� ��ġ
    public void TakeSummon(int plateIndex, bool reSummon)
    {
        summonController.randomTakeSummon();
        summonController.TakeSummonPanel.SetActive(true);

        if (reSummon) //���ȯ�ϰ��
            StartCoroutine(ReSummonSelection(plateIndex));
        else //�Ϲݼ�ȯ
            StartCoroutine(TakeSummonSelection(plateIndex));
    }

    // ��ȯ �ڷ�ƾ (���� ��ȯ ����)
    private IEnumerator TakeSummonSelection(int plateIndex)
    {
        while (summonController.GetSelectedSummon() == null)
        {
            yield return null;
        }

        Summon selectedSummon = summonController.GetSelectedSummon();
        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon);
            lastSummonedPlateIndex = plateIndex; // ��ȯ�� �÷���Ʈ ��ȣ ����
            hasSummonedThisTurn = true;
            Debug.Log($"�÷���Ʈ {plateIndex}�� ��ȯ �Ϸ�.");
        }
        else
        {
            Debug.Log("���õ� ��ȯ���� �����ϴ�.");
        }
    }

    // ���ȯ �ڷ�ƾ
    private IEnumerator ReSummonSelection(int plateIndex)
    {
        while (summonController.GetSelectedSummon() == null)
        {
            yield return null;
        }

        Summon selectedSummon = summonController.GetSelectedSummon();
        if (selectedSummon != null)
        {
            playerPlates[plateIndex].SummonPlaceOnPlate(selectedSummon, isResummon: true);
            Debug.Log($"�÷���Ʈ {plateIndex}�� ���ȯ �Ϸ�.");
        }
        else
        {
            Debug.Log("���õ� ��ȯ���� �����ϴ�.");
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

    ///������� ��ư�� �޼ҵ� <summary>
    /// 
    /// 
    public void OnSummonBtnClick() //��ȯ ��ư Ŭ�� �޼ҵ�
    {
        Debug.Log($"{gameObject.name} �� ����: {mana}");
        if (hasSummonedThisTurn)
        {
            Debug.Log("�� �Ͽ����� �̹� ��ȯ�� �߽��ϴ�. ���� �Ͽ� ��ȯ�� �� �ֽ��ϴ�.");
            return; // ��ȯ�� �ϴ� 1ȸ�� ����
        }

        if (mana > 0) //���ȯ�� �ʿ��� �������� ���� 0���� ���ƾ���.
        {
            for (int i = 0; i < playerPlates.Count; i++) //����ִ� �÷���Ʈ�� ã���� ���������� ��ȸ
            {
                if (playerPlates[i].isInSummon == false) //���� ����� ����ִ� ���� ������ ����.
                {
                    Debug.Log(i + "��° �÷���Ʈ�� ��ȯ ����");
                    TakeSummon(i, false); // ��ȯ
                    mana -= 1; // �Ϲ� ��ȯ �� ���� 1 �Ҹ�
                    hasSummonedThisTurn = true; //�̹��� ��ȯ������ �˸�.
                    UpdateManaUI();
                    return; //�޼ҵ� �����ع�����
                }
            }
            Debug.Log("��� �÷���Ʈ�� ��ȯ���� �ֽ��ϴ�."); //��ȯ�� �����ߴµ� ����ִ°��� ��� ���
            return;
        }
        else
        {
            takeAction();
            Debug.Log("������ �����Ͽ� ��ȯ �Ұ���");
        }
    }

    //���ȯ ��ư Ŭ�� �̺�Ʈ
    public void OnReSummonBtnClick()
    {
        if (!hasSummonedThisTurn)
        {
            Debug.Log("�� �Ͽ� ��ȯ�� ���� �ʾҽ��ϴ�.");
            return; // ��ȯ�� �ϴ� 1ȸ�� ����
        }

        Debug.Log($"{gameObject.name} �� ����: {mana}");
        if (mana > usedMana) // ���ȯ�� �ʿ��� �������� ���� ���� ������ �� ������ Ȯ��
        {
            if (mana >= usedMana && lastSummonedPlateIndex != -1) // ���ȯ ������ ������ �ְ�, ��ȯ�� �÷���Ʈ�� �ִٸ�
            {
                TakeSummon(lastSummonedPlateIndex, true); // ��ϵ� �÷���Ʈ ��ȣ�� ���ȯ
                mana -= usedMana; // ���ȯ ���� �Ҹ�
                usedMana += 1; // ���ȯ �ʿ丶�� ����
                UpdateManaUI();
                Debug.Log("��ϵ� �÷���Ʈ�� ���ȯ �Ϸ�");
            }
            else
            {
                Debug.Log("�ش� �Ͽ� ��ȯ�� ��ȯ���� �����ϴ�.");
            }
        }
        else
        {
            Debug.Log("���ȯ�� �ʿ��� ������ �����մϴ�.");
        }
    }

    //�÷��̾� ���� �ʱ�ȭ
    private void ResetPlayerSetting()
    {
        mana = 10; //ù ���� ���۽� ���� 10���� ����
        usedMana = 1; //���� ����
        lastSummonedPlateIndex = -1; // ��ȯ ��� �ʱ�ȭ
        UpdateManaUI();
    }

    // ���� UI ������Ʈ �޼ҵ� �߰�
    public void UpdateManaUI()
    {
        for (int i = 0; i < manaList.Count; i++)
        {
            if (i < mana) // ���� ���� �ִ� ������ŭ ä����
            {
                // ������ ������ "Have" �ؽ�ó�� ����
                manaList[i].texture = haveTexture;
            }
            else
            {
                // ������ ������ "Not Have" �ؽ�ó�� ����
                manaList[i].texture = notHaveTexture;
            }
        }
    }

}

