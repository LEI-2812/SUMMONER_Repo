using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class PredictionBehaviorTests
{
    private readonly List<GameObject> createdObjects = new List<GameObject>();

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject createdObject in createdObjects)
        {
            if (createdObject != null)
            {
                Object.DestroyImmediate(createdObject);
            }
        }

        createdObjects.Clear();
    }

    [Test]
    public void CatPrediction_UsesAvailableSpecialAttack_WhenFirstSpecialAttackIsOnCooldown()
    {
        TestCat cat = CreateComponent<TestCat>("Cat");
        cat.SummonInitialize();
        AttackData expectedSpecialAttack = PutFirstSpecialAttackOnCooldown(cat);
        IReadOnlyList<IPlateState> enemyPlates = CreateEnemyPlates();

        CatAttackPrediction prediction = CreateComponent<CatAttackPrediction>("CatPrediction");
        AttackPredictionData result = prediction.GetAttackPrediction(cat, 0, new List<IPlateState>(), enemyPlates);

        Assert.AreSame(expectedSpecialAttack, result.GetAttackStrategy());
        Assert.AreEqual(1, result.GetSpecialAttackArrayIndex());
    }

    [Test]
    public void SnakePrediction_UsesAvailableSpecialAttack_WhenFirstSpecialAttackIsOnCooldown()
    {
        TestSnake snake = CreateComponent<TestSnake>("Snake");
        snake.SummonInitialize();
        AttackData expectedSpecialAttack = PutFirstSpecialAttackOnCooldown(snake);
        IReadOnlyList<IPlateState> enemyPlates = CreateEnemyPlates();

        SnakeAttackPrediction prediction = CreateComponent<SnakeAttackPrediction>("SnakePrediction");
        AttackPredictionData result = prediction.GetAttackPrediction(snake, 0, new List<IPlateState>(), enemyPlates);

        Assert.AreSame(expectedSpecialAttack, result.GetAttackStrategy());
        Assert.AreEqual(1, result.GetSpecialAttackArrayIndex());
    }

    [Test]
    public void PoisonAllReaction_UsesAttackSummonPlateIndex_WhenFallingBackToNormalAttack()
    {
        string source = File.ReadAllText("Assets/Script/Battle/1_Application/Enemy/EnemySpecialAttackReaction.cs");
        int methodStart = source.IndexOf("private bool TryReactToPoisonAllAttack", System.StringComparison.Ordinal);
        Assert.GreaterOrEqual(methodStart, 0);

        int nextMethodStart = source.IndexOf("private bool TryReactToTargetAttack", methodStart, System.StringComparison.Ordinal);
        Assert.Greater(nextMethodStart, methodStart);

        string methodBody = source.Substring(methodStart, nextMethodStart - methodStart);

        StringAssert.DoesNotContain("playerPrediction.GetTargetPlateIndex()", methodBody);
        StringAssert.Contains("playerPrediction.GetAttackSummonPlateIndex()", methodBody);
    }

    private T CreateComponent<T>(string objectName) where T : Component
    {
        GameObject gameObject = new GameObject(objectName);
        createdObjects.Add(gameObject);
        return gameObject.AddComponent<T>();
    }

    private AttackData PutFirstSpecialAttackOnCooldown(Summon summon)
    {
        AttackData[] specialAttacks = summon.GetSpecialAttackStrategy();
        specialAttacks[0].ApplyCooldown();
        return specialAttacks[1];
    }

    private IReadOnlyList<IPlateState> CreateEnemyPlates()
    {
        TestEnemySummon enemy = CreateComponent<TestEnemySummon>("Enemy");
        enemy.SummonInitialize();
        return new List<IPlateState>
        {
            new FakePlateState(0, enemy),
            new FakePlateState(1, null)
        };
    }

    private sealed class TestCat : Cat
    {
        protected override void ApplyFallbackData()
        {
            SetFallbackStatus("TestCat", SummonRank.Low, 100, 30, 40);
            SetAttackStrategies(
                new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 30, 0),
                new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 40, 2),
                new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 60, 0));
        }
    }

    private sealed class TestSnake : Snake
    {
        protected override void ApplyFallbackData()
        {
            SetFallbackStatus("TestSnake", SummonRank.Medium, 100, 30, 0);
            SetAttackStrategies(
                new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 30, 0),
                new AttackData(new AttackAllEnemiesStrategy(), StatusType.Poison, 0.2, 2, 2),
                new AttackData(new AttackAllEnemiesStrategy(), StatusType.Burn, 0.2, 0, 2));
        }
    }

    private sealed class TestEnemySummon : Summon
    {
        protected override void ApplyFallbackData()
        {
            SetFallbackStatus("TestEnemy", SummonRank.Normal, 100, 10, 0);
            SetAttackStrategies(new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 10, 0));
        }
    }

    private sealed class FakePlateState : IPlateState
    {
        private readonly int plateIndex;
        private readonly Summon summon;

        public FakePlateState(int plateIndex, Summon summon)
        {
            this.plateIndex = plateIndex;
            this.summon = summon;
        }

        public Summon GetCurrentSummon() => summon;
        public int GetPlateIndex() => plateIndex;
    }
}
