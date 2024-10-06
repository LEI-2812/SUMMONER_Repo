using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage5_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("표현할 오브젝트들")]
    public GameObject bangImage;

    private int scenarioFlowCount = 0; //대사 카운트

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;

    [Header("컨트롤러")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private EnemyMove enemyMove;

    private int isSameDialgueIndex = -1;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        bangImage.SetActive(false);
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
            case 1://36~40
                Debug.Log(scenarioFlowCount);
                //오른쪽으로 걸어오는 주인공
                playerMove.CharacterMove(980f, 200f);
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //떨어지는 소리
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                //적이 왼쪽으로 걸어옴
                enemyMove.CharacterMove(-600f, 200f);
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
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
    public void showBangEffect() //Confuse효과
    {
        // 혼란 이미지 활성화 및 애니메이션 실행
        bangImage.SetActive(true);
        interactionController.stopNextDialogue(); //여기서 알아서 대사를 멈추게함

        Invoke("endBangEffect", 2f);
    }
    private void endBangEffect()
    {
        // 혼란 이미지 비활성화 및 캐릭터 상태 초기화
        bangImage.SetActive(false);
        interactionController.startNextDialogue(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
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
