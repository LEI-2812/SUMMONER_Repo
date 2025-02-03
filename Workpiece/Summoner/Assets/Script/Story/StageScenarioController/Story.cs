using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    [Header("스킵 알림 핸들러")]
    private SkipAlertHandler skipAlertHandler;
    public GameObject SkipBtn; //Button 으로 하면 비활성화가 안됨

    [SerializeField] private AudioSource skipSound;

    private HUDManager hudManager;

    private string playingStage;   // 플레이 중인 스테이지 번호

    private void Start()
    {
        hudManager = FindObjectOfType<HUDManager>();
        skipAlertHandler = FindObjectOfType<SkipAlertHandler>();
        if (skipAlertHandler == null) { Debug.LogWarning("스킵할당안됨"); }
 

        playingStage = PlayerPrefs.GetInt("playingStage", 1).ToString();
        Debug.Log("현재 플레이 스테이지 : " + playingStage);
        isSkipActive(); // 스킵버튼 활성화 여부 판별
    }

    private void Update()
    {
        // 버튼이 활성화 되어있고 그 상태에서 엔터를 누르면 스킵창 출력
        if (Input.GetKeyDown(KeyCode.Return) && SkipBtn.activeSelf)
        {
            skipAlert();
        }
        isSkipActive();
    }

    public void isSkipActive() // Setting의 스킵 토글에 따라 스킵버튼 활성화 여부 동작 메소드
    {
        if (hudManager != null)
        {
            if (hudManager.settingController.isStorySkip.isOn)
            {
                Debug.Log("활성화됨");
                SkipBtn.SetActive(true);
            }
            else
            {
                Debug.Log("비활성화됨");
                SkipBtn.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("HUDManager가 할당안됨");
        }
    }

    public void skipAlert() // 스킵 Alert 동작 메소드
    {
        if (skipAlertHandler == null)
        {
            Debug.LogError("SkipAlertHandler가 할당되지 않았습니다.");
            return;
        }

        skipSound.Play();
        skipAlertHandler.ShowAlert(result =>
        {
            if (result)
            {
                Debug.Log("스킵하여 전투 씬으로 이동");
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
