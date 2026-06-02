using UnityEngine;

[CreateAssetMenu(menuName = "Summoner/Summon Data")]
public class SummonData : ScriptableObject
{
    [SerializeField] private string summonName;
    [SerializeField] private SummonRank summonRank;
    [SerializeField] private SummonType summonType;
    [SerializeField] private double maxHp;
    [SerializeField] private double attackPower;
    [SerializeField] private double heavyAttackPower;
    [SerializeField] private SummonAttackData normalAttack;
    [SerializeField] private SummonAttackData[] specialAttacks;

    public string SummonNameGet() => summonName;
    public SummonRank SummonRankGet() => summonRank;
    public SummonType SummonTypeGet() => summonType;
    public double MaxHpGet() => maxHp;
    public double AttackPowerGet() => attackPower;
    public double HeavyAttackPowerGet() => heavyAttackPower;
    public SummonAttackData NormalAttackGet() => normalAttack;
    public SummonAttackData[] SpecialAttacksGet() => specialAttacks;

    public IAttackStrategy NormalAttackStrategyCreate()
    {
        return normalAttack == null ? null : normalAttack.AttackStrategyCreate();
    }

    public IAttackStrategy[] SpecialAttackStrategiesCreate()
    {
        if (specialAttacks == null)
        {
            return new IAttackStrategy[0];
        }

        IAttackStrategy[] attackStrategies = new IAttackStrategy[specialAttacks.Length];
        for (int i = 0; i < specialAttacks.Length; i++)
        {
            attackStrategies[i] = specialAttacks[i] == null ? null : specialAttacks[i].AttackStrategyCreate();
        }

        return attackStrategies;
    }
}
