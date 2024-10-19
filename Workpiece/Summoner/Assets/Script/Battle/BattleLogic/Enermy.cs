using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    private List<Plate> enermyPlates; // 적이 사용할 플레이트 목록

    [Header("컨트롤러")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private EnermyAttackController enermyAttackController;

    private void Start()
    {
        enermyPlates = enermyAttackController.getPlateController().getEnermyPlates(); //적 플레이트를 가져온다.
    }

    public override void startTurn()
    {
        base.startTurn();
        // 적의 행동을 자동으로 결정함 (예: 플레이어를 공격)
        takeAction();
    }

    public override void takeAction() //여기에 AI로직 작성
    {
        for (int i = 0; i < enermyPlates.Count; i++) //몬스터 순서대로 공격
        {
            if (enermyPlates[i].getCurrentSummon() != null && !enermyPlates[i].getCurrentSummon().IsCursed())
            {
                enermyAttackController.EnermyAttackStart(enermyPlates[i].getCurrentSummon());
            }
        }



        EndTurn();
    }

    public override void EndTurn()
    {
        Debug.Log("적 턴 종료");
        turnController.EndTurn();
    }


    public EnermyAttackController getEnermyAttackController()
    {
        return enermyAttackController;
    }
}
