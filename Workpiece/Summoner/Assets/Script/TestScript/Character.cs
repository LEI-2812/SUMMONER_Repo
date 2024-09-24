using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
   
    public virtual void startTurn()
    {
        Debug.Log($"{gameObject.name}�� �� ����");
        //�� ���۽� �� �ൿ �߰� ����
    }

    public virtual void takeAction()
    {
        //�ش� �÷��̾ ���� �׼� ����
    }

    public virtual void EndTurn()
    {
        Debug.Log($"{gameObject.name}�� �� ����");
    }

}
