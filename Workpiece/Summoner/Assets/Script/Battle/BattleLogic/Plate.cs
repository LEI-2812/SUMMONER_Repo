using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Plate : MonoBehaviour, 
    IPointerEnterHandler, //�÷���Ʈ�� ���콺 �÷����� �̺�Ʈ �������̽�
    IPointerExitHandler,  //�÷���Ʈ�� ���콺�� ������� �̺�Ʈ �������̽�
    IPointerClickHandler //�÷���Ʈ Ŭ���� ����â 
{
    //plate�� �����ս��Ѽ� ������.
    public bool isInSummon = false; // ���� ��ȯ���� �ִ��� ����
    public Summon currentSummon;   // �÷���Ʈ ���� �ִ� ��ȯ��
    public GameObject statePanel;  // ���� �г� (On/Off)
    public StatePanel onMousePlateScript; // ���� �гο� ��ȯ�� ������ ������Ʈ�ϴ� ��ũ��Ʈ
    public Image summonImg;

    private Image plateImage; // �ڱ� �ڽ��� Image ������Ʈ
    private Color originalColor;

    [Header("��Ʈ�ѷ���")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private BattleController battleController;

    void Start()
    {
        statePanel.SetActive(false);
        plateImage = GetComponent<Image>(); // �ڽ��� Image ������Ʈ ��������
        originalColor = plateImage.color; // ���� ���� ����
    }

    // ��ȯ���� �÷���Ʈ�� ��ġ
    public void SummonPlaceOnPlate(Summon summon, bool isResummon = false)
    {
        // �̹� ��ȯ���� �־ ���ȯ�̸� ����
        if (!isInSummon || isResummon)
        {
            // ���� ��ȯ���� ������ �ı� (���ȯ ��)
            if (currentSummon != null && isResummon)
            {
                Destroy(currentSummon.gameObject);
                Debug.Log("���� ��ȯ���� �ı��Ǿ����ϴ�.");
            }

            // ��ȯ�� �������� Ŭ���Ͽ� ����
            Summon summonClone = Instantiate(summon);

            // Ŭ���� ���� �÷���Ʈ�� �ڽ����� ��ġ
            summonClone.transform.SetParent(this.transform, false);
            summonClone.transform.localPosition = Vector3.zero;  // �ʿ��� ��� ��ġ �ʱ�ȭ

            // Ŭ���� �̹��� ���� ���� (������ �����ϰ�)
            if (summonClone.image != null)
            {
                Color cloneColor = summonClone.image.color;
                cloneColor.a = 0.0f;  // Ŭ���� ���� ���� 0���� �����Ͽ� �����ϰ� ����
                summonClone.image.color = cloneColor;
            }

            // �÷���Ʈ�� summonImg�� ��ȯ���� �̹��� ����
            if (summonImg != null && summonClone.image != null && summonClone.image.sprite != null)
            {
                summonImg.sprite = summonClone.image.sprite; // summonImg�� ��ȯ�� �̹��� ����

                // summonImg�� ������ 1�� �����Ͽ� ������ ���̰�
                Color plateColor = summonImg.color;
                plateColor.a = 1.0f;  // ���� ���� 1�� ���� (���� ������)
                summonImg.color = plateColor;
            }

            // Ŭ�е� ��ȯ���� currentSummon���� ����
            currentSummon = summonClone;
            isInSummon = true;

            // ��ȯ�� �ʱ�ȭ ���� ȣ�� (�ʱ� �ɷ�ġ�� ��ų ����)
            summonClone.summonInitialize();

            Debug.Log($"��ȯ�� {summonClone.getSummonName()} �� {(isResummon ? "���ȯ" : "��ȯ")}�߽��ϴ�.");
        }
        else
        {
            Debug.Log("�̹� �� �÷���Ʈ�� ��ȯ���� �ֽ��ϴ�.");
        }
    }

    // ��ȯ���� ����ϰų� �÷���Ʈ���� ���� ��
    public void RemoveSummon()
    {
        if (isInSummon)
        {
            // ��ȯ�� �̹����� �ʱ� �������� �ǵ�����
            if (summonImg != null)
            {
                summonImg.sprite = null; // �̹����� ��� (�Ǵ� �⺻ �̹����� ����)

                // ������ 0���� ���� (������ �����ϰ�)
                Color color = summonImg.color;
                color.a = 0f;
                summonImg.color = color;
            }

            // ��ȯ�� ����
            currentSummon = null;
            isInSummon = false;
            Debug.Log("��ȯ�� ����.");
        }
    }

    // �÷���Ʈ ���� (���� ����)
    public void Highlight()
    {
        plateImage.color = Color.yellow; // �̹����� ������ ��������� ����
    }

    // ���� ����
    public void Unhighlight()
    {
        plateImage.color = originalColor; // �̹����� ������ ������� ����
    }


    //���콺 �÷����� �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSummon != null) //��ȯ���� ������
        {
            SetSummonImageTransparency(1.0f); // ������ ���� �� ���ϰ� ���̰�
        }
        if (isInSummon && summonController.IsSummoning()) //��ȯ���� �÷���Ʈ�� �ְ� ��ȯ��
        {
            Highlight(); // �÷���Ʈ ����
            SetSummonImageTransparency(1.0f); // ���� ���̱�
        }
       
        if (isInSummon && battleController.getIsAttaking()){ //�������϶�
            if (battleController.getIsHeal() && IsPlayerPlate())//���̸鼭 �Ʊ��÷���Ʈ�� ����
            {
                Highlight(); // �÷���Ʈ ����
            }
            else if (!battleController.getIsHeal() && IsEnemyPlate()) //���� �ƴ� �����̻��� �� �÷���Ʈ�� ����
            {
                Highlight();
            }
        }

    }


    //���콺�� �����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && summonController.IsSummoning()) //��ȯ���� �÷���Ʈ�� �ְ� ��ȯ��
        {
            Unhighlight(); // ���� ����
            SetSummonImageTransparency(0.5f); // �ٽ� �帮��
        }
        if (isInSummon && battleController.getIsAttaking())
        { //�������϶�
            if (battleController.getIsHeal() && IsPlayerPlate())//���̰� �Ʊ��÷���Ʈ��
            {
                Unhighlight(); // �÷���Ʈ ����
            }
            else if(!battleController.getIsHeal() && IsEnemyPlate()) //���̾ƴϸ� �������� ����
            {
                Unhighlight();
            }
        }

    }

    //�ش� �÷���Ʈ Ŭ���� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        // �÷��̾ ���ȯ ���̶�� ���� �г��� ���� �ʵ��� ��
        if (summonController.IsSummoning() && isInSummon)
        {
            summonController.SelectPlate(this);
            Unhighlight(); // ���� ����
            SetSummonImageTransparency(1.0f); //���� �ǵ�����
        }

        //����â Ȱ��ȭ
        else if (currentSummon != null && !summonController.IsSummoning() && !battleController.getIsAttaking())
        {
            Debug.Log("Ŭ���� �÷���Ʈ�� ��ȯ��:" + currentSummon.getSummonName());
            statePanel.SetActive(true); //���� �г� Ȱ��ȭ

            // ���� plate�� �ε����� ���� (�÷���Ʈ ����Ʈ���� �ڽ��� ã��)
            int plateIndex = summonController.GetPlayerPlateIndex(this);  // GetPlateIndex �޼ҵ带 ���� �ڽ��� �� ��°���� Ȯ��
            // BattleController�� ���õ� �÷���Ʈ �ε��� ����
            summonController.setPlayerSelectedIndex(plateIndex);

            onMousePlateScript.setStatePanel(currentSummon); // �гο� ��ȯ�� ���� ���� 
        }
        // ���� �߿� Ŭ���� ���
        if (battleController.getIsAttaking() && isInSummon && battleController)
        {
            if (!battleController.getIsHeal()) //���� �ƴϸ� �� �÷���Ʈ
            {
                // ���� �÷���Ʈ �ε����� ������
                int plateIndex = summonController.GetEnermyPlateIndex(this);

                if (plateIndex >= 0)
                {
                    // BattleController�� ���õ� �÷���Ʈ �ε��� ����
                    summonController.setPlayerSelectedIndex(plateIndex);
                    Debug.Log($"���� �÷���Ʈ {plateIndex}�� ���õǾ����ϴ�.");
                    Unhighlight(); // ���� ����
                }
                else
                {
                    Debug.Log("��ȿ�� ���� �÷���Ʈ�� ���õ��� �ʾҽ��ϴ�.");
                }
            }
            else
            {
                // ���� �÷���Ʈ �ε����� ������
                int plateIndex = summonController.GetPlayerPlateIndex(this);

                if (plateIndex >= 0)
                {
                    // BattleController�� ���õ� �÷���Ʈ �ε��� ����
                    summonController.setPlayerSelectedIndex(plateIndex);
                    Debug.Log($"�Ʊ��� �÷���Ʈ {plateIndex}�� ���õǾ����ϴ�.");
                    Unhighlight(); // ���� ����
                }
                else
                {
                    Debug.Log("��ȿ�� �Ʊ��� �÷���Ʈ�� ���õ��� �ʾҽ��ϴ�.");
                }
            }
        }

    }

    // ��ȯ�� �̹��� ���� ����
    public void SetSummonImageTransparency(float alpha)
    {
        if (summonImg != null)
        {
            Color color = summonImg.color;
            color.a = alpha; // ���� ����
            summonImg.color = color;
        }
    }

    // ���� �÷���Ʈ�� ���� �÷���Ʈ���� �˻��ϴ� �޼ҵ�
    public bool IsEnemyPlate()
    {
        PlateController plateController = battleController.GetPlateController(); // summonController�� ���� PlateController�� ����
        return plateController.getEnermyPlates().Contains(this);
    }

    // ���� �÷���Ʈ�� �÷��̾��� �÷���Ʈ���� �˻��ϴ� �޼ҵ�
    public bool IsPlayerPlate()
    {
        PlateController plateController = battleController.GetPlateController(); // summonController�� ���� PlateController�� ����
        return plateController.getPlayerPlates().Contains(this);
    }


    public Summon getSummon() //�÷���Ʈ�� ��ȯ���� ��ȯ
    {
        return currentSummon;
    }
}
