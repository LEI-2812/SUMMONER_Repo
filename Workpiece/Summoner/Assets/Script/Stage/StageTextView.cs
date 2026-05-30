using UnityEngine;
using UnityEngine.UI;

public class StageTextView : MonoBehaviour
{
    public Text stageText; // 표시할 Text 컴포넌트

    void Start()
    {
        int savedStageValue = Mathf.Min(GameSaveController.GetGameSaveOrDefault().savedStage, 7);

        // 패널에 savedStage 값을 표시
        stageText.text = "<b>[" + savedStageValue.ToString() + " 스테이지 : " + ShowTextSavedStage(savedStageValue) + "]</b>" +
                 "\n여기부터 진행하시겠습니까?";
    }

    private string ShowTextSavedStage(int stageNum)
    {
        string stageName = "";
        switch (stageNum)
        {
            case 1:
                stageName = "광활한 브리뉴 평원 동쪽";
                break;
            case 2:
                stageName = "광활한 브리뉴 평원 서쪽";
                break;
            case 3:
                stageName = "정령 숲지대의 왼편";
                break;
            case 4:
                stageName = "정령 숲지대의 오른편";
                break;
            case 5:
                stageName = "중간계의 오염지대";
                break;
            case 6:
                stageName = "화염구의 무덤";
                break;
            case 7:
                stageName = "다크 드래곤의 둥지";
                break;
        }

        return stageName;
    }
}
