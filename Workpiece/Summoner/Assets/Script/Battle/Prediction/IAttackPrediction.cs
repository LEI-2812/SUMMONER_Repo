using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public interface IAttackPrediction
{
    int getIndexOfNormalAttackCanKill(Summon attackingSummon, List<Plate> enermyPlates);
    int getClosestEnermyIndex(List<Plate> enermyPlates);

    public AttackPrediction getAttackPrediction(Summon summon, int attackSummonPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates);

    SummonType getPreSummonType();
}
