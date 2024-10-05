using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    [Header("ID ������ ���缭 �̹��� �ֱ�")]
    [SerializeField] private Sprite[] spriteArray; //��������Ʈ �迭

    [Header("���� Sprite�� ���� Image������Ʈ")]
    [SerializeField] private Image targetImage;

    public void ShowImage(int number)
    {
        Debug.Log(number);
        targetImage.sprite = spriteArray[number];
    }
}