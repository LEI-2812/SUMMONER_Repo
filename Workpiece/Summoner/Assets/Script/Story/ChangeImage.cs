using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    [Header("ID 순서에 맞춰서 이미지 넣기")]
    [SerializeField] private Sprite[] spriteArray; //스프라이트 배열

    [Header("받은 Sprite를 넣을 Image오브젝트")]
    [SerializeField] private Image targetImage;

    [Header("Interaction Controller 참조")]
    [SerializeField] private InteractionController interactionController; // InteractionController 참조

    private void Update()
    {
        ShowImage();
    }

    public void ShowImage()
    {
        int currentDialogueIndex = interactionController.getCurrentDialogueIndex(); // currentDialogueIndex 가져오기
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