using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public Player player;
    public Enermy enermy;
    public enum Turn { PlayerTurn, EnermyTurn }
    public Turn currentTurn; // 현재 턴을 나타내는 변수
    public int turnCount;
                            
    void Start()
    {
        currentTurn = Turn.PlayerTurn; // 첫 번째 턴은 플레이어 턴으로 시작
        turnCount = 1;
        StartTurn();
    }

    public void StartTurn() //해당 플레이어의 턴 시작
    {
        if (currentTurn == Turn.PlayerTurn)  // 플레이어 턴일 경우
        {
            player.startTurn();
        }
        else if (currentTurn == Turn.EnermyTurn)  // 적의 턴일 경우
        {
            enermy.startTurn();
        }
    }

    public void EndTurn()
    {
        if (currentTurn == Turn.PlayerTurn)
        {
            currentTurn = Turn.EnermyTurn;
            StartTurn();
        }
        else
        {
            currentTurn = Turn.PlayerTurn;
            turnCount++; // 적의 턴이 끝났을 때 턴 카운트를 증가
            Debug.Log("현재 턴: " + turnCount);
            StartTurn();
        }
    }
} 
