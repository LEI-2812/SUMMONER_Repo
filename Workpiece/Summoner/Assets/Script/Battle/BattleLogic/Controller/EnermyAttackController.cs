using System.Collections.Generic;
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

        for (int index = 0; index < plateController.getEnermySummonCount(); index++) //적이 순차적으로 공격준비
        {
            Summon attackingSummon = enermyPlate[index].getCurrentSummon(); //플레이트에 소환수를 차례로 가져와서
            // 소환수가 스턴 상태인지 확인
            if (attackingSummon.IsStun())
            {
                continue; // 스턴 상태면 다음 소환수로 넘어감
            }

            // 소환수의 지속 상태 검사
            if(HandleStatusAndReactPrediction(attackingSummon, enermyPlate, index))
            {
                return;
            }
                                    
            //맞대응 시작
            playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, index, playerAttackPredictionsList); //최소 1번 수행
            for (int seq = 0; seq < 2; seq++)
            {
                if (continuesAttackByRank(attackingSummon))
                {
                    Debug.Log("연속공격 발동");
                    playerAttackPredictionsList = enermyAlgorithm.HandleReactPrediction(attackingSummon, index, playerAttackPredictionsList);
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
                if(useHealIfAvailable(attackingSummon, enermyPlate, enermyPlateIndex)) //힐스킬을 사용 했는가
                    return true;
            }
        }

        return false; // 변경된 리스트 반환
    }

    //힐 사용이 가능하다면 힐 사용
    private bool useHealIfAvailable(Summon attackingSummon, List<Plate> enermyPlate, int enermyPlateIndex)
    {
        IAttackStrategy[] specialAttackStrategies = attackingSummon.getSpecialAttackStrategy(); //스킬들을 가져온다.

        for (int i = 0; i < specialAttackStrategies.Length; i++)
        {
            // 스킬들 중 힐 스킬이 있는 경우 자기 자신에게 사용
            if (specialAttackStrategies[i].getStatusType() == StatusType.Heal && specialAttackStrategies[i].getCurrentCooldown()<=0) //힐이여야하고 쿨타임이 0 아래여야한다.
            {
                attackingSummon.SpecialAttack(enermyPlate, enermyPlateIndex, i); // 자기 자신에게 힐 사용
                return true; // 힐을 사용했으면 루프 탈출
            }
        }
        return false;
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

    public PlateController getPlateController()
    {
        return enermyAlgorithm.getPlateController();
    }


    public EnermyAlgorithm getEnermyAlgorithmController()
    {
        return this.enermyAlgorithm;
    }
}
