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

    public void ShowImage(int number)
    {
        Debug.Log(number);
        targetImage.sprite = spriteArray[number];
    }
}