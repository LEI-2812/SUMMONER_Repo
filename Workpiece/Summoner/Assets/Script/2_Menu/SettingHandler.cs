using UnityEngine;


// 설정 처리 클래스
// 역할: 설정 버튼 입력을 받아 설정 패널 표시 흐름을 제어한다.
public class SettingHandler : MonoBehaviour
{
    public GameObject settingPanel;
    [SerializeField] private SettingPanelView setting;
    [SerializeField] private AudioSource menuClick;

    public void OpenSettings()
    {
        if (setting == null || settingPanel == null)
        {
            Debug.LogError("Setting 또는 Setting Panel이 할당되지 않았습니다.");
            return;
        }

        setting.OpenOption();
        settingPanel.SetActive(true);
        menuClick?.Play();
    }

    public void CloseSettings()
    {
        settingPanel?.SetActive(false);
    }
}
