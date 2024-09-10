using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enermy : Character
{
    public List<Plate> Enermyplates; // 적이 사용할 플레이트 목록

    private void Start()
    {
        InitializeSummons(); //소환수 소환
    }

    private void InitializeSummons() //게임 시작시 소환수가 미리 생성되도록
    {
        Debug.Log("적 소환수 미리 소환");
    }
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
        base.takeAction(); // 턴 종료 처리
        EndTurn();
    }

    public override void EndTurn()
    {
        base.EndTurn();
        // 턴 종료 후 TurnController에서 플레이어의 턴으로 넘어가게 함
        TurnController turnController = FindObjectOfType<TurnController>();
        turnController.EndTurn();
    }
}
