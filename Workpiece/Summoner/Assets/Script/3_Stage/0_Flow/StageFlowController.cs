using UnityEngine;

// 스테이지 흐름과 씬 이동 판단만 담당한다.
// 저장, 알림창 표시, 전투 로직은 이 클래스에 넣지 않는다.
// 역할: 스테이지 선택, 전투 진입, 전투 후 이동 같은 스테이지 흐름을 제어한다.
public class StageFlowController : MonoBehaviour
{
    public static StageFlowController instance;

    public static StageFlowController GetOrCreate()
    {
        if (instance != null)
        {
            return instance;
        }

        StageFlowController stageFlowController = FindObjectOfType<StageFlowController>();
        if (stageFlowController != null)
        {
            instance = stageFlowController;
            return instance;
        }

        GameObject stageFlowObject = new GameObject(nameof(StageFlowController));
        return stageFlowObject.AddComponent<StageFlowController>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        // 씬 이동으로 중복 생성된 흐름 컨트롤러는 하나만 남긴다.
        Destroy(gameObject);
    }

    // 전투 클리어 후 다음 이동 위치를 결정한다.
    public void SendNextStageAfterBattle(int clearedStage)
    {
        if (clearedStage >= 7)
        {
            SendEpilogue();
            return;
        }

        SendStage(clearedStage + 1);
    }

    // 스테이지 번호에 따라 스토리 씬 또는 전투 씬으로 이동한다.
    public void SendStage(int stage)
    {
        if (!TrySetStoryStageMultiplier(stage))
        {
            Debug.LogWarning("잘못된 스테이지 번호입니다: " + stage);
            return;
        }

        if (ShouldOpenStory(stage))
        {
            SendStory(stage);
            return;
        }

        SendFight(stage);
    }

    // 승리/패배창처럼 스토리를 거치지 않고 전투 씬으로 바로 이동한다.
    public void SendFightStage(int stage)
    {
        if (!TrySetFightStageMultiplier(stage))
        {
            Debug.LogWarning("잘못된 전투 스테이지 번호입니다: " + stage);
            return;
        }

        SendFight(stage);
    }

    private bool TrySetStoryStageMultiplier(int stage)
    {
        switch (stage)
        {
            case 1:
            case 2:
                Summon.StatMultiplierSet(1);
                return true;
            case 3:
            case 4:
                Summon.StatMultiplierSet(1.2);
                return true;
            case 5:
            case 6:
                Summon.StatMultiplierSet(1.5);
                return true;
            case 7:
                Summon.StatMultiplierSet(2.5);
                return true;
            default:
                return false;
        }
    }

    private bool TrySetFightStageMultiplier(int stage)
    {
        switch (stage)
        {
            case 1:
            case 2:
                Summon.StatMultiplierSet(1);
                return true;
            case 3:
            case 4:
                Summon.StatMultiplierSet(1.5);
                return true;
            case 5:
            case 6:
                Summon.StatMultiplierSet(2);
                return true;
            case 7:
                Summon.StatMultiplierSet(4);
                return true;
            default:
                return false;
        }
    }

    private bool ShouldOpenStory(int stage)
    {
        return stage == 1 || stage == 2 || stage == 3 || stage == 5 || stage == 7;
    }

    public void SendStory(int stage)
    {
        GameSceneFlow.LoadStory(stage);
    }

    public void SendFight(int stage)
    {
        GameSceneFlow.LoadFight(stage);
    }

    public void SendStageSelect()
    {
        GameSceneFlow.LoadStageSelect();
    }

    public void SendEpilogue()
    {
        GameSceneFlow.LoadEpilogue();
    }
}
