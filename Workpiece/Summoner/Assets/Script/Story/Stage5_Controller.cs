using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage5_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject bangImage;

    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField] private Animator playerAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private EnemyMove enemyMove;

    private int isSameDialgueIndex = -1;

    void Awake() //���⿡�� ������Ʈ���� �ʱ� ������ ���ش�.
    {
        bangImage.SetActive(false);
    }

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
            case 1://36~40
                Debug.Log(scenarioFlowCount);
                //���������� �ɾ���� ���ΰ�
                playerMove.CharacterMove(980f, 200f);
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //�������� �Ҹ�
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //���� �������� �ɾ��
                enemyMove.CharacterMove(-600f, 200f);
                break;
            case 5:
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
    public void showBangEffect() //Confuseȿ��
    {
        // ȥ�� �̹��� Ȱ��ȭ �� �ִϸ��̼� ����
        bangImage.SetActive(true);
        interactionController.stopNextDialogue(); //���⼭ �˾Ƽ� ��縦 ���߰���

        Invoke("endBangEffect", 2f);
    }
    private void endBangEffect()
    {
        // ȥ�� �̹��� ��Ȱ��ȭ �� ĳ���� ���� �ʱ�ȭ
        bangImage.SetActive(false);
        interactionController.startNextDialogue(); //Idle�� ���ƿ��� ���� ��縦 �̾ �� �ְ� ����
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
