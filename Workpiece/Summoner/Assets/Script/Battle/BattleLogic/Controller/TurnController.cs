using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    
    [SerializeField] private Player player;
    [SerializeField] private Enermy enermy;
    public enum Turn { PlayerTurn, EnermyTurn }
    private Turn currentTurn; // ���� ���� ��Ÿ���� ����
    private int turnCount;
    [SerializeField] private int clearTurn;

    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnClearText;

    StageController stageController;

    void Start()
    {
        stageController = FindAnyObjectByType<StageController>();

        currentTurn = Turn.PlayerTurn; // ù ��° ���� �÷��̾� ������ ����
        turnCount = 1;
        UpdateTurnCountUI();
        SetClearCountUI();
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

            // �¸� ����
            if (enermy.getEnermyAttackController().getPlateController().IsEnermyPlateClear() && (player.clearTurn >= player.currentTurn))
            {
                Debug.Log("�¸�!");
                player.battleAlert.clearAlert(stageController.stageNum);
            }


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

    private void SetClearCountUI()
    {
        turnClearText.text = $"Clear Turn : {clearTurn}";
    }

    public Turn getCurrentTurn()
    {
        return currentTurn;
    }

    public int GetTurnCount()
    {
        return turnCount;
    }

    public int GetClearTurn()
    {
        return clearTurn;
    }
} 
