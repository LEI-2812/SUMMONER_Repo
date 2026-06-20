using UnityEngine;

// 역할: 스테이지 선택 화면의 스테이지 진입 요청을 처리한다.
public class StageController : MonoBehaviour
{
    public int stageNum;  // 현재 플레이할 스테이지 번호 받기
    public StoryStage storystage;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        stageNum = GameSaveController.GetGameSaveOrDefault().savedStage;
    }
    void Start()
    {
        Debug.Log("savedStage 값: " + stageNum);
    }

    public void StageLoader(int stage)
    {
        Debug.Log("버튼 클릭");
        if (audioSource != null)
        {
            audioSource.Play();
        }

        SelectStage(stage);
    }

    public void SelectStage(int stage)
    {
        stageNum = stage;

        if (GameSaveController.instance == null)
        {
            Debug.LogError("GameSaveController가 StageController 씬에 없습니다.");
            return;
        }

        GameSaveController.instance.SavePlayingStage(stage);
        SendStage(stage);
    }

    private void SendStage(int stage)    // 스테이지 선택 화면에서 보낼 씬(스토리 + 전투)
    {
        StageFlowController.GetOrCreate().SendStage(stage);
    }
    public int GetStageNum()
    {
        return stageNum;
    }
    public void SetStageNum(int stage)
    {
        stageNum = stage;
    }

}
