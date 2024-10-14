using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage3_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("표현할 오브젝트들")]
    public GameObject angryImage;
    public GameObject characterFox;
    public GameObject dialogueBox;

    private int scenarioFlowCount = 0; //대사 카운트

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;
    [SerializeField] private Animator foxAni;

    [Header("컨트롤러")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private FoxMove foxMove;

    private int isSameDialgueIndex = -1;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        angryImage.SetActive(false);
        characterFox.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        scenarioFlow();
    }

    void Update()
    {
        // 유저의 입력으로 대사 넘기기 (예: 스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickDialogue();
        }
    }

    public void scenarioFlow()
    {
        if (checkCSVDialogueID()) //다음 대사의 ID가 이전 ID와 같으면 그냥 대사만 출력시킴.
        {
            return;
        }

        switch (scenarioFlowCount)
        {
            case 1: //  34 ~ 41
                Debug.Log(scenarioFlowCount);
                //  (오른쪽으로 걸어 나간다.)
                offDialgueBox();
                playerMove.CharacterMove(860f, 400f); // x좌표로 +860 이동, 속도 200
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //  숲의 안쪽으로 들어왔네. 지도 상에서는 이 숲을 지나야 한다던데.
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //  슬슬 먹을 것도 떨어져가서 좀 위태로운걸.
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //  (소환술을 진행하고, 여우가 나타난다.)
                showBlueEffect();
                offDialgueBox();
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /*  *쯧* 늑대였으면 더 좋았을텐데.
                 *  너, 가서 고기 좀 사냥해와. 이왕이면 큰 놈으로.
                 */
                onDialgueBox();
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                //  (소환수 '여우'가 화를 낸다.)
                foxMove.playAngryAni();
                showAngryEffect();
                offDialgueBox();
                break;
            case 7:
                Debug.Log(scenarioFlowCount);
                //  어쩔 수 없잖아. 내가 먹고 살아야지 너희도 나올 수 있다고.
                onDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                // (소환수 '여우'가 오른쪽으로 걸어가 사라진다.)
                offDialgueBox();
                endAngryEffect();
                foxMove.CharacterMove(1200f, 400f);
                break;          
        }
    }

    // 대사 ID를 비교하는 메소드
    private bool checkCSVDialogueID()
    {
        // InteractionController에서 현재 대사 CSV ID 가져오기
        int currentDialogueIndex = interactionController.getCurrentDialogueIndex();

        // 현재 대사의 ID가 이전 대사의 ID와 같으면 대사만 진행하고 종료
        if (currentDialogueIndex == isSameDialgueIndex)
        {
            //interactionController.ShowNextLine();
            return true;
        }
        // 대사의 ID가 변경된 경우만 이동 처리
        isSameDialgueIndex = currentDialogueIndex; // 이전 대사 ID 업데이트
        // 스위치문 실행 전 scenarioFlow 증가
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
        scenarioFlowCount++; //다음 대사 및 시나리오 진행을 위해 값 올리기
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
    {   //플레이어가 움직이지 않는 상황일때만 클릭 허용
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
