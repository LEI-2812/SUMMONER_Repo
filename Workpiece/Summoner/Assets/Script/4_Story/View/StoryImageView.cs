using UnityEngine;
using UnityEngine.UI;

// 역할: 스토리 장면의 이미지 표시를 담당한다.
public class StoryImageView : MonoBehaviour
{
    [Header("ID 순서에 맞춰서 이미지 넣기")]
    [SerializeField] private Sprite[] spriteArray; //스프라이트 배열

    [Header("받은 Sprite를 넣을 Image오브젝트")]
    [SerializeField] private Image targetImage;

    public void ShowImage(int currentDialogueIndex)
    {
        Debug.Log(currentDialogueIndex);
        
        if (currentDialogueIndex >= 0 && currentDialogueIndex < spriteArray.Length)
        {
            targetImage.sprite = spriteArray[currentDialogueIndex];
        }
        else
        {
            Debug.LogWarning("유효하지 않은 인덱스입니다.");
        }
    }
}
