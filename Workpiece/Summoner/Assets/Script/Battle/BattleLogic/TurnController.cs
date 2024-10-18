using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    
    [SerializeField]private Player player;
    [SerializeField] private Enermy enermy;
    public enum Turn { PlayerTurn, EnermyTurn }
    private Turn currentTurn; // ���� ���� ��Ÿ���� ����
    private int turnCount;

    [SerializeField] private TextMeshProUGUI turnCountText;
                            
    void Start()
    {
        currentTurn = Turn.PlayerTurn; // ù ��° ���� �÷��̾� ������ ����
        turnCount = 1;
        UpdateTurnCountUI();
        StartTurn();
    }

    public void StartTurn() //�ش� �÷��̾��� �� ����
    {
        if (currentTurn == Turn.PlayerTurn)  // �÷��̾� ���� ���
        {
            player.startTurn();

            foreach (var summon in enermy.getEnermyAttackController().getPlateController().getEnermySummons()) //�÷��̾� �� ���۽� �� �÷���Ʈ�� �����̻� ������ ����
            {
                summon.UpdateStatusEffectsAndCooldowns(); // �����̻� ������Ʈ
                summon.getAttackStrategy().ReduceCooldown(); // �Ϲ� ���� ��Ÿ�� ����
            }
        }
        else if (currentTurn == Turn.EnermyTurn)  // ���� ���� ���
        {
            enermy.startTurn();

            // �� �� ���� ��, �� ��ȯ���� �����̻� �� ��Ÿ�� ������Ʈ
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateStatusEffectsAndCooldowns(); // �����̻� ������Ʈ
                summon.getAttackStrategy().ReduceCooldown(); // �Ϲ� ���� ��Ÿ�� ����
            }
        }
    }

    public void EndTurn()
    {
        if (currentTurn == Turn.PlayerTurn)
        {
            // ���� ���� ���� ������ ����
            currentTurn = Turn.EnermyTurn;

            // �÷��̾� ���� ������ �� ī��Ʈ�� ������Ű�� �ʰ� �ٷ� �� �� ����
            StartTurn();
        }
        else if (currentTurn == Turn.EnermyTurn)
        {

            // �� ���� ���� �� �� ī��Ʈ�� ������Ű�� �÷��̾� �� ����
            currentTurn = Turn.PlayerTurn;
            turnCount++; // �� ���� ������ �� ī��Ʈ�� ������Ŵ
            UpdateTurnCountUI();
            player.AddMana();
            Debug.Log("���� ��: " + turnCount);

            StartTurn();
        }
    }
    private void UpdateTurnCountUI()
    {
        turnCountText.text = $"Current Turn : {turnCount}";
    }

    public Turn getCurrentTurn()
    {
        return currentTurn;
    }
} 
