using UnityEngine;
using UnityEngine.UI;

// 역할: 개별 플레이트의 강조 색상과 소환수 이미지 투명도를 표시한다.
public class PlateVisualView
{
    private readonly Image plateImage;
    private readonly Image summonImage;
    private readonly Color originalColor;

    public PlateVisualView(Image plateImage, Image summonImage)
    {
        this.plateImage = plateImage;
        this.summonImage = summonImage;
        originalColor = plateImage == null ? Color.white : plateImage.color;
    }

    public void ShowHighlight()
    {
        if (plateImage != null)
        {
            plateImage.color = Color.yellow;
        }
    }

    public void HideHighlight()
    {
        if (plateImage != null)
        {
            plateImage.color = originalColor;
        }
    }

    public void SetSummonImageTransparency(float alpha)
    {
        if (summonImage == null)
        {
            return;
        }

        Color color = summonImage.color;
        color.a = alpha;
        summonImage.color = color;
    }
}
