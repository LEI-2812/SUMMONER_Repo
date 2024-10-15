using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    [Header("적 플레이트")]
    [SerializeField] private List<Plate> enermyPlates; // 적이 사용할 플레이트 목록

    [Header("컨트롤러")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private EnermyAttackController enermyAttackController;

    public override void startTurn()
    {
        base.startTurn();
        // 적의 행동을 자동으로 결정함 (예: 플레이어를 공격)
        takeAction();
    }

    public override void takeAction() //여기에 AI로직 작성
    {
        for (int i = 0; i < enermyPlates.Count; i++)
        {
            if (enermyPlates[i].getSummon() != null && !enermyPlates[i].getSummon().IsCursed())
            {
                enermyAttackController.EnermyAttackStart(enermyPlates[i].getSummon());
            }
        }



        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("적 턴 종료");
        turnController.EndTurn();
    }
}
