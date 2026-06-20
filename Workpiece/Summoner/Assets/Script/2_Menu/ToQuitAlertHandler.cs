

// 게임 종료 알림 핸들러
// 역할: 게임 종료 확인 알림에서 종료 동작을 처리한다.
public class ToQuitAlertHandler : BaseAlertHandler
{
    public override void ShowAlert(System.Action<bool> callback)
    {
        base.ShowAlert(callback);
    }
}