using UnityEngine;

public class Stage2_Controller : StoryScenarioControllerBase
{
    [Header("표현할 오브젝트들")]
    public GameObject bangImage;
    public GameObject confusebubbleImage;
    public GameObject dialogueBox;

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;
    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        // 느낌표 들어갈 곳, 미리 비활성화
        bangImage.SetActive(false);
        confusebubbleImage.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        ScenarioFlow();
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: // 28 ~ 33
                Debug.Log(scenarioStep);
                /*  (옷을 툭툭 털며)
                 *  생각보다 소환수가 약하잖아?
                 */
                ShowConfuseEffect();
                break;
            case 2:
                Debug.Log(scenarioStep);
                //  이렇게 힘을 못 추는데 드래곤을 어떻게 잡아, 말도 안 되는 소리를 하고 있어!                
                break;
            case 3:
                Debug.Log(scenarioStep);
                /*  (바닥에 떨어진 보석을 줍는다.)
                 *  이건 정수 보석?
                 *  몬스터들이 만들어낸 건가?
                 */
                ShowBangEffect();
                break;
            case 4:
                Debug.Log(scenarioStep);
                //흠, 이걸로 마력을 좀 더 강력하게 만들 수 있겠는데.
                break;
            case 5:
                Debug.Log(scenarioStep);
                //  여기서 PlayerYellow() ani 한번만 실행하고 다시 idle 상태로 복귀
                OffDialgueBox();
                ShowYellowEffect();
                break;
            case 6:
                Debug.Log(scenarioStep);
                /*  이 정도면 다음 적을 상대할 수는 있겠어.
                 *  드래곤을 잡을 정도는 안되지만.
                 */
                OnDialgueBox();
                break;
        }
    }

    public void ShowConfuseEffect() //Confuse효과
    {
        // 혼란 이미지 활성화 및 애니메이션 실행
        confusebubbleImage.SetActive(true);
        interactionController.StopNextDialogue(); //여기서 알아서 대사를 멈추게함

        // 2초 후에 `endConfuseEffect` 메서드 호출
        Invoke("endConfuseEffect", 1.5f);
    }
    private void EndConfuseEffect()
    {
        // 혼란 이미지 비활성화 및 캐릭터 상태 초기화
        confusebubbleImage.SetActive(false);
        interactionController.StartNextDialogue(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
    }

    public void ShowBangEffect() //Confuse효과
    {
        // 혼란 이미지 활성화 및 애니메이션 실행
        bangImage.SetActive(true);
        interactionController.StopNextDialogue(); //여기서 알아서 대사를 멈추게함

        Invoke("endBangEffect", 1f);
    }
    private void EndBangEffect()
    {
        // 혼란 이미지 비활성화 및 캐릭터 상태 초기화
        bangImage.SetActive(false);
        interactionController.StartNextDialogue(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
    }

    public void ShowYellowEffect()
    {
        playerMove.PlayYellowAni();
    }

    private void OnDialgueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void OffDialgueBox()
    {
        Debug.Log("대사창 끄기");
        dialogueBox.SetActive(false);
    }
}
