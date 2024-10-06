using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour, IPointerClickHandler
{
    [Header("캐릭터 이름 텍스트")]
    public Text nameText; //대사 출력 캐릭터이름 텍스트

    [Header("캐릭터 대사 텍스트")]
    public Text dialogueText; //대사 텍스트

    [Header("표현할 오브젝트들")]
    public GameObject characterPlayer; //주인공 캐릭터 오브젝트
    public GameObject confusebubbleImage;

    private int scenarioFlow = 0; //대사 카운트

    //플레이어 애니메이션
    [SerializeField]private Animator playerAni;

    [SerializeField]private InteractionController interactionController;

    private Vector3 targetPosition; // 이동할 목표 위치
    private bool isMoving; // 이동 여부 확인
    private int isSameDialgueIndex = -1;

    void Awake() //여기에서 오브젝트들의 초기 설정을 해준다.
    {
        confusebubbleImage.SetActive(false);
    }

    void Start() //Start에서 처음 실행할 메소드나 오브젝트를 지정해주도록 한다.
    {
        stage_1_Flow();
    }

    void Update()
    {
        // 유저의 입력으로 대사 넘기기 (예: 스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stage_1_Flow();
        }

        if (isMoving)
        {
            MoveToTarget(200f); // 이동 속도를 인자로 전달
        }
    }

    public void stage_1_Flow()
    {

        if (checkCSVDialogueID()) //다음 대사의 ID가 이전 ID와 같으면 그냥 대사만 출력시킴.
        {
            return;
        }

        switch (scenarioFlow)
        {
            case 1:
                Debug.Log(scenarioFlow);
                /*
                 * (오른쪽으로 걸어간다.)[1]
                 *  ..그래서 일단 걷고 있긴 한데, 어느 쪽으로 가야 하는 거지?
                 *  *어깨를 으쓱하며* 아무 쪽이든 상관 없나.
                 * 
                 */
                CharacterMove(700f); // x좌표로 +700(오른쪽)으로 이동
                break;
            case 2:
                Debug.Log(scenarioFlow);
                /*
                 *    (오른쪽으로 몇 발자국 더 나아간다.) [2]
                 *    잠깐. 이 방향이 정말 맞아? 아닌 것 같은데.
                 */
                CharacterMove(50f); // x좌표로 +50(오른쪽)으로 이동
                break;
            case 3:
                Debug.Log(scenarioFlow);
                /*
                 * (왼쪽으로 발걸음을 돌려 조금 더 걷는다.) [3]
                 * *부스럭 부스럭* 지도는 반대 방향인데, 그럼 아까 갔던 방향이 맞는 건가?
                 * 
                 */
                CharacterMove(-50f); // x좌표로 +50(오른쪽)으로 이동
                break;
            case 4: 
                Debug.Log(scenarioFlow);
                /*
                 * (다시 오른쪽으로 돌아 앞으로 걸어간다.) [4]
                 * *한숨* 왜 하필이면 나야.
                 * 
                 */
                CharacterMove(50f); // x좌표로 +50(오른쪽)으로 이동
                break;
            case 5:
                Debug.Log(scenarioFlow);
                /*
                 * (이마를 짚는다.) [5]
                 *  스승님만 아니었어도 난 조용히 살 수 있는 건데!
                 *  쓸데없이 마력이 존재하는 나같은 인간이 뭐가 된다고 드래곤을 잡는다는 거야.
                 *  마나 보석 없이는 작은 마법 하나도 못 쓰는데.
                 */
                ShowConfuseEffect();

                break;
            case 6:
                Debug.Log(scenarioFlow);
                /*
                 * (잠시 시간이 흐르고, 천천히 일어선다.) [6]
                 *  …
                 *  보상은 준다니 가야지, 어쩌겠어.
                 *  마침 쪼들리던 참이니 목숨 값 한 번 두둑이 받아보지, 뭐.
                 * 
                 * 
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





    // 목표 위치를 설정하는 메소드
    private void CharacterMove(float distance)
    {
        targetPosition = characterPlayer.transform.position + new Vector3(distance, 0f, 0f);

        // 이동 방향에 따라 캐릭터의 방향 전환
        if (distance < 0)
        {
            // 왼쪽이동시 캐릭터를 뒤집음
            characterPlayer.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (distance > 0)
        {
            // 오른쪽이동시 캐릭터를 원래 방향으로 돌림
            characterPlayer.transform.localScale = new Vector3(1, 1, 1);
        }

        //이동 시작하면서 다음대사를 못하도록 막고 애니메이션 재생시작
        isMoving = true;
        interactionController.setIsStory(isMoving);
        playerAni.Play("PlayerWalk");
    }


    // 캐릭터를 목표 위치로 이동시키는 메소드
    private void MoveToTarget(float speed)
    {
        // 현재 위치에서 목표 위치까지 일정한 속도로 이동
        characterPlayer.transform.position = Vector3.MoveTowards(characterPlayer.transform.position, targetPosition, speed * Time.deltaTime);

        // 목표 위치에 도달하면 이동 중지
        if (Vector3.Distance(characterPlayer.transform.position, targetPosition) < 0.01f)
        {
            StopMoving();
        }
    }

    private void StopMoving()
    {
        playerAni.Play("Idle");
        isMoving = false; // 이동 중지
        interactionController.setIsStory(isMoving); //다음 대사로 넘길 수 있게 설정
    }

    private bool isConfuseEffectActive = false;
    public void ShowConfuseEffect() //Confuse효과
    {
        //Debug.Log("Showconfuse발동");
        // 다음 대사를 못하도록 막기
        isMoving = true;
        interactionController.setIsStory(true);
        isConfuseEffectActive = true;

        // 혼란 이미지 활성화 및 애니메이션 실행
        confusebubbleImage.SetActive(true);
        playerAni.Play("Confuse");

        // 2초 후에 `EndConfuseEffect` 메서드 호출
        Invoke("EndConfuseEffect", 2f);
    }
    private void EndConfuseEffect()
    {
        if (!isConfuseEffectActive)
        {
            // 혼란 이미지 비활성화 및 캐릭터 상태 초기화
            confusebubbleImage.SetActive(false);
            StopMoving(); // 이곳에서 isStory가 false로 바뀜
        }
    }

    private void nextScenarioFlow()
    {
        scenarioFlow++; //다음 대사 및 시나리오 진행을 위해 값 올리기
    }

    public bool getIsMoving()
    {
        return isMoving;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if( !isMoving )
            stage_1_Flow();
    }
}
