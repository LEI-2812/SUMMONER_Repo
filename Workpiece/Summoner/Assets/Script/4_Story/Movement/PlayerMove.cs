using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 역할: 스토리 장면에서 플레이어 이동을 담당한다.
public class PlayerMove : MonoBehaviour
{
    private Vector3 targetPosition; // 이동할 목표 위치
    private bool isMoving; // 이동 여부 확인
    private float moveSpeed; // 이동 속도

    [Header("시나리오씬 플레이어")]
    [SerializeField] private GameObject player;
    [SerializeField] private RectTransform rectTr;

    [Header("애니메이터")]
    [SerializeField] private Animator playerAni; // 플레이어 애니메이터

    [Header("interRaction 컨트롤러")]
    [SerializeField] private InteractionController interactionController; // InteractionEvent 연결

    [Header("플레이어 소리")]
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource effectSound;


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
        // UI의 실질적 위치를 나타내는 transform은 RectTransform이다!
        float targetX = rectTr.anchoredPosition.x + distance;
        targetPosition = new Vector3(targetX, rectTr.anchoredPosition.y, 0f);
        moveSpeed = speed;

        // 이동 방향에 따라 캐릭터의 방향 전환
        if (distance < 0)
        {
            // 왼쪽 이동 시 캐릭터를 뒤집음
            player.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (distance > 0)
        {
            // 오른쪽 이동 시 캐릭터를 원래 방향으로 돌림
            player.transform.localScale = new Vector3(1, 1, 1);
        }

        // 이동 시작하면서 애니메이션 재생
        isMoving = true;
        interactionController.StopNextDialogue();
        playerAni.Play("PlayerWalk");
       
        walkSound.loop = true;
        walkSound.Play();        
    }

    // 캐릭터를 목표 위치로 이동시키는 메서드
    public void MoveToTarget()
    {
        if (!isMoving) return;

        // 현재 위치에서 목표 위치까지 일정한 속도로 이동
        rectTr.anchoredPosition = Vector3.MoveTowards(rectTr.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);

        // 목표 위치에 도달하면 이동 중지
        if (Vector3.Distance(rectTr.anchoredPosition, targetPosition) < 0.01f)
        {
            isMoving = false;
            playerAni.Play("Idle");
            interactionController.StartNextDialogue();

            walkSound.loop = false;
            walkSound.Stop();
        }              
    }

    public void PlayConfuseAni()
    {
        playerAni.Play("Confuse");
        interactionController.StopNextDialogue();
    }

    public void StopConfuseAni()
    {
        playerAni.Play("Idle");
        interactionController.StartNextDialogue();
    }

    public void PlayBlueAni()
    {
        playerAni.Play("PlayerBlue");
        effectSound.Play();
        interactionController.StopNextDialogue();
        Invoke(nameof(StopBlueAni), 1.2f);
    }
    
    public void StopBlueAni()
    {
        playerAni.Play("Idle");
        interactionController.StartNextDialogue();
    }

    public void PlayYellowAni()
    {
        playerAni.Play("PlayerYellow");
        effectSound.Play();
        interactionController.StopNextDialogue();
        Invoke(nameof(StopYellowAni), 1.2f);
    }
    public void StopYellowAni()
    {
        playerAni.Play("Idle");
        interactionController.StartNextDialogue();
    }

    //getter setter
    public bool GetIsMoving()
    {
        return isMoving;
    }
}
