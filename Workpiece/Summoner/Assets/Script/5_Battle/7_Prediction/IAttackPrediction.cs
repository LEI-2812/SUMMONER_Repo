using System.Collections.Generic;

// Role: exposes the prediction rule used by PlayerAttackPrediction.
public interface IAttackPrediction
{
    bool CanPredict(Summon summon);

    AttackPrediction GetAttackPrediction(
        Summon summon,
        int attackSummonPlateIndex,
        IReadOnlyList<Plate> playerPlates,
        IReadOnlyList<Plate> enermyPlates);
}
