

// ��ŵ �˸� �ڵ鷯
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
