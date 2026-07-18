using UnityEngine;

// 역할: EnemyTurnRuntime의 책임을 정의한다.
public class EnemyTurnRuntime : MonoBehaviour
{
    [SerializeField] private BattleSceneRuntime battleSceneRuntime;
    [SerializeField] private PlateBoardView plateBoardController;
    private PlayerAttackPrediction playerAttackPrediction;

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

        playerAttackPrediction = new PlayerAttackPrediction();

        if (battleSceneRuntime == null)
        {
            Debug.LogError("EnemyTurnRuntime에 BattleSceneRuntime이 필요합니다.");
        }
    }

    public void ConnectAttackState(AttackStateMachine attackStateMachine)
    {
        this.attackStateMachine = attackStateMachine;
        executeEnemyTurnUseCase = null;
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
        plateBoardController.ResetAllPlateHighlight();
        return true;
    }

    private ExecuteEnemyTurnUseCase GetExecuteEnemyTurnUseCase()
    {
        if (executeEnemyTurnUseCase == null)
        {
            executeEnemyTurnUseCase = new ExecuteEnemyTurnUseCase(
                plateBoardController.GetBattleBoardData(),
                playerAttackPrediction,
                attackStateMachine);
        }

        return executeEnemyTurnUseCase;
    }

    private bool CanStartEnemyAttack()
    {
        if (plateBoardController == null)
        {
            Debug.LogError("PlateBoardView 없이는 EnemyTurnRuntime이 공격할 수 없습니다.");
            return false;
        }

        if (attackStateMachine == null)
        {
            Debug.LogError("AttackStateMachine 없이는 EnemyTurnRuntime이 공격할 수 없습니다.");
            return false;
        }

        return true;
    }
}
