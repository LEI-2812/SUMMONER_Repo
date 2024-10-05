using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage1_Controller : MonoBehaviour
{
    [Header("ĳ���� �̸� �ؽ�Ʈ")]
    public Text nameText; //��� ��� ĳ�����̸� �ؽ�Ʈ

    [Header("ĳ���� ��� �ؽ�Ʈ")]
    public Text dialogueText; //��� �ؽ�Ʈ

    [Header("���ΰ�")]
    public GameObject MainCharacter; //���ΰ� ĳ���� ������Ʈ

    private int scenarioFlow = 1; //��� ī��Ʈ

    
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
