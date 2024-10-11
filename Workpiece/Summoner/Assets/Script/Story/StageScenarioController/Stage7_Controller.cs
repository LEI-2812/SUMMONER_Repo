using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage7_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject dialogueBox;

    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField] private Animator playerAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;

    private int isSameDialgueIndex = -1;

    void Start()
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
            case 1: //  50 ~ 58
                Debug.Log(scenarioFlowCount);
                //  (���������� ���ɽ����� �̵��Ѵ�.)
                offDialgueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //  *�ұټұ�* �巡���� �� �ڽ��ΰ�����.
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //  ������, �� ���� ���� �ڴ�.
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                /*  ��, �ȳ�.
                 *  �� �׳� �ʶ� ����Ϸ� �� �ǵ�.
                 */
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /*  �� �ΰ� ������ ������� �ʴ´�.
                 *  �� �������� ���� �ο�� ���� ������� �� ���̰���.
                 */
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                /*  �±� �ѵ�, �� �������� �ذ��ϰ� �Ͱŵ�. ���� ���⼭ �װ� ���� ������ ������ ����.
                 *  �� ���� ������ ����.
                 *  �׳� ������ ���� �ϳ��� ��. ������ �� �װ� �ϳ� ���ٰ� ���� ���ݾ�.
                 */ 
                break;
            case 7:
                Debug.Log(scenarioFlowCount);
                /*  ��, �� ������ �޶��?
                 *  �׷� �� ����.
                 *  ��� ���� ������, �׷��ٰ� �ʸ� �̷��� ������ ������ ���� ����.
                 *  ��, � ������!
                 */
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                /*  (�̰� �ߴܳ���.)
                 *  ���� �ذ��� �� �־��µ�, �װ� ������ �ž�. �� �𸥴�?
                 */
                break;
            case 9:
                Debug.Log(scenarioFlowCount);
                /*  ��� ������ �̹� ������ �߳� ���� ��������?
                 *  �ʾ߸��� ���� ���⼭ ���� �غ��ض�!
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
    public void OnPointerClick(PointerEventData eventData)
    {
        //�÷��̾ �������� �ʴ� ��Ȳ�϶��� Ŭ�� ���
        if (!playerMove.getIsMoving())
            scenarioFlow();
    }
}
