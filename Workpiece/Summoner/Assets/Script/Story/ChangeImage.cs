using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    [SerializeField]
    public Sprite[] spriteArray;
    public Image targetImage;

    public void ShowImage(int number)
    {
        Debug.Log(number);
        targetImage.sprite = spriteArray[number];
    }
}