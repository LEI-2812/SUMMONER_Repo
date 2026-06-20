using System.Collections.Generic;

// 역할: 적 턴 시작 시 사용할 플레이어 공격 예측 목록을 만든다.
// 책임 아님: 예측 결과에 대한 적 대응 실행.
internal class PlayerAttackPredictionListBuilder
{
    private readonly PlateController plateController;
    private readonly PlayerAttackPrediction playerAttackPrediction;

    public PlayerAttackPredictionListBuilder(
        PlateController plateController,
        PlayerAttackPrediction playerAttackPrediction)
    {
        this.plateController = plateController;
        this.playerAttackPrediction = playerAttackPrediction;
    }

    public List<AttackPrediction> Build()
    {
        EnemyPredictionPlateState predictionPlateState = new EnemyPredictionPlateState(plateController);

        try
        {
            return playerAttackPrediction.GetPlayerAttackPredictionList(
                predictionPlateState.PlayerPlates,
                predictionPlateState.EnemyPlates);
        }
        finally
        {
            predictionPlateState.Restore();
        }
    }
}
