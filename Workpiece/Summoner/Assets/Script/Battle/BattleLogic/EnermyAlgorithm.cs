using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackProbability
{
    public float normalAttackProbability;
    public float specialAttackProbability;

    // 생성자를 추가하여 초기화할 수 있도록 함
    public AttackProbability(float normalProb, float specialProb)
    {
        normalAttackProbability = normalProb;
        specialAttackProbability = specialProb;
    }
}


public class EnermyAlgorithm : MonoBehaviour
{
    [SerializeField] private PlateController plateController;
    [SerializeField] private PlayerAttackPrediction playerAttackPrediction;
     private List<AttackPrediction> playerAttackPredictionsList;


    // 알고리즘 순서대로 실행
    public void ExecuteEnermyAlgorithm(Summon attackingSummon, int targetIndex)
    {

        // 1. 소환수의 상태 체크
        List<Plate> playerPlates = CheckPlayerPlateState(); // 현재 playerPlates들

        // 2. 몬스터의 상태 체크 (새 리스트에 상태 조정된 enermyPlates 추가)
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

        //3. 소환수의 예측공격 리스트를 받아온다.
        playerAttackPredictionsList = playerAttackPrediction.getPlayerAttackPredictionList(playerPlates, applyEnermyPlates);

        // 4. 몬스터의 공격 알고리즘 실행
        //추후 추가예정
    }










    // 1. 현재 playerPlate들을 공격이나 데미지 적용이 안되게 새 리스트로 가져온다.
    public List<Plate> CheckPlayerPlateState()
    {
        List<Plate> playerPlateStates = new List<Plate>();

        foreach (Plate plate in plateController.getPlayerPlates()) // playerPlates의 리스트를 순회
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                // 기존 플레이트를 새 리스트에 추가 (상태만 관리)
                playerPlateStates.Add(plate);
            }
        }

        return playerPlateStates;
    }

    // 2. 적 몬스터의 상태를 조정하여 새로운 플레이트 리스트 반환
    private List<Plate> GetApplyStatusEnermyPlates()
    {
        List<Plate> applyEnermyPlates = new List<Plate>(); // 새 리스트

        foreach (Plate plate in plateController.getEnermyPlates()) // enermyPlates를 하나씩 가져온다
        {
            Summon originalSummon = plate.getCurrentSummon(); // 해당 플레이트의 소환수를 가져와서
            if (originalSummon != null)
            {
                // 기존 몬스터를 가져와 상태를 조정 후 새 플레이트 리스트에 추가
                Summon applySummon = ApplyEnermyStatus(originalSummon);
                plate.setCurrentSummon(applySummon); //null이여도 넣어줌. 죽었을땐 null이므로 공격대상이 되지 않게
                applyEnermyPlates.Add(plate);
            }
        }

        return applyEnermyPlates;
    }

    // 2.(1) 몬스터 상태에 따라 수치 조정
    private Summon ApplyEnermyStatus(Summon enermySummon)
    {
        // 독성: 최대 체력의 10% 데미지 적용
        if (enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.1); // 10% 체력 감소
            if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
        }

        // 화상: 최대 체력의 20% 데미지 적용
        if (enermySummon.getAllStatusTypes().Contains(StatusType.Burn))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% 체력 감소
            if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
        }

        // 흡혈: 최대 체력의 20% 데미지 적용
        if (enermySummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% 체력 감소
            if (enermySummon.getNowHP() <= 0) return null; // 체력이 0 이하라면 제외
        }

        return enermySummon; // 상태 적용된 몬스터 반환
    }
}
