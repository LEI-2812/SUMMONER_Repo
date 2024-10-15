using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;
    private enum AttackType{ NormalAttack, SpecialAttack}; //특수스킬을 사용할지 일반공격을 사용할지를 위한 Enum
    
    public void EnermyAttackStart(Summon attackingSummon)
    {
        if (attackingSummon == null)
        {
            Debug.LogError("공격할 소환수가 없습니다.");
            return;
        }

        for (int i = 0; i < 2; i++) //연속공격 가능성
        {
            EnerymyAttackLogic(attackingSummon); //최소1번은 실행

            if (continuesAttackByRank(attackingSummon)) //매개변수로 보낸 소환수의 등급에따라 연속공격이 가능하면 기존 로직 최대 3번 수행
            {
                Debug.Log("연속공격 발동!");
                continue; //계속실행
            }
            else //연속공격가능성이 false면 바로 종료
            {
                return;
            }
        }
    }

    private void EnerymyAttackLogic(Summon attackingSummon)
    {
        if (!attackingSummon.IsCooltime()) //쿨타임중인 스킬이 없을경우
        {
            AttackType selectedAttakType = SelectAttackType(); //일반공격과 특수공격을 랜덤으로 받아옴
            if (selectedAttakType == AttackType.SpecialAttack)
            {
                //쿨타임이 없는 특수스킬을 사용하게 한다.
                List<int> availableSpecialAttacks = attackingSummon.getAvailableSpecialAttack();  // 쿨타임이 없는 특수 스킬 목록을 가져옴
                int selectSpecialAttackIndex = getRandomAvilableSpecialAttackIndex(availableSpecialAttacks); //랜덤의 특수스킬 번호를 가져옴

                int selectedPlateIndex = plateController.getClosestPlayerPlatesIndex(); //임시로 가장 가까운적 공격하게 함. 나중에 수정필요

                battleController.SpecialAttackLogic(attackingSummon, selectedPlateIndex, selectSpecialAttackIndex); //특수스킬 사용
            }
            else
            {
                enermyNormalAttackLogic(attackingSummon); //평타
            }
        }
        else //스킬들이 쿨타임이여서 평타만 공격
        {
            enermyNormalAttackLogic(attackingSummon);
        }
    }


    //일반 공격
    private void enermyNormalAttackLogic(Summon attackingSummon)
    {
        int selectAttackIndex = plateController.getClosestPlayerPlatesIndex(); //플레이어 플레이트에서 가장 가까운 소환수의 인덱스를 받아온다.
        if (selectAttackIndex < 0)
        {
            Debug.Log("공격할 소환수가 없습니다."); return;
        }

        float randomValue = Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값
        if (randomValue < 30f) //강공격
        {
            Debug.Log($"{attackingSummon.name} 의 강공격");
            attackingSummon.setAttackPower(attackingSummon.getHeavyAttakPower()); //공격력을 강공격력으로 전환
            attackingSummon.normalAttack(plateController.getPlayerPlates(), selectAttackIndex); //일반공격 수행
            attackingSummon.setAttackPower(attackingSummon.getAttackPower()); //원래 공격력으로 되돌리기
        }
        else //일반 공격력으로 공격
        {
            attackingSummon.normalAttack(plateController.getPlayerPlates(), selectAttackIndex); //일반공격 수행
        }
    }


    
    //사용가능한 랜덤의 특수스킬 인덱스 받기 ---- 곧 랜덤에서 혼합전략 로직으로 바뀔예정 수정필요
    private int getRandomAvilableSpecialAttackIndex(List<int> availableSpecialAttacks)
    {
        if (availableSpecialAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpecialAttacks.Count); //사용가능한 특수스킬들중 랜덤값을 받는다.
            int selectedSpecialAttackIndex = availableSpecialAttacks[randomIndex]; //특수스킬 랜덤 인덱스
            return selectedSpecialAttackIndex;
        }
        return -1;
    }



    //등급별 연속공격 가능여부
    private bool continuesAttackByRank(Summon summon)
    {
        float randomValue = Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값

        if(summon.getSummonRank() == SummonRank.Normal) //노말등급은 연속공격 X
        {
            return false;
        }
        else if(summon.getSummonRank() == SummonRank.Special) //특급은 20%
        {
            if (randomValue <= 20) //20%면 연속공격
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (summon.getSummonRank() == SummonRank.Boss) //보스 30%
        {
            if (randomValue <= 30) //30%면 연속공격
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    //50% 50% 일반공격과 특수공격을 받아온다.
    private AttackType SelectAttackType()
    {
        float randomValue = Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값

        if (randomValue <= 50)
        {
            return AttackType.NormalAttack;
        }
        else
        {
            return AttackType.SpecialAttack;
        }
    }



}
