using System.Collections.Generic;
using UnityEngine;

// 역할: 적이 선택한 일반 공격과 특수 공격을 실제로 실행한다.
// 책임 아님: 어떤 공격을 고를지 판단하거나 확률을 굴리는 일.
public class EnemyAttackExecutor
{
    // 역할: 실제 공격 실행 때 플레이어/적 플레이트 목록과 대상 위치를 조회한다.
    private readonly PlateController plateController;

    // 역할: 전투 컨트롤러를 통해 특수 공격 실행 흐름을 호출할 때 사용한다.
    private readonly BattleController battleController;

    public EnemyAttackExecutor(PlateController plateController)
    {
        // 직접 SpecialAttack 호출만 필요한 흐름에서는 BattleController 없이 사용한다.
        this.plateController = plateController;
    }

    public EnemyAttackExecutor(PlateController plateController, BattleController battleController)
        : this(plateController)
    {
        // BattleController를 통한 특수공격 실행이 필요한 대응 흐름에서 사용한다.
        this.battleController = battleController;
    }

    public void ExecuteNormalAttack(Summon attacker, int targetPlateIndex)
    {
        attacker.NormalAttack(plateController.GetPlayerPlates(), targetPlateIndex);
    }

    public void ExecuteSpecialAttack(Summon attacker, int targetPlateIndex, int specialAttackIndex, string logMessage)
    {
        battleController.SpecialAttackExecute(attacker, targetPlateIndex, specialAttackIndex);
        Debug.Log(logMessage);
    }

    public void ExecuteDirectSpecialAttack(Summon attacker, List<Plate> targetPlates, int targetPlateIndex, int specialAttackIndex)
    {
        attacker.SpecialAttack(targetPlates, targetPlateIndex, specialAttackIndex);
    }

    public void ExecuteHeavyNormalAttack(Summon attacker)
    {
        // 강공격은 공격력만 잠시 바꾸고 즉시 원래 공격력으로 되돌린다.
        Debug.Log($"{attacker.name} 의 강공격");
        double originPower = attacker.GetAttackPower();
        attacker.SetAttackPower(attacker.GetHeavyAttackPower());
        attacker.NormalAttack(plateController.GetPlayerPlates(), plateController.GetClosestPlayerPlateIndexExcept(attacker));
        attacker.SetAttackPower(originPower);
    }
}
