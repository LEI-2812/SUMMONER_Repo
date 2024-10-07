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
            case 1://32 33 34 35
                Debug.Log(scenarioFlowCount);
                playerMove.CharacterMove(860f, 200f); // x��ǥ�� +860 �̵�, �ӵ� 200
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //Ǫ�� ���� ��¦ �� ���� ��ȯ
                //Ǫ�� ���� ��¦
                characterFox.SetActive(true);
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //���찡 ȭ��
                showAngryEffect();
                break;
            /*
            case 5: �� �ȵǳ�?
                Debug.Log(scenarioFlowCount);
                //�� ��縦 ġ�� ���찡 ���ߴ�
                endAngryEffect();
                foxMove.CharacterMove(1000f, 200f);
                break;  
            */
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
        interactionController.stopNextDialogue();
    }
    private void endAngryEffect()
    {
        angryImage.SetActive(false);
        interactionController.startNextDialogue();
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
