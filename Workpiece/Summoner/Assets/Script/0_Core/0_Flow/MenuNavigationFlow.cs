// 역할: 어디서든 메인 시작 화면으로 돌아가는 이동 요청을 한 흐름으로 모은다.
public static class MenuNavigationFlow
{
    public static void ReturnToStartScreen()
    {
        GameSceneFlow.LoadStartScreen();
    }
}
