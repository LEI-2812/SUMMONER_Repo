using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    private PlateController plateController;
    private EnermyAlgorithm enermyAlgorithm;

    private enum AttackType{ NormalAttack, SpecialAttack}; //특수스킬을 사용할지 일반공격을 사용할지를 위한 Enum


    private void Start()
    {
        enermyAlgorithm = GetComponent<EnermyAlgorithm>();
        plateController = enermyAlgorithm.getPlateController();
    }


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
            if(HandleStatusAndReactPrediction(attackingSummon, enermyPlate, enermyPlateIndex))
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

    //화상, 흡혈, 독성에 대해서는 힐스킬이 있을경우 힐사용
    private bool HandleStatusAndReactPrediction(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex)
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

    public PlateController getPlateController()
    {
        return enermyAlgorithm.getPlateController();
    }


    public EnermyAlgorithm getEnermyAlgorithmController()
    {
        return this.enermyAlgorithm;
    }
}
