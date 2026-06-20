using UnityEngine;

// PlayerPrefs를 통해 게임 진행 데이터를 읽고 쓴다.
// PlayerPrefs 접근을 이 클래스에 모아두면 나중에 저장 방식을 교체하기 쉽다.
// 역할: PlayerPrefs를 사용해 저장 데이터를 실제 로컬 저장소에 기록하고 읽는다.
public class PlayerPrefsSaveStore
{
    private const string SavedStageKey = "savedStage";
    private const string PlayingStageKey = "playingStage";

    // 이어하기 버튼을 보여줄지 판단할 때 사용한다.
    public bool HasGameSave()
    {
        return PlayerPrefs.HasKey(SavedStageKey)
            && PlayerPrefs.GetInt(SavedStageKey, 0) >= 1;
    }

    // 저장된 진행 데이터를 불러온다. 값이 없으면 새 게임 기준인 1스테이지를 사용한다.
    public GameSaveData LoadGameSave()
    {
        return new GameSaveData
        {
            savedStage = PlayerPrefs.GetInt(SavedStageKey, 1),
            playingStage = PlayerPrefs.GetInt(PlayingStageKey, 1)
        };
    }

    // 진행 데이터를 PlayerPrefs에 저장한다.
    public void SaveGameSave(GameSaveData saveData)
    {
        PlayerPrefs.SetInt(SavedStageKey, saveData.savedStage);
        PlayerPrefs.SetInt(PlayingStageKey, saveData.playingStage);
        PlayerPrefs.Save();
    }

    // 진행 데이터 키만 삭제한다. DeleteAll은 설정값까지 지울 수 있으므로 사용하지 않는다.
    public void ResetGameProgress()
    {
        PlayerPrefs.DeleteKey(SavedStageKey);
        PlayerPrefs.DeleteKey(PlayingStageKey);
        PlayerPrefs.Save();
    }
}
