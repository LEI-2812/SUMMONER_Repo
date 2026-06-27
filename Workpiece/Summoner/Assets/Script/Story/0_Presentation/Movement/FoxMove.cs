using UnityEngine;

// 역할: 스토리 씬의 여우 이동과 이동 중 대사 일시정지를 처리한다.
public class FoxMove : MonoBehaviour
{
    private const string WalkStateName = "Stage3_Fox";
    private const string IdleStateName = "Fox_Idle";
    private const string AngryStateName = "AngryFox";

    private Vector3 targetPosition;
    private bool isMoving;
    private float moveSpeed;
    [SerializeField] private StoryScenarioBase storyController;

    [Header("참조")]
    [SerializeField] private GameObject fox;

    [Header("참조")]
    [SerializeField] private Animator foxAni;

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
        targetPosition = fox.transform.position + new Vector3(distance, 0f, 0f);
        moveSpeed = speed;

        if (distance < 0)
        {
            fox.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (distance > 0)
        {
            fox.transform.localScale = new Vector3(-1, 1, 1);
        }

        isMoving = true;
        PlayAnimationState(WalkStateName);
    }

    public void MoveToTarget()
    {
        if (!isMoving)
        {
            return;
        }

        fox.transform.position = Vector3.MoveTowards(fox.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(fox.transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            PlayAnimationState(IdleStateName);
            ResumeDialogue();
        }
    }

    public void PlayAngryAni()
    {
        PlayAnimationState(AngryStateName);
    }

    public void StopAngryAni()
    {
        PlayAnimationState(IdleStateName);
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

    private void PlayAnimationState(string stateName)
    {
        if (foxAni == null)
        {
            Debug.LogWarning("FoxMove에 Animator가 필요합니다.");
            return;
        }

        if (!foxAni.isActiveAndEnabled || !foxAni.gameObject.activeInHierarchy)
        {
            return;
        }

        if (!foxAni.HasState(0, Animator.StringToHash(stateName)))
        {
            Debug.LogWarning("Fox Animator state를 찾을 수 없습니다: " + stateName);
            return;
        }

        foxAni.Play(stateName, 0);
    }
}
