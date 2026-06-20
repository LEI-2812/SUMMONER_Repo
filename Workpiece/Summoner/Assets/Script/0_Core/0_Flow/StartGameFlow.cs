using UnityEngine;

// 역할: 시작 화면에서 새 게임과 이어하기의 저장 처리와 첫 이동을 한 곳에서 실행한다.
public static class StartGameFlow
{
    private const int NewGameStartStage = 1;

    public static bool HasSavedGame()
    {
        return GameSaveController.instance != null && GameSaveController.instance.HasGameSave();
    }

    public static void LoadStartScreenHud()
    {
        GameSceneFlow.LoadHudAdditive();
    }

    public static bool TryGetSavedStage(out int savedStage)
    {
        savedStage = 0;

        if (!HasSavedGame())
        {
            return false;
        }

        savedStage = GameSaveController.instance.GetGameSave().savedStage;
        return true;
    }

    public static bool TryStartNewGame(StageController stageController)
    {
        if (stageController == null)
        {
            Debug.LogError("checkStage가 null입니다. 올바르게 설정되었는지 확인하세요.");
            return false;
        }

        if (GameSaveController.instance == null)
        {
            Debug.LogError("GameSaveController가 Start Screen 씬에 없습니다.");
            return false;
        }

        stageController.SetStageNum(NewGameStartStage);
        GameSaveController.instance.StartNewGame();
        Debug.Log($"저장된 스테이지 번호: {NewGameStartStage}");
        Debug.Log("저장되어있던 데이터를 모두 삭제후 새게임 시작");
        GameSceneFlow.LoadPrologue();
        return true;
    }

    public static bool TryContinueSavedGame(StageController stageController)
    {
        if (stageController == null)
        {
            Debug.LogError("checkStage가 null입니다. 올바르게 설정되었는지 확인하세요.");
            return false;
        }

        if (!TryGetSavedStage(out int savedStage))
        {
            return false;
        }

        Debug.Log($"저장된 스테이지 번호: {savedStage}");
        stageController.SetStageNum(savedStage);
        GameSceneFlow.LoadStageSelect();
        return true;
    }
}
