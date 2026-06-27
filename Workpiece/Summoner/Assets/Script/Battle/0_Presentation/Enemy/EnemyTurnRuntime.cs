using UnityEngine;

// 역할: EnemyTurnRuntime의 책임을 정의한다.
public class EnemyTurnRuntime : MonoBehaviour
{
    [UnityEngine.Serialization.FormerlySerializedAs("turnController")]
    [UnityEngine.Serialization.FormerlySerializedAs("turnRuntime")]
    [SerializeField] private BattleSceneRuntime battleSceneRuntime;
    [UnityEngine.Serialization.FormerlySerializedAs("plateController")]
    [SerializeField] private PlateBoardView plateBoardController;
    [SerializeField] private PlayerAttackPrediction playerAttackPrediction;
    [UnityEngine.Serialization.FormerlySerializedAs("battleController")]
    [UnityEngine.Serialization.FormerlySerializedAs("attackStateMachineProvider")]
    [SerializeField] private AttackStateMachineHost attackStateMachineHost;

    private AttackStateMachine attackStateMachine;
    private ExecuteEnemyTurnUseCase executeEnemyTurnUseCase;

    private void Awake()
    {
        if (battleSceneRuntime == null)
        {
            battleSceneRuntime = FindObjectOfType<BattleSceneRuntime>();
        }

        if (plateBoardController == null)
        {
            Debug.LogError("EnemyTurnRuntime에 PlateBoardView가 필요합니다.");
        }

        if (playerAttackPrediction == null)
        {
            Debug.LogError("EnemyTurnRuntime에 PlayerAttackPrediction이 필요합니다.");
        }

        if (attackStateMachineHost == null)
        {
            Debug.LogError("EnemyTurnRuntime에 AttackStateMachineHost가 필요합니다.");
        }

        if (battleSceneRuntime == null)
        {
            Debug.LogError("EnemyTurnRuntime에 BattleSceneRuntime이 필요합니다.");
        }
    }

    public void StartEnemyTurn()
    {
        Debug.Log("적 턴을 시작했습니다.");
        ExecuteEnemyTurn();
    }

    private void ExecuteEnemyTurn()
    {
        if (!CanRunEnemyTurn())
        {
            return;
        }

        if (!TryStartEnemyAttack())
        {
            EndEnemyTurn();
            Debug.LogError("EnemyTurnRuntime이 적 공격을 시작하지 못했습니다.");
            return;
        }

        EndEnemyTurn();
    }

    private void EndEnemyTurn()
    {
        Debug.Log("적 턴을 종료했습니다.");
        battleSceneRuntime.CompleteEnemyTurn();
    }

    private bool CanRunEnemyTurn()
    {
        if (battleSceneRuntime == null)
        {
            Debug.LogError("BattleSceneRuntime 없이는 EnemyTurnRuntime이 턴을 종료할 수 없습니다.");
            return false;
        }

        return true;
    }

    private bool TryStartEnemyAttack()
    {
        if (!CanStartEnemyAttack())
        {
            return false;
        }

        GetExecuteEnemyTurnUseCase().Execute();
        return true;
    }

    private ExecuteEnemyTurnUseCase GetExecuteEnemyTurnUseCase()
    {
        if (executeEnemyTurnUseCase == null)
        {
            executeEnemyTurnUseCase = new ExecuteEnemyTurnUseCase(
                plateBoardController,
                playerAttackPrediction,
                GetAttackStateMachine());
        }

        return executeEnemyTurnUseCase;
    }

    private AttackStateMachine GetAttackStateMachine()
    {
        if (attackStateMachine == null)
        {
            attackStateMachine = attackStateMachineHost.GetAttackStateMachine();
        }

        return attackStateMachine;
    }

    private bool CanStartEnemyAttack()
    {
        if (plateBoardController == null)
        {
            Debug.LogError("PlateBoardView 없이는 EnemyTurnRuntime이 공격할 수 없습니다.");
            return false;
        }

        if (playerAttackPrediction == null)
        {
            Debug.LogError("PlayerAttackPrediction 없이는 EnemyTurnRuntime이 공격할 수 없습니다.");
            return false;
        }

        if (attackStateMachineHost == null)
        {
            Debug.LogError("AttackStateMachineHost 없이는 EnemyTurnRuntime이 공격할 수 없습니다.");
            return false;
        }

        return true;
    }
}
