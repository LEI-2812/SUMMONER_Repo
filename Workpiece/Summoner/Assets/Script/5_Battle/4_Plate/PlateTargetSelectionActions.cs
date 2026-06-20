using UnityEngine;

// 역할: 공격 타겟 선택 중 Plate 클릭을 전투 타겟 선택 상태로 전달한다.
public class PlateTargetSelectionActions
{
    private readonly Plate plate;
    private readonly BattleController battleController;
    private readonly PlateController plateController;

    public PlateTargetSelectionActions(
        Plate plate,
        BattleController battleController,
        PlateController plateController)
    {
        this.plate = plate;
        this.battleController = battleController;
        this.plateController = plateController;
    }

    public bool TrySelectAttackTargetPlate()
    {
        if (!TryGetAttackTargetPlate(out int plateIndex, out string plateName))
        {
            Debug.Log("유효한 플레이트가 선택되지 않았습니다.");
            return false;
        }

        battleController.SelectSpecialAttackTargetPlate(plateIndex);
        Debug.Log($"{plateName}의 플레이트 {plateIndex}가 선택되었습니다.");
        plate.Unhighlight();
        return true;
    }

    public bool CanSelectCurrentPlateAsAttackTarget()
    {
        return TryGetAttackTargetPlate(out _, out _);
    }

    private bool TryGetAttackTargetPlate(out int plateIndex, out string plateName)
    {
        plateIndex = -1;
        plateName = "알 수 없음";

        if (plateController == null)
        {
            return false;
        }

        bool targetsPlayerPlate = battleController != null && battleController.DoesCurrentSpecialAttackTargetPlayerPlate();
        return plateController.TryGetAttackTargetPlate(plate, targetsPlayerPlate, out plateIndex, out plateName);
    }
}
