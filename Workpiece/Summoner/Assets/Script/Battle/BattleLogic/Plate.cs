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
    private bool isInSummon = false; // ���� ��ȯ���� �ִ��� ����
    [SerializeField]private Summon currentSummon;   // �÷���Ʈ ���� �ִ� ��ȯ��
    [SerializeField]private GameObject statePanel;  // ���� �г� (On/Off)
    [SerializeField] private StatePanel onMousePlateScript; // ���� �гο� ��ȯ�� ������ ������Ʈ�ϴ� ��ũ��Ʈ
    [SerializeField] private Image summonImg;

    private Image plateImage; // �ڱ� �ڽ��� Image ������Ʈ
    private Color originalColor;

    [Header("��Ʈ�ѷ���")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private BattleController battleController;

    void Start()
    {
        statePanel.SetActive(false);
        plateImage = GetComponent<Image>(); // �ڽ��� Image ������Ʈ ��������
       // summonImg = GetComponentInChildren<Image>();
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
            if (summonClone.getImage() != null)
            {
                Color cloneColor = summonClone.getImage().color;
                cloneColor.a = 0.0f;  // Ŭ���� ���� ���� 0���� �����Ͽ� �����ϰ� ����
                summonClone.getImage().color = cloneColor;
            }

            // �÷���Ʈ�� summonImg�� ��ȯ���� �̹��� ����
            if (summonImg != null && summonClone.getImage() != null && summonClone.getImage().sprite != null)
            {
                summonImg.sprite = summonClone.getImage().sprite; // summonImg�� ��ȯ�� �̹��� ����

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
       
        //Ÿ�ٰ����� ���� ���콺 ȿ��
        if (isInSummon && battleController.getIsAttaking()){ //�������϶�
            if (battleController.getAttakingSummon().getSpecialAttackStrategy()[0].getStatusType() == StatusType.Heal
                || battleController.getAttakingSummon().getSpecialAttackStrategy()[0].getStatusType() == StatusType.Upgrade
                || battleController.getAttakingSummon().getSpecialAttackStrategy()[0].getStatusType() == StatusType.Shield)//���̸鼭 �Ʊ��÷���Ʈ�� ����
            { //���϶�
                if (IsPlayerPlate())
                {
                    Highlight(); // �÷���Ʈ ����
                }
                else
                {
                    SetSummonImageTransparency(0.5f); //�� �÷���Ʈ ���� ���̱�
                }
            }
            else //Ÿ�ٰ����϶�
            {
                if (IsEnermyPlate())
                {
                    Highlight();
                }
                else
                {
                    SetSummonImageTransparency(0.5f); // ���� ���̱�
                }
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

        //Ÿ�ٰ����� ���� ���콺 ȿ��
        if (isInSummon && battleController.getIsAttaking())
        {
            Unhighlight();// �÷���Ʈ ��������
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

            onMousePlateScript.setStatePanel(currentSummon, IsEnermyPlate()); // �гο� ��ȯ�� ���� ���� 
        }

        // ���� �߿� Ŭ���� ���
        if (battleController.getIsAttaking() && isInSummon && battleController)
        {
            //���϶�
            if (battleController.getAttakingSummon().getSpecialAttackStrategy()[0].getStatusType() == StatusType.Heal
                || battleController.getAttakingSummon().getSpecialAttackStrategy()[0].getStatusType() == StatusType.Upgrade
                || battleController.getAttakingSummon().getSpecialAttackStrategy()[0].getStatusType() == StatusType.Shield) //��, ���׷��̵�, ����
            {
                if (IsPlayerPlate())
                {
                    // �Ʊ��� �÷���Ʈ �ε����� ������
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
            else //������ �����϶�
            {
                if (IsEnermyPlate()) //�� �÷���Ʈ���� �˻�
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
    public bool IsEnermyPlate()
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


    public Summon getCurrentSummon() //�÷���Ʈ�� ��ȯ���� ��ȯ
    {
        return currentSummon;
    }
    public void setCurrentSummon(Summon currentSummon)
    {
        this.currentSummon = currentSummon;
    }
    public bool getIsInSummon()
    {
        return isInSummon;
    }

    //public void ResetAllPlatesState()
    //{
    //    // �Ʊ� �÷���Ʈ�� ���� ����
    //    foreach (var plate in battleController.GetPlateController().getPlayerPlates())
    //    {
    //        if (plate.currentSummon != null)
    //        {
    //            plate.SetSummonImageTransparency(1.0f); // ���� ����
    //            plate.Unhighlight(); // ���� ����
    //        }
    //    }

    //    // �� �÷���Ʈ�� ���� ����
    //    foreach (var plate in battleController.GetPlateController().getEnermyPlates())
    //    {
    //        if (plate.currentSummon != null)
    //        {
    //            plate.SetSummonImageTransparency(1.0f); // ���� ����
    //            plate.Unhighlight(); // ���� ����
    //        }
    //    }
    //}
}
