using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage5_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("ǥ���� ������Ʈ��")]
    public GameObject bangImage;
    public GameObject dialogueBox;

    private int scenarioFlowCount = 0; //��� ī��Ʈ

    //�÷��̾� �ִϸ��̼�
    [SerializeField] private Animator playerAni;

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private EnemyMove enemyMove;

    [Header("����")]
    [SerializeField] private AudioClip bangSound;
    [SerializeField] private AudioSource audioSource;

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
            case 1: // 42 ~ 49
                Debug.Log(scenarioFlowCount);
                //  (���������� �ɾ��.)
                offDialgueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //  ���� ���� ��������. ������ �߰��� ���������̶�� �ϴ���.
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //  (���� �������� �Ҹ��� ����.)
                playBangSound();
                showBangEffect();
                offDialgueBox();
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //  ����, ������!
                onDialgueBox();               
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                //  ...
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                //  �ƹ��� ���� �ǰ�..?
                break;
            case 7:
                Debug.Log(scenarioFlowCount);
                //  (�����ʿ��� ���ڱ� �� �� ���� Ƣ��´�.)
                enemyMove.CharacterMove(-600f, 550f);
                offDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                //  ���! �ϱ� �Ǹ��ΰ�? ����ġ �ʰڴµ�..
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
    private void playBangSound()
    {
        if (audioSource != null && bangSound != null)
        {
            audioSource.PlayOneShot(bangSound); // ȿ������ �� �� ���
        }
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
