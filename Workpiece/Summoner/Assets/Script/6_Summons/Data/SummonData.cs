using UnityEngine;

[CreateAssetMenu(menuName = "Summoner/Summon Data")]
// 역할: 소환수 기본 능력치와 공격 데이터를 Unity 에셋으로 보관한다.
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
