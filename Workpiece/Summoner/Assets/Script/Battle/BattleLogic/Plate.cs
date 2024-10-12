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
        if (isInSummon && summonController.IsReSummoning()) // ���ȯ ���̰� ��ȯ���� �ִ� ���
        {
            Highlight(); // �÷���Ʈ ����
            SetSummonImageTransparency(1.0f); // ���� ���̱�
        }
    }


    //���콺�� �����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSummon != null && summonController.IsReSummoning()) //���ȯ���϶� �� ���ϰ�
        {
            Unhighlight(); // ���� ����
            SetSummonImageTransparency(0.5f); // �ٽ� �帮��
        }

    }

    //�ش� �÷���Ʈ Ŭ���� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        // �÷��̾ ���ȯ ���̶�� ���� �г��� ���� �ʵ��� ��
        if (summonController.IsReSummoning() && isInSummon)
        {
                summonController.SelectPlate(this);
                Unhighlight(); // ���� ����
                SetSummonImageTransparency(1.0f); //���� �ǵ�����
        }

        //����â Ȱ��ȭ
        else if (currentSummon != null && !summonController.IsReSummoning())
        {
            Debug.Log("Ŭ���� �÷���Ʈ�� ��ȯ��:" + currentSummon.getSummonName());
            statePanel.SetActive(true); //���� �г� Ȱ��ȭ

            // ���� plate�� �ε����� ���� (�÷���Ʈ ����Ʈ���� �ڽ��� ã��)
            int plateIndex = summonController.GetPlateIndex(this);  // GetPlateIndex �޼ҵ带 ���� �ڽ��� �� ��°���� Ȯ��
            // BattleController�� ���õ� �÷���Ʈ �ε��� ����
            summonController.setPlayerSelectedIndex(plateIndex);

            onMousePlateScript.setStatePanel(currentSummon); // �гο� ��ȯ�� ���� ���� 
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


    public Summon getSummon() //�÷���Ʈ�� ��ȯ���� ��ȯ
    {
        return currentSummon;
    }
}
