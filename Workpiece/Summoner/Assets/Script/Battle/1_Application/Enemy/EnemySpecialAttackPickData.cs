struct EnemySpecialAttackPickData
{
    public bool HasValue { get; }

    public int TargetPlateIndex { get; }

    public int SpecialAttackIndex { get; }

    public string LogMessage { get; }

    private EnemySpecialAttackPickData(bool hasValue, int targetPlateIndex, int specialAttackIndex, string logMessage)
    {
        HasValue = hasValue;
        TargetPlateIndex = targetPlateIndex;
        SpecialAttackIndex = specialAttackIndex;
        LogMessage = logMessage;
    }

    public static EnemySpecialAttackPickData Create(int targetPlateIndex, int specialAttackIndex, string logMessage)
    {
        return new EnemySpecialAttackPickData(true, targetPlateIndex, specialAttackIndex, logMessage);
    }

    public static EnemySpecialAttackPickData None()
    {
        return new EnemySpecialAttackPickData(false, -1, -1, string.Empty);
    }
}
