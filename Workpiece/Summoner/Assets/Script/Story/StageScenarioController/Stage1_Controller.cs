using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("표현할 오브젝트들")]
    public GameObject confusebubbleImage; // 끙앓는 이미지
    public GameObject dotbubbleImage; // ... 이미지
    public GameObject dialogueBox;
    public GameObject skipBtn;

    private int scenarioFlowCount = 0; //대사 카운트

    //플레이어 애니메이션
    [SerializeField]private Animator playerAni;

    [Header("컨트롤러")]
    [SerializeField]private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;

    private int isSameDialgueIndex = -1;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        confusebubbleImage.SetActive(false);
        dotbubbleImage.SetActive(false);
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
        { //시나리오 참고해서 코드 보면 이해하기 쉽습니다.
            case 1:
                Debug.Log(scenarioFlowCount);
                /*
                 * (오른쪽으로 걸어간다.)[1]
                 */
                offDialgueBox(); //텍스트를 임시로 꺼둔다.
                playerMove.CharacterMove(700f, 400f); // x좌표로 +700 이동, 속도 400 움직이는 동안 다음대사로 못넘어감
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                /* <<대사출력>>
                *..그래서 일단 걷고 있긴 한데, 어느 쪽으로 가야 하는 거지?
                 */
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                /* <<대사출력>>
                 **어깨를 으쓱하며* 아무 쪽이든 상관 없나.
                 */
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                /*
                 * (오른쪽으로 몇 발자국 더 나아간다.) [2]
                 */
                offDialgueBox();
                playerMove.CharacterMove(150f, 400f); // x좌표로 +150 이동, 속도 400
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /* <<대사출력>>
                 *    잠깐. 이 방향이 정말 맞아? 아닌 것 같은데.
                 */
                onDialgueBox();
                playerMove.CharacterMove(-150f, 400f); // x좌표로 -150 이동, 속도 400
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                /*
                 * (왼쪽으로 발걸음을 돌려 조금 더 걷는다.) [3]
                 * *부스럭 부스럭* 지도는 반대 방향인데, 그럼 아까 갔던 방향이 맞는 건가?
                 * 
                 */               
                playerMove.CharacterMove(150f, 300f); // x좌표로 +150 이동, 속도 300
                break;
            case 7: 
                Debug.Log(scenarioFlowCount);
                /*
                 * (다시 오른쪽으로 돌아 앞으로 걸어간다.) [4]
                 * 
                 */
                offDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                //*한숨* 왜 하필이면 나야.
                onDialgueBox();
                break;
            case 9:
                Debug.Log(scenarioFlowCount);
                //이마를 짚는다.
                offDialgueBox();
                showConfuseEffect(); //꼬인 이미지 출력
                break;
            case 10:
                Debug.Log(scenarioFlowCount);
                /*
                 * 스승님만 아니었어도 난 조용히 살 수 있는 건데!
                 * 쓸데없이 마력이 존재하는 나같은 인간이 뭐가 된다고 드래곤을 잡는다는 거야.
                 * 마나 보석 없이는 작은 마법 하나도 못 쓰는데
                 */
                onDialgueBox();
                break;
            case 11:
                Debug.Log(scenarioFlowCount);
                /*
                 * (잠시 시간이 흐르고, 천천히 일어선다.)
                 * ...
                 */
                offDialgueBox();
                showDotbubbleEffect(); //... 이미지 출력
                break;
            case 12:
                Debug.Log(scenarioFlowCount);
                /*
                 * 보상은 준다니 가야지, 어쩌겠어.
                 * 마침 쪼들리던 참이니 목숨 값 한 번 두둑이 받아보지, 뭐.
                 */
                onDialgueBox();
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

    public void showConfuseEffect() //Confuse효과 시작
    {
        Invoke("onConfuseImage", 0.4f); // 0.4초 후 혼란 이미지 활성화
        playerMove.playConfuseAni(); //애니메이션은 바로 실행
    }
    private void onConfuseImage() //0.4초후에 이미지 활성화시키고
    {
        confusebubbleImage.SetActive(true);
        //1초위 이미지가 꺼지게
        Invoke("offConfuseImage", 1f);
    }
    private void offConfuseImage() //1.4초때 이미지는 비활성화 후
    {
        confusebubbleImage.SetActive(false);
        Invoke("endConfuseEffect", 0.4f);
    }
    private void endConfuseEffect() //1.8초 뒤에는 끝내게
    {
        playerMove.stopConfuseAni(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
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
        scenarioFlowCount++; //다음 대사 및 시나리오 진행을 위해 값 올리기
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