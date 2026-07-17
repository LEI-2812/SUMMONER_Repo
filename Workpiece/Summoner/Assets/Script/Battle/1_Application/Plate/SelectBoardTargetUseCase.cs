public enum BoardClickAction
{
    None,
    SelectRedrawPlate,
    SelectAttackTarget,
    InvalidAttackTarget,
    ShowSummonStatus
}

public readonly struct BoardTargetInput
{
    public readonly Summon CurrentSummon;
    public readonly bool IsSummonSelectionActive;
    public readonly bool IsAttackTargetSelectionActive;
    public readonly bool CanSelectAttackTarget;
    public readonly int AttackTargetPlateIndex;
    public readonly string AttackTargetPlateName;
    public readonly int PlayerPlateIndex;
    public readonly bool IsEnemyPlate;
    public readonly bool CanShowSummonStatus;

    public BoardTargetInput(
        Summon currentSummon,
        bool isSummonSelectionActive,
        bool isAttackTargetSelectionActive,
        bool canSelectAttackTarget,
        int attackTargetPlateIndex,
        string attackTargetPlateName,
        int playerPlateIndex,
        bool isEnemyPlate,
        bool canShowSummonStatus)
    {
        CurrentSummon = currentSummon;
        IsSummonSelectionActive = isSummonSelectionActive;
        IsAttackTargetSelectionActive = isAttackTargetSelectionActive;
        CanSelectAttackTarget = canSelectAttackTarget;
        AttackTargetPlateIndex = attackTargetPlateIndex;
        AttackTargetPlateName = attackTargetPlateName;
        PlayerPlateIndex = playerPlateIndex;
        IsEnemyPlate = isEnemyPlate;
        CanShowSummonStatus = canShowSummonStatus;
    }
}

// 역할: 보드 입력 상태를 보고 실행할 전투 행동을 결정한다.
public class SelectBoardTargetUseCase
{
    private readonly AttackStateMachine attackStateMachine;

    public SelectBoardTargetUseCase(AttackStateMachine attackStateMachine)
    {
        this.attackStateMachine = attackStateMachine;
    }

    public BoardClickAction Execute(BoardTargetInput input)
    {
        if (input.CurrentSummon != null && input.IsSummonSelectionActive)
        {
            return BoardClickAction.SelectRedrawPlate;
        }

        if (input.CurrentSummon != null && input.IsAttackTargetSelectionActive)
        {
            return SelectAttackTarget(input);
        }

        return SelectSummonStatus(input);
    }

    private BoardClickAction SelectAttackTarget(BoardTargetInput input)
    {
        if (!input.CanSelectAttackTarget || attackStateMachine == null)
        {
            return BoardClickAction.InvalidAttackTarget;
        }

        attackStateMachine.SelectTargetPlate(input.AttackTargetPlateIndex);
        return BoardClickAction.SelectAttackTarget;
    }

    private BoardClickAction SelectSummonStatus(BoardTargetInput input)
    {
        if (input.CurrentSummon == null
            || input.IsSummonSelectionActive
            || input.IsAttackTargetSelectionActive)
        {
            return BoardClickAction.None;
        }

        if (!input.CanShowSummonStatus)
        {
            attackStateMachine?.ClearPlayerAttackSource();
            return BoardClickAction.None;
        }

        if (attackStateMachine == null)
        {
            return BoardClickAction.None;
        }

        if (input.IsEnemyPlate)
        {
            attackStateMachine.ClearPlayerAttackSource();
        }
        else
        {
            attackStateMachine.SelectPlayerAttackSource(
                input.CurrentSummon,
                input.PlayerPlateIndex);
        }

        return BoardClickAction.ShowSummonStatus;
    }
}
