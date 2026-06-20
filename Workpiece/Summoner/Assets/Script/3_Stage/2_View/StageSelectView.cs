using UnityEngine;
using UnityEngine.UI;

// 역할: 스테이지 선택 화면의 UI 표시와 잠금 상태를 관리한다.
public class StageSelectView : MonoBehaviour
{
    [Header("스테이지 버튼들(순서대로)")]
    public Button[] buttons;    // 활성화/비활성화할 버튼들

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private StageController stageController;


    private void Start()
    {
        stageController = FindObjectOfType<StageController>();
        int stageNum = stageController.GetStageNum();
        ButtonInteractivity(stageNum);
        //ButtonInteractivity(7);
    }

    public void StageLoader(int stage)
    {
        Debug.Log("버튼 클릭");
        if (audioSource != null)
        {
            audioSource.Play();
        }

        stageController.SelectStage(stage);
    }

    // 기존 Stage Select Scene 버튼 이벤트가 호출하는 이름을 유지한다.
    public void stageLoader(int stage)
    {
        StageLoader(stage);
    }

    public void SendStage(int stage)    // 스테이지 선택 화면에서 보낼 씬(스토리 + 전투)
    {
        stageController.SelectStage(stage);
    }

    void ButtonInteractivity(int stageNumber)
    {
        // 모든 버튼 비활성화
        foreach (Button button in buttons)
        {
            button.interactable = false; // 기본적으로 비활성화
        }

        for (int i = 0; i < stageNumber; i++)
        {
            if (i < buttons.Length) // 배열의 범위를 벗어나지 않도록 체크
            {
                buttons[i].interactable = true; // 해당 단계의 버튼 활성화
            }
        }
    }
}
