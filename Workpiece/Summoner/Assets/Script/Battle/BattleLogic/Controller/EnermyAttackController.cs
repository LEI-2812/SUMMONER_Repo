using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private PlateController plateController;
    private EnermyAlgorithm enermyAlgorithm;

    double normalAttackValue = 50f; double specialAttackValue = 50f;
    private enum AttackType{ NormalAttack, SpecialAttack}; //특수스킬을 사용할지 일반공격을 사용할지를 위한 Enum


    private void Start()
    {
        enermyAlgorithm = GetComponent<EnermyAlgorithm>();
    }


    //public void EnermyAttackStart(Summon attackingSummon)
    //{
    //    if (attackingSummon == null)
    //    {
    //        Debug.LogError("공격할 소환수가 없습니다.");
    //        return;
    //    }

    //    for (int targetIndex = 0; targetIndex < plateController.getPlayerPlates().Count; targetIndex++) //플레이어 플레이트를 순차적으로 공격
    //    {
    //        Plate targetPlate = plateController.getPlayerPlates()[targetIndex];
    //        Summon target = targetPlate.getCurrentSummon();

    //        for (int ii = 0; ii < 2; ii++) //연속공격 가능성
    //        {
    //            EnerymyAttackLogic(attackingSummon, target, targetIndex); //최소1번은 실행

    //            if (continuesAttackByRank(attackingSummon)) //매개변수로 보낸 소환수의 등급에따라 연속공격이 가능하면 기존 로직 최대 3번 수행
    //            {
    //                Debug.Log("연속공격 발동!");
    //                continue; //계속실행
    //            }
    //            else //연속공격가능성이 false면 바로 종료
    //            {
    //                return;
    //            }
    //        }
    //    }
    //}

    public void EnermyAttackStart(List<AttackPrediction> playerAttackPredictionsList)
    {
        List<Plate> enermyPlate = plateController.getEnermyPlates();

        for (int enermyPlateIndex = 0; enermyPlateIndex < plateController.getEnermySummonCount(); enermyPlateIndex++) //적이 순차적으로 공격준비
        {
            Summon attackingSummon = enermyPlate[enermyPlateIndex].getCurrentSummon(); //플레이트에 소환수를 차례로 가져와서
            // 소환수가 스턴 상태인지 확인
            if (IsSummonStunned(attackingSummon))
            {
                continue; // 스턴 상태면 다음 소환수로 넘어감
            }

            // 소환수의 지속 상태 검사
            if(HandleStatusAndReactPrediction(attackingSummon, enermyPlate, enermyPlateIndex, playerAttackPredictionsList))
            {
                return;
            }
                                    
            //맞대응 시작
            playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, enermyPlateIndex, playerAttackPredictionsList); //최소 1번 수행
            for (int seq = 0; seq < 2; seq++)
            {
                if (continuesAttackByRank(attackingSummon))
                {
                    Debug.Log("연속공격 발동");
                    playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, enermyPlateIndex, playerAttackPredictionsList);
                }
                else
                {
                    break; //연속공격 for문 종료
                }
            }
        }
    }


    private bool HandleStatusAndReactPrediction(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex, List<AttackPrediction> playerAttackPredictionsList)
    {
        List<StatusType> statusList = attackingSummon.getAllStatusTypes();
        // 지속 상태가 있는지 검사
        foreach (StatusType statusType in statusList)
        {
            if (statusType == StatusType.Burn || statusType == StatusType.LifeDrain || statusType == StatusType.Poison)
            {
                useHealIfAvailable(attackingSummon, enermyPlate, enermyPlateIndex);
                return true;
            }
        }

        return false; // 변경된 리스트 반환
    }


    private bool IsSummonStunned(Summon summon)
    {
        List<StatusType> statusList = summon.getAllStatusTypes();
        return statusList.Contains(StatusType.Stun);
    }
    private void useHealIfAvailable(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex)
    {
        IAttackStrategy[] specialAttackStrategies = attackingSummon.getSpecialAttackStrategy(); // 사용가능한 스킬들을 가져옴

        for (int i = 0; i < specialAttackStrategies.Length; i++)
        {
            // 스킬들 중 힐 스킬이 있는 경우 자기 자신에게 사용
            if (specialAttackStrategies[i].getStatusType() == StatusType.Heal)
            {
                attackingSummon.SpecialAttack(enermyPlate, enermyPlateIndex, i); // 자기 자신에게 힐 사용
                return; // 힐을 사용했으면 루프 탈출
            }
        }
    }

    //private void EnerymyAttackLogic(Summon attackingSummon, Summon target, int targetIndex)
    //{
    //    if (!attackingSummon.IsCooltime()) //쿨타임중인 스킬이 없을경우
    //    {
    //        AttackType selectedAttakType = SelectAttackType(); //일반공격과 특수공격을 랜덤으로 받아옴
    //        if (selectedAttakType == AttackType.SpecialAttack)
    //        {
    //            //쿨타임이 없는 특수스킬을 사용하게 한다.
    //            List<int> availableSpecialAttacks = attackingSummon.getAvailableSpecialAttack();  // 쿨타임이 없는 특수 스킬 목록을 가져옴


    //            int selectSpecialAttackIndex = getRandomAvilableSpecialAttackIndex(availableSpecialAttacks); //랜덤의 특수스킬 번호를 가져옴
    //            int selectedPlateIndex = plateController.getClosestPlayerPlatesIndex(attackingSummon); //임시로 가장 가까운적 공격하게 함. 나중에 수정필요
    //            algorithm.ExecuteEnermyAlgorithm(attackingSummon, targetIndex); //알고리즘 실행

    //            battleController.SpecialAttackLogic(attackingSummon, selectedPlateIndex, selectSpecialAttackIndex); //특수스킬 사용
    //        }
    //        else
    //        {
    //            enermyNormalAttackLogic(attackingSummon); //평타
    //        }
    //    }
    //    else //스킬들이 쿨타임이여서 평타만 공격
    //    {
    //        enermyNormalAttackLogic(attackingSummon);
    //    }
    //}   



    //일반 공격
    private void enermyNormalAttackLogic(Summon attackingSummon)
    {
        int selectAttackIndex = plateController.getClosestPlayerPlatesIndex(attackingSummon); //플레이어 플레이트에서 가장 가까운 소환수의 인덱스를 받아온다.
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

    public PlateController getPlateController()
    {
        return plateController;
    }





    public void setNormalAttackValue(float value)
    {
        normalAttackValue = value;
    }
    public void setSpecialAttackValue(float value)
    {
        specialAttackValue = value;
    }
}
