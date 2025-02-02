

// 스킵 알림 핸들러
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
