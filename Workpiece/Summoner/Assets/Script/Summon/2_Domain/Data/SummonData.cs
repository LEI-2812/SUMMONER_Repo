using UnityEngine;

[CreateAssetMenu(menuName = "Summoner/Summon Data")]
// 역할: SummonData의 책임을 정의한다.
public class SummonData : ScriptableObject
{
    [SerializeField] private string summonName;
    [SerializeField] private SummonRank summonRank;
    [SerializeField] private double maxHp;
    [SerializeField] private double attackPower;
    [SerializeField] private double heavyAttackPower;
    [SerializeField] private SummonAttackData normalAttack;
    [SerializeField] private SummonAttackData[] specialAttacks;

    public string GetSummonName() => summonName;
    public SummonRank GetSummonRank() => summonRank;
    public double GetMaxHp() => maxHp;
    public double GetAttackPower() => attackPower;
    public double GetHeavyAttackPower() => heavyAttackPower;
    public SummonAttackData GetNormalAttack() => normalAttack;
    public SummonAttackData[] GetSpecialAttacks() => specialAttacks;

    public SummonAttackData GetSpecialAttack(int index)
    {
        if (specialAttacks == null || index < 0 || index >= specialAttacks.Length)
        {
            return null;
        }

        return specialAttacks[index];
    }

    public AttackData CreateNormalAttackStrategy()
    {
        return normalAttack == null ? null : normalAttack.CreateAttackStrategy();
    }

    public AttackData[] CreateSpecialAttackStrategies()
    {
        if (specialAttacks == null)
        {
            return new AttackData[0];
        }

        AttackData[] attackStrategies = new AttackData[specialAttacks.Length];
        for (int i = 0; i < specialAttacks.Length; i++)
        {
            attackStrategies[i] = specialAttacks[i] == null ? null : specialAttacks[i].CreateAttackStrategy();
        }

        return attackStrategies;
    }
}
