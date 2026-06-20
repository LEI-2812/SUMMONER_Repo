using UnityEngine;

// 역할: 3스테이지 스토리 시나리오 진행을 담당한다.
public class Stage3_Controller : StoryScenarioControllerBase
{
    [Header("표현할 오브젝트들")]
    public GameObject angryImage;
    public GameObject characterFox;
    public GameObject dialogueBox;

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;
    [SerializeField] private Animator foxAni;

    [Header("컨트롤러")]
    [SerializeField] private FoxMove foxMove;

    [Header("효과음")]
    [SerializeField] private AudioSource angrySound;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        angryImage.SetActive(false);
        characterFox.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        ScenarioFlow();
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        {
            case 1: //  34 ~ 40
                Debug.Log(scenarioStep);
                //  (오른쪽으로 걸어 나간다.)
                OffDialgueBox();
                playerMove.CharacterMove(860f, 400f); // x좌표로 +860 이동, 속도 200
                break;
            case 2:
                Debug.Log(scenarioStep);
                //  숲의 안쪽으로 들어왔네. 지도 상에서는 이 숲을 지나야 한다던데.
                OnDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                //  슬슬 먹을 것도 떨어져가서 좀 위태로운걸.
                break;
            case 4:
                Debug.Log(scenarioStep);
                //  (소환술을 진행하고, 여우가 나타난다.)
                ShowBlueEffect();
                OffDialgueBox();
                break;
            case 5:
                Debug.Log(scenarioStep);
                /*  *쯧* 늑대였으면 더 좋았을텐데.
                 *  너, 가서 고기 좀 사냥해와. 이왕이면 큰 놈으로.
                 */
                OnDialgueBox();
                break;
            case 6:
                Debug.Log(scenarioStep);
                //  (소환수 '여우'가 화를 낸다.)
                ShowAngryEffect();
                angrySound.Play();
                OffDialgueBox();
                break;
            case 7:
                Debug.Log(scenarioStep);
                //  어쩔 수 없잖아. 내가 먹고 살아야지 너희도 나올 수 있다고.
                OnDialgueBox();
                break;
        }
    }

    public void ShowAngryEffect()
    {
        angryImage.SetActive(true);
        foxMove.PlayAngryAni();
        //interactionController.StopNextDialogue();
    }
    private void EndAngryEffect()
    {
        angryImage.SetActive(false);
        foxMove.StopAngryAni();
        //interactionController.StartNextDialogue();
    }

    public void ShowBlueEffect()
    {
        playerMove.PlayBlueAni();
        Invoke("showFox", 1.5f);
    }

    private void ShowFox()
    {
        characterFox.SetActive(true);
        foxAni.Play("Fox_Idle");
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
