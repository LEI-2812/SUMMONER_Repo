using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxMove : MonoBehaviour
{
    private Vector3 targetPosition; // 이동할 목표 위치
    private bool isMoving; // 이동 여부 확인
    private float moveSpeed; // 이동 속도

    [Header("시나리오씬 플레이어")]
    [SerializeField] private GameObject fox;

    [Header("애니메이터")]
    [SerializeField] private Animator foxAni; // 플레이어 애니메이터

    [Header("interRaction 컨트롤러")]
    [SerializeField] private InteractionController interactionController; // InteractionEvent 연결


    void Update()
    {
        // 캐릭터가 움직이고 있을 때만 이동 처리
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    // 목표 위치와 이동 속도를 설정하는 메서드
    public void CharacterMove(float distance, float speed)
    {
        interactionController.stopNextDialogue();
        targetPosition = fox.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;

        // 이동 방향에 따라 캐릭터의 방향 전환
        if (distance < 0)
        {
            // 왼쪽 이동 시 캐릭터를 뒤집음
            fox.transform.localScale = new Vector3(1, 1, 1);
            //fox.transform.localScale = new Vector3(-1, 1, 1); 원래 이상태였는데 왜이러지
        }
        else if (distance > 0)
        {
            // 오른쪽 이동 시 캐릭터를 원래 방향으로 돌림
            fox.transform.localScale = new Vector3(-1, 1, 1);
            //fox.transform.localScale = new Vector3(1, 1, 1); 원래 이거였는데 애가 반대로 돌아가요 뭐지?
        }

        // 이동 시작하면서 애니메이션 재생
        isMoving = true;
        interactionController.stopNextDialogue();
        foxAni.Play("Stage3_Fox");
    }

    // 캐릭터를 목표 위치로 이동시키는 메서드
    public void MoveToTarget()
    {
        if (!isMoving) return;

        // 현재 위치에서 목표 위치까지 일정한 속도로 이동
        fox.transform.position = Vector3.MoveTowards(fox.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 목표 위치에 도달하면 이동 중지
        if (Vector3.Distance(fox.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            foxAni.Play("Fox_Idle");
            interactionController.startNextDialogue();
        }
    }

    public void playAngryAni()
    {
        foxAni.Play("AngryFox");
    }

    public void stopAngryAni()
    {
        foxAni.Play("Fox_Idle");
    }

    //getter setter
    public bool getIsMoving()
    {
        return isMoving;
    }
}
