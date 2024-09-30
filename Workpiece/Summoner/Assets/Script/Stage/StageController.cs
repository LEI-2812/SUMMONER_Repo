using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{

    [Header("스테이지 버튼들(순서대로)")]
    public Button[] buttons;    // 활성화/비활성화할 버튼들

    void Start()
    {
        int stageNumber = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage 값 : " + stageNumber);
        ButtonInteractivity(stageNumber);
    }

    // 현재 스테이지 진행 정도
    [SerializeField]
    [Header("스테이지 진행도")]
    private int savedStage = 0;
    
    // PlayerPrefs로 스테이지 진행 정도 저장
    void SaveStage()
    {
        PlayerPrefs.SetInt("savedStage", savedStage);
        PlayerPrefs.Save();
    }

    //스테이지 저장 (매 스테이지 클리어마다 호출하면됨.)
    public void SaveStage(int stageNumber)
    {
        // "CurrentStage"라는 키로 스테이지 번호를 저장합니다.
        PlayerPrefs.SetInt("savedStage", stageNumber);
        PlayerPrefs.Save(); // 저장을 강제 실행합니다.
    }

    public void stageLoader(int stage)
    {
        SceneManager.LoadScene("Stage" + stage.ToString());   
    }

    //임시코드
    public void TestSceneLoad()
    {
        SceneManager.LoadScene("Fight Screen");
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
