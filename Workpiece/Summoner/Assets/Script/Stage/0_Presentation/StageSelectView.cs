using UnityEngine;
using UnityEngine.UI;

// 역할: StageSelectView의 책임을 정의한다.
public class StageSelectView : MonoBehaviour
{
    [Header("참조")]
    public Button[] buttons;

    [Header("참조")]
    [SerializeField] private AudioSource audioSource;

    private readonly StageSelectUseCase stageSelectUseCase = new StageSelectUseCase();

    private void Start()
    {
        int stageNum = stageSelectUseCase.GetUnlockedStage();
        ButtonInteractivity(stageNum);
    }

    public void StageLoader(int stage)
    {
        Debug.Log("버튼 클릭");
        if (audioSource != null)
        {
            audioSource.Play();
        }

        stageSelectUseCase.SelectStage(stage);
    }

    public void stageLoader(int stage)
    {
        StageLoader(stage);
    }

    void ButtonInteractivity(int stageNumber)
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        for (int i = 0; i < stageNumber; i++)
        {
            if (i < buttons.Length)
            {
                buttons[i].interactable = true;
            }
        }
    }
}
