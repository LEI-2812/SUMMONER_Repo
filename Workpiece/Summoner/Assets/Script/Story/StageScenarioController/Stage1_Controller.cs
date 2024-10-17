using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject confusebubbleImage; // ���δ� �̹���
    public GameObject dotbubbleImage; // ... �̹���
    public GameObject dialogueBox;
    public GameObject skipBtn;

    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField]private Animator playerAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField]private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;

    private int isSameDialgueIndex = -1;

    void Awake() //���⿡�� ������Ʈ���� �ʱ� ������ ���ش�.
    {
        confusebubbleImage.SetActive(false);
        dotbubbleImage.SetActive(false);
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
        { //�ó����� �����ؼ� �ڵ� ���� �����ϱ� �����ϴ�.
            case 1:
                Debug.Log(scenarioFlowCount);
                /*
                 * (���������� �ɾ��.)[1]
                 */
                offDialgueBox(); //�ؽ�Ʈ�� �ӽ÷� ���д�.
                playerMove.CharacterMove(700f, 400f); // x��ǥ�� +700 �̵�, �ӵ� 400 �����̴� ���� �������� ���Ѿ
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                /* <<������>>
                *..�׷��� �ϴ� �Ȱ� �ֱ� �ѵ�, ��� ������ ���� �ϴ� ����?
                 */
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                /* <<������>>
                 **����� �����ϸ�* �ƹ� ���̵� ��� ����.
                 */
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                /*
                 * (���������� �� ���ڱ� �� ���ư���.) [2]
                 */
                offDialgueBox();
                playerMove.CharacterMove(150f, 400f); // x��ǥ�� +150 �̵�, �ӵ� 400
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /* <<������>>
                 *    ���. �� ������ ���� �¾�? �ƴ� �� ������.
                 */
                onDialgueBox();
                playerMove.CharacterMove(-150f, 400f); // x��ǥ�� -150 �̵�, �ӵ� 400
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                /*
                 * (�������� �߰����� ���� ���� �� �ȴ´�.) [3]
                 * *�ν��� �ν���* ������ �ݴ� �����ε�, �׷� �Ʊ� ���� ������ �´� �ǰ�?
                 * 
                 */               
                playerMove.CharacterMove(150f, 300f); // x��ǥ�� +150 �̵�, �ӵ� 300
                break;
            case 7: 
                Debug.Log(scenarioFlowCount);
                /*
                 * (�ٽ� ���������� ���� ������ �ɾ��.) [4]
                 * 
                 */
                offDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                //*�Ѽ�* �� �����̸� ����.
                onDialgueBox();
                break;
            case 9:
                Debug.Log(scenarioFlowCount);
                //�̸��� ¤�´�.
                offDialgueBox();
                showConfuseEffect(); //���� �̹��� ���
                break;
            case 10:
                Debug.Log(scenarioFlowCount);
                /*
                 * ���´Ը� �ƴϾ�� �� ������ �� �� �ִ� �ǵ�!
                 * �������� ������ �����ϴ� ������ �ΰ��� ���� �ȴٰ� �巡���� ��´ٴ� �ž�.
                 * ���� ���� ���̴� ���� ���� �ϳ��� �� ���µ�
                 */
                onDialgueBox();
                break;
            case 11:
                Debug.Log(scenarioFlowCount);
                /*
                 * (��� �ð��� �帣��, õõ�� �Ͼ��.)
                 * ...
                 */
                offDialgueBox();
                showDotbubbleEffect(); //... �̹��� ���
                break;
            case 12:
                Debug.Log(scenarioFlowCount);
                /*
                 * ������ �شٴ� ������, ��¼�ھ�.
                 * ��ħ �ɵ鸮�� ���̴� ��� �� �� �� �ε��� �޾ƺ���, ��.
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

    public void showConfuseEffect() //Confuseȿ�� ����
    {
        Invoke("onConfuseImage", 0.4f); // 0.4�� �� ȥ�� �̹��� Ȱ��ȭ
        playerMove.playConfuseAni(); //�ִϸ��̼��� �ٷ� ����
    }
    private void onConfuseImage() //0.4���Ŀ� �̹��� Ȱ��ȭ��Ű��
    {
        confusebubbleImage.SetActive(true);
        //1���� �̹����� ������
        Invoke("offConfuseImage", 1f);
    }
    private void offConfuseImage() //1.4�ʶ� �̹����� ��Ȱ��ȭ ��
    {
        confusebubbleImage.SetActive(false);
        Invoke("endConfuseEffect", 0.4f);
    }
    private void endConfuseEffect() //1.8�� �ڿ��� ������
    {
        playerMove.stopConfuseAni(); //Idle�� ���ƿ��� ���� ��縦 �̾ �� �ְ� ����
    }
   

    public void showDotbubbleEffect()
    {
        dotbubbleImage.SetActive(true);
        interactionController.stopNextDialogue();

        Invoke("endDotbubbleEffect", 2f);
    }
    private void endDotbubbleEffect()
    {
        dotbubbleImage.SetActive(false);
        interactionController.startNextDialogue();
    }

    private void onDialgueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void offDialgueBox()
    {
        dialogueBox.SetActive(false);
    }

    private void nextScenarioFlow()
    {
        scenarioFlowCount++; //���� ��� �� �ó����� ������ ���� �� �ø���
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