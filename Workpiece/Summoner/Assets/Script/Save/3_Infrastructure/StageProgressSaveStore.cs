using UnityEngine;

// 역할: 스테이지 진행도 저장과 조회를 PlayerPrefs에 위임한다.
public class StageProgressSaveStore
{
    private const string SavedStageKey = "savedStage";
    private const string PlayingStageKey = "playingStage";

    public bool HasGameSave()
    {
        return PlayerPrefs.HasKey(SavedStageKey)
            && PlayerPrefs.GetInt(SavedStageKey, 0) >= 1;
    }

    public GameSaveData LoadGameSave()
    {
        return new GameSaveData
        {
            savedStage = PlayerPrefs.GetInt(SavedStageKey, 1),
            playingStage = PlayerPrefs.GetInt(PlayingStageKey, 1)
        };
    }

    public void SaveGameSave(GameSaveData saveData)
    {
        PlayerPrefs.SetInt(SavedStageKey, saveData.savedStage);
        PlayerPrefs.SetInt(PlayingStageKey, saveData.playingStage);
        PlayerPrefs.Save();
    }

    public void ResetGameProgress()
    {
        PlayerPrefs.DeleteKey(SavedStageKey);
        PlayerPrefs.DeleteKey(PlayingStageKey);
        PlayerPrefs.Save();
    }
}
