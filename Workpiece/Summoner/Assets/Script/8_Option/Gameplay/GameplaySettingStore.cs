using UnityEngine;

// 역할: 게임플레이 설정 값을 저장하고 다시 불러오는 저장소 역할을 한다.
public class GameplaySettingStore
{
    private const string StorySkipKey = "IsStorySkip";
    private const string OnlyMouseKey = "IsOnlyMouse";

    public bool LoadStorySkipEnabled()
    {
        return PlayerPrefs.GetInt(StorySkipKey, 0) == 1;
    }

    public void SaveStorySkipEnabled(bool isEnabled)
    {
        PlayerPrefs.SetInt(StorySkipKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadOnlyMouseEnabled()
    {
        return PlayerPrefs.GetInt(OnlyMouseKey, 0) == 1;
    }

    public void SaveOnlyMouseEnabled(bool isEnabled)
    {
        PlayerPrefs.SetInt(OnlyMouseKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
