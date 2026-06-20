using UnityEngine;

// 역할: 선택된 소환수의 공격 시작 상태를 관리하고 특수 공격을 실제 공격 전략으로 실행한다.
public class BattleController : MonoBehaviour
{
    public bool isAttacking = false; //공격중인이 판별

    private PlateController plateController;
    private BattleSpecialAttackExecutor specialAttackExecutor;
    private readonly BattleAttackState attackState = new BattleAttackState();

    public Summon GetAttackingSummon()
    {
        return attackState.AttackingSummon;
    }

    void Awake()
    {
        plateController = GetComponent<PlateController>();
        specialAttackExecutor = new BattleSpecialAttackExecutor(plateController);
    }

    public Summon AttackStart(int buttonIndex)
    {
        attackState.StartAttackFromSelectedSource();

        if (attackState.AttackingSummon == null)
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
            return null;
        }

        if (IsValidSpecialAttackIndex(attackState.AttackingSummon, buttonIndex))
        {
            attackState.SetCurrentSpecialAttackInfo(
                new SpecialAttackInfo(
                    attackState.AttackingSummon.GetSpecialAttackStrategy()[buttonIndex],
                    buttonIndex));
        }
        return attackState.AttackingSummon;
    }

    public void SelectPlayerAttackSource(Summon summon, int plateIndex)
    {
        attackState.SelectPlayerAttackSource(summon, plateIndex);
    }

    public void ClearPlayerAttackSource()
    {
        attackState.ClearPlayerAttackSource();
    }

    public bool SpecialAttackExecute(Summon attackSummon, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer = false)
    {
        if (!specialAttackExecutor.Execute(
            attackSummon,
            selectedPlateIndex,
            selectSpecialAttackIndex,
            isPlayer))
        {
            return false;
        }

        BattleAttackStateReset();
        return true;
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int selectSpecialAttackIndex)
    {
        if (attackSummon == null || attackSummon.GetSpecialAttackStrategy() == null)
        {
            return false;
        }

        return selectSpecialAttackIndex >= 0 && selectSpecialAttackIndex < attackSummon.GetSpecialAttackStrategy().Length;
    }

    public void BattleAttackStateReset()
    {
        attackState.Reset();
        isAttacking = attackState.IsSpecialAttackTargetSelectionActive;
        plateController.ResetAllPlateHighlight();
    }


    public PlateController GetPlateController()
    {
        return plateController;
    }

    public SpecialAttackInfo GetCurrentSpecialAttackInfo()
    {
        return attackState.CurrentSpecialAttackInfo;
    }

    public bool HasCurrentSpecialAttackInfo()
    {
        return attackState.HasCurrentSpecialAttackInfo();
    }

    public int GetCurrentSpecialAttackInfoIndex()
    {
        return attackState.GetCurrentSpecialAttackInfoIndex();
    }

    public bool DoesCurrentSpecialAttackTargetPlayerPlate()
    {
        return attackState.DoesCurrentSpecialAttackTargetPlayerPlate();
    }

    public bool IsSpecialAttackTargetSelectionActive()
    {
        return attackState.IsSpecialAttackTargetSelectionActive;
    }

    public int GetSelectedSpecialAttackTargetPlateIndex()
    {
        return attackState.SelectedSpecialAttackTargetPlateIndex;
    }

    public int GetAttackingPlateIndex()
    {
        return attackState.AttackingPlateIndex;
    }

    public void SelectSpecialAttackTargetPlate(int plateIndex)
    {
        attackState.SelectSpecialAttackTargetPlate(plateIndex);
    }

    public void ClearSpecialAttackTargetSelection()
    {
        attackState.ClearSpecialAttackTargetSelection();
    }

    public void StartSpecialAttackTargetSelection()
    {
        attackState.StartSpecialAttackTargetSelection();
        isAttacking = attackState.IsSpecialAttackTargetSelectionActive;
    }

    public void CancelSpecialAttackTargetSelection()
    {
        attackState.CancelSpecialAttackTargetSelection();
        isAttacking = attackState.IsSpecialAttackTargetSelectionActive;
    }
}
