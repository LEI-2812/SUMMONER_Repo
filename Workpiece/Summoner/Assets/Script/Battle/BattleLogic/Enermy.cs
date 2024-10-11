using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    [Header("적 플레이트")]
    [SerializeField] private List<Plate> enermyPlates; // 적이 사용할 플레이트 목록

    [Header("턴 컨트롤러")]
    [SerializeField] private TurnController turnController;

    public override void startTurn()
    {
        base.startTurn();
        // 적의 행동을 자동으로 결정함 (예: 플레이어를 공격)
        takeAction();
    }

    public override void takeAction() //여기에 AI로직 작성
    {
        // 적의 행동을 정의 (간단한 공격 행동)
        Debug.Log($"{gameObject.name} 공격!");
        
        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("적 턴 종료");
        turnController.EndTurn();
    }
}
