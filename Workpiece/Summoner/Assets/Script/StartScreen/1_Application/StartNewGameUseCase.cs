using System;
using UnityEngine;

// 역할: StartNewGameUseCase의 책임을 정의한다.
public class StartNewGameUseCase
{
    private const int NewGameStartStage = 1;

    private readonly GameSaveUseCase gameSaveUseCase;

    public StartNewGameUseCase()
        : this(new GameSaveUseCase(new StageProgressSaveStore()))
    {
    }

    public StartNewGameUseCase(GameSaveUseCase gameSaveUseCase)
    {
        this.gameSaveUseCase = gameSaveUseCase ?? throw new ArgumentNullException(nameof(gameSaveUseCase));
    }

    public bool TryStartNewGame()
    {
        gameSaveUseCase.StartNewGame();
        Debug.Log($"저장된 스테이지 번호: {NewGameStartStage}");
        Debug.Log("저장된 진행도를 초기화하고 새 게임을 시작합니다.");
        GameSceneUseCase.LoadPrologue();
        return true;
    }
}
