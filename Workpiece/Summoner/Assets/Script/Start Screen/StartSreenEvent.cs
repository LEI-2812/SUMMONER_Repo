using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSreenEvent : MonoBehaviour
{

    public GameObject option;

    //스테이지 저장 (매 스테이지 클리어마다 호출하면됨.)
    public void SaveStage(int stageNumber)
    {
        // "CurrentStage"라는 키로 스테이지 번호를 저장합니다.
        PlayerPrefs.SetInt("SaveStage", stageNumber);
        PlayerPrefs.Save(); // 저장을 강제 실행합니다.
    }

    //스테이지 로드
    public int LoadStage()
    {
        if (PlayerPrefs.HasKey("SaveStage"))
        {
            return PlayerPrefs.GetInt("SaveStage");
        }
        else
        {
            // 저장된 값이 없을 때 추가작업
            return 1; // 또는 원하는 다른 값
        }
    }

    //이어하기
    public void StartSavedStage() 
    {
        int savedStage = LoadStage();
        Debug.Log("저장된 스테이지 "+ savedStage+"로 이동");
        // 예시: 저장된 스테이지로 씬 로드
        //SceneManager.LoadScene("Stage" + savedStage);
    }

    //새게임
    public void NewStart()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("저장되어있던 데이터를 모두 삭제후 새게임 시작");
    }

    //설정창 끄기 키기
    public void openOption()
    {
        if(option.activeSelf==true)
            option.SetActive(false);
        else
            option.SetActive(true);
    }

    //게임 종료
    public void ExitGame()
    {
        Application.Quit(); //빌드해야 작동함.
    }

}
