using System;

// 역할: GameSaveData의 책임을 정의한다.
// 역할: GameSaveData의 책임을 정의한다.
[Serializable]
// 역할: GameSaveData의 책임을 정의한다.
public class GameSaveData
{
    public int savedStage = 1;

    public int playingStage = 1;

    public static GameSaveData CreateNewGameProgress()
    {
        return new GameSaveData
        {
            savedStage = 1,
            playingStage = 1
        };
    }

    public GameSaveData Clone()
    {
        return new GameSaveData
        {
            savedStage = savedStage,
            playingStage = playingStage
        };
    }
}
