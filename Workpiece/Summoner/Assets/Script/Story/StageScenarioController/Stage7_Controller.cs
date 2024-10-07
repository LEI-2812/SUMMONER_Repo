using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage7_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField] private Animator playerAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;

    private int isSameDialgueIndex = -1;

    void Start() //Start���� ó�� ������ �޼ҵ峪 ������Ʈ�� �������ֵ��� �Ѵ�.
    {
        scenarioFlow();
    }

    void Update()
    {
        // ������ �Է����� ��� �ѱ�� (��: �����̽���)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            scenarioFlow();
        }
    }

    public void scenarioFlow()
    {
        if (checkCSVDialogueID()) //���� ����� ID�� ���� ID�� ������ �׳� ��縸 ��½�Ŵ.
        {
            return;
        }

        switch (scenarioFlowCount)
        {
            case 1://41~48
                Debug.Log(scenarioFlowCount);
                //���������� �ɾ���� ���ΰ�
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                break;
            case 7:
                Debug.Log(scenarioFlowCount);
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                break;
        }
    }

    // ��� ID�� ���ϴ� �޼ҵ�
    private bool checkCSVDialogueID()
    {
        // InteractionController���� ���� ��� CSV ID ��������
        int currentDialogueIndex = interactionController.getCurrentDialogueIndex();

        // ���� ����� ID�� ���� ����� ID�� ������ ��縸 �����ϰ� ����
        if (currentDialogueIndex == isSameDialgueIndex)
        {
            //interactionController.ShowNextLine();
            return true;
        }
        // ����� ID�� ����� ��츸 �̵� ó��
        isSameDialgueIndex = currentDialogueIndex; // ���� ��� ID ������Ʈ
        // ����ġ�� ���� �� scenarioFlow ����
        nextScenarioFlow();

        return false;
    }
    private void nextScenarioFlow()
    {
        scenarioFlowCount++; //���� ��� �� �ó����� ������ ���� �� �ø���
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //�÷��̾ �������� �ʴ� ��Ȳ�϶��� Ŭ�� ���
        if (!playerMove.getIsMoving())
            scenarioFlow();

    }
}
