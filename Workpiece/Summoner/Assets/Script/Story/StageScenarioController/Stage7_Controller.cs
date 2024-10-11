using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage7_Controller : MonoBehaviour, ScenarioBase, IPointerClickHandler
{
    [Header("표현할 오브젝트들")]
    public GameObject dialogueBox;

    private int scenarioFlowCount = 0; //대사 카운트

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;

    [Header("컨트롤러")]
    [SerializeField] private InteractionController interactionController;
    [SerializeField] private PlayerMove playerMove;

    private int isSameDialgueIndex = -1;

    void Start()
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
            case 1: //  50 ~ 58
                Debug.Log(scenarioFlowCount);
                //  (오른쪽으로 조심스럽게 이동한다.)
                offDialgueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioFlowCount);
                //  *소근소근* 드래곤이 이 자식인가본데.
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioFlowCount);
                //  누군가, 내 잠을 깨운 자는.
                break;
            case 4:
                Debug.Log(scenarioFlowCount);
                /*  어, 안녕.
                 *  난 그냥 너랑 얘기하러 온 건데.
                 */
                break;
            case 5:
                Debug.Log(scenarioFlowCount);
                /*  난 인간 따위랑 얘기하지 않는다.
                 *  넌 보나마나 나와 싸우기 위해 여기까지 온 것이겠지.
                 */
                break;
            case 6:
                Debug.Log(scenarioFlowCount);
                /*  맞긴 한데, 난 좋게좋게 해결하고 싶거든. 나도 여기서 죽고 싶은 생각은 더더욱 없고.
                 *  널 죽일 생각은 없어.
                 *  그냥 심장의 보석 하나면 돼. 어차피 넌 그거 하나 없다고 죽지 않잖아.
                 */ 
                break;
            case 7:
                Debug.Log(scenarioFlowCount);
                /*  하, 내 심장을 달라고?
                 *  그럴 순 없지.
                 *  없어도 죽진 않지만, 그렇다고 너를 이렇게 순순히 보내줄 수는 없다.
                 *  자, 어서 덤벼라!
                 */
                break;
            case 8:
                Debug.Log(scenarioFlowCount);
                /*  (이거 야단났네.)
                 *  쉽게 해결할 수 있었는데, 네가 자초한 거야. 난 모른다?
                 */
                break;
            case 9:
                Debug.Log(scenarioFlowCount);
                /*  어디서 하찮은 미물 따위가 잘난 듯이 지껄이지?
                 *  너야말로 오늘 여기서 죽을 준비해라!
                 */
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
    public void OnPointerClick(PointerEventData eventData)
    {
        //플레이어가 움직이지 않는 상황일때만 클릭 허용
        if (!playerMove.getIsMoving())
            scenarioFlow();
    }
}
