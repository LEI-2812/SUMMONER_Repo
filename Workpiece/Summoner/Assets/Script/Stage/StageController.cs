using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    public int stageNum;  //현재 플레이할 스테이지 번호 받기
    public StoryStage storystage;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        stageNum = PlayerPrefs.GetInt("savedStage");
    }
    void Start()
    {
        //int stageNumber = PlayerPrefs.GetInt("savedStage");
        Debug.Log("savedStage 값 : " + stageNum);
    }

    // PlayerPrefs로 스테이지 진행 정도 저장
    void SaveStage()
    {
        PlayerPrefs.SetInt("savedStage", stageNum);
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
        SendStage(stage);
    }

    public void SendStage(int stage)    // 스테이지 선택 화면에서 보낼 씬(스토리 + 전투)
    {
        switch (stage)
        {
            case 1:
                SendStory(stage);
                Summon.multiple = 1;
                break;
            case 2:
                SendStory(stage);
                Summon.multiple = 1;
                break;
            case 3:
                SendStory(stage);
                Summon.multiple = 1.5;
                break;
            case 4:
                SendFight(stage);
                Summon.multiple = 1.5;
                break;
            case 5:
                SendStory(stage);
                Summon.multiple = 2;
                break;
            case 6:
                SendFight(stage);
                Summon.multiple = 2;
                break;
            case 7:
                SendStory(stage);
                Summon.multiple = 4;
                break;
            // 필요한 스테이지만큼 추가
            default:
                Debug.Log("잘못된 스테이지입니다.");
                break;
        }
    }
    public void SendFightStage(int stage)   // 승리/패배창에서 보낼 씬(오직 전투)
    {
        switch (stage)
        {
            case 1:
                SendFight(stage);
                Summon.multiple = 1;
                break;
            case 2:
                SendFight(stage);
                Summon.multiple = 1;
                break;
            case 3:
                SendFight(stage);
                Summon.multiple = 1.5;
                break;
            case 4:
                SendFight(stage);
                Summon.multiple = 1.5;                
                break;
            case 5:
                SendFight(stage);
                Summon.multiple = 2;
                break;
            case 6:
                SendFight(stage);
                Summon.multiple = 2;
                break;
            case 7:
                SendFight(stage);
                Summon.multiple = 4;
                break;
            // 필요한 스테이지만큼 추가
            default:
                Debug.Log("잘못된 스테이지입니다.");
                break;
        }
    }

    public void SendStory(int stage)
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

    public void SendEpilogue()
    {
        SceneManager.LoadScene("Pro_Epi Screen 1");
    }
    public int getStageNum()
    {
        return stageNum;
    }
    public void setStageNum(int stage)
    {
        stageNum = stage;
    }

}
