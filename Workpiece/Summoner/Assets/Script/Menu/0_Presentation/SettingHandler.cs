using UnityEngine;


// 역할: SettingHandler의 책임을 정의한다.
public class SettingHandler : MonoBehaviour
{
    public GameObject settingPanel;
    [SerializeField] private SettingPanelView setting;
    [SerializeField] private AudioSource menuClick;

    public void OpenSettings()
    {
        if (!ResolveSetting참조())
        {
            Debug.LogError("Setting 또는 Setting Panel이 할당되지 않았습니다.");
            return;
        }

        settingPanel.SetActive(true);
        menuClick?.Play();
    }

    public void CloseSettings()
    {
        if (ResolveSetting참조())
        {
            settingPanel.SetActive(false);
        }
    }

    public bool IsSettingsOpen()
    {
        return ResolveSetting참조() && settingPanel.activeSelf;
    }

    private bool ResolveSetting참조()
    {
        if (setting == null)
        {
            setting = FindObjectOfType<SettingPanelView>();
        }

        if (settingPanel == null && setting != null)
        {
            settingPanel = setting.settingPanel;
        }

        return setting != null && settingPanel != null;
    }
}
