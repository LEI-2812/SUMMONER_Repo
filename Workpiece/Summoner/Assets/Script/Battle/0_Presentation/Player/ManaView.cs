using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: ManaView의 책임을 정의한다.
public sealed class ManaView
{
    private readonly List<RawImage> manaImages;
    private readonly Texture emptyManaTexture;
    private readonly Texture filledManaTexture;

    public ManaView(
        List<RawImage> manaImages,
        Texture emptyManaTexture,
        Texture filledManaTexture)
    {
        this.manaImages = manaImages;
        this.emptyManaTexture = emptyManaTexture;
        this.filledManaTexture = filledManaTexture;
    }

    public void ShowMana(int currentMana)
    {
        for (int i = 0; i < manaImages.Count; i++)
        {
            manaImages[i].texture = i < currentMana ? filledManaTexture : emptyManaTexture;
        }
    }
}
