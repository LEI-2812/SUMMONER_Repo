using UnityEngine;

// 역할: 5스테이지 스토리 시나리오 진행을 담당한다.
public class Stage5_Controller : StoryScenarioControllerBase
{
    [Header("표현할 오브젝트들")]
    public GameObject bangImage;
    public GameObject dialogueBox;

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;

    [Header("컨트롤러")]
    [SerializeField] private EnemyMove enemyMove;

    [Header("사운드")]
    [SerializeField] private AudioClip bangSound;
    [SerializeField] private AudioSource audioSource;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        bangImage.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        ScenarioFlow();
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: // 42 ~ 49
                Debug.Log(scenarioStep);
                //  (오른쪽으로 걸어간다.)
                OffDialgueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioStep);
                //  슬슬 길이 험해지네. 이쯤이 중간계 시작지점이라고 하던데.
                OnDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                //  (무언가 떨어지는 소리가 난다.)
                PlayBangSound();
                ShowBangEffect();
                OffDialgueBox();
                break;
            case 4:
                Debug.Log(scenarioStep);
                //  뭐야, 누구야!
                OnDialgueBox();               
                break;
            case 5:
                Debug.Log(scenarioStep);
                //  ...
                break;
            case 6:
                Debug.Log(scenarioStep);
                //  아무도 없는 건가..?
                break;
            case 7:
                Debug.Log(scenarioStep);
                //  (오른쪽에서 갑자기 적 한 명이 튀어나온다.)
                enemyMove.CharacterMove(-600f, 550f);
                OffDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioStep);
                //  우왓! 하급 악마인가? 만만치 않겠는데..
                OnDialgueBox();
                break;
        }
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
    private void PlayBangSound()
    {
        if (audioSource != null && bangSound != null)
        {
            audioSource.PlayOneShot(bangSound); // 효과음을 한 번 재생
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
