using UnityEngine;

// 역할: OpenStartOptionUseCase의 책임을 정의한다.
public class OpenStartOptionUseCase
{
    public bool TryOpenOption(SettingPanelView settingPanelView)
    {
        if (settingPanelView == null)
        {
            Debug.LogError("SettingPanelView를 찾을 수 없습니다.");
            return false;
        }

        settingPanelView.OpenOption();
        return true;
    }
}
