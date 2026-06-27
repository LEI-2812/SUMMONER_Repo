using UnityEngine;

// 역할: 공격 흐름의 상태 전환을 관리한다.
public class AttackStateMachine
{
    private readonly BattleAttackData battleAttackData;

    public AttackStateMachine(BattleAttackData battleAttackData)
    {
        this.battleAttackData = battleAttackData;
        CurrentState = AttackState.Idle;
    }

    public AttackState CurrentState { get; private set; }

    public bool IsTargetSelecting => CurrentState == AttackState.TargetSelecting;

    public Summon StartAttack(int buttonIndex)
    {
        StartAttackFromSelectedSource();

        if (battleAttackData.AttackingSummon == null)
        {
            Debug.Log("선택된 공격 소환수가 없습니다.");
            return null;
        }

        if (IsValidSpecialAttackIndex(battleAttackData.AttackingSummon, buttonIndex))
        {
            SetCurrentSpecialAttackInfo(
                new SpecialAttackInfo(
                    battleAttackData.AttackingSummon.GetSpecialAttackStrategy()[buttonIndex],
                    buttonIndex));
        }

        return battleAttackData.AttackingSummon;
    }

    public bool HasCurrentSpecialAttackInfo()
    {
        return battleAttackData.HasCurrentSpecialAttackInfo();
    }

    public int GetCurrentSpecialAttackInfoIndex()
    {
        return battleAttackData.GetCurrentSpecialAttackInfoIndex();
    }

    public bool DoesCurrentSpecialAttackTargetPlayerPlate()
    {
        return battleAttackData.DoesCurrentSpecialAttackTargetPlayerPlate();
    }

    public bool IsSpecialAttackTargetSelectionActive()
    {
        return battleAttackData.IsSpecialAttackTargetSelectionActive;
    }

    public int GetSelectedSpecialAttackTargetPlateIndex()
    {
        return battleAttackData.SelectedSpecialAttackTargetPlateIndex;
    }

    public int GetAttackingPlateIndex()
    {
        return battleAttackData.AttackingPlateIndex;
    }

    public Summon GetAttackingSummon()
    {
        return battleAttackData.AttackingSummon;
    }

    public SpecialAttackInfo GetCurrentSpecialAttackInfo()
    {
        return battleAttackData.CurrentSpecialAttackInfo;
    }

    public void SelectPlayerAttackSource(Summon summon, int plateIndex)
    {
        battleAttackData.SelectPlayerAttackSource(summon, plateIndex);
        CurrentState = AttackState.SourceSelected;
    }

    public void ClearPlayerAttackSource()
    {
        battleAttackData.ClearPlayerAttackSource();
        if (CurrentState == AttackState.SourceSelected)
        {
            CurrentState = AttackState.Idle;
        }
    }

    public void StartAttackFromSelectedSource()
    {
        battleAttackData.StartAttackFromSelectedSource();
        CurrentState = battleAttackData.AttackingSummon == null
            ? AttackState.Idle
            : AttackState.Executing;
    }

    public void SetCurrentSpecialAttackInfo(SpecialAttackInfo specialAttackInfo)
    {
        battleAttackData.SetCurrentSpecialAttackInfo(specialAttackInfo);
    }

    public void StartTargetSelection()
    {
        battleAttackData.StartSpecialAttackTargetSelection();
        CurrentState = AttackState.TargetSelecting;
    }

    public void SelectTargetPlate(int plateIndex)
    {
        battleAttackData.SelectSpecialAttackTargetPlate(plateIndex);
    }

    public void ClearTargetSelection()
    {
        battleAttackData.ClearSpecialAttackTargetSelection();
    }

    public void CancelTargetSelection()
    {
        battleAttackData.CancelSpecialAttackTargetSelection();
        CurrentState = AttackState.Canceled;
    }

    public void CompleteAttack()
    {
        CurrentState = AttackState.Completed;
    }

    public void Reset()
    {
        battleAttackData.ResetAttackSelection();
        CurrentState = AttackState.Idle;
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int specialAttackIndex)
    {
        if (attackSummon == null || attackSummon.GetSpecialAttackStrategy() == null)
        {
            return false;
        }

        return specialAttackIndex >= 0
            && specialAttackIndex < attackSummon.GetSpecialAttackStrategy().Length
            && attackSummon.GetSpecialAttackStrategy()[specialAttackIndex] != null;
    }
}
