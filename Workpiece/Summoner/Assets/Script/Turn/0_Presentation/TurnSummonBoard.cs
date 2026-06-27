using System.Collections.Generic;

internal sealed class TurnSummonBoard : ITurnSummonBoard
{
    private readonly PlateBoardView plateBoardController;

    public TurnSummonBoard(PlateBoardView plateBoardController)
    {
        this.plateBoardController = plateBoardController;
    }

    public IEnumerable<Summon> GetPlayerSummons()
    {
        return GetSummons(plateBoardController.GetPlayerPlates());
    }

    public IEnumerable<Summon> GetEnemySummons()
    {
        return GetSummons(plateBoardController.GetEnemyPlates());
    }

    private IEnumerable<Summon> GetSummons(IReadOnlyList<BattleBoardInputController> plates)
    {
        foreach (BattleBoardInputController plate in plates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon == null)
            {
                continue;
            }

            yield return summon;
        }
    }
}
