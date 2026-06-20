

// 스킵 알림 핸들러
// 역할: 스토리 스킵 확인 알림에서 스킵 동작을 처리한다.
public class SkipAlertHandler : BaseAlertHandler
{
    public override void ShowAlert(System.Action<bool> callback)
    {
        base.ShowAlert(callback);
    }

    public void HideAlert()
    {
        alertObject.SetActive(false);
    }
}
