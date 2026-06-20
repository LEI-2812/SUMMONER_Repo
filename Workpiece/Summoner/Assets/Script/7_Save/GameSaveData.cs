using System;

// 게임 진행 상태를 복원하는 데 필요한 최소 데이터만 담는다.
// 저장할 값이 늘어나면 이 클래스에 필드를 추가하고 저장소 코드도 함께 확장한다.
[Serializable]
// 역할: 저장해야 하는 게임 진행 값을 담는 데이터 객체다.
public class GameSaveData
{
    // 스테이지 선택 화면에서 열려 있어야 하는 최대 진행 스테이지 번호.
    public int savedStage = 1;

    // 스토리 씬과 전투 씬이 공유하는 현재 플레이 중인 스테이지 번호.
    public int playingStage = 1;

    public static GameSaveData CreateNewGameProgress()
    {
        return new GameSaveData
        {
            savedStage = 1,
            playingStage = 1
        };
    }

    // 외부 코드가 저장 원본을 실수로 직접 수정하지 못하도록 복사본을 만든다.
    public GameSaveData Clone()
    {
        return new GameSaveData
        {
            savedStage = savedStage,
            playingStage = playingStage
        };
    }
}
