using UnityEngine;
using UnityEngine.UI;

// 역할: 확인/취소 알림창의 문구와 버튼 표시를 담당한다.
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

    // 상태를 초기화하는 메서드 추가
    public void ResetAlert()
    {
        isClicked = false; // 다시 클릭할 수 있도록 상태 초기화
        result = false;    // 기본 값 초기화
    }
}
