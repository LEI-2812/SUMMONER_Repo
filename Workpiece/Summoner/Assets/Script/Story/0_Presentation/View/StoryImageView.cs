using UnityEngine;
using UnityEngine.UI;

// 역할: StoryImageView의 책임을 정의한다.
public class StoryImageView : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Sprite[] spriteArray;

    [Header("참조")]
    [SerializeField] private Image targetImage;

    public void ShowImage(int currentDialogueIndex)
    {
        if (currentDialogueIndex >= 0 && currentDialogueIndex < spriteArray.Length)
        {
            targetImage.sprite = spriteArray[currentDialogueIndex];
        }
        else
        {
            Debug.LogWarning("유효하지 않은 이미지 인덱스입니다.");
        }
    }
}
