using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
// 역할: 소환수 이미지와 스프라이트 표시를 담당한다.
public class SummonImageView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;

    private void Awake()
    {
        ImageEnsure();
    }

    public void ImageDataSet(Image sourceImage, Sprite[] sourceSprites)
    {
        if (image == null)
        {
            image = sourceImage;
        }

        if ((sprites == null || sprites.Length == 0) && sourceSprites != null)
        {
            sprites = sourceSprites;
        }

        ImageEnsure();
    }

    public void SpriteSet(int index)
    {
        ImageEnsure();

        if (sprites == null || index < 0 || index >= sprites.Length)
        {
            Debug.LogWarning("유효하지 않은 소환수 스프라이트 인덱스입니다.");
            return;
        }

        image.sprite = sprites[index];
    }

    public void ImageSet(Image image)
    {
        this.image = image;
        ImageEnsure();
    }

    public Image GetImage()
    {
        ImageEnsure();
        return image;
    }

    private void ImageEnsure()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }
}
