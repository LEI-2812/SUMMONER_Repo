using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxMove : MonoBehaviour
{
    private Vector3 targetPosition; // �̵��� ��ǥ ��ġ
    private bool isMoving; // �̵� ���� Ȯ��
    private float moveSpeed; // �̵� �ӵ�

    [Header("�ó������� �÷��̾�")]
    [SerializeField] private GameObject fox;

    [Header("�ִϸ�����")]
    [SerializeField] private Animator foxAni; // �÷��̾� �ִϸ�����

    [Header("interRaction ��Ʈ�ѷ�")]
    [SerializeField] private InteractionController interactionController; // InteractionEvent ����


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
        interactionController.stopNextDialogue();
        targetPosition = fox.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;

        // �̵� ���⿡ ���� ĳ������ ���� ��ȯ
        if (distance < 0)
        {
            // ���� �̵� �� ĳ���͸� ������
            fox.transform.localScale = new Vector3(1, 1, 1);
            //fox.transform.localScale = new Vector3(-1, 1, 1); ���� �̻��¿��µ� ���̷���
        }
        else if (distance > 0)
        {
            // ������ �̵� �� ĳ���͸� ���� �������� ����
            fox.transform.localScale = new Vector3(-1, 1, 1);
            //fox.transform.localScale = new Vector3(1, 1, 1); ���� �̰ſ��µ� �ְ� �ݴ�� ���ư��� ����?
        }

        // �̵� �����ϸ鼭 �ִϸ��̼� ���
        isMoving = true;
        interactionController.stopNextDialogue();
        foxAni.Play("Stage3_Fox");
    }

    // ĳ���͸� ��ǥ ��ġ�� �̵���Ű�� �޼���
    public void MoveToTarget()
    {
        if (!isMoving) return;

        // ���� ��ġ���� ��ǥ ��ġ���� ������ �ӵ��� �̵�
        fox.transform.position = Vector3.MoveTowards(fox.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ��ǥ ��ġ�� �����ϸ� �̵� ����
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
