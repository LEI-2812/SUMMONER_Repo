using UnityEngine;

// 역할: PlayerMove의 책임을 정의한다.
public class PlayerMove : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving;
    private float moveSpeed;
    [SerializeField] private StoryScenarioBase storyController;

    [Header("참조")]
    [SerializeField] private GameObject player;
    [SerializeField] private RectTransform rectTr;

    [Header("참조")]
    [SerializeField] private Animator playerAni;

    [Header("참조")]
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource effectSound;

    private void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void CharacterMove(float distance, float speed)
    {
        float targetX = rectTr.anchoredPosition.x + distance;
        targetPosition = new Vector3(targetX, rectTr.anchoredPosition.y, 0f);
        moveSpeed = speed;

        if (distance < 0)
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (distance > 0)
        {
            player.transform.localScale = new Vector3(1, 1, 1);
        }

        isMoving = true;
        PauseDialogue();
        playerAni.Play("PlayerWalk");

        walkSound.loop = true;
        walkSound.Play();
    }

    public void MoveToTarget()
    {
        if (!isMoving)
        {
            return;
        }

        rectTr.anchoredPosition = Vector3.MoveTowards(rectTr.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(rectTr.anchoredPosition, targetPosition) < 0.01f)
        {
            isMoving = false;
            playerAni.Play("Idle");
            ResumeDialogue();

            walkSound.loop = false;
            walkSound.Stop();
        }
    }

    public void PlayConfuseAni()
    {
        playerAni.Play("Confuse");
        PauseDialogue();
    }

    public void StopConfuseAni()
    {
        playerAni.Play("Idle");
        ResumeDialogue();
    }

    public void PlayBlueAni()
    {
        playerAni.Play("PlayerBlue");
        effectSound.Play();
        PauseDialogue();
        Invoke(nameof(StopBlueAni), 1.2f);
    }

    public void StopBlueAni()
    {
        playerAni.Play("Idle");
        ResumeDialogue();
    }

    public void PlayYellowAni()
    {
        playerAni.Play("PlayerYellow");
        effectSound.Play();
        PauseDialogue();
        Invoke(nameof(StopYellowAni), 1.2f);
    }

    public void StopYellowAni()
    {
        playerAni.Play("Idle");
        ResumeDialogue();
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
