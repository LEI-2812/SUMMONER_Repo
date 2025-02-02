using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISettingHandler
{
    void OpenSettings();
}


// 설정 처리 클래스
public class SettingHandler : MonoBehaviour, ISettingHandler
{
    public GameObject settingPanel;
    [SerializeField] private Setting setting;
    [SerializeField] private AudioSource menuClick;

    public void OpenSettings()
    {
        if (setting == null || settingPanel == null)
        {
            Debug.LogError("Setting 또는 Setting Panel이 할당되지 않았습니다.");
            return;
        }

        setting.openOption();
        settingPanel.SetActive(true);
        menuClick?.Play();
    }

    public void CloseSettings()
    {
        settingPanel?.SetActive(false);
    }

    public void ApplySettings()
    {
        if (setting != null)
        {
            //setting.GetAudioController().ApplySettings();
            //setting.GetVideoController().ApplySettings();
            //setting.GetGamePlayController().ApplySettings();
            Debug.Log("설정이 적용되었습니다.");
        }
        else
        {
            Debug.LogError("Setting 객체가 할당되지 않았습니다.");
        }
    }
}
