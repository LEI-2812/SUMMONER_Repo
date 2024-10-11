using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
   
    public virtual void startTurn()
    {
        Debug.Log($"{gameObject.name}의 턴 시작");
        //턴 시작시 할 행동 추가 가능
    }

    public virtual void takeAction()
    {
        //해당 플레이어가 취할 액션 시작
    }

    public virtual void EndTurn()
    {
        Debug.Log($"{gameObject.name}의 턴 종료");
    }

}
