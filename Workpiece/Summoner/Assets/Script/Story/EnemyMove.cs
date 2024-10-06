using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private Vector3 targetPosition; // 이동할 목표 위치
    private bool isMoving; // 이동 여부 확인
    private float moveSpeed; // 이동 속도

    [Header("시나리오씬 플레이어")]
    [SerializeField] private GameObject enemy;

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
        targetPosition = enemy.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;

        // 이동 시작하면서 애니메이션 재생
        isMoving = true;
        interactionController.stopNextDialogue();
    }

    // 캐릭터를 목표 위치로 이동시키는 메서드
    public void MoveToTarget()
    {
        if (!isMoving) return;

        // 현재 위치에서 목표 위치까지 일정한 속도로 이동
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 목표 위치에 도달하면 이동 중지
        if (Vector3.Distance(enemy.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            interactionController.startNextDialogue();
        }
    }

    //getter setter
    public bool getIsMoving()
    {
        return isMoving;
    }
}
