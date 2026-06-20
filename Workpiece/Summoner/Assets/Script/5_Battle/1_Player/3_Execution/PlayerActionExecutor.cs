using System.Collections.Generic;

// 역할: 플레이어 행동에서 확정된 소환, 재소환 비용, 공격 실행, 공격 후처리를 실제로 수행한다.
public class PlayerActionExecutor
{
    public void StartSummon(
        SummonController summonController,
        PlayerTurnResourceState resourceState,
        PlayerView playerView,
        PlayerFeedbackView feedbackView,
        int plateIndex)
    {
        feedbackView.Log(plateIndex + "번째 플레이트에 소환 예정");
        summonController.StartSummon(plateIndex, false);
        resourceState.UseSummonMana();
        playerView.UpdateMana(resourceState.Mana, resourceState.CanShowSummonAvailable());
        feedbackView.PlayClick();
        playerView.ShowSummonButtonDisabled();
    }

    public void UseRedrawMana(
        PlayerTurnResourceState resourceState,
        PlayerView playerView,
        PlayerFeedbackView feedbackView)
    {
        resourceState.UseRedrawMana();
        playerView.UpdateMana(resourceState.Mana, resourceState.CanShowSummonAvailable());
        feedbackView.PlayClick();
    }

    public void ExecuteNormalAttack(
        Summon attackSummon,
        List<Plate> enemyPlates,
        int selectedPlateIndex,
        PlayerFeedbackView feedbackView)
    {
        attackSummon.NormalAttack(enemyPlates, selectedPlateIndex);
        feedbackView.PlayClick();
    }

    public bool ExecuteImmediateSpecialAttack(
        BattleController battleController,
        Summon attackSummon,
        int selectedPlateIndex,
        PlayerFeedbackView feedbackView)
    {
        if (!battleController.SpecialAttackExecute(attackSummon, selectedPlateIndex, 0, true))
        {
            return false;
        }

        feedbackView.PlayClick();
        return true;
    }

    public void ProcessAfterPlayerAttack(
        PlateController plateController,
        PlayerView playerView)
    {
        plateController.CompactEnermyPlates();
        playerView.HideStatePanel();
    }
}
