using UnityEngine;
using UnityEngine.EventSystems;

public class Stage2_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("표현할 오브젝트들")]
    public GameObject bangImage;
    public GameObject confusebubbleImage;

    private int scenarioFlowCount = 0; //대사 카운트

    [Header("컨트롤러")]
    [SerializeField] private InteractionController interactionController;

    private int isSameDialgueIndex = -1;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        // 느낌표 들어갈 곳, 미리 비활성화
        bangImage.SetActive(false);
        confusebubbleImage.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        scenarioFlow();
    }

    void Update()
    {
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
            case 1:
                Debug.Log(scenarioFlowCount);
                /*
                 * 
                 *  
                 *  
                 * 
                 */
                showConfuseEffect();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                /*
                 *    (오른쪽으로 몇 발자국 더 나아간다.) [2]
                 *    잠깐. 이 방향이 정말 맞아? 아닌 것 같은데.
                 */
                showBangEffect();
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                /*
                 * (왼쪽으로 발걸음을 돌려 조금 더 걷는다.) [3]
                 * *부스럭 부스럭* 지도는 반대 방향인데, 그럼 아까 갔던 방향이 맞는 건가?
                 * 
                 */      
                // 여기는 빛 번짐 효과가 나와야 해요!
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

    public void showConfuseEffect() //Confuse효과
    {
        // 혼란 이미지 활성화 및 애니메이션 실행
        confusebubbleImage.SetActive(true);
        interactionController.stopNextDialogue(); //여기서 알아서 대사를 멈추게함

        // 2초 후에 `endConfuseEffect` 메서드 호출
        Invoke("endConfuseEffect", 2f);
    }
    private void endConfuseEffect()
    {
        // 혼란 이미지 비활성화 및 캐릭터 상태 초기화
        confusebubbleImage.SetActive(false);
        interactionController.startNextDialogue(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
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
            scenarioFlow();
    }
}
