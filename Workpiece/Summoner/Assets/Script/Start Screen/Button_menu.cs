using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Button_menu : MonoBehaviour
{
    TextMeshProUGUI t;
    public void Point_enter()
    {
        TextMeshProUGUI t = GetComponent<TextMeshProUGUI>();
        t.text = "> 처음부터 <";
        Debug.Log("올라옴");
    }
    public void Point_exit()
    {
        Debug.Log("내려감");
    }
}


// public Button b;
//public TextMeshProUGUI t;
/*private void Update()
{

    if (Input.GetMouseButtonDown(0))
    {
        //t.color = new Color32(255, 255, 255, 255);
    }
    */