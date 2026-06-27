using UnityEngine;

// 역할: PlayerCommandController의 책임을 정의한다.
public class PlayerCommandController : MonoBehaviour, ICoroutineRunner
{
    private HandlePlayerCommandUseCase handlePlayerCommandUseCase;

    internal void SetHandlePlayerCommandUseCase(HandlePlayerCommandUseCase useCase)
    {
        handlePlayerCommandUseCase = useCase;
    }

    public void OnClickSummon()
    {
        handlePlayerCommandUseCase?.ExecuteSummon();
    }

    public void OnClickEndTurn()
    {
        handlePlayerCommandUseCase?.ExecuteEndTurn();
    }

    public void OnClickRedraw()
    {
        handlePlayerCommandUseCase?.ExecuteRedraw();
    }

    public void OnClickNormalAttack()
    {
        handlePlayerCommandUseCase?.ExecuteNormalAttack();
    }

    public void OnClickSpecialAttack()
    {
        handlePlayerCommandUseCase?.ExecuteSpecialAttack();
    }

    public void OnClickSpecialAttack(int specialAttackIndex)
    {
        handlePlayerCommandUseCase?.ExecuteSpecialAttack(specialAttackIndex);
    }
}