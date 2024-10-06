using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private Vector3 targetPosition; // �̵��� ��ǥ ��ġ
    private bool isMoving; // �̵� ���� Ȯ��
    private float moveSpeed; // �̵� �ӵ�

    [Header("�ó������� �÷��̾�")]
    [SerializeField] private GameObject enemy;

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
        targetPosition = enemy.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;

        // �̵� �����ϸ鼭 �ִϸ��̼� ���
        isMoving = true;
        interactionController.stopNextDialogue();
    }

    // ĳ���͸� ��ǥ ��ġ�� �̵���Ű�� �޼���
    public void MoveToTarget()
    {
        if (!isMoving) return;

        // ���� ��ġ���� ��ǥ ��ġ���� ������ �ӵ��� �̵�
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ��ǥ ��ġ�� �����ϸ� �̵� ����
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
