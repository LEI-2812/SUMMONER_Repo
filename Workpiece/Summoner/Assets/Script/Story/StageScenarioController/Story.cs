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

    private HUDManager hudManager;

    private string playingStage;   // �÷��� ���� �������� ��ȣ

    private void Start()
    {
        hudManager = FindObjectOfType<HUDManager>();
        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
        if (skipAlertHandler == null) { Debug.LogWarning("��ŵ�Ҵ�ȵ�"); }
 

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
        if (hudManager != null)
        {
            if (hudManager.settingController.isStorySkip.isOn)
            {
                Debug.Log("Ȱ��ȭ��");
                SkipBtn.SetActive(true);
            }
            else
            {
                Debug.Log("��Ȱ��ȭ��");
                SkipBtn.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("HUDManager�� �Ҵ�ȵ�");
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
