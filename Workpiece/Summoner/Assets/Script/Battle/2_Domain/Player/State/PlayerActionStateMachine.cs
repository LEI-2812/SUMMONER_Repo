using UnityEngine;

public enum PlayerActionState
{
    Idle,
    Summoning,
    Redrawing,
    NormalAttacking,
    SpecialAttacking,
    SelectingTarget,
    EndingTurn
}

public enum PlayerActionResult
{
    Failed,
    Completed,
    WaitingForTarget
}

// 역할: PlayerActionStateMachine의 책임을 정의한다.
public sealed class PlayerActionStateMachine
{
    public PlayerActionState CurrentState { get; private set; }
    public int Mana { get; private set; }
    public int RedrawCost { get; private set; }
    public bool HasSummonedThisTurn { get; private set; }

    public void Reset()
    {
        CurrentState = PlayerActionState.Idle;
        Mana = 10;
        RedrawCost = 1;
        HasSummonedThisTurn = false;
    }

    public void StartTurn()
    {
        CurrentState = PlayerActionState.Idle;
        HasSummonedThisTurn = false;
    }

    public bool TryStartSummon()
    {
        if (!IsIdle() || !CanUseSummon())
        {
            return false;
        }

        CurrentState = PlayerActionState.Summoning;
        return true;
    }

    public bool TryStartRedraw()
    {
        if (!IsIdle() || !CanRedraw())
        {
            return false;
        }

        CurrentState = PlayerActionState.Redrawing;
        return true;
    }

    public bool TryStartNormalAttack()
    {
        if (!IsIdle())
        {
            return false;
        }

        CurrentState = PlayerActionState.NormalAttacking;
        return true;
    }

    public bool TryStartSpecialAttack()
    {
        if (!IsIdle())
        {
            return false;
        }

        CurrentState = PlayerActionState.SpecialAttacking;
        return true;
    }

    public bool TryStartEndTurn()
    {
        if (!IsIdle())
        {
            return false;
        }

        CurrentState = PlayerActionState.EndingTurn;
        return true;
    }

    public bool CanEndTurn()
    {
        return IsIdle();
    }

    public void WaitTargetSelection()
    {
        CurrentState = PlayerActionState.SelectingTarget;
    }

    public void CompleteAction()
    {
        CurrentState = PlayerActionState.Idle;
    }

    public bool CanUseSummon()
    {
        return Mana > 0 && !HasSummonedThisTurn;
    }

    public bool CanShowSummonAvailable()
    {
        return CanUseSummon();
    }

    public void UseSummonMana()
    {
        Mana -= 1;
        HasSummonedThisTurn = true;
    }

    public bool CanRedraw()
    {
        return Mana >= RedrawCost;
    }

    public void UseRedrawMana()
    {
        Mana -= RedrawCost;
        RedrawCost += 1;
    }

    public void AddMana(int maxMana)
    {
        Mana = Mathf.Min(Mana + 1, maxMana);
    }

    public void SetHasSummonedThisTurn(bool value)
    {
        HasSummonedThisTurn = value;
    }

    public bool IsActing()
    {
        return !IsIdle();
    }

    private bool IsIdle()
    {
        return CurrentState == PlayerActionState.Idle;
    }
}
