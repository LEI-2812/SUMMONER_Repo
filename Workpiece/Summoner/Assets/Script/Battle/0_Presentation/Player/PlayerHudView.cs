using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 역할: PlayerHudView의 책임을 정의한다.
public class PlayerHudView
{
    private readonly Button summonButton;
    private readonly TextMeshProUGUI summonButtonText;
    private readonly Button redrawButton;
    private readonly TextMeshProUGUI redrawButtonText;
    private readonly Image statePanel;

    public PlayerHudView(
        Button summonButton,
        Button redrawButton,
        Image statePanel)
    {
        this.summonButton = summonButton;
        this.redrawButton = redrawButton;
        this.statePanel = statePanel;
        summonButtonText = summonButton.GetComponentInChildren<TextMeshProUGUI>();
        redrawButtonText = redrawButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ShowSummonAvailable(bool canSummon)
    {
        if (canSummon)
        {
            ShowSummonButtonEnabled();
            return;
        }

        ShowSummonButtonDisabled();
    }

    public void ShowRedrawAvailable(bool canRedraw)
    {
        if (canRedraw)
        {
            ShowRedrawButtonEnabled();
            return;
        }

        ShowRedrawButtonDisabled();
    }

    public void HideStatePanel()
    {
        statePanel.gameObject.SetActive(false);
    }

    private void ShowSummonButtonEnabled()
    {
        summonButton.image.color = new Color32(227, 138, 64, 255);
        summonButtonText.color = new Color32(233, 197, 135, 255);
    }

    private void ShowSummonButtonDisabled()
    {
        summonButton.image.color = new Color32(137, 125, 115, 255);
        summonButtonText.color = new Color32(159, 159, 159, 255);
    }

    private void ShowRedrawButtonEnabled()
    {
        redrawButton.image.color = new Color32(249, 247, 196, 255);
        redrawButtonText.color = new Color32(249, 247, 196, 255);
    }

    private void ShowRedrawButtonDisabled()
    {
        redrawButton.image.color = new Color32(174, 174, 174, 255);
        redrawButtonText.color = new Color32(209, 209, 209, 255);
    }
}
