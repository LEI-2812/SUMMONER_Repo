using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageText : MonoBehaviour
{
    public TMP_Text stageText; // 표시할 TextMeshPro 컴포넌트

    void Start()
    {
        // PlayerPrefs에서 savedStage 값 불러오기
        int savedStageValue = PlayerPrefs.GetInt("savedStage", 0); // 기본값 0

        // 패널에 savedStage 값을 표시
        stageText.text = "<b>[" + savedStageValue.ToString() + "스테이지 : 임시 스테이지]</b>" +
                            "<br>여기부터 진행하시겠습니까?";
        // 나중에 스테이지 이름들도 정해지면 각 단계에 맞는 스테이지명을 넣어버리면 되지 않을까..?
    }
}
