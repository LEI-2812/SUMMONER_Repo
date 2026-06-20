using TMPro;
using UnityEngine;

// 역할: 플레이어 턴과 적 턴을 전환하고 턴 시작/종료 상태 갱신을 실행한다.
public class TurnController : MonoBehaviour
{
    
    [SerializeField] private PlayerController player;
    [SerializeField] private Enermy enermy;
    [SerializeField] private PlateController plateController;
    [SerializeField] private BattleResultController battleResultController;
    // 역할: 현재 턴이 플레이어 턴인지 적 턴인지 나타낸다.
    public enum Turn { PlayerTurn, EnermyTurn }
    [SerializeField] private int clearTurn;

    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnClearText;

    private readonly TurnProgressState turnProgressState = new TurnProgressState();
    private TurnSummonStateUpdater turnSummonStateUpdater;
    private TurnPhaseActions turnPhaseActions;
    private TurnView turnView;
    

    internal void StartBattleTurnFlow()
    {
        EnsurePlateController();
        EnsureBattleResultController();

        if (!CanStartTurnFlow())
        {
            return;
        }

        InitializeFirstTurn();
        GetTurnPhaseActions().StartCurrentTurn();
    }

    private void EnsurePlateController()
    {
        if (plateController == null)
        {
            plateController = FindObjectOfType<PlateController>();
        }

        if (plateController == null)
        {
            Debug.LogError("TurnController needs PlateController.");
        }
    }

    private void InitializeFirstTurn()
    {
        turnProgressState.InitializeFirstTurn();
        GetTurnView().ShowTurnCount(turnProgressState.TurnCount);
        GetTurnView().ShowClearTurn(clearTurn);
    }

    internal void EndCurrentTurn()
    {
        GetTurnPhaseActions().EndCurrentTurn();
    }

    private TurnSummonStateUpdater GetTurnSummonStateUpdater()
    {
        if (turnSummonStateUpdater == null)
        {
            turnSummonStateUpdater = new TurnSummonStateUpdater(plateController);
        }

        return turnSummonStateUpdater;
    }

    private TurnView GetTurnView()
    {
        if (turnView == null)
        {
            turnView = new TurnView(turnCountText, turnClearText);
        }

        return turnView;
    }

    private TurnPhaseActions GetTurnPhaseActions()
    {
        if (turnPhaseActions == null)
        {
            turnPhaseActions = new TurnPhaseActions(
                player,
                enermy,
                plateController,
                battleResultController,
                clearTurn,
                turnProgressState,
                GetTurnSummonStateUpdater(),
                GetTurnView());
        }

        return turnPhaseActions;
    }

    public Turn GetCurrentTurn()
    {
        return turnProgressState.CurrentTurn;
    }

    public int GetTurnCount()
    {
        return turnProgressState.TurnCount;
    }

    public int GetClearTurn()
    {
        return clearTurn;
    }
    private void EnsureBattleResultController()
    {
        if (battleResultController != null)
        {
            return;
        }

        if (player != null)
        {
            battleResultController = player.GetComponent<BattleResultController>();
        }

        if (battleResultController == null)
        {
            battleResultController = FindObjectOfType<BattleResultController>();
        }

        if (battleResultController == null)
        {
            Debug.LogError("TurnController needs BattleResultController.");
        }
    }

    private bool CanStartTurnFlow()
    {
        if (player == null)
        {
            Debug.LogError("TurnController cannot start without Player.");
            return false;
        }

        if (enermy == null)
        {
            Debug.LogError("TurnController cannot start without Enermy.");
            return false;
        }

        if (plateController == null)
        {
            Debug.LogError("TurnController cannot start without PlateController.");
            return false;
        }

        if (battleResultController == null)
        {
            Debug.LogError("TurnController cannot start without BattleResultController.");
            return false;
        }

        return true;
    }
} 
