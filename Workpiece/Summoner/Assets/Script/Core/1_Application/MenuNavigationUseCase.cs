// 역할: MenuNavigationUseCase의 책임을 정의한다.
public static class MenuNavigationUseCase
{
    public static void ReturnToStartScreen()
    {
        GameSceneUseCase.LoadStartScreen();
    }
}
