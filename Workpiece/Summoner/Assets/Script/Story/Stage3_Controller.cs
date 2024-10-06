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
            scenarioFlow();
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
            case 1://32 33 34 35
                Debug.Log(scenarioFlowCount);
                playerMove.CharacterMove(860f, 200f); // x좌표로 +860 이동, 속도 200
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //푸른 빛이 반짝 후 여우 소환
                //푸른 빛이 반짝
                characterFox.SetActive(true);
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //여우가 화냄
                showAngryEffect();
                break;
            /*
            case 5: 가 안되넹?
                Debug.Log(scenarioFlowCount);
                //이 대사를 치고 여우가 가야댐
                endAngryEffect();
                foxMove.CharacterMove(1000f, 200f);
                break;  
            */
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
        interactionController.stopNextDialogue();
    }
    private void endAngryEffect()
    {
        angryImage.SetActive(false);
        interactionController.startNextDialogue();
    }

    private void nextScenarioFlow()
    {
        scenarioFlowCount++; //다음 대사 및 시나리오 진행을 위해 값 올리기
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        //플레이어가 움직이지 않는 상황일때만 클릭 허용
        if (!playerMove.getIsMoving())
            scenarioFlow();
        
    }
}
