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
    private bool isHeal = false;

    [Header("(외부 오브젝트)컨트롤러")]
    [SerializeField] private SummonController summonController;
    private PlateController plateController;

    //공격당할 플레이트
    private Plate AttackedPlate;

    void Awake()
    {
        plateController = GetComponent<PlateController>();
    }

    public Summon attackStart()
    {
        return statePanel.getStatePanelSummon(); //상태창에 있는 소환수를 반환
    }

    // 특수 공격 처리 메서드
    public void SpecialAttackLogic(Summon attackSummon, int selectedPlateIndex, bool isPlayer = false)
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
                Debug.Log("TargetedAttackStrategy를 사용하여 공격을 수행합니다.");

                if (isPlayer) // 플레이어가 호출하는 경우
                {
                    if (selectedPlateIndex >= 0 && selectedPlateIndex < plateController.getEnermyPlates().Count)
                    {
                        if (isHeal)
                        {
                            attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex); // 아군의 플레이트와 인덱스 전달
                            return;
                        }
                        Debug.Log($"플레이어가 선택한 아군의 플레이트 {selectedPlateIndex}가 공격 대상입니다.");
                    }
                    else
                    {
                        Debug.Log("유효한 적의 플레이트 인덱스가 선택되지 않았습니다.");
                        return; // 선택된 플레이트가 없으면 공격을 중단
                    }
                }
                else // 적이 호출하는 경우, 플레이어의 플레이트 인덱스를 사용
                {
                    if (selectedPlateIndex >= 0 && selectedPlateIndex < plateController.getPlayerPlates().Count)
                    {
                        Debug.Log($"적이 플레이어의 플레이트 {selectedPlateIndex}를 공격합니다.");
                    }
                    else
                    {
                        Debug.Log("유효한 플레이어의 플레이트 인덱스가 선택되지 않았습니다.");
                        return; // 선택된 인덱스가 유효하지 않으면 공격을 중단
                    }
                }
            }

            //타겟 공격이 아닐경우
            // 특수 공격 수행 (플레이어는 적의 플레이트, 적은 플레이어의 플레이트)
            if (isPlayer) //플레이어 입장
            {
                attackSummon.SpecialAttack(plateController.getEnermyPlates(), selectedPlateIndex); // 적의 플레이트와 인덱스 전달
            }
            else //적의 입장
            {
                attackSummon.SpecialAttack(plateController.getPlayerPlates(), selectedPlateIndex); // 플레이어의 플레이트와 인덱스 전달
            }

            Debug.Log("특수 공격이 성공적으로 수행되었습니다.");
        }
        else
        {
            Debug.Log("선택된 plate에 소환수가 없습니다.");
        }

        isHeal = false;
        isAttacking = false; // 공격 종료
    }



    public void selectAttackPlate(Plate plate)
    {
        for (int i = 0; i < plateController.getEnermyPlates().Count; i++)
        {
            if (plateController.getEnermyPlates()[i] == plate)
            {
                AttackedPlate = plateController.getEnermyPlates()[i]; // 선택된 플레이트 설정
                Debug.Log($"공격할 플레이트 {i} 선택됨.");
                return;
            }
        }

        Debug.Log("적의 플레이트가 선택되지 않았습니다.");

    }


    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking= isAttacking;
    }

















    //적 플레이트에 소환수가 존재하는지
    public bool IsEnermyPlateClear()
    {
        foreach(Plate plate in plateController.getPlayerPlates()) //플레이트를 순환
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
        foreach (Plate plate in plateController.getPlayerPlates()) //플레이트를 순환
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
        foreach (Plate plate in plateController.getPlayerPlates())
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
        foreach (Plate plate in plateController.getEnermyPlates())
        {
            Summon summon = plate.getSummon();
            if (summon != null)
            {
                enermySummons.Add(summon);
            }
        }
        return enermySummons;
    }


    public Plate getAttackedPlate()
    {
        return AttackedPlate;
    }

    // SummonController에 PlateController 접근 메서드 추가
    public PlateController GetPlateController()
    {
        return plateController; // 이미 SummonController에서 PlateController를 참조하고 있다고 가정
    }

    public void setIsHeal(bool heal)
    {
        this.isHeal = heal;
    }
    public bool getIsHeal()
    {
        return isHeal;
    }
}
