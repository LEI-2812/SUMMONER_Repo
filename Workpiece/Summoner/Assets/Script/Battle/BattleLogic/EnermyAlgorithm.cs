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
    private PlateController plateController;
    private List<Plate> playerPlates;
    private PlayerAttackPrediction playerAttackPrediction;
    Dictionary<int, AttackProbability> attackProbabilityMap = new Dictionary<int, AttackProbability>();

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
        playerAttackPrediction = GetComponent<PlayerAttackPrediction>();
    }

    // 알고리즘 순서대로 실행
    public void ExecuteEnermyAlgorithm(Summon attackingSummon, int targetIndex)
    {
       
        // 1. 소환수의 상태 체크
        playerPlates = CheckPlayerPlateState(); // 현재 playerPlates들

        // 2. 몬스터의 상태 체크 (새 리스트에 상태 조정된 enermyPlates 추가)
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

        for (int i = 0; i < playerPlates.Count; i++) //플레이어 플레이트 수만큼 검사
        {
            // 3. 소환수의 공격 예측 알고리즘 실행 (특수공격 확률을 받음)
            AttackProbability defaultProbability = new AttackProbability(0.5f, 0.5f); //기본 50% 50%
            attackProbabilityMap[targetIndex] = defaultProbability; //플레이어 플레이트의 인덱스와 키값을 일치하도록

            // 3.2 PlayerAttackPrediction 클래스에서 재계산된 값을 가져와서 업데이트
            //AttackProbability recalculatedProbability = playerAttackPrediction.ExecutePlayerAttackPrediction(playerPlates, applyEnermyPlates, defaultProbability);
            //attackProbabilityMap[targetIndex] = recalculatedProbability;
        }
        // 4. 몬스터의 공격 알고리즘 실행
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
