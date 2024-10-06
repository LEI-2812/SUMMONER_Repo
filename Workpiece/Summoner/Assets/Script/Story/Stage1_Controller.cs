using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour, ScenarioBase,IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject confusebubbleImage;
    public GameObject dotbubbleImage;

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
            case 1:
                Debug.Log(scenarioFlowCount);
                /*
                 * (���������� �ɾ��.)[1]
                 *  ..�׷��� �ϴ� �Ȱ� �ֱ� �ѵ�, ��� ������ ���� �ϴ� ����?
                 *  *����� �����ϸ�* �ƹ� ���̵� ��� ����.
                 * 
                 */
                playerMove.CharacterMove(700f, 200f); // x��ǥ�� +700 �̵�, �ӵ� 200
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                /*
                 *    (���������� �� ���ڱ� �� ���ư���.) [2]
                 *    ���. �� ������ ���� �¾�? �ƴ� �� ������.
                 */
                playerMove.CharacterMove(50f, 150f); // x��ǥ�� +50 �̵�, �ӵ� 150
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                /*
                 * (�������� �߰����� ���� ���� �� �ȴ´�.) [3]
                 * *�ν��� �ν���* ������ �ݴ� �����ε�, �׷� �Ʊ� ���� ������ �´� �ǰ�?
                 * 
                 */
                playerMove.CharacterMove(-50f, 250f); // x��ǥ�� +50 �̵�, �ӵ� 250
                break;
            case 4: 
                Debug.Log(scenarioFlowCount);
                /*
                 * (�ٽ� ���������� ���� ������ �ɾ��.) [4]
                 * *�Ѽ�* �� �����̸� ����.
                 * 
                 */
                playerMove.CharacterMove(50f, 150f); // x��ǥ�� +50 �̵�, �ӵ� 150
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /*
                 * (�̸��� ¤�´�.) [5]
                 *  ���´Ը� �ƴϾ�� �� ������ �� �� �ִ� �ǵ�!
                 *  �������� ������ �����ϴ� ������ �ΰ��� ���� �ȴٰ� �巡���� ��´ٴ� �ž�.
                 *  ���� ���� ���̴� ���� ���� �ϳ��� �� ���µ�.
                 */
                showConfuseEffect(); //Confuse �޼ҵ� ����
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                /*
                 * (��� �ð��� �帣��, õõ�� �Ͼ��.) [6]
                 *  ��
                 *  ������ �شٴ� ������, ��¼�ھ�.
                 *  ��ħ �ɵ鸮�� ���̴� ��� �� �� �� �ε��� �޾ƺ���, ��.
                 * 
                 * 
                 */
                showDotbubbleEffect();
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
        playerMove.playConfuseAni(); //���⼭ �˾Ƽ� ��縦 ���߰���

        // 2�� �Ŀ� `endConfuseEffect` �޼��� ȣ��
        Invoke("endConfuseEffect", 2f);
    }
    private void endConfuseEffect()
    {
        // ȥ�� �̹��� ��Ȱ��ȭ �� ĳ���� ���� �ʱ�ȭ
        confusebubbleImage.SetActive(false);
        playerMove.stopConfuseAni(); //Idle�� ���ƿ��� ���� ��縦 �̾ �� �ְ� ����
    }

    public void showDotbubbleEffect()
    {
        dotbubbleImage.SetActive(true);
        playerMove.stopNextDialogue();

        Invoke("endDotbubbleEffect", 2f);
    }
    private void endDotbubbleEffect()
    {
        dotbubbleImage.SetActive(false);
        playerMove.startNextDialogue();
    }

    private void nextScenarioFlow()
    {
        scenarioFlowCount++; //���� ��� �� �ó����� ������ ���� �� �ø���
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        //�÷��̾ �������� �ʴ� ��Ȳ�϶��� Ŭ�� ���
        if(!playerMove.getIsMoving())
            scenarioFlow();
    }
}
