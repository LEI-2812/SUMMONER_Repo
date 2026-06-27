using System.IO;
using NUnit.Framework;

public class StatusEffectBehaviorTests
{
    [Test]
    public void Curse_RestoresAttackPower_WhenStatusExpires()
    {
        StatusStore statusStore = new StatusStore();
        FakeStatusTarget target = new FakeStatusTarget
        {
            AttackPower = 100
        };

        statusStore.Apply(StatusDataFactory.Create(StatusType.Curse, 1, 0.2), target);
        Assert.AreEqual(80, target.AttackPower);

        statusStore.Update(StatusTiming.StunAndCurse, target);

        Assert.AreEqual(100, target.AttackPower);
    }

    [Test]
    public void LifeDrain_WithOneTurnDuration_DamagesAndHealsOnce()
    {
        StatusStore statusStore = new StatusStore();
        FakeStatusTarget target = new FakeStatusTarget();
        FakeStatusTarget attacker = new FakeStatusTarget();

        statusStore.Apply(StatusDataFactory.Create(StatusType.LifeDrain, 1, 20, attacker), target);
        statusStore.Update(StatusTiming.Damage, target);

        Assert.AreEqual(20, target.DamageTaken);
        Assert.AreEqual(20, attacker.HealReceived);
    }

    [Test]
    public void Shield_Reapply_RefreshesToNewShieldAmount()
    {
        StatusStore statusStore = new StatusStore();
        FakeStatusTarget target = new FakeStatusTarget();

        statusStore.Apply(StatusDataFactory.Create(StatusType.Shield, 2, 50), target);
        statusStore.Apply(StatusDataFactory.Create(StatusType.Shield, 2, 80), target);

        Assert.AreEqual(80, target.Shield);
    }

    [Test]
    public void StatusDomain_DoesNotContainPlaceholderLogMessages()
    {
        string[] statusFiles = Directory.GetFiles("Assets/Script/Battle/2_Domain/Status", "*.cs");

        foreach (string statusFile in statusFiles)
        {
            string source = File.ReadAllText(statusFile);
            StringAssert.DoesNotContain("Debug.Log(\"Log\")", source, statusFile);
        }
    }

    [Test]
    public void SummonStageMultiplier_ScalesFixedDamage_ButKeepsPercentEffects()
    {
        SummonEntity summonEntity = new SummonEntity();
        AttackData normalAttack = new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 30, 0);
        AttackData fixedDamageSpecial = new AttackData(new TargetedAttackStrategy(), StatusType.None, 160, 0);
        AttackData percentDamageSpecial = new AttackData(new AttackAllEnemiesStrategy(), StatusType.Poison, 0.2, 3, 3);
        AttackData percentHealSpecial = new AttackData(new TargetedAttackStrategy(), StatusType.Heal, 0.3, 3);

        summonEntity.SetBattleStats("TestSummon", SummonRank.Low, 100, 30, 160);
        summonEntity.SetAttackStrategies(
            normalAttack,
            fixedDamageSpecial,
            percentDamageSpecial,
            percentHealSpecial);

        summonEntity.ScaleStats(2);

        Assert.AreEqual(200, summonEntity.MaxHp);
        Assert.AreEqual(60, summonEntity.AttackPower);
        Assert.AreEqual(320, summonEntity.HeavyAttackPower);
        Assert.AreEqual(60, normalAttack.GetSpecialDamage());
        Assert.AreEqual(320, fixedDamageSpecial.GetSpecialDamage());
        Assert.AreEqual(0.2, percentDamageSpecial.GetSpecialDamage());
        Assert.AreEqual(0.3, percentHealSpecial.GetSpecialDamage());
    }

    private sealed class FakeStatusTarget : IStatusTarget
    {
        public double AttackPower { get; set; }
        public double DamageTaken { get; private set; }
        public double HealReceived { get; private set; }
        public double Shield { get; private set; }
        public bool CanAttack { get; private set; } = true;
        public bool IsOnceInvincible { get; private set; }
        public int StatusChangedCount { get; private set; }

        public string GetStatusTargetName() => "FakeTarget";
        public double GetHealth() => 100 - DamageTaken + HealReceived;
        public double GetAttackPower() => AttackPower;

        public void DamageTake(double damage) => DamageTaken += damage;
        public void HealReceive(double healAmount) => HealReceived += healAmount;
        public void SetAttackAvailable(bool canAttack) => CanAttack = canAttack;
        public void ShieldAdd(double shieldAmount) => Shield += shieldAmount;
        public void SetShield(double shieldAmount) => Shield = shieldAmount;
        public void AttackPowerUpgrade(double multiplier) => AttackPower *= 1 + multiplier;
        public void AttackPowerCurse(double curseRate) => AttackPower *= 1 - curseRate;
        public void AttackPowerRestore(double originAttack) => AttackPower = originAttack;
        public void SetOnceInvincibility(bool isInvincibility) => IsOnceInvincible = isInvincibility;

        public void StatusHitColorShow() { }
        public void DebuffSoundPlay() { }
        public void BuffSoundPlay() { }
        public void StatusChangedNotify() => StatusChangedCount++;
    }
}
