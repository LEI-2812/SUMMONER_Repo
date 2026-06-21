using System.Collections.Generic;
using UnityEngine;

// 역할: 소환 후보 패널과 어두운 배경 표시를 담당한다.
public class SummonPickView
{
    private readonly GameObject darkBackground;
    private readonly GameObject redrawPanel;
    private readonly List<DrawOptionPanelView> redrawOptionPanels;

    public SummonPickView(
        GameObject darkBackground,
        GameObject redrawPanel,
        List<DrawOptionPanelView> redrawOptionPanels)
    {
        this.darkBackground = darkBackground;
        this.redrawPanel = redrawPanel;
        this.redrawOptionPanels = redrawOptionPanels;
    }

    public void SetDarkBackground(bool active)
    {
        if (darkBackground != null)
        {
            darkBackground.SetActive(active);
        }
    }

    public bool IsDarkBackgroundActive()
    {
        return darkBackground != null && darkBackground.activeSelf;
    }

    public void ShowDrawOptions(List<Summon> drawOptions)
    {
        ShowPanel(redrawPanel);
        SetOptionPanels(redrawOptionPanels, drawOptions);
    }

    public void ShowRedrawOptions(List<Summon> drawOptions)
    {
        ShowPanel(redrawPanel);
        SetOptionPanels(redrawOptionPanels, drawOptions);
    }

    public void HideOptionPanels()
    {
        HidePanel(redrawPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    private void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void SetOptionPanels(
        List<DrawOptionPanelView> optionPanels,
        List<Summon> drawOptions)
    {
        if (optionPanels == null)
        {
            return;
        }

        ClearOptionPanels(optionPanels);

        if (drawOptions == null)
        {
            return;
        }

        for (int i = 0; i < optionPanels.Count && i < drawOptions.Count; i++)
        {
            Summon summon = drawOptions[i];
            optionPanels[i].SetAssignedSummon(summon);

            if (summon.GetImage() != null && summon.GetImage().sprite != null)
            {
                optionPanels[i].SetSummonImage(summon.GetImage());
            }
        }
    }

    private void ClearOptionPanels(List<DrawOptionPanelView> optionPanels)
    {
        foreach (DrawOptionPanelView optionPanel in optionPanels)
        {
            if (optionPanel != null)
            {
                optionPanel.ClearAssignedSummon();
            }
        }
    }
}
