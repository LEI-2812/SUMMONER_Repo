using UnityEngine;

// 역할: AttackStateMachineHost의 책임을 정의한다.
public sealed class AttackStateMachineHost : MonoBehaviour
{
    private BattleRuntimeData battleRuntimeData;
    private AttackStateMachine attackStateMachine;

    private void Awake()
    {
        EnsureAttackStateMachine();
    }

    public AttackStateMachine GetAttackStateMachine()
    {
        EnsureAttackStateMachine();
        return attackStateMachine;
    }

    private void EnsureAttackStateMachine()
    {
        if (attackStateMachine != null)
        {
            return;
        }

        EnsureBattleRuntimeData();
        if (battleRuntimeData == null)
        {
            Debug.LogError("AttackStateMachineHost에 BattleRuntimeData가 필요합니다.");
            return;
        }

        attackStateMachine = new AttackStateMachine(battleRuntimeData.AttackData);
    }

    private void EnsureBattleRuntimeData()
    {
        if (battleRuntimeData != null)
        {
            return;
        }

        battleRuntimeData = GetComponent<BattleRuntimeData>();
        if (battleRuntimeData == null)
        {
            battleRuntimeData = FindObjectOfType<BattleRuntimeData>();
        }
    }
}