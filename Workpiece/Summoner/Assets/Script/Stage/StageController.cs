using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{

    [Header("스테이지 버튼들(순서대로)")]
    public Button[] buttons;    // 활성화/비활성화할 버튼들

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private CheckStage checkStage;

    void Start()
    {
        checkStage = FindObjectOfType<CheckStage>();
        int stageNumber = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage 값 : " + stageNumber);
        ButtonInteractivity(7);
    }

    // 현재 스테이지 진행 정도
    [SerializeField]
    [Header("스테이지 진행도")]
    private int savedStage = 7;

    // PlayerPrefs로 스테이지 진행 정도 저장
    void SaveStage()
    {
        PlayerPrefs.SetInt("savedStage", savedStage);
        PlayerPrefs.Save();
    }

    //스테이지 저장 (매 스테이지 클리어마다 호출하면됨.)
    public void SaveStage(int stageNumber)
    {
        // "savedStage"라는 키로 스테이지 번호를 저장합니다.
        PlayerPrefs.SetInt("savedStage", stageNumber);
        PlayerPrefs.Save(); // 저장을 강제 실행합니다.
    }

    public void stageLoader(int stage)
    {
        Debug.Log("버튼 클릭");
        audioSource.Play();
        // 현재 플레이 중인 스테이지를 "playingStage"로 저장
        PlayerPrefs.SetInt("playingStage", stage);
        PlayerPrefs.Save();
        string sceneName;

        switch (stage)
        {
            case 1:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 1;
                break;
            case 2:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 1;
                break;
            case 3:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 1.5;
                break;
            case 4:
                Summon.multiple = 1.5;
                sceneName = "Fight Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                break;
            case 5:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 2;
                break;
            case 6:
                sceneName = "Fight Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 2;
                break;
            case 7:
                sceneName = "Story Screen_" + stage.ToString() + "Stage";
                SceneManager.LoadScene(sceneName);
                Summon.multiple = 4;
                break;
            // 필요한 스테이지만큼 추가
            default:
                Debug.Log("잘못된 스테이지입니다.");
                break;
        }

        //if (stage == 4 || stage == 6)
        //{
        //    string sceneName = "Fight Screen_" + stage.ToString() + "Stage";
        //    SceneManager.LoadScene(sceneName);
        //}
        //else
        //{
        //    string sceneName = "Story Screen_" + stage.ToString() + "Stage";
        //    SceneManager.LoadScene(sceneName);
        //}
    }

    void ButtonInteractivity(int stageNumber)
    {
        // 모든 버튼 비활성화
        foreach (Button button in buttons)
        {
            button.interactable = false; // 기본적으로 비활성화
        }

        for (int i = 0; i < stageNumber; i++)
        {
            if (i < buttons.Length) // 배열의 범위를 벗어나지 않도록 체크
            {
                buttons[i].interactable = true; // 해당 단계의 버튼 활성화
            }
        }
    }

}
