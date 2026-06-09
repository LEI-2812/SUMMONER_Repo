using UnityEngine;
using UnityEngine.SceneManagement;

// 스테이지 흐름과 씬 이동 판단만 담당한다.
// 저장, 알림창 표시, 전투 로직은 이 클래스에 넣지 않는다.
public class StageFlowController : MonoBehaviour
{
    public static StageFlowController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        // 씬 이동으로 중복 생성된 흐름 컨트롤러는 하나만 남긴다.
        Destroy(gameObject);
    }

    // 전투 클리어 후 다음 이동 위치를 결정한다.
    public void SendNextStageAfterBattle(int clearedStage)
    {
        if (clearedStage >= 7)
        {
            SendEpilogue();
            return;
        }

        SendStage(clearedStage + 1);
    }

    // 스테이지 번호에 따라 스토리 씬 또는 전투 씬으로 이동한다.
    public void SendStage(int stage)
    {
        switch (stage)
        {
            case 1:
                Summon.StatMultiplierSet(1);
                SendStory(stage);
                break;
            case 2:
                Summon.StatMultiplierSet(1);
                SendStory(stage);
                break;
            case 3:
                Summon.StatMultiplierSet(1.2);
                SendStory(stage);
                break;
            case 4:
                Summon.StatMultiplierSet(1.2);
                SendFight(stage);
                break;
            case 5:
                Summon.StatMultiplierSet(1.5);
                SendStory(stage);
                break;
            case 6:
                Summon.StatMultiplierSet(1.5);
                SendFight(stage);
                break;
            case 7:
                Summon.StatMultiplierSet(2.5);
                SendStory(stage);
                break;
            default:
                Debug.LogWarning("잘못된 스테이지 번호입니다: " + stage);
                break;
        }
    }

    public void SendStory(int stage)
    {
        SceneManager.LoadScene("Story Screen_" + stage + "Stage");
    }

    public void SendFight(int stage)
    {
        SceneManager.LoadScene("Fight Screen_" + stage + "Stage");
    }

    public void SendStageSelect()
    {
        SceneManager.LoadScene("Stage Select Screen");
    }

    public void SendEpilogue()
    {
        SceneManager.LoadScene("Epilogue Screen");
    }
}
