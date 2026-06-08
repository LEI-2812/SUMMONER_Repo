using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage1_Controller : StoryScenarioControllerBase
{
    [Header("표현할 오브젝트들")]
    public GameObject confusebubbleImage; // 끙앓는 이미지
    public GameObject dotbubbleImage; // ... 이미지
    public GameObject dialogueBox;
    public GameObject skipBtn;

    //플레이어 애니메이션
    [SerializeField] private Animator playerAni;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        confusebubbleImage.SetActive(false);
        dotbubbleImage.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        ScenarioFlow();       
    }

    protected override void PlayScenarioStep(int scenarioStep)
    {
        switch (scenarioStep)
        { //시나리오 참고해서 코드 보면 이해하기 쉽습니다.
            case 1:
                Debug.Log(scenarioStep);
                /*
                 * (오른쪽으로 걸어간다.)[1]
                 */
                OffDialgueBox(); //텍스트를 임시로 꺼둔다.
                playerMove.CharacterMove(700f, 400f); // x좌표로 +700 이동, 속도 400 움직이는 동안 다음대사로 못넘어감
                break;
            case 2:
                Debug.Log(scenarioStep);
                /* <<대사출력>>
                *..그래서 일단 걷고 있긴 한데, 어느 쪽으로 가야 하는 거지?
                 */
                OnDialgueBox();
                break;
            case 3:
                Debug.Log(scenarioStep);
                /* <<대사출력>>
                 **어깨를 으쓱하며* 아무 쪽이든 상관 없나.
                 */
                break;
            case 4:
                Debug.Log(scenarioStep);
                /*
                 * (오른쪽으로 몇 발자국 더 나아간다.) [2]
                 */
                OffDialgueBox();
                playerMove.CharacterMove(150f, 400f); // x좌표로 +150 이동, 속도 400
                break;
            case 5:
                Debug.Log(scenarioStep);
                /* <<대사출력>>
                 *    잠깐. 이 방향이 정말 맞아? 아닌 것 같은데.
                 */
                OnDialgueBox();
                playerMove.CharacterMove(-150f, 400f); // x좌표로 -150 이동, 속도 400
                break;
            case 6:
                Debug.Log(scenarioStep);
                /*
                 * (왼쪽으로 발걸음을 돌려 조금 더 걷는다.) [3]
                 * *부스럭 부스럭* 지도는 반대 방향인데, 그럼 아까 갔던 방향이 맞는 건가?
                 * 
                 */               
                playerMove.CharacterMove(150f, 300f); // x좌표로 +150 이동, 속도 300
                break;
            case 7: 
                Debug.Log(scenarioStep);
                /*
                 * (다시 오른쪽으로 돌아 앞으로 걸어간다.) [4]
                 * 
                 */
                OffDialgueBox();
                break;
            case 8:
                Debug.Log(scenarioStep);
                //*한숨* 왜 하필이면 나야.
                OnDialgueBox();
                break;
            case 9:
                Debug.Log(scenarioStep);
                //이마를 짚는다.
                OffDialgueBox();
                ShowConfuseEffect(); //꼬인 이미지 출력
                break;
            case 10:
                Debug.Log(scenarioStep);
                /*
                 * 스승님만 아니었어도 난 조용히 살 수 있는 건데!
                 * 쓸데없이 마력이 존재하는 나같은 인간이 뭐가 된다고 드래곤을 잡는다는 거야.
                 * 마나 보석 없이는 작은 마법 하나도 못 쓰는데
                 */
                OnDialgueBox();
                break;
            case 11:
                Debug.Log(scenarioStep);
                /*
                 * (잠시 시간이 흐르고, 천천히 일어선다.)
                 * ...
                 */
                OffDialgueBox();
                ShowDotbubbleEffect(); //... 이미지 출력
                break;
            case 12:
                Debug.Log(scenarioStep);
                /*
                 * 보상은 준다니 가야지, 어쩌겠어.
                 * 마침 쪼들리던 참이니 목숨 값 한 번 두둑이 받아보지, 뭐.
                 */
                OnDialgueBox();
                break;
        }
    }


    public void ShowConfuseEffect() //Confuse효과 시작
    {
        Invoke("onConfuseImage", 0.4f); // 0.4초 후 혼란 이미지 활성화
        playerMove.PlayConfuseAni(); //애니메이션은 바로 실행
    }
    private void OnConfuseImage() //0.4초후에 이미지 활성화시키고
    {
        confusebubbleImage.SetActive(true);
        //1초위 이미지가 꺼지게
        Invoke("offConfuseImage", 1f);
    }
    private void OffConfuseImage() //1.4초때 이미지는 비활성화 후
    {
        confusebubbleImage.SetActive(false);
        Invoke("endConfuseEffect", 0.4f);
    }
    private void EndConfuseEffect() //1.8초 뒤에는 끝내게
    {
        playerMove.StopConfuseAni(); //Idle로 돌아오고 다음 대사를 이어갈 수 있게 설정
    }
   

    public void ShowDotbubbleEffect()
    {
        dotbubbleImage.SetActive(true);
        interactionController.StopNextDialogue();

        Invoke("endDotbubbleEffect", 2f);
    }
    private void EndDotbubbleEffect()
    {
        dotbubbleImage.SetActive(false);
        interactionController.StartNextDialogue();
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
