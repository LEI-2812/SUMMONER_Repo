using UnityEngine;

// 역할: EnemyMove의 책임을 정의한다.
public class EnemyMove : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving;
    private float moveSpeed;
    [SerializeField] private StoryScenarioBase storyController;

    [Header("참조")]
    [SerializeField] private GameObject enemy;

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void CharacterMove(float distance, float speed)
    {
        PauseDialogue();
        targetPosition = enemy.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;
        isMoving = true;
    }

    public void MoveToTarget()
    {
        if (!isMoving)
        {
            return;
        }

        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(enemy.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            ResumeDialogue();
        }
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

    private void PauseDialogue()
    {
        storyController?.PauseDialogue();
    }

    private void ResumeDialogue()
    {
        storyController?.ResumeDialogue();
    }
}
