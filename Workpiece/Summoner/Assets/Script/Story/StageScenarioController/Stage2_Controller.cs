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
            case 1: // 28 ~ 32
                Debug.Log(scenarioFlowCount);
                /*
                 *      (옷을 툭툭 털며)
                 *      생각보다 소환수가 약하잖아?
                 */
                showConfuseEffect();
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                /*
                 *      이렇게 힘을 못 추는데 드래곤을 어떻게 잡아, 말도 안 되는 소리를 하고 있어! 
                 */
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                /*
                 *      (바닥에 떨어진 보석을 줍는다.)
                 *      이건 정수 보석?
                 *      몬스터들이 만들어낸 건가?
                 */
                showBangEffect();
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                /*
                 *      흠, 이걸로 마력을 좀 더 강력하게 만들 수 있겠는데. 
                 */      
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /*
                 *      (잠시 빛이 번쩍이고 몸에 스며든다.)
                 *      이 정도면 다음 적을 상대할 수는 있겠어.
                 *      드래곤을 잡을 정도는 안되지만.
                 */
                //  빛이 번쩍이고 몸에 스며드는 이미지 필요
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
