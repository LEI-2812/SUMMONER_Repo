using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Player : Character
{
    public List<Plate> Playerplates; // 플레이어가 사용할 플레이트 목록
    public int Mana = 10; // 플레이어는 마나를 가지고 있음
    public TurnController turnController; // TurnController 참조

    //턴 시작시 마나를 확인함
    public override void startTurn()
    {
        base.startTurn();
        Debug.Log($"{gameObject.name} 의 마나: {Mana}");
        if (Mana > 0)
        {
            takeSummon();
        }
        else
        {
            takeAction();
            Debug.Log("마나가 부족하여 소환을 스킵합니다.");
        }
    }


    //플레이어의 활동 로직 구현하면됨
    public override void takeAction()
    {
        Debug.Log("플레이어 takeAction 실행");
    }

    public override void takeSummon()
    {
        Debug.Log("소환수 소환");
    }


    //플레이어는 버튼 클릭을 통해서만 턴종료를 시킨다.
    public void PlayerTurnOverBtn() //버튼에 넣을 메소드
    {
        // 플레이어 턴일 때만 턴 종료 가능
        if (turnController.currentTurn == TurnController.Turn.PlayerTurn)
        {
            EndTurn();
        }
        else
        {
            Debug.Log("플레이어 턴이 아닙니다.");
        }
    }

    public override void EndTurn()
    {
        base.EndTurn(); //그냥 턴 종료 디버그만 찍음.

        turnController.EndTurn();
    }
}
