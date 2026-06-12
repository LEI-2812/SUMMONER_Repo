using System.Collections.Generic;

interface IAttackTargetSelector
{
    List<Summon> SelectTargets(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex);
}

class ClosestEnemyAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null)
        {
            return targets;
        }

        for (int i = 0; i < targetPlates.Count; i++)
        {
            if (targetPlates[i] == null)
            {
                continue;
            }

            Summon enemySummon = targetPlates[i].GetCurrentSummon();
            if (enemySummon != null)
            {
                targets.Add(enemySummon);
                return targets;
            }
        }

        return targets;
    }
}

class SelectedPlateAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null || selectedPlateIndex < 0 || selectedPlateIndex >= targetPlates.Count)
        {
            return targets;
        }

        Plate targetPlate = targetPlates[selectedPlateIndex];
        if (targetPlate == null)
        {
            return targets;
        }

        Summon target = targetPlate.GetCurrentSummon();
        if (target != null)
        {
            targets.Add(target);
        }

        return targets;
    }
}

class AllEnemiesAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (targetPlates == null)
        {
            return targets;
        }

        foreach (Plate plate in targetPlates)
        {
            if (plate == null)
            {
                continue;
            }

            Summon target = plate.GetCurrentSummon();
            if (target != null)
            {
                targets.Add(target);
            }
        }

        return targets;
    }
}

class SelfAttackTargetSelector : IAttackTargetSelector
{
    public List<Summon> SelectTargets(Summon attacker, List<Plate> targetPlates, int selectedPlateIndex)
    {
        List<Summon> targets = new List<Summon>();

        if (attacker != null)
        {
            targets.Add(attacker);
        }

        return targets;
    }
}
