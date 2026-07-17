using UnityEngine;

// 역할: 게임 종료 요청을 Unity Application 경계로 전달한다.
public class ExitGameUseCase
{
    public void ExitGame()
    {
        Application.Quit();
    }
}
