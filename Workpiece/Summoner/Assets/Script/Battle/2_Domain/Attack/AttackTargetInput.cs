using System.Collections.Generic;

public class AttackTargetInput
{
    public AttackTargetInput(
        Summon attacker,
        BattleBoardData board,
        int selectedPlateIndex,
        bool isPlayerAttack)
    {
        Attacker = attacker;
        Board = board;
        SelectedPlateIndex = selectedPlateIndex;
        IsPlayerAttack = isPlayerAttack;
    }

    public Summon Attacker { get; }
    public BattleBoardData Board { get; }
    public int SelectedPlateIndex { get; }
    public bool IsPlayerAttack { get; }

    public IReadOnlyList<PlateData> GetTargetPlates(bool targetsOwnPlates)
    {
        return Board == null
            ? null
            : Board.GetTargetPlates(IsPlayerAttack, targetsOwnPlates);
    }
}
