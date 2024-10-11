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
    public int turnCount;

    private BattleController battleController;
                            
    void Start()
    {
        battleController = GetComponent<BattleController>();
        currentTurn = Turn.PlayerTurn; // ù ��° ���� �÷��̾� ������ ����
        turnCount = 1;
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
            // �÷��̾� �� ���� ��, �����̻� �� ��Ÿ�� ������Ʈ
            foreach (var summon in battleController.getPlayerSummons())
            {
                summon.UpdateStatusEffectsAndCooldowns();
            }
            currentTurn = Turn.EnermyTurn;
            StartTurn();
        }
        else
        {
            // �� �� ���� ��, �����̻� �� ��Ÿ�� ������Ʈ
            foreach (var summon in battleController.getEnermySummons())
            {
                summon.UpdateStatusEffectsAndCooldowns();
            }
            currentTurn = Turn.PlayerTurn;
            turnCount++; // ���� ���� ������ �� �� ī��Ʈ�� ����
            Debug.Log("���� ��: " + turnCount);
            StartTurn();
        }
    }
} 
