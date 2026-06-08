using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public interface IAttackPrediction
{
    int GetIndexOfNormalAttackCanKill(Summon attackingSummon, List<Plate> enermyPlates);
    int GetClosestEnermyIndex(List<Plate> enermyPlates);

    public AttackPrediction GetAttackPrediction(Summon summon, int attackSummonPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates);

    SummonType GetPreSummonType();
}
