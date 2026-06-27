using UnityEngine;
using UnityEngine.UI;

// 역할: ConfirmAlertView의 책임을 정의한다.
public class ConfirmAlertView : MonoBehaviour
{
    [Header("Yes 버튼")]
    public Button yesBtn;

    [Header("No 버튼")]
    public Button noBtn;

    private bool isClicked = false;
    private bool result = false;

    public void OnYesButtonClick()
    {
        result = true;
        isClicked = true;
        Debug.Log("yes 버튼 클릭");
    }

    public void OnNoButtonClick()
    {
        result = false;
        isClicked = true;
    }

    public bool GetIsClicked()
    {
        return isClicked;
    }

    public bool GetResult()
    {
        return result;
    }

    public void ResetAlert()
    {
        isClicked = false;
        result = false;
    }
}
