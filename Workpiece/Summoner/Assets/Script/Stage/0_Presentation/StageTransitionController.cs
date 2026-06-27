using UnityEngine;

public class StageTransitionController : MonoBehaviour
{
    public static StageTransitionController instance;

    private readonly StageTransitionUseCase transitionUseCase = new StageTransitionUseCase();

    public static StageTransitionController GetOrCreate()
    {
        if (instance != null)
        {
            return instance;
        }

        StageTransitionController stageTransitionController = FindObjectOfType<StageTransitionController>();
        if (stageTransitionController != null)
        {
            instance = stageTransitionController;
            return instance;
        }

        GameObject stageTransitionObject = new GameObject(nameof(StageTransitionController));
        return stageTransitionObject.AddComponent<StageTransitionController>();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    public void SendNextStageAfterBattle(int clearedStage)
    {
        if (transitionUseCase.ShouldOpenEpilogueAfterBattle(clearedStage))
        {
            SendEpilogue();
            return;
        }

        SendStage(transitionUseCase.GetNextStageAfterBattle(clearedStage));
    }

    public void SendStage(int stage)
    {
        if (!transitionUseCase.TryPrepareStage(stage))
        {
            Debug.LogWarning("유효하지 않은 스테이지 번호: " + stage);
            return;
        }

        if (transitionUseCase.ShouldOpenStory(stage))
        {
            SendStory(stage);
            return;
        }

        SendFight(stage);
    }

    public void SendFightStage(int stage)
    {
        if (!transitionUseCase.TryPrepareFightStage(stage))
        {
            Debug.LogWarning("유효하지 않은 전투 스테이지 번호: " + stage);
            return;
        }

        SendFight(stage);
    }

    public void SendStory(int stage)
    {
        GameSceneUseCase.LoadStory(stage);
    }

    public void SendFight(int stage)
    {
        GameSceneUseCase.LoadFight(stage);
    }

    public void SendStageSelect()
    {
        GameSceneUseCase.LoadStageSelect();
    }

    public void SendEpilogue()
    {
        GameSceneUseCase.LoadEpilogue();
    }
}
