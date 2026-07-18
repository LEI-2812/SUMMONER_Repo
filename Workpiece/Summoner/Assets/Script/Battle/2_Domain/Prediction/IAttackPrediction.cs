// 역할: IAttackPrediction의 책임을 정의한다.
public interface IAttackPrediction
{
    bool CanPredict(Summon summon);

    AttackPredictionData GetAttackPrediction(
        Summon summon,
        PredictionBoardData board);
}
