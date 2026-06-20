

// 메인 화면 알림 핸들러
// 역할: 메인 화면 이동 확인 알림에서 화면 이동 동작을 처리한다.
public class ToMainAlertHandler : BaseAlertHandler
{
    public override void ShowAlert(System.Action<bool> callback)
    {
        base.ShowAlert(callback);
    }
}
