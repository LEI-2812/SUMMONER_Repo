using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Vector3 targetPosition; // �̵��� ��ǥ ��ġ
    private bool isMoving; // �̵� ���� Ȯ��
    private float moveSpeed; // �̵� �ӵ�

    [Header("�ó������� �÷��̾�")]
    [SerializeField] private GameObject player;

    [Header("�ִϸ�����")]
    [SerializeField] private Animator playerAni; // �÷��̾� �ִϸ�����

    [Header("interRaction ��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController; // InteractionEvent ����

    [Header("�÷��̾� �Ҹ�")]
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource effectSound;


    void Update()
    {
        // ĳ���Ͱ� �����̰� ���� ���� �̵� ó��
        if (isMoving)
        {
            MoveToTarget();
        }
    }


    // ��ǥ ��ġ�� �̵� �ӵ��� �����ϴ� �޼���
    public void CharacterMove(float distance, float speed)
    {       
        targetPosition = player.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;

        // �̵� ���⿡ ���� ĳ������ ���� ��ȯ
        if (distance < 0)
        {
            // ���� �̵� �� ĳ���͸� ������
            player.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (distance > 0)
        {
            // ������ �̵� �� ĳ���͸� ���� �������� ����
            player.transform.localScale = new Vector3(1, 1, 1);
        }

        // �̵� �����ϸ鼭 �ִϸ��̼� ���
        isMoving = true;
        interactionController.stopNextDialogue();
        playerAni.Play("PlayerWalk");
       
        walkSound.loop = true;
        walkSound.Play();        
    }

    // ĳ���͸� ��ǥ ��ġ�� �̵���Ű�� �޼���
    public void MoveToTarget()
    {
        if (!isMoving) return;

        // ���� ��ġ���� ��ǥ ��ġ���� ������ �ӵ��� �̵�
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ��ǥ ��ġ�� �����ϸ� �̵� ����
        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            playerAni.Play("Idle");
            interactionController.startNextDialogue();

            walkSound.loop = false;
            walkSound.Stop();
        }              
    }

    public void playConfuseAni()
    {
        playerAni.Play("Confuse");
        interactionController.stopNextDialogue();
    }

    public void stopConfuseAni()
    {
        playerAni.Play("Idle");
        interactionController.startNextDialogue();
    }

    public void playBlueAni()
    {
        playerAni.Play("PlayerBlue");
        effectSound.Play();
        interactionController.stopNextDialogue();
        Invoke("stopBlueAni", 1.2f);
    }
    
    public void stopBlueAni()
    {
        playerAni.Play("Idle");
        interactionController.startNextDialogue();
    }

    public void playYellowAni()
    {
        playerAni.Play("PlayerYellow");
        effectSound.Play();
        interactionController.stopNextDialogue();
        Invoke("stopYellowAni", 1.2f);
    }
    public void stopYellowAni()
    {
        playerAni.Play("Idle");
        interactionController.startNextDialogue();
    }

    //getter setter
    public bool getIsMoving()
    {
        return isMoving;
    }
}
