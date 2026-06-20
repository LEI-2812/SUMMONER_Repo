using UnityEngine;

// 역할: 화면 설정 값을 저장하고 다시 불러오는 저장소 역할을 한다.
public class VideoSettingStore
{
    private const string ResolutionIndexKey = "resolutionIndex";
    private const string ScreenModeIndexKey = "screenModeIndex";

    public int LoadValidResolutionIndex(int itemCount)
    {
        return GetValidSavedIndex(ResolutionIndexKey, itemCount);
    }

    public void SaveResolutionIndex(int index)
    {
        PlayerPrefs.SetInt(ResolutionIndexKey, index);
        PlayerPrefs.Save();
    }

    public int LoadValidScreenModeIndex(int itemCount)
    {
        return GetValidSavedIndex(ScreenModeIndexKey, itemCount);
    }

    public void SaveScreenModeIndex(int index)
    {
        PlayerPrefs.SetInt(ScreenModeIndexKey, index);
        PlayerPrefs.Save();
    }

    private static int GetValidSavedIndex(string prefsKey, int itemCount)
    {
        int savedIndex = PlayerPrefs.GetInt(prefsKey, 0);

        if (savedIndex >= 0 && savedIndex < itemCount)
        {
            return savedIndex;
        }

        PlayerPrefs.SetInt(prefsKey, 0);
        PlayerPrefs.Save();
        return 0;
    }
}
