using UnityEngine;
using UnityEngine.UI;

// 역할: StageTextView의 책임을 정의한다.
public class StageTextView : MonoBehaviour
{
    public Text stageText;

    public void Show(StageDisplayData stageDisplayData)
    {
        stageText.text = "<b>[" + stageDisplayData.StageNumber + " 스테이지 : " + stageDisplayData.StageName + "]</b>" +
                 "\n여기부터 진행하시겠습니까?";
    }
}
