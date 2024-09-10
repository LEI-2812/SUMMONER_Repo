using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public Player player;
    public Enermy enermy;
    public enum Turn { PlayerTurn, EnermyTurn }
    public Turn currentTurn; // ���� ���� ��Ÿ���� ����
                            
    void Start()
    {
        currentTurn = Turn.PlayerTurn; // ù ��° ���� �÷��̾� ������ ����
        StartTurn();
    }

    public void StartTurn() //�ش� �÷��̾��� �� ����
    {
        if (currentTurn == Turn.PlayerTurn)  // �÷��̾� ���� ���
        {
            player.startTurn();
        }
        else if (currentTurn == Turn.EnermyTurn)  // ���� ���� ���
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
            StartTurn();
        }
    }
} 
