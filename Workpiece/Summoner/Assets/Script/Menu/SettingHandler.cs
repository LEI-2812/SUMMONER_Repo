using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISettingHandler
{
    void OpenSettings();
}


// ���� ó�� Ŭ����
public class SettingHandler : MonoBehaviour, ISettingHandler
{
    public GameObject settingPanel;
    [SerializeField] private Setting setting;
    [SerializeField] private AudioSource menuClick;

    public void OpenSettings()
    {
        if (setting == null || settingPanel == null)
        {
            Debug.LogError("Setting �Ǵ� Setting Panel�� �Ҵ���� �ʾҽ��ϴ�.");
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
            Debug.Log("������ ����Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("Setting ��ü�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
