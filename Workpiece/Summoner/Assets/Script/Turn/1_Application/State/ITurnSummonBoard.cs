using System.Collections.Generic;

internal interface ITurnSummonBoard
{
    IEnumerable<Summon> GetPlayerSummons();
    IEnumerable<Summon> GetEnemySummons();
}
