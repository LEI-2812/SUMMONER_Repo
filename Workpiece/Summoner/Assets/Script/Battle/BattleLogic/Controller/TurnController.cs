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
            // �� ��ȯ�� ���� ������Ʈ �� ������ ó��
            var enermyPlates = enermy.getEnermyAttackController().getPlateController().getEnermySummons();

            // ���� ������Ʈ�� ���� ����
            foreach (var summon in enermyPlates)
            {
                summon.UpdateDamageStatusEffects(); // �������� �ִ� �����̻� ������Ʈ
                summon.UpdateStunAndCurseStatus();  // ���� �� ���� ���� ������Ʈ
                summon.getAttackStrategy().ReduceCooldown(); // �Ϲ� ���� ��Ÿ�� ����
            }

            player.getPlateController().CompactEnermyPlates(); //�մ���


            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateSpecialAttackCooldowns(); // Ư�� ���� ��Ÿ�� ������Ʈ
            }

            player.startTurn();
        }
        else if (currentTurn == Turn.EnermyTurn)  // ���� ���� ���
        {
            // �� �� ���۽� �Ʊ� ��ȯ�� �����̻� �� ��Ÿ�� ������Ʈ
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateDamageStatusEffects(); // �������� �ִ� �����̻� ������Ʈ
                summon.UpdateStunAndCurseStatus(); // ���� �� ���� ���� ������Ʈ
                summon.getAttackStrategy().ReduceCooldown(); // �Ϲ� ���� ��Ÿ�� ����
            }

            foreach (var summon in enermy.getEnermyAttackController().getPlateController().getEnermySummons())
            {
                summon.UpdateSpecialAttackCooldowns(); // Ư�� ���� ��Ÿ�� ������Ʈ
            }

            enermy.startTurn();
        }
    }

    public void EndTurn()
    {
        if (currentTurn == Turn.PlayerTurn)
        {
            // ���� ���� ���� ������ ����
            currentTurn = Turn.EnermyTurn;

            // �� �� ���۽� �Ʊ� ��ȯ�� �����̻� �� ��Ÿ�� ������Ʈ
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateUpgradeStatus(); //��ȭ ���� ������Ʈ
            }

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

            //�� �� ������ �� �÷���Ʈ�� ��ȭ�� ǰ
            foreach (var summon in enermy.getEnermyAttackController().getPlateController().getEnermySummons())
            {
                summon.UpdateUpgradeStatus(); //��ȭ ���� ������Ʈ
            }

            // �÷��̾� ���� ���� �� ȥ�� ���°� �ƴ� ��ȯ������ ���� ���� ���θ� ����
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                if (!summon.IsStun()) // ���� ���°� �ƴ� ���
                {
                    summon.setIsAttack(true); // ���� �����ϰ� ����
                }
            }

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
