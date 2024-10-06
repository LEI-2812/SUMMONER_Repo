using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour, IPointerClickHandler
{
    [Header("ĳ���� �̸� �ؽ�Ʈ")]
    public Text nameText; //��� ��� ĳ�����̸� �ؽ�Ʈ

    [Header("ĳ���� ��� �ؽ�Ʈ")]
    public Text dialogueText; //��� �ؽ�Ʈ

    [Header("ǥ���� ������Ʈ��")]
    public GameObject characterPlayer; //���ΰ� ĳ���� ������Ʈ
    public GameObject confusebubbleImage;

    private int scenarioFlow = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField]private Animator playerAni;

    [SerializeField]private InteractionController interactionController;

    private Vector3 targetPosition; // �̵��� ��ǥ ��ġ
    private bool isMoving; // �̵� ���� Ȯ��
    private int isSameDialgueIndex = -1;

    void Awake() //���⿡�� ������Ʈ���� �ʱ� ������ ���ش�.
    {
        confusebubbleImage.SetActive(false);
    }

    void Start() //Start���� ó�� ������ �޼ҵ峪 ������Ʈ�� �������ֵ��� �Ѵ�.
    {
        stage_1_Flow();
    }

    void Update()
    {
        // ������ �Է����� ��� �ѱ�� (��: �����̽���)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stage_1_Flow();
        }

        if (isMoving)
        {
            MoveToTarget(200f); // �̵� �ӵ��� ���ڷ� ����
        }
    }

    public void stage_1_Flow()
    {

        if (checkCSVDialogueID()) //���� ����� ID�� ���� ID�� ������ �׳� ��縸 ��½�Ŵ.
        {
            return;
        }

        switch (scenarioFlow)
        {
            case 1:
                Debug.Log(scenarioFlow);
                /*
                 * (���������� �ɾ��.)[1]
                 *  ..�׷��� �ϴ� �Ȱ� �ֱ� �ѵ�, ��� ������ ���� �ϴ� ����?
                 *  *����� �����ϸ�* �ƹ� ���̵� ��� ����.
                 * 
                 */
                CharacterMove(700f); // x��ǥ�� +700(������)���� �̵�
                break;
            case 2:
                Debug.Log(scenarioFlow);
                /*
                 *    (���������� �� ���ڱ� �� ���ư���.) [2]
                 *    ���. �� ������ ���� �¾�? �ƴ� �� ������.
                 */
                CharacterMove(50f); // x��ǥ�� +50(������)���� �̵�
                break;
            case 3:
                Debug.Log(scenarioFlow);
                /*
                 * (�������� �߰����� ���� ���� �� �ȴ´�.) [3]
                 * *�ν��� �ν���* ������ �ݴ� �����ε�, �׷� �Ʊ� ���� ������ �´� �ǰ�?
                 * 
                 */
                CharacterMove(-50f); // x��ǥ�� +50(������)���� �̵�
                break;
            case 4: 
                Debug.Log(scenarioFlow);
                /*
                 * (�ٽ� ���������� ���� ������ �ɾ��.) [4]
                 * *�Ѽ�* �� �����̸� ����.
                 * 
                 */
                CharacterMove(50f); // x��ǥ�� +50(������)���� �̵�
                break;
            case 5:
                Debug.Log(scenarioFlow);
                /*
                 * (�̸��� ¤�´�.) [5]
                 *  ���´Ը� �ƴϾ�� �� ������ �� �� �ִ� �ǵ�!
                 *  �������� ������ �����ϴ� ������ �ΰ��� ���� �ȴٰ� �巡���� ��´ٴ� �ž�.
                 *  ���� ���� ���̴� ���� ���� �ϳ��� �� ���µ�.
                 */
                ShowConfuseEffect();

                break;
            case 6:
                Debug.Log(scenarioFlow);
                /*
                 * (��� �ð��� �帣��, õõ�� �Ͼ��.) [6]
                 *  ��
                 *  ������ �شٴ� ������, ��¼�ھ�.
                 *  ��ħ �ɵ鸮�� ���̴� ��� �� �� �� �ε��� �޾ƺ���, ��.
                 * 
                 * 
                 */


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





    // ��ǥ ��ġ�� �����ϴ� �޼ҵ�
    private void CharacterMove(float distance)
    {
        targetPosition = characterPlayer.transform.position + new Vector3(distance, 0f, 0f);

        // �̵� ���⿡ ���� ĳ������ ���� ��ȯ
        if (distance < 0)
        {
            // �����̵��� ĳ���͸� ������
            characterPlayer.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (distance > 0)
        {
            // �������̵��� ĳ���͸� ���� �������� ����
            characterPlayer.transform.localScale = new Vector3(1, 1, 1);
        }

        //�̵� �����ϸ鼭 ������縦 ���ϵ��� ���� �ִϸ��̼� �������
        isMoving = true;
        interactionController.setIsStory(isMoving);
        playerAni.Play("PlayerWalk");
    }


    // ĳ���͸� ��ǥ ��ġ�� �̵���Ű�� �޼ҵ�
    private void MoveToTarget(float speed)
    {
        // ���� ��ġ���� ��ǥ ��ġ���� ������ �ӵ��� �̵�
        characterPlayer.transform.position = Vector3.MoveTowards(characterPlayer.transform.position, targetPosition, speed * Time.deltaTime);

        // ��ǥ ��ġ�� �����ϸ� �̵� ����
        if (Vector3.Distance(characterPlayer.transform.position, targetPosition) < 0.01f)
        {
            StopMoving();
        }
    }

    private void StopMoving()
    {
        playerAni.Play("Idle");
        isMoving = false; // �̵� ����
        interactionController.setIsStory(isMoving); //���� ���� �ѱ� �� �ְ� ����
    }

    private bool isConfuseEffectActive = false;
    public void ShowConfuseEffect() //Confuseȿ��
    {
        //Debug.Log("Showconfuse�ߵ�");
        // ���� ��縦 ���ϵ��� ����
        isMoving = true;
        interactionController.setIsStory(true);
        isConfuseEffectActive = true;

        // ȥ�� �̹��� Ȱ��ȭ �� �ִϸ��̼� ����
        confusebubbleImage.SetActive(true);
        playerAni.Play("Confuse");

        // 2�� �Ŀ� `EndConfuseEffect` �޼��� ȣ��
        Invoke("EndConfuseEffect", 2f);
    }
    private void EndConfuseEffect()
    {
        if (!isConfuseEffectActive)
        {
            // ȥ�� �̹��� ��Ȱ��ȭ �� ĳ���� ���� �ʱ�ȭ
            confusebubbleImage.SetActive(false);
            StopMoving(); // �̰����� isStory�� false�� �ٲ�
        }
    }

    private void nextScenarioFlow()
    {
        scenarioFlow++; //���� ��� �� �ó����� ������ ���� �� �ø���
    }

    public bool getIsMoving()
    {
        return isMoving;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if( !isMoving )
            stage_1_Flow();
    }
}
