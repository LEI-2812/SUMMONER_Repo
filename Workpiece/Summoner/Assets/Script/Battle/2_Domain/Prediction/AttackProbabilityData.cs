public struct AttackProbabilityData
{
    public float normalAttackProbability;

    public float specialAttackProbability;

    private string predictionReason;

    public AttackProbabilityData(
        float normalProb,
        float specialProb,
        string predictionReason = null)
    {
        normalAttackProbability = normalProb;
        specialAttackProbability = specialProb;
        this.predictionReason = predictionReason;
    }

    public AttackProbabilityData AddPredictionReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return this;
        }

        predictionReason = string.IsNullOrWhiteSpace(predictionReason)
            ? reason
            : predictionReason + ", " + reason;
        return this;
    }

    public string GetPredictionReason()
    {
        return string.IsNullOrWhiteSpace(predictionReason)
            ? "추가 조건이 없어 기본 확률 사용"
            : predictionReason;
    }
}
