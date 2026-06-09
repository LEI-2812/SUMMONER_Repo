using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{

    [SerializeField] private SummonStatePanelView statePanel;

    public bool isAttacking = false; //공격중인이 판별

    private PlateController plateController;

    Summon attackingSummon;
    SpecialAttackInfo currentSpecialAttackInfo;

    public Summon GetAttackingSummon()
    {
        return attackingSummon;
    }

    void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    public Summon AttackStart(int buttonIndex)
    {
        attackingSummon = statePanel.GetStatePanelSummon();
        currentSpecialAttackInfo = null;

        if (attackingSummon == null)
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
            return null;
        }

        if (IsValidSpecialAttackIndex(attackingSummon, buttonIndex))
        {
            currentSpecialAttackInfo = new SpecialAttackInfo(attackingSummon.GetSpecialAttackStrategy()[buttonIndex], buttonIndex);
        }
        return attackingSummon;
    }

    public void SpecialAttackExecute(Summon attackSummon, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer = false)
    {
        if (attackSummon == null)
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
            return;
        }

        // 특수 공격 배열의 범위를 확인하여 인덱스가 유효한지 검증
        if (!IsValidSpecialAttackIndex(attackSummon, selectSpecialAttackIndex))
        {
            Debug.LogError("유효하지 않은 특수 공격 인덱스입니다. 인덱스: " + selectSpecialAttackIndex);
            return;
        }

        // 사용 가능한 특수 공격을 배열 인덱스로 가져옴
        IAttackStrategy attackStrategy = attackSummon.GetSpecialAttackStrategy()[selectSpecialAttackIndex];

        // 공격 타입별로 로직 수행
        if (attackStrategy is TargetedAttackStrategy targetedAttack)
        {
            attackSummon.AttackSoundPlay();
            HandleTargetedAttack(attackSummon, targetedAttack, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);   
        }
        else if (attackStrategy is AttackAllEnemiesStrategy attackAll)
        {
            attackSummon.AttackSoundPlay();
            HandleAttackAll(attackSummon, attackAll, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else if (attackStrategy is ClosestEnemyAttackStrategy closestAttack)
        {
            attackSummon.AttackSoundPlay();
            HandleClosestEnemyAttack(attackSummon, closestAttack, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else
        {
            Debug.LogWarning("알 수 없는 공격 전략입니다.");
        }

        BattleAttackStateReset();
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int selectSpecialAttackIndex)
    {
        if (attackSummon == null || attackSummon.GetSpecialAttackStrategy() == null)
        {
            return false;
        }

        return selectSpecialAttackIndex >= 0 && selectSpecialAttackIndex < attackSummon.GetSpecialAttackStrategy().Length;
    }

    //타겟지정 로직
    private void HandleTargetedAttack(Summon attackSummon, TargetedAttackStrategy targetedAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {
        List<Plate> targetPlates = SpecialAttackTargetPlatesGet(targetedAttack, isPlayer);

        if (!targetedAttack.BenefitEffectCheck() && !IsValidPlateIndex(selectedPlateIndex, targetPlates.Count))
        {
            Debug.Log(TargetedAttackInvalidLogGet(isPlayer));
            return;
        }

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            selectSpecialAttackIndex,
            TargetedAttackSuccessLogGet(targetedAttack, selectedPlateIndex, isPlayer));
    }


    //전체공격 로직
    private void HandleAttackAll(Summon attackSummon, AttackAllEnemiesStrategy allAttackstrategy, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {
        List<Plate> targetPlates = SpecialAttackTargetPlatesGet(allAttackstrategy, isPlayer);

        if (isPlayer)
        {
            SpecialAttackApply(
                attackSummon,
                targetPlates,
                selectedPlateIndex,
                selectSpecialAttackIndex,
                "아군의 특수 전체 공격이 성공적으로 수행되었습니다.");
            return;
        }

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            selectSpecialAttackIndex,
            "적의 특수 전체 공격이 성공적으로 수행되었습니다.");
    }

    //근접공격 로직
    private void HandleClosestEnemyAttack(Summon attackSummon, ClosestEnemyAttackStrategy closestAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {
        List<Plate> targetPlates = SpecialAttackTargetPlatesGet(closestAttack, isPlayer);

        if (isPlayer)
        {
            SpecialAttackApply(
                attackSummon,
                targetPlates,
                selectedPlateIndex,
                selectSpecialAttackIndex,
                "아군의 특수 근접 공격이 성공적으로 수행되었습니다.");
            return;
        }

        SpecialAttackApply(
            attackSummon,
            targetPlates,
            selectedPlateIndex,
            selectSpecialAttackIndex,
            "적의 특수 근접 공격이 성공적으로 수행되었습니다.");
    }


    //유효한 플레이트인지 검사
    private bool IsValidPlateIndex(int selectedPlateIndex, int plateCount)
    {
        return selectedPlateIndex >= 0 && selectedPlateIndex < plateCount;
    }

    private List<Plate> SpecialAttackTargetPlatesGet(IAttackStrategy attackStrategy, bool isPlayer)
    {
        bool targetsOwnPlates = attackStrategy.BenefitEffectCheck();

        if (isPlayer == targetsOwnPlates)
        {
            return plateController.GetPlayerPlates();
        }

        return plateController.GetEnermyPlates();
    }

    private string TargetedAttackSuccessLogGet(TargetedAttackStrategy targetedAttack, int selectedPlateIndex, bool isPlayer)
    {
        if (isPlayer)
        {
            return targetedAttack.BenefitEffectCheck()
                ? $"플레이어가 선택한 아군의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다."
                : $"플레이어가 선택한 적의 플레이트 {selectedPlateIndex}가 공격 대상입니다.";
        }

        return targetedAttack.BenefitEffectCheck()
            ? $"적이 선택한 적의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다."
            : $"적이 선택한 플레이어의 플레이트 {selectedPlateIndex}가 공격 대상입니다.";
    }

    private string TargetedAttackInvalidLogGet(bool isPlayer)
    {
        return isPlayer
            ? "유효한 적의 플레이트 인덱스가 선택되지 않았습니다."
            : "유효한 플레이어의 플레이트 인덱스가 선택되지 않았습니다.";
    }

    private void SpecialAttackApply(
        Summon attackSummon,
        List<Plate> targetPlates,
        int selectedPlateIndex,
        int selectSpecialAttackIndex,
        string successLog)
    {
        attackSummon.SpecialAttack(targetPlates, selectedPlateIndex, selectSpecialAttackIndex);
        Debug.Log(successLog);
    }



    public void BattleAttackStateReset()
    {
        isAttacking = false;
        attackingSummon = null;
        currentSpecialAttackInfo = null;
        plateController.ResetAllPlateHighlight();
    }


    public PlateController GetPlateController()
    {
        return plateController;
    }

    public SpecialAttackInfo GetCurrentSpecialAttackInfo()
    {
        return currentSpecialAttackInfo;
    }

    public bool HasCurrentSpecialAttackInfo()
    {
        return GetCurrentSpecialAttackInfo() != null;
    }

    public int GetCurrentSpecialAttackInfoIndex()
    {
        SpecialAttackInfo attackInfo = GetCurrentSpecialAttackInfo();
        return attackInfo == null ? -1 : attackInfo.GetAttackInfoIndex();
    }

    public bool DoesCurrentSpecialAttackTargetPlayerPlate()
    {
        SpecialAttackInfo attackInfo = GetCurrentSpecialAttackInfo();
        if (attackInfo == null || attackInfo.GetAttackInfoStrategy() == null)
        {
            return false;
        }

        return DoesAttackStrategyTargetPlayerPlate(attackInfo.GetAttackInfoStrategy());
    }

    private bool DoesAttackStrategyTargetPlayerPlate(IAttackStrategy attackStrategy)
    {
        StatusType attackStatusType = attackStrategy.GetStatusType();
        return attackStatusType == StatusType.Heal
            || attackStatusType == StatusType.Upgrade
            || attackStatusType == StatusType.Shield;
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }
    public void SetIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }
}
