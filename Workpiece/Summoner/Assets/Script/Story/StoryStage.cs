using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StoryStage : MonoBehaviour
{
    [Header("스토리 스테이지 번호입력")]
    [SerializeField]private int storyNum;  //스테이지 번호 받기
    private int x = 0;
    private int y = 0;

    public void checkStage() // 스테이지 번호별로 출력할 스토리 다르게 설정
    {
        switch (storyNum)
        {
            case 0: // 프롤로그 대사 시작, 끝값 지정
                x = 1; y = 9;
                break;
            case 1:  // 1스테이지 대사 시작, 끝값 지정
                x = 16; y = 27;
                break;
            case 2:  // 2스테이지 대사 시작, 끝값 지정
                x = 28; y = 33;
                break;
            case 3:  // 3스테이지 대사 시작, 끝값 지정
                x = 34; y = 40;
                break;
            case 5:  // 5스테이지 대사 시작, 끝값 지정
                x = 41; y = 48;
                break;
            case 7:  // 7스테이지 대사 시작, 끝값 지정
                x = 49; y = 57;
                break;
            case 8:  // 에필로그 대사 시작, 끝값 지정
                x = 10; y = 15;
                break;
            default:
                Debug.Log("잘못된 스테이지 번호입니다.");
                break;
        }
    }

    public int getStoryNum()
    {
        return storyNum;
    }
    public void setStoryNum(int stage)
    {
        storyNum = stage;
    }
    public int getX()
    {
        return x;
    }
    public int getY()
    {
        return y;
    }
}
