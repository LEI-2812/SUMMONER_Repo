using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : MonoBehaviour
{

    [SerializeField] private StatePanel statePanel;

    private bool isAttacking = false; //공격중인이 판별

    private PlateController plateController;

    Summon attakingSummon;
    SpecialAttackInfo SpecialAttackInfo;

    public Summon getAttakingSummon()
    {
        return attakingSummon;
    }

    void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    public Summon attackStart(int buttonIndex)
    {
        attakingSummon = statePanel.getStatePanelSummon();
        SpecialAttackInfo = new SpecialAttackInfo(attakingSummon.getSpecialAttackStrategy()[buttonIndex], buttonIndex);
        return attakingSummon; //상태창에 있는 소환수를 반환
    }

    // 특수 공격 처리 메서드
    public void SpecialAttackLogic(Summon attackSummon, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer = false)
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

        // 특수 공격을 배열 인덱스로 가져옴
        IAttackStrategy attackStrategy = attackSummon.getSpecialAttackStrategy()[selectSpecialAttackIndex];

        // 공격 타입별로 로직 수행
        if (attackStrategy is TargetedAttackStrategy targetedAttack)
        {
            HandleTargetedAttack(attackSummon, targetedAttack, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else if (attackStrategy is AttackAllEnemiesStrategy attackAll)
        {
            HandleAttackAll(attackSummon, attackAll, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else if (attackStrategy is ClosestEnemyAttackStrategy closestAttack)
        {
            HandleClosestEnemyAttack(attackSummon, closestAttack, selectedPlateIndex, selectSpecialAttackIndex, isPlayer);
        }
        else
        {
            Debug.LogWarning("알 수 없는 공격 전략입니다.");
        }

        ResetBattleSummonAndAttackInfo();
    }

    private bool IsValidSpecialAttackIndex(Summon attackSummon, int selectSpecialAttackIndex)
    {
        return selectSpecialAttackIndex >= 0 && selectSpecialAttackIndex < attackSummon.getSpecialAttackStrategy().Length;
    }

    //타겟지정 로직
    private void HandleTargetedAttack(Summon attackSummon, TargetedAttackStrategy targetedAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {
        StatusType attackStatusType = targetedAttack.getStatusType();

        if (isPlayer) //플레이어
        {
            if (!IsValidPlateIndex(selectedPlateIndex, plateController.getEnermyPlates().Count))
            {
                Debug.Log("유효한 적의 플레이트 인덱스가 선택되지 않았습니다.");
                return;
            }

            if (attackStatusType == StatusType.Heal || attackStatusType == StatusType.Upgrade || attackStatusType == StatusType.Shield)
            {
                attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 아군 플레이트에 이로운 효과
                Debug.Log($"플레이어가 선택한 아군의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다.");
            }
            else
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적 플레이트에 공격
                Debug.Log($"플레이어가 선택한 적의 플레이트 {selectedPlateIndex}가 공격 대상입니다.");
            }
        }
        else //적
        {
            if (!IsValidPlateIndex(selectedPlateIndex, plateController.getPlayerPlates().Count))
            {
                Debug.Log("유효한 플레이어의 플레이트 인덱스가 선택되지 않았습니다.");
                return;
            }

            if (attackStatusType == StatusType.Heal || attackStatusType == StatusType.Upgrade || attackStatusType == StatusType.Shield)
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적 플레이트에 이로운 효과
                Debug.Log($"적이 선택한 적의 플레이트 {selectedPlateIndex}가 이로운 효과 대상입니다.");
            }
            else
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적 플레이트에 공격
                Debug.Log($"적이 선택한 플레이어의 플레이트 {selectedPlateIndex}가 공격 대상입니다.");
            }

        }
    }


    //전체공격 로직
    private void HandleAttackAll(Summon attackSummon, AttackAllEnemiesStrategy AllusAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {

        if (isPlayer)
        {
            attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적의 플레이트에 공격
            Debug.Log("아군의 특수 전체 공격이 성공적으로 수행되었습니다.");
        }
        else
        {
            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적이 플레이어 플레이트에 공격
            Debug.Log("적의 특수 전체 공격이 성공적으로 수행되었습니다.");
        }
    }

    //근접공격 로직
    private void HandleClosestEnemyAttack(Summon attackSummon, ClosestEnemyAttackStrategy closestAttack, int selectedPlateIndex, int selectSpecialAttackIndex, bool isPlayer)
    {

        if (isPlayer)
        {
            attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적의 플레이트에 공격
            Debug.Log("아군의 특수 근접 공격이 성공적으로 수행되었습니다.");
        }
        else
        {
            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex, selectSpecialAttackIndex); // 적이 플레이어 플레이트에 공격
            Debug.Log("적의 특수 근접 공격이 성공적으로 수행되었습니다.");
        }
    }


    //유효한 플레이트인지 검사
    private bool IsValidPlateIndex(int selectedPlateIndex, int plateCount)
    {
        return selectedPlateIndex >= 0 && selectedPlateIndex < plateCount;
    }



    public void ResetBattleSummonAndAttackInfo()
    {
        isAttacking = false;
        attakingSummon = null;
        SpecialAttackInfo = null;
        plateController.ResetAllPlateHighlight();
    }


    // SummonController에 PlateController 접근 메서드 추가
    public PlateController GetPlateController()
    {
        return plateController; // 이미 SummonController에서 PlateController를 참조하고 있다고 가정
    }

    public SpecialAttackInfo getNowSpecialAttackInfo()
    {
        return SpecialAttackInfo;
    }

    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }
}
