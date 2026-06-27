using UnityEngine;
using UnityEngine.UI;

// 역할: PlateVisualView의 책임을 정의한다.
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
