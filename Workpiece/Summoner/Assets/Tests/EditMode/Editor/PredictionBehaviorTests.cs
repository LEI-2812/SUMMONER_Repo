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
        PredictionBoardData board = CreatePredictionBoard(cat);

        var prediction = new CatAttackPrediction();
        AttackPredictionData result = prediction.GetAttackPrediction(cat, board);

        Assert.AreSame(expectedSpecialAttack, result.GetAttackStrategy());
        Assert.AreEqual(1, result.GetSpecialAttackArrayIndex());
    }

    [Test]
    public void SnakePrediction_UsesAvailableSpecialAttack_WhenFirstSpecialAttackIsOnCooldown()
    {
        TestSnake snake = CreateComponent<TestSnake>("Snake");
        snake.SummonInitialize();
        AttackData expectedSpecialAttack = PutFirstSpecialAttackOnCooldown(snake);
        PredictionBoardData board = CreatePredictionBoard(snake);

        var prediction = new SnakeAttackPrediction();
        AttackPredictionData result = prediction.GetAttackPrediction(snake, board);

        Assert.AreSame(expectedSpecialAttack, result.GetAttackStrategy());
        Assert.AreEqual(1, result.GetSpecialAttackArrayIndex());
    }

    [Test]
    public void PoisonAllReaction_UsesAttackSummonPlateIndex_WhenFallingBackToNormalAttack()
    {
        string source = File.ReadAllText("Assets/Script/Battle/1_Application/Enemy/ExecuteEnemyReactionUseCase.cs");
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

    private PredictionBoardData CreatePredictionBoard(Summon playerSummon)
    {
        TestEnemySummon enemy = CreateComponent<TestEnemySummon>("Enemy");
        enemy.SummonInitialize();
        var playerPlate = new PlateData(0);
        playerPlate.SetCurrentSummon(playerSummon);
        var firstEnemyPlate = new PlateData(0);
        firstEnemyPlate.SetCurrentSummon(enemy);
        var board = new BattleBoardData(
            new List<PlateData> { playerPlate },
            new List<PlateData> { firstEnemyPlate, new PlateData(1) });
        return new PredictionBoardData(board);
    }

    private class TestCat : Cat
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

    private class TestSnake : Snake
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

    private class TestEnemySummon : Summon
    {
        protected override void ApplyFallbackData()
        {
            SetFallbackStatus("TestEnemy", SummonRank.Normal, 100, 10, 0);
            SetAttackStrategies(new AttackData(new ClosestEnemyAttackStrategy(), StatusType.None, 10, 0));
        }
    }

}
