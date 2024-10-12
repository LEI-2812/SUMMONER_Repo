using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("아군 및 적 플레이트")]
    [SerializeField] private List<Plate> playerPlates; //플레이어 플레이트
    [SerializeField] private List<Plate> enermyPlates; //적 플레이트

    [SerializeField] private StatePanel statePanel;

    private bool isAttacking = false; //공격중인이 판별

    [Header("소환수 컨트롤러")]
    [SerializeField] private SummonController summonController;

    public Summon attackStart()
    {
        isAttacking = true; //공격시작
        return statePanel.getStatePanelSummon(); //상태창에 있는 소환수를 반환
    }

    // 특수 공격 처리 메서드
    public void SpecialAttack(Summon attackSummon, int selectedPlateIndex)
    {
        if (attackSummon != null)
        {
            // 스킬이 쿨타임 중인지 확인
            if (attackSummon.IsSkillOnCooldown("SpecialAttack"))
            {
                Debug.Log("특수 스킬이 쿨타임 중입니다. 사용할 수 없습니다.");
                return;
            }

            // TargetedAttackStrategy를 사용하는지 확인
            if (attackSummon.getSpecialAttackStrategy() is TargetedAttackStrategy)
            {
                Debug.Log("TargetedAttackStrategy를 사용합니다. 적의 플레이트를 선택하세요.");

                // 사용자가 선택한 플레이트 인덱스를 가져옵니다 (선택 대기 중)
                Plate selectedEnemyPlate = summonController.SelectPlateAndResummon(); // 적의 플레이트를 선택하는 로직 필요
                if (selectedEnemyPlate != null)
                {
                    selectedPlateIndex = getEnermyPlate().IndexOf(selectedEnemyPlate); // 선택한 플레이트의 인덱스 저장
                    Debug.Log($"플레이트 {selectedPlateIndex} 선택됨.");
                }
                else
                {
                    Debug.Log("플레이트가 선택되지 않았습니다.");
                    return; // 선택된 플레이트가 없으면 공격을 중단
                }
            }

            // 특수 공격 수행
            attackSummon.SpecialAttack(getEnermyPlate(), selectedPlateIndex);
        }
        else
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
        }

    }






    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking= isAttacking;
    }

    public List<Plate> getEnermyPlate()
    {
        return enermyPlates;
    }
















    //적 플레이트에 소환수가 존재하는지
    public bool IsEnermyPlateClear()
    {
        foreach(Plate plate in enermyPlates) //플레이트를 순환
        {
            Summon summon = plate.getSummon(); //플레이트마다 소환수를 가져온다
            if(summon != null) //만약 소환수가 하나라도 있다면 true를 반환
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPlayerPlateClear()
    {
        foreach (Plate plate in playerPlates) //플레이트를 순환
        {
            Summon summon = plate.getSummon(); //플레이트마다 소환수를 가져온다
            if (summon != null) //만약 소환수가 하나라도 있다면 true를 반환
            {
                return false;
            }
        }

        return true;
    }

    // 플레이어의 플레이트에 있는 모든 소환수들을 반환하는 메소드
    public List<Summon> getPlayerSummons()
    {
        List<Summon> playerSummons = new List<Summon>();
        foreach (Plate plate in playerPlates)
        {
            Summon summon = plate.getSummon();
            if (summon != null)
            {
                playerSummons.Add(summon);
            }
        }
        return playerSummons;
    }

    // 적의 플레이트에 있는 모든 소환수들을 반환하는 메소드
    public List<Summon> getEnermySummons()
    {
        List<Summon> enermySummons = new List<Summon>();
        foreach (Plate plate in enermyPlates)
        {
            Summon summon = plate.getSummon();
            if (summon != null)
            {
                enermySummons.Add(summon);
            }
        }
        return enermySummons;
    }

}
