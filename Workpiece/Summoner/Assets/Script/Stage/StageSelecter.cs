using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelecter : MonoBehaviour
{
    [Header("스테이지 버튼들(순서대로)")]
    public Button[] buttons;    // 활성화/비활성화할 버튼들

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private StageController stageController;

    private void Awake()
    {
        stageController = FindObjectOfType<StageController>();
    }

    private void Start()
    {
        int stageNum = stageController.getStageNum();
        ButtonInteractivity(stageNum);
    }

    public void stageLoader(int stage)
    {
        Debug.Log("버튼 클릭");
        audioSource.Play();
        stageController.setStageNum(stage); //현재 스테이지 정보를 컨트롤러에 저장
        PlayerPrefs.SetInt("playingStage", stage);
        PlayerPrefs.Save();
        SendStage(stage);
    }

    public void SendStage(int stage)    // 스테이지 선택 화면에서 보낼 씬(스토리 + 전투)
    {
        switch (stage)
        {
            case 1:
                Summon.multiple = 1;
                SendStory(stage);
                break;
            case 2:
                Summon.multiple = 1;
                SendStory(stage);
                break;
            case 3:
                Summon.multiple = 1.5;
                SendStory(stage);
                break;
            case 4:
                Summon.multiple = 1.5;
                SendFight(stage);
                break;
            case 5:
                Summon.multiple = 2;
                SendStory(stage);
                break;
            case 6:
                Summon.multiple = 2;
                SendFight(stage);
                break;
            case 7:
                Summon.multiple = 4;
                SendStory(stage);
                break;
            // 필요한 스테이지만큼 추가
            default:
                Debug.Log("잘못된 스테이지입니다.");
                break;
        }
    }

    //스토리씬으로 이동
    private void SendStory(int stage)
    {
        string sceneName;
        sceneName = "Story Screen_" + stage.ToString() + "Stage";
        SceneManager.LoadScene(sceneName);
    }

    public void SendFight(int stage)
    {
        string sceneName;
        sceneName = "Fight Screen_" + stage.ToString() + "Stage";
        SceneManager.LoadScene(sceneName);
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
