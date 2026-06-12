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

    public string GetSummonName() => summonName;
    public SummonRank GetSummonRank() => summonRank;
    public SummonType GetSummonType() => summonType;
    public double GetMaxHp() => maxHp;
    public double GetAttackPower() => attackPower;
    public double GetHeavyAttackPower() => heavyAttackPower;
    public SummonAttackData GetNormalAttack() => normalAttack;
    public SummonAttackData[] GetSpecialAttacks() => specialAttacks;

    public IAttackStrategy CreateNormalAttackStrategy()
    {
        return normalAttack == null ? null : normalAttack.CreateAttackStrategy();
    }

    public IAttackStrategy[] CreateSpecialAttackStrategies()
    {
        if (specialAttacks == null)
        {
            return new IAttackStrategy[0];
        }

        IAttackStrategy[] attackStrategies = new IAttackStrategy[specialAttacks.Length];
        for (int i = 0; i < specialAttacks.Length; i++)
        {
            attackStrategies[i] = specialAttacks[i] == null ? null : specialAttacks[i].CreateAttackStrategy();
        }

        return attackStrategies;
    }
}
