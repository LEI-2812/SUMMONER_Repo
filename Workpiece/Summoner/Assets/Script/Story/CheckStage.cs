using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CheckStage : MonoBehaviour
{
    public int stageNum;  //스테이지 번호 받기
    public static int x = 0;
    public static int y = 0;

    public void checkStage() // 스테이지 번호별로 출력할 스토리 다르게 설정
    {
        switch (stageNum)
        {
            case 0: // 프롤로그 대사 시작, 끝값 지정
                x = 1; y = 9;
                break;
            case 1:  // 1스테이지 대사 시작, 끝값 지정
                x = 16; y = 26;
                break;
            case 2:  // 2스테이지 대사 시작, 끝값 지정
                x = 27; y = 31;
                break;
            case 3:  // 3스테이지 대사 시작, 끝값 지정
                x = 32; y = 35;
                break;
            case 5:  // 5스테이지 대사 시작, 끝값 지정
                x = 36; y = 40;
                break;
            case 7:  // 7스테이지 대사 시작, 끝값 지정
                x = 41; y = 50;
                break;
            case 8:  // 에필로그 대사 시작, 끝값 지정
                x = 10; y = 15;
                break;
            default:
                Debug.Log("잘못된 스테이지 번호입니다.");
                break;
        }
    }
}
