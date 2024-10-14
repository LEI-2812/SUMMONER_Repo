using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage3_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject angryImage;
    public GameObject characterFox;
    public GameObject dialogueBox;

    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField] private Animator playerAni;
    [SerializeField] private Animator foxAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private FoxMove foxMove;

    private int isSameDialgueIndex = -1;

    void Awake() //���⿡�� ������Ʈ���� �ʱ� ������ ���ش�.
    {
        angryImage.SetActive(false);
        characterFox.SetActive(false);
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
            case 1: //  34 ~ 41
                Debug.Log(scenarioFlowCount);
                //  (���������� �ɾ� ������.)
                offDialgueBox();
                playerMove.CharacterMove(860f, 400f); // x��ǥ�� +860 �̵�, �ӵ� 200
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //  ���� �������� ���Գ�. ���� �󿡼��� �� ���� ������ �Ѵٴ���.
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //  ���� ���� �͵� ���������� �� ���·ο��.
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //  (��ȯ���� �����ϰ�, ���찡 ��Ÿ����.)
                showBlueEffect();
                offDialgueBox();
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /*  *��* ���뿴���� �� �������ٵ�.
                 *  ��, ���� ��� �� ����ؿ�. �̿��̸� ū ������.
                 */
                onDialgueBox();
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                //  (��ȯ�� '����'�� ȭ�� ����.)
                foxMove.playAngryAni();
                showAngryEffect();
                offDialgueBox();
                break;
            case 7:
                Debug.Log(scenarioFlowCount);
                //  ��¿ �� ���ݾ�. ���� �԰� ��ƾ��� ���� ���� �� �ִٰ�.
                onDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                // (��ȯ�� '����'�� ���������� �ɾ �������.)
                offDialgueBox();
                endAngryEffect();
                foxMove.CharacterMove(1200f, 400f);
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

    public void showAngryEffect()
    {
        angryImage.SetActive(true);
        foxMove.playAngryAni();
        //interactionController.stopNextDialogue();
    }
    private void endAngryEffect()
    {
        angryImage.SetActive(false);
        foxMove.stopAngryAni();
        //interactionController.startNextDialogue();
    }

    public void showBlueEffect()
    {
        playerMove.playBlueAni();
        Invoke("showFox", 1.5f);
    }

    private void showFox()
    {
        characterFox.SetActive(true);
        foxAni.Play("Fox_Idle");
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
