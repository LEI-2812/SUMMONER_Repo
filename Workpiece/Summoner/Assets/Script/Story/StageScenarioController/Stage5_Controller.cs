using UnityEngine;

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
        scenarioFlow();
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: // 42 ~ 49
                Debug.Log(scenarioStep);
                //  (오른쪽으로 걸어간다.)
                offDialgueBox();
                playerMove.CharacterMove(700f, 400f);
                break;
            case 2:
                Debug.Log(scenarioStep);
                //  슬슬 길이 험해지네. 이쯤이 중간계 시작지점이라고 하던데.
                onDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                //  (무언가 떨어지는 소리가 난다.)
                playBangSound();
                showBangEffect();
                offDialgueBox();
                break;
            case 4:
                Debug.Log(scenarioStep);
                //  뭐야, 누구야!
                onDialgueBox();               
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
                offDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioStep);
                //  우왓! 하급 악마인가? 만만치 않겠는데..
                onDialgueBox();
                break;
        }
    }

    public void showBangEffect() //Confuse효과
    {
        // 혼란 이미지 활성화 및 애니메이션 실행
        bangImage.SetActive(true);
        interactionController.stopNextDialogue(); //여기서 알아서 대사를 멈추게함

        Invoke("endBangEffect", 1f);
    }
    private void endBangEffect()
    {
        // 혼란 이미지 비활성화 및 캐릭터 상태 초기화
        bangImage.SetActive(false);
        interactionController.startNextDialogue(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
    }
    private void playBangSound()
    {
        if (audioSource != null && bangSound != null)
        {
            audioSource.PlayOneShot(bangSound); // 효과음을 한 번 재생
        }
    }

    private void onDialgueBox()
    {
        dialogueBox.SetActive(true);
    }

    private void offDialgueBox()
    {
        dialogueBox.SetActive(false);
    }
}
