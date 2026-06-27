using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// 역할: BattleSceneRuntime의 책임을 정의한다.
public class BattleSceneRuntime : MonoBehaviour, IPlayerTurnProgress
{
    [SerializeField] private PlayerCommandController player;
    [FormerlySerializedAs("enermy")]
    [SerializeField] private EnemyTurnRuntime enemyTurnController;
    [FormerlySerializedAs("plateController")]
    [SerializeField] private PlateBoardView plateBoardController;
    [SerializeField] private BattleResultController battleResultController;
    [FormerlySerializedAs("stageContext")]
    [SerializeField] private BattleRuntimeData battleRuntimeData;
    [SerializeField] private StageEnemyPlacementData stageEnemyPlacementData;
    [Header("플레이어 UI")]
    [FormerlySerializedAs("manaList")]
    [SerializeField] private List<RawImage> manaList;
    [FormerlySerializedAs("notHaveTexture")]
    [SerializeField] private Texture notHaveTexture;
    [FormerlySerializedAs("haveTexture")]
    [SerializeField] private Texture haveTexture;
    [FormerlySerializedAs("summonButton")]
    [SerializeField] private Button summonButton;
    [FormerlySerializedAs("reSummonButton")]
    [SerializeField] private Button reSummonButton;
    [FormerlySerializedAs("statePanel")]
    [SerializeField] private Image statePanel;
    [FormerlySerializedAs("clickSound")]
    [SerializeField] private AudioSource clickSound;
    [FormerlySerializedAs("failSound")]
    [SerializeField] private AudioSource failSound;
    [FormerlySerializedAs("summonController")]
    [SerializeField] private SummonSelectionController summonSelectionController;
    [FormerlySerializedAs("stageBattleRuleData")]
    [SerializeField] private StageResultData stageResultData;
    [FormerlySerializedAs("defaultClearTurn")]
    [SerializeField] private int clearTurn = 1;
    [Header("턴 UI")]
    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnClearText;

    private readonly TurnStateMachine turnStateMachine = new TurnStateMachine();
    private readonly StartBattleUseCase startBattleUseCase = new StartBattleUseCase();
    private readonly StageDataStore stageDataStore = new StageDataStore();
    private TurnSummonStateUpdater turnSummonStateUpdater;
    private ChangeTurnUseCase changeTurnUseCase;
    private HandlePlayerCommandUseCase handlePlayerCommandUseCase;
    private PlayerTurnState playerTurnState;
    private EnemyTurnState enemyTurnState;
    private AttackStateMachineHost attackStateMachineHost;
    private PlayerHudView playerHudView;
    private ManaView manaView;
    private PlayerFeedbackView feedbackView;
    private SummonStatePanelView summonStatePanelView;
    private int resolvedClearTurn;
    private bool hasStartedBattleFlow;
    private bool hasStartedTurnFlow;

    private void Start()
    {
        StartBattleFlow();
    }

    private void StartBattleFlow()
    {
        if (hasStartedBattleFlow)
        {
            return;
        }

        Ensure참조();

        if (!CanStartTurnFlow())
        {
            return;
        }

        hasStartedBattleFlow = true;
        startBattleUseCase.Execute(
            battleRuntimeData.StageData,
            plateBoardController.GetEnemyPlates(),
            stageEnemyPlacementData);
        StartTurnFlow();
    }

    internal void StartTurnFlow()
    {
        if (hasStartedTurnFlow)
        {
            return;
        }

        Ensure참조();

        if (!CanStartTurnFlow())
        {
            return;
        }

        hasStartedTurnFlow = true;
        GetChangeTurnUseCase().StartPlayerTurn();
        ShowTurnProgress();
    }

    public void EndPlayerTurn()
    {
        GetChangeTurnUseCase().TryEndPlayerTurn();
        ShowTurnProgress();
    }

    public void CompleteEnemyTurn()
    {
        GetChangeTurnUseCase().TryCompleteEnemyTurn();
        ShowTurnProgress();
    }

    public int GetTurnCount()
    {
        return turnStateMachine.TurnCount;
    }

    public int GetClearTurn()
    {
        return resolvedClearTurn;
    }

    public bool IsPlayerTurn()
    {
        return turnStateMachine.IsPlayerTurn();
    }

    internal TurnPhase GetCurrentTurn()
    {
        return turnStateMachine.CurrentTurn;
    }

    private void Ensure참조()
    {
        EnsurePlateBoardView();
        EnsureAttackStateMachineHost();
        EnsureBattleResultController();
        EnsureBattleRuntimeData();
        EnsureStageResultData();
        ResolveClearTurn();
        EnsureTurnProgressText();
    }

    private void EnsurePlateBoardView()
    {
        if (plateBoardController == null)
        {
            plateBoardController = FindObjectOfType<PlateBoardView>();
        }

        if (plateBoardController == null)
        {
            Debug.LogError("BattleSceneRuntime에 PlateBoardView가 필요합니다.");
        }
    }

    private TurnSummonStateUpdater GetTurnSummonStateUpdater()
    {
        if (turnSummonStateUpdater == null)
        {
            turnSummonStateUpdater = new TurnSummonStateUpdater(new TurnSummonBoard(plateBoardController));
        }

        return turnSummonStateUpdater;
    }

    private ChangeTurnUseCase GetChangeTurnUseCase()
    {
        if (changeTurnUseCase == null)
        {
            changeTurnUseCase = new ChangeTurnUseCase(
                turnStateMachine,
                GetPlayerTurnState,
                GetEnemyTurnState);
        }

        return changeTurnUseCase;
    }

    private PlayerTurnState GetPlayerTurnState()
    {
        if (playerTurnState == null)
        {
            playerTurnState = new PlayerTurnState(
                GetHandlePlayerCommandUseCase(),
                new PlayerTurnBoard(plateBoardController),
                () => turnStateMachine.TurnCount,
                GetTurnSummonStateUpdater(),
                attackStateMachineHost.GetAttackStateMachine());
        }

        return playerTurnState;
    }

    private HandlePlayerCommandUseCase GetHandlePlayerCommandUseCase()
    {
        if (handlePlayerCommandUseCase == null)
        {
            handlePlayerCommandUseCase = CreateHandlePlayerCommandUseCase();
            player.SetHandlePlayerCommandUseCase(handlePlayerCommandUseCase);
            ConnectSummonStatePanelView();
            handlePlayerCommandUseCase.ResetPlayerSetting();
        }

        return handlePlayerCommandUseCase;
    }

    private HandlePlayerCommandUseCase CreateHandlePlayerCommandUseCase()
    {
        AttackStateMachine attackStateMachine = attackStateMachineHost.GetAttackStateMachine();
        var playerActionStateMachine = new PlayerActionStateMachine();
        var startSummonSelectionUseCase = new StartSummonSelectionUseCase(
            summonSelectionController,
            plateBoardController.GetFirstEmptyPlayerPlateIndex,
            playerActionStateMachine,
            GetPlayerFeedbackView());
        IReadOnlyList<BattleBoardInputController> playerPlates = plateBoardController.GetPlayerPlates();
        IReadOnlyList<BattleBoardInputController> enemyPlates = plateBoardController.GetEnemyPlates();
        var targetSelectionView = new PlayerTargetSelectionView(summonSelectionController, plateBoardController);
        var attackUseCase = new ExecutePlayerAttackUseCase(
            player,
            attackStateMachine,
            playerPlates,
            enemyPlates,
            new BoardOutsideClickInput(),
            targetSelectionView,
            GetPlayerFeedbackView());

        return new HandlePlayerCommandUseCase(
            player.gameObject.name,
            playerActionStateMachine,
            this,
            battleResultController,
            plateBoardController,
            startSummonSelectionUseCase,
            attackUseCase,
            GetPlayerHudView(),
            GetManaView(),
            GetPlayerFeedbackView());
    }

    private PlayerHudView GetPlayerHudView()
    {
        if (playerHudView == null)
        {
            playerHudView = new PlayerHudView(
                summonButton,
                reSummonButton,
                statePanel);
        }

        return playerHudView;
    }

    private ManaView GetManaView()
    {
        if (manaView == null)
        {
            manaView = new ManaView(
                manaList,
                notHaveTexture,
                haveTexture);
        }

        return manaView;
    }

    private PlayerFeedbackView GetPlayerFeedbackView()
    {
        if (feedbackView == null)
        {
            feedbackView = new PlayerFeedbackView(clickSound, failSound);
        }

        return feedbackView;
    }

    private void ConnectSummonStatePanelView()
    {
        if (statePanel != null)
        {
            summonStatePanelView = statePanel.GetComponent<SummonStatePanelView>();
        }

        if (summonStatePanelView == null)
        {
            summonStatePanelView = FindObjectOfType<SummonStatePanelView>();
        }

        if (summonStatePanelView == null)
        {
            Debug.LogError("BattleSceneRuntime에 SummonStatePanelView가 필요합니다.");
            return;
        }

        summonStatePanelView.SetSpecialAttackRequestedHandler(player.OnClickSpecialAttack);
    }

    private EnemyTurnState GetEnemyTurnState()
    {
        if (enemyTurnState == null)
        {
            enemyTurnState = new EnemyTurnState(
                enemyTurnController,
                GetTurnSummonStateUpdater());
        }

        return enemyTurnState;
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
            Debug.LogError("BattleSceneRuntime에 BattleResultController가 필요합니다.");
        }
    }

    private void EnsureAttackStateMachineHost()
    {
        if (attackStateMachineHost != null)
        {
            return;
        }

        attackStateMachineHost = GetComponent<AttackStateMachineHost>();
        if (attackStateMachineHost == null)
        {
            attackStateMachineHost = FindObjectOfType<AttackStateMachineHost>();
        }

        if (attackStateMachineHost == null)
        {
            Debug.LogError("BattleSceneRuntime에 AttackStateMachineHost가 필요합니다.");
        }
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

    private void EnsureStageResultData()
    {
        if (stageResultData != null)
        {
            return;
        }

        stageResultData = stageDataStore.LoadStageResultData();
    }

    private void ResolveClearTurn()
    {
        resolvedClearTurn = clearTurn;

        if (battleRuntimeData == null || stageResultData == null)
        {
            return;
        }

        resolvedClearTurn = stageResultData.GetClearTurn(battleRuntimeData.CurrentStage, clearTurn);
    }

    private void EnsureTurnProgressText()
    {
        if (turnCountText == null)
        {
            GameObject turnCountObject = GameObject.Find("CurrentTurnText");
            if (turnCountObject != null)
            {
                turnCountText = turnCountObject.GetComponent<TextMeshProUGUI>();
            }
        }

        if (turnClearText == null)
        {
            GameObject turnClearObject = GameObject.Find("ClearTurnText");
            if (turnClearObject != null)
            {
                turnClearText = turnClearObject.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void ShowTurnProgress()
    {
        if (turnCountText != null)
        {
            turnCountText.text = $"Current Turn : {GetTurnCount()}";
        }

        if (turnClearText != null)
        {
            turnClearText.text = $"Clear Turn : {GetClearTurn()}";
        }
    }

    private bool CanStartTurnFlow()
    {
        if (player == null)
        {
            Debug.LogError("Player 없이는 BattleSceneRuntime을 시작할 수 없습니다.");
            return false;
        }

        if (enemyTurnController == null)
        {
            Debug.LogError("EnemyTurnRuntime 없이는 BattleSceneRuntime을 시작할 수 없습니다.");
            return false;
        }

        if (plateBoardController == null)
        {
            Debug.LogError("PlateBoardView 없이는 BattleSceneRuntime을 시작할 수 없습니다.");
            return false;
        }

        if (attackStateMachineHost == null)
        {
            Debug.LogError("AttackStateMachineHost 없이는 BattleSceneRuntime을 시작할 수 없습니다.");
            return false;
        }

        if (battleResultController == null)
        {
            Debug.LogError("BattleResultController 없이는 BattleSceneRuntime을 시작할 수 없습니다.");
            return false;
        }

        return true;
    }
}
