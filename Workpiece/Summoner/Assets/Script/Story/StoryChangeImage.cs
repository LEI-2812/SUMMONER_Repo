using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryChangeImage : MonoBehaviour
{
    [Header("ID ������ ���缭 �̹��� �ֱ�")]
    [SerializeField] private Sprite[] spriteArray; //��������Ʈ �迭

    [Header("���� Sprite�� ���� Image������Ʈ")]
    [SerializeField] private Image targetImage;

    private InteractionController interactionController; // InteractionController ����

    private void Awake()
    {
        interactionController = GetComponent<InteractionController>();
    }

    public void ShowImage()
    {
        int currentDialogueIndex = interactionController.getCurrentDialogueIndex(); // currentDialogueIndex ��������
        Debug.Log(currentDialogueIndex);
        
        if (currentDialogueIndex >= 0 && currentDialogueIndex < spriteArray.Length)
        {
            targetImage.sprite = spriteArray[currentDialogueIndex];
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� �ε����Դϴ�.");
        }
    }
}