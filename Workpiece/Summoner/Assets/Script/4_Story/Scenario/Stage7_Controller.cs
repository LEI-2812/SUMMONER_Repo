using UnityEngine;

// 역할: 7스테이지 스토리 시나리오 진행을 담당한다.
public class Stage7_Controller : StoryScenarioControllerBase
{
    [Header("표현할 오브젝트들")]
    public GameObject dialogueBox;

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;

    void Start()
    {
        ScenarioFlow();
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: //  50 ~ 58
                Debug.Log(scenarioStep);
                //  (오른쪽으로 조심스럽게 이동한다.)
                OffDialgueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioStep);
                //  *소근소근* 드래곤이 이 자식인가본데.
                OnDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                //  누군가, 내 잠을 깨운 자는.
                break;
            case 4:
                Debug.Log(scenarioStep);
                /*  어, 안녕.
                 *  난 그냥 너랑 얘기하러 온 건데.
                 */
                break;
            case 5:
                Debug.Log(scenarioStep);
                /*  난 인간 따위랑 얘기하지 않는다.
                 *  넌 보나마나 나와 싸우기 위해 여기까지 온 것이겠지.
                 */
                break;
            case 6:
                Debug.Log(scenarioStep);
                /*  맞긴 한데, 난 좋게좋게 해결하고 싶거든. 나도 여기서 죽고 싶은 생각은 더더욱 없고.
                 *  널 죽일 생각은 없어.
                 *  그냥 심장의 보석 하나면 돼. 어차피 넌 그거 하나 없다고 죽지 않잖아.
                */ 
                break;
            case 7:
                Debug.Log(scenarioStep);
                /*  하, 내 심장을 달라고?
                 *  그럴 순 없지.
                 *  없어도 죽진 않지만, 그렇다고 너를 이렇게 순순히 보내줄 수는 없다.
                 *  자, 어서 덤벼라!
                */
                break;
            case 8:
                Debug.Log(scenarioStep);
                /*  (이거 야단났네.)
                 *  쉽게 해결할 수 있었는데, 네가 자초한 거야. 난 모른다?
                 */
                break;
            case 9:
                Debug.Log(scenarioStep);
                /*  어디서 하찮은 미물 따위가 잘난 듯이 지껄이지?
                 *  너야말로 오늘 여기서 죽을 준비해라!
                 */
                break;
        }
    }

    private void OnDialgueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void OffDialgueBox()
    {
        dialogueBox.SetActive(false);
    }
}
