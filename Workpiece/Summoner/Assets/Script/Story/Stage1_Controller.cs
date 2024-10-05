using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour
{
    [Header("캐릭터 이름 텍스트")]
    public Text nameText; //대사 출력 캐릭터이름 텍스트

    [Header("캐릭터 대사 텍스트")]
    public Text dialogueText; //대사 텍스트

    [Header("주인공")]
    public GameObject MainCharacter; //주인공 캐릭터 오브젝트

    private int scenarioFlow = 1; //대사 카운트

    
    [SerializeField]private Animator playerAni;

    private void Start()
    {
        stage_1_Flow();
    }

    public void stage_1_Flow()
    {
        switch (scenarioFlow)
        {
            case 1:
                playerAni.Play("Stage1_Scenario1");
                break;
            case 2:

            case 3:

            case 4:
            
            case 5:

            case 6:

            break;
        }
    }
}
