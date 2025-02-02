using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    [Header("��ŵ �˸� �ڵ鷯")]
    private SkipAlertHandler skipAlertHandler;
    public GameObject SkipBtn; //Button ���� �ϸ� ��Ȱ��ȭ�� �ȵ�

    [SerializeField] private AudioSource skipSound;

    private Setting setting;  // SettingMenuController�� �ν��Ͻ��� ����
    private string playingStage;   // �÷��� ���� �������� ��ȣ

    private void Start()
    {
        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
        if (skipAlertHandler == null) { Debug.LogWarning("��ŵ�Ҵ�ȵ�"); }
        // Setting �̱��� �ν��Ͻ��� ����
        setting = Setting.instance;

        if (setting == null)
        {
            Debug.LogWarning("SettingMenuController �ν��Ͻ��� �������� �ʽ��ϴ�.");
        }

        playingStage = PlayerPrefs.GetInt("playingStage", 1).ToString();
        Debug.Log("���� �÷��� �������� : " + playingStage);
        isSkipActive(); // ��ŵ��ư Ȱ��ȭ ���� �Ǻ�
    }

    private void Update()
    {
        // ��ư�� Ȱ��ȭ �Ǿ��ְ� �� ���¿��� ���͸� ������ ��ŵâ ���
        if (Input.GetKeyDown(KeyCode.Return) && SkipBtn.activeSelf)
        {
            skipAlert();
        }
        isSkipActive();
    }

    public void isSkipActive() // Setting�� ��ŵ ��ۿ� ���� ��ŵ��ư Ȱ��ȭ ���� ���� �޼ҵ�
    {
        if (setting != null)
        {
            if (setting.GetGamePlayController().getIsStorySkip())
            {
                SkipBtn.SetActive(true);
            }
            else
            {
                SkipBtn.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("������ �Ҵ�ȵ�");
        }
    }

    public void skipAlert() // ��ŵ Alert ���� �޼ҵ�
    {
        if (skipAlertHandler == null)
        {
            Debug.LogError("SkipAlertHandler�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        skipSound.Play();
        skipAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                Debug.Log("��ŵ�Ͽ� ���� ������ �̵�");
                string sceneName = "Fight Screen_" + playingStage + "Stage";
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                skipAlertHandler.HideAlert();
            }
        });
    }
}
