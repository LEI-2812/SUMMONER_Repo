using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    public int stageNum;  // 현재 플레이할 스테이지 번호 받기
    public StoryStage storystage;

    [Header("버튼 클릭음")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        stageNum = GameSaveController.GetGameSaveOrDefault().savedStage;
    }
    void Start()
    {
        Debug.Log("savedStage 값: " + stageNum);
    }

    // 스테이지 진행도 저장은 저장 시스템을 통해 처리한다.
    public void SaveStage(int stageNumber)
    {
        if (GameSaveController.instance == null)
        {
            Debug.LogError("GameSaveController가 StageController 씬에 없습니다.");
            return;
        }

        GameSaveController.instance.SaveClearedStage(stageNumber);
    }

    public void StageLoader(int stage)
    {
        Debug.Log("버튼 클릭");
        audioSource.Play();
        // 현재 플레이 중인 스테이지도 저장 시스템을 통해 기록한다.
        if (GameSaveController.instance == null)
        {
            Debug.LogError("GameSaveController가 StageController 씬에 없습니다.");
            return;
        }

        GameSaveController.instance.SavePlayingStage(stage);
        SendStage(stage);
    }

    public void SendStage(int stage)    // 스테이지 선택 화면에서 보낼 씬(스토리 + 전투)
    {
        switch (stage)
        {
            case 1:
                SendStory(stage);
                Summon.StatMultiplierSet(1);
                SaveStage(1);
                break;
            case 2:
                SendStory(stage);
                Summon.StatMultiplierSet(1);
                SaveStage(2);
                break;
            case 3:
                SendStory(stage);
                Summon.StatMultiplierSet(1.2);
                SaveStage(3);
                break;
            case 4:
                SendFight(stage);
                Summon.StatMultiplierSet(1.2);
                SaveStage(4);
                break;
            case 5:
                SendStory(stage);
                Summon.StatMultiplierSet(1.5);
                SaveStage(5);
                break;
            case 6:
                SendFight(stage);
                Summon.StatMultiplierSet(1.5);
                SaveStage(6);
                break;
            case 7:
                SendStory(stage);
                Summon.StatMultiplierSet(2.5);
                SaveStage(7);
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
                Summon.StatMultiplierSet(1);
                break;
            case 2:
                SendFight(stage);
                Summon.StatMultiplierSet(1);
                break;
            case 3:
                SendFight(stage);
                Summon.StatMultiplierSet(1.5);
                break;
            case 4:
                SendFight(stage);
                Summon.StatMultiplierSet(1.5);
                break;
            case 5:
                SendFight(stage);
                Summon.StatMultiplierSet(2);
                break;
            case 6:
                SendFight(stage);
                Summon.StatMultiplierSet(2);
                break;
            case 7:
                SendFight(stage);
                Summon.StatMultiplierSet(4);
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
        SceneManager.LoadScene("Epilogue Screen");
    }
    public int GetStageNum()
    {
        return stageNum;
    }
    public void SetStageNum(int stage)
    {
        stageNum = stage;
    }

}
