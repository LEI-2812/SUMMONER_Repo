using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public Button yesBtn;
    public Button noBtn;

    private bool isClicked = false;
    private bool result = false;

    public void OnYesButtonClick()
    {
        result = true;
        isClicked = true;
    }

    public void OnNoButtonClick()
    {
        result = false;
        isClicked = true;
    }

    public bool getIsClicked()
    {
        return isClicked;
    }

    public bool getResult()
    {
        return result;
    }

    // ���¸� �ʱ�ȭ�ϴ� �޼��� �߰�
    public void ResetAlert()
    {
        isClicked = false; // �ٽ� Ŭ���� �� �ֵ��� ���� �ʱ�ȭ
        result = false;    // �⺻ �� �ʱ�ȭ
    }
}