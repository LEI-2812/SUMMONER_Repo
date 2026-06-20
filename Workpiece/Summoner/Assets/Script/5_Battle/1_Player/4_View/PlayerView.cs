using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 역할: 플레이어 마나, 버튼, 상태 패널 표시만 담당한다.
public class PlayerView
{
    private readonly List<RawImage> manaList;
    private readonly Texture notHaveTexture;
    private readonly Texture haveTexture;
    private readonly Button summonButton;
    private readonly TextMeshProUGUI summonButtonText;
    private readonly Button reSummonButton;
    private readonly TextMeshProUGUI reSummonButtonText;
    private readonly Image statePanel;

    public PlayerView(
        List<RawImage> manaList,
        Texture notHaveTexture,
        Texture haveTexture,
        Button summonButton,
        Button reSummonButton,
        Image statePanel)
    {
        this.manaList = manaList;
        this.notHaveTexture = notHaveTexture;
        this.haveTexture = haveTexture;
        this.summonButton = summonButton;
        this.reSummonButton = reSummonButton;
        this.statePanel = statePanel;
        summonButtonText = summonButton.GetComponentInChildren<TextMeshProUGUI>();
        reSummonButtonText = reSummonButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateMana(int mana, bool canSummon)
    {
        for (int i = 0; i < manaList.Count; i++)
        {
            manaList[i].texture = i < mana ? haveTexture : notHaveTexture;
        }

        if (canSummon)
        {
            ShowSummonButtonEnabled();
        }
    }

    public void ShowSummonButtonDisabled()
    {
        summonButton.image.color = new Color32(137, 125, 115, 255);
        summonButtonText.color = new Color32(159, 159, 159, 255);
    }

    public void ShowRedrawButtonEnabled()
    {
        reSummonButton.image.color = new Color32(249, 247, 196, 255);
        reSummonButtonText.color = new Color32(249, 247, 196, 255);
    }

    public void ShowRedrawButtonDisabled()
    {
        reSummonButton.image.color = new Color32(174, 174, 174, 255);
        reSummonButtonText.color = new Color32(209, 209, 209, 255);
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
}
