using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage2_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject bangImage;
    public GameObject confusebubbleImage;
    public GameObject dialogueBox;

    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField] private Animator playerAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;

    private int isSameDialgueIndex = -1;
    void Awake() //���⿡�� ������Ʈ���� �ʱ� ������ ���ش�.
    {
        // ����ǥ �� ��, �̸� ��Ȱ��ȭ
        bangImage.SetActive(false);
        confusebubbleImage.SetActive(false);
    }

    void Start() //Start���� ó�� ������ �޼ҵ峪 ������Ʈ�� �������ֵ��� �Ѵ�.
    {
        scenarioFlow();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickDialogue();
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
            case 1: // 28 ~ 33
                Debug.Log(scenarioFlowCount);
                /*  (���� ���� �и�)
                 *  �������� ��ȯ���� �����ݾ�?
                 */
                showConfuseEffect();
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //  �̷��� ���� �� �ߴµ� �巡���� ��� ���, ���� �� �Ǵ� �Ҹ��� �ϰ� �־�!                
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                /*  (�ٴڿ� ������ ������ �ݴ´�.)
                 *  �̰� ���� ����?
                 *  ���͵��� ���� �ǰ�?
                 */
                showBangEffect();
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //��, �̰ɷ� ������ �� �� �����ϰ� ���� �� �ְڴµ�.
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                //  ���⼭ PlayerYellow() ani �ѹ��� �����ϰ� �ٽ� idle ���·� ����
                offDialgueBox();
                showYellowEffect();
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                /*  �� ������ ���� ���� ����� ���� �ְھ�.
                 *  �巡���� ���� ������ �ȵ�����.
                 */
                onDialgueBox();
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

    public void showConfuseEffect() //Confuseȿ��
    {
        // ȥ�� �̹��� Ȱ��ȭ �� �ִϸ��̼� ����
        confusebubbleImage.SetActive(true);
        interactionController.stopNextDialogue(); //���⼭ �˾Ƽ� ��縦 ���߰���

        // 2�� �Ŀ� `endConfuseEffect` �޼��� ȣ��
        Invoke("endConfuseEffect", 1.5f);
    }
    private void endConfuseEffect()
    {
        // ȥ�� �̹��� ��Ȱ��ȭ �� ĳ���� ���� �ʱ�ȭ
        confusebubbleImage.SetActive(false);
        interactionController.startNextDialogue(); //Idle�� ���ƿ��� ���� ��縦 �̾ �� �ְ� ����
    }

    public void showBangEffect() //Confuseȿ��
    {
        // ȥ�� �̹��� Ȱ��ȭ �� �ִϸ��̼� ����
        bangImage.SetActive(true);
        interactionController.stopNextDialogue(); //���⼭ �˾Ƽ� ��縦 ���߰���

        Invoke("endBangEffect", 1f);
    }
    private void endBangEffect()
    {
        // ȥ�� �̹��� ��Ȱ��ȭ �� ĳ���� ���� �ʱ�ȭ
        bangImage.SetActive(false);
        interactionController.startNextDialogue(); //Idle�� ���ƿ��� ���� ��縦 �̾ �� �ְ� ����
    }

    public void showYellowEffect()
    {
        playerMove.playYellowAni();
    }

    private void nextScenarioFlow()
    {
        scenarioFlowCount++; //���� ��� �� �ó����� ������ ���� �� �ø���
    }
    private void onDialgueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void offDialgueBox()
    {
        Debug.Log("���â ����");
        dialogueBox.SetActive(false);
    }

    public void OnClickDialogue()
    {   //�÷��̾ �������� �ʴ� ��Ȳ�϶��� Ŭ�� ���
        if (!playerMove.getIsMoving())
        {
            interactionController.ShowNextLine();
            scenarioFlow();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickDialogue();
    }
}
