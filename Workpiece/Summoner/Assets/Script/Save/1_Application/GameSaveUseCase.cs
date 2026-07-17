using System;

// 역할: GameSaveUseCase의 책임을 정의한다.
public class GameSaveUseCase
{
    private readonly StageProgressSaveStore saveStore = new StageProgressSaveStore();
    private GameSaveData currentSaveData;

    public void LoadGameSave()
    {
        currentSaveData = saveStore.LoadGameSave();
    }

    public bool HasGameSave()
    {
        return saveStore.HasGameSave();
    }

    public GameSaveData GetGameSave()
    {
        EnsureGameSaveLoaded();
        return currentSaveData.Clone();
    }

    public void StartNewGame()
    {
        currentSaveData = GameSaveData.CreateNewGameProgress();
        saveStore.SaveGameSave(currentSaveData);
    }

    public void SavePlayingStage(int stage)
    {
        EnsureGameSaveLoaded();
        currentSaveData.playingStage = stage;
        saveStore.SaveGameSave(currentSaveData);
    }

    public void SaveClearedStage(int nextStage)
    {
        EnsureGameSaveLoaded();
        currentSaveData.savedStage = Math.Max(currentSaveData.savedStage, nextStage);
        currentSaveData.playingStage = nextStage;
        saveStore.SaveGameSave(currentSaveData);
    }

    public void ResetGameProgress()
    {
        saveStore.ResetGameProgress();
        currentSaveData = GameSaveData.CreateNewGameProgress();
    }

    private void EnsureGameSaveLoaded()
    {
        if (currentSaveData == null)
        {
            LoadGameSave();
        }
    }
}
