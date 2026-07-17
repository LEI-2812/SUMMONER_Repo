using System.Collections.Generic;

interface ITurnSummonBoard
{
    IEnumerable<Summon> GetPlayerSummons();
    IEnumerable<Summon> GetEnemySummons();
}
