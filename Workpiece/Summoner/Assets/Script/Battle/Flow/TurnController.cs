using TMPro;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    
    [SerializeField] private Player player;
    [SerializeField] private Enermy enermy;
    [SerializeField] private BattleResultController battleResultController;
    public enum Turn { PlayerTurn, EnermyTurn }
    private Turn currentTurn; // 현재 턴을 나타내는 변수
    private int turnCount;
    [SerializeField] private int clearTurn;

    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnClearText;

    

    public void TurnStartInitialize()
    {
        EnsureBattleResultController();

        currentTurn = Turn.PlayerTurn; // 첫 번째 턴은 플레이어 턴으로 시작
        turnCount = 1;
        TurnCountTextUpdate();
        ClearTurnTextSet();
        StartTurn();
    }

    public void StartTurn() //해당 플레이어의 턴 시작
    {
        if (currentTurn == Turn.PlayerTurn)  // 플레이어 턴일 경우
        {
            PlayerTurnStart();
            return;
        }

        if (currentTurn == Turn.EnermyTurn)  // 적의 턴일 경우
        {
            EnermyTurnStart();
        }
    }

    public void EndTurn()
    {
        if (currentTurn == Turn.PlayerTurn)
        {
            PlayerTurnEnd();
            return;
        }

        if (currentTurn == Turn.EnermyTurn)
        {
            EnermyTurnEnd();
        }
    }

    private void PlayerTurnStart()
    {
        EnemySummonTurnStartEffectsApply();
        GetEnermyPlateController().CompactEnermyPlates();
        BattleClearResultTry();
        PlayerSummonSpecialCooldownsUpdate();
        player.PlayerTurnStart();
    }

    private void EnermyTurnStart()
    {
        PlayerSummonTurnStartEffectsApply();
        EnemySummonSpecialCooldownsUpdate();
        enermy.EnermyTurnStart();
    }

    private void PlayerTurnEnd()
    {
        // 플레이어 턴이 끝나면 턴 카운트를 증가시키지 않고 바로 적 턴 시작
        currentTurn = Turn.EnermyTurn;
        PlayerSummonUpgradeStatusUpdate();
        StartTurn();
    }

    private void EnermyTurnEnd()
    {
        currentTurn = Turn.PlayerTurn;
        turnCount++;
        TurnCountTextUpdate();
        player.AddMana();
        EnemySummonUpgradeStatusUpdate();
        PlayerSummonAttackReadyReset();
        Debug.Log("현재 턴: " + turnCount);
        StartTurn();
    }

    private void EnemySummonTurnStartEffectsApply()
    {
        foreach (var summon in GetEnermyPlateController().GetEnermySummons())
        {
            SummonTurnStartEffectsApply(summon);
        }
    }

    private void PlayerSummonTurnStartEffectsApply()
    {
        foreach (var summon in player.GetPlateController().GetPlayerSummons())
        {
            SummonTurnStartEffectsApply(summon);
        }
    }

    private void SummonTurnStartEffectsApply(Summon summon)
    {
        summon.UpdateDamageStatusEffects();
        summon.UpdateStunAndCurseStatus();
        summon.GetAttackStrategy().ReduceCooldown();
    }

    private void PlayerSummonSpecialCooldownsUpdate()
    {
        foreach (var summon in player.GetPlateController().GetPlayerSummons())
        {
            summon.UpdateSpecialAttackCooldowns();
        }
    }

    private void EnemySummonSpecialCooldownsUpdate()
    {
        foreach (var summon in GetEnermyPlateController().GetEnermySummons())
        {
            summon.UpdateSpecialAttackCooldowns();
        }
    }

    private void PlayerSummonUpgradeStatusUpdate()
    {
        foreach (var summon in player.GetPlateController().GetPlayerSummons())
        {
            summon.UpdateUpgradeStatus();
        }
    }

    private void EnemySummonUpgradeStatusUpdate()
    {
        foreach (var summon in GetEnermyPlateController().GetEnermySummons())
        {
            summon.UpdateUpgradeStatus();
        }
    }

    private void PlayerSummonAttackReadyReset()
    {
        foreach (var summon in player.GetPlateController().GetPlayerSummons())
        {
            if (!summon.IsStun())
            {
                summon.SetIsAttack(true);
            }
        }
    }

    private void BattleClearResultTry()
    {
        battleResultController.ClearResultTry(
            GetEnermyPlateController().IsEnermyPlateClear(),
            player.clearTurn,
            player.currentTurn);
    }

    private PlateController GetEnermyPlateController()
    {
        return enermy.GetEnermyAttackController().GetPlateController();
    }

    private void TurnCountTextUpdate()
    {
        turnCountText.text = $"Current Turn : {turnCount}";
    }

    private void ClearTurnTextSet()
    {
        turnClearText.text = $"Clear Turn : {clearTurn}";
    }

    public Turn GetCurrentTurn()
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
    private void EnsureBattleResultController()
    {
        if (battleResultController != null)
        {
            return;
        }

        if (player != null)
        {
            battleResultController = player.GetComponent<BattleResultController>();
            if (battleResultController == null)
            {
                battleResultController = player.gameObject.AddComponent<BattleResultController>();
            }
        }

        if (battleResultController == null)
        {
            battleResultController = FindObjectOfType<BattleResultController>();
        }
    }
} 
