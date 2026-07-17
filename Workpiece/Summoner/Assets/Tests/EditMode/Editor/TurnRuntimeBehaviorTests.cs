using System;
using System.IO;
using NUnit.Framework;

public class TurnRuntimeBehaviorTests
{
    [Test]
    public void EnemyTurnRuntime_CompletesTurn_WhenEnemyAttackCannotStart()
    {
        string source = File.ReadAllText("Assets/Script/Battle/0_Presentation/Enemy/EnemyTurnRuntime.cs");
        string methodBody = GetMethodBody(source, "private void ExecuteEnemyTurn()");

        int failedAttackBranch = methodBody.IndexOf("if (!TryStartEnemyAttack())", StringComparison.Ordinal);
        Assert.GreaterOrEqual(failedAttackBranch, 0);

        int failedAttackReturn = methodBody.IndexOf("return;", failedAttackBranch, StringComparison.Ordinal);
        Assert.Greater(failedAttackReturn, failedAttackBranch);

        string failedBranchBody = methodBody.Substring(failedAttackBranch, failedAttackReturn - failedAttackBranch);
        StringAssert.Contains("EndEnemyTurn();", failedBranchBody);
    }

    [Test]
    public void BattleSceneRuntime_StartTurnFlow_DoesNotRestartAlreadyStartedTurnFlow()
    {
        string source = File.ReadAllText("Assets/Script/Battle/0_Presentation/Runtime/BattleSceneRuntime.cs");
        string methodBody = GetMethodBody(source, "private void StartTurnFlow()");

        StringAssert.Contains("if (hasStartedTurnFlow)", methodBody);
        StringAssert.Contains("hasStartedTurnFlow = true;", methodBody);
    }

    [Test]
    public void EnemyTurnRuntime_AttackStartFailureLog_IsReadable()
    {
        string source = File.ReadAllText("Assets/Script/Battle/0_Presentation/Enemy/EnemyTurnRuntime.cs");
        string methodBody = GetMethodBody(source, "private void ExecuteEnemyTurn()");

        StringAssert.Contains("EnemyTurnRuntime이 적 공격을 시작하지 못했습니다.", methodBody);
        StringAssert.DoesNotContain("???", methodBody);
    }

    private static string GetMethodBody(string source, string methodSignature)
    {
        int methodStart = source.IndexOf(methodSignature, StringComparison.Ordinal);
        Assert.GreaterOrEqual(methodStart, 0, methodSignature);

        int bodyStart = source.IndexOf('{', methodStart);
        Assert.Greater(bodyStart, methodStart, methodSignature);

        int depth = 0;
        for (int i = bodyStart; i < source.Length; i++)
        {
            if (source[i] == '{')
            {
                depth++;
            }
            else if (source[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source.Substring(bodyStart, i - bodyStart + 1);
                }
            }
        }

        Assert.Fail("메서드 본문을 찾지 못했습니다: " + methodSignature);
        return string.Empty;
    }
}
