using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

// 역할: BattleSceneRuntime의 책임을 정의한다.
public class BattleSceneRuntime : MonoBehaviour, IPlayerTurnProgress
{
    [SerializeField] private PlayerCommandController player;
    [SerializeField] private EnemyTurnRuntime enemyTurnController;
    [SerializeField] private PlateBoardView plateBoardController;
    [SerializeField] private BattleResultController battleResultController;
    [SerializeField] private int defaultStage = 1;
    [SerializeField] private StageEnemyPlacementData stageEnemyPlacementData;
    [Header("플레이어 UI")]
    [SerializeField] private List<RawImage> manaList;
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;
    [SerializeField] private Button summonButton;
    [SerializeField] private Button reSummonButton;
    [SerializeField] private Image statePanel;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource failSound;
    [SerializeField] private SummonSelectionController summonSelectionController;
    [SerializeField] private StageResultData stageResultData;
    [SerializeField] private int clearTurn = 1;
    [Header("턴 UI")]
    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnClearText;

    private readonly TurnStateMachine turnStateMachine = new TurnStateMachine();
    private readonly StartBattleUseCase startBattleUseCase = new StartBattleUseCase();
    private readonly StageDataStore stageDataStore = new StageDataStore();
    private UpdateTurnSummonStateUseCase updateTurnSummonStateUseCase;
    private ChangeTurnUseCase changeTurnUseCase;
    private HandlePlayerCommandUseCase handlePlayerCommandUseCase;
    private PlayerTurnState playerTurnState;
    private EnemyTurnState enemyTurnState;
    private BattleStageData battleStageData;
    private BattleAttackData battleAttackData;
    private AttackStateMachine attackStateMachine;
    private PlayerHudView playerHudView;
    private ManaView manaView;
    private PlayerFeedbackView feedbackView;
    private SummonStatePanelView summonStatePanelView;
    private int resolvedClearTurn;
    private bool hasStartedBattleFlow;
    private bool hasStartedTurnFlow;

    private void Awake()
    {
        CreateBattleState();
        Ensure참조();
        ConnectBattleState();
    }

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
        IReadOnlyList<BattleBoardInputController> enemyPlates = plateBoardController.GetEnemyPlates();
        StartBattleResult startResult = startBattleUseCase.Execute(
            battleStageData,
            stageEnemyPlacementData,
            enemyPlates == null ? 0 : enemyPlates.Count);
        ApplyStartBattleResult(startResult, enemyPlates);
        StartTurnFlow();
    }

    private void ApplyStartBattleResult(
        StartBattleResult result,
        IReadOnlyList<BattleBoardInputController> enemyPlates)
    {
        if (result == null)
        {
            return;
        }

        Summon.StatMultiplierSet(result.SummonStatMultiplier);

        for (int index = 0; index < result.Warnings.Count; index++)
        {
            Debug.LogWarning(result.Warnings[index]);
        }

        if (enemyPlates == null)
        {
            return;
        }

        for (int index = 0; index < result.EnemyPlacements.Count; index++)
        {
            EnemyPlacementResult placement = result.EnemyPlacements[index];
            BattleBoardInputController targetPlate = enemyPlates[placement.PlateIndex];
            if (targetPlate == null
                || targetPlate.GetCurrentSummon() != null
                || targetPlate.GetComponentInChildren<Summon>(true) != null)
            {
                continue;
            }

            targetPlate.SummonPlaceOnPlate(placement.EnemySummonPrefab);
            targetPlate.GetCurrentSummon()?.ApplyStageMultiplier(result.SummonStatMultiplier);
        }
    }

    private void StartTurnFlow()
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

    private void Ensure참조()
    {
        EnsurePlateBoardView();
        EnsureBattleResultController();
        EnsureStageResultData();
        ResolveClearTurn();
        EnsureTurnProgressText();
    }

    private void CreateBattleState()
    {
        battleStageData = new BattleStageData(defaultStage);
        battleAttackData = new BattleAttackData();
        attackStateMachine = new AttackStateMachine(battleAttackData);
    }

    private void ConnectBattleState()
    {
        if (enemyTurnController != null)
        {
            enemyTurnController.ConnectAttackState(attackStateMachine);
        }

        if (battleResultController != null)
        {
            battleResultController.ConnectBattleStage(battleStageData);
        }

        if (plateBoardController == null)
        {
            return;
        }

        ConnectBoardInputs(plateBoardController.GetPlayerPlates());
        ConnectBoardInputs(plateBoardController.GetEnemyPlates());
    }

    private void ConnectBoardInputs(IReadOnlyList<BattleBoardInputController> boardInputs)
    {
        if (boardInputs == null)
        {
            return;
        }

        for (int index = 0; index < boardInputs.Count; index++)
        {
            if (boardInputs[index] != null)
            {
                boardInputs[index].ConnectAttackState(attackStateMachine);
            }
        }
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

    private UpdateTurnSummonStateUseCase GetUpdateTurnSummonStateUseCase()
    {
        if (updateTurnSummonStateUseCase == null)
        {
            updateTurnSummonStateUseCase = new UpdateTurnSummonStateUseCase(
                new TurnSummonBoard(plateBoardController));
        }

        return updateTurnSummonStateUseCase;
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
            GetHandlePlayerCommandUseCase();
            playerTurnState = new PlayerTurnState(
                player.StartPlayerTurn,
                player.AddMana,
                player.TryStopTurnForClearResult,
                new PlayerTurnBoard(plateBoardController),
                GetUpdateTurnSummonStateUseCase(),
                attackStateMachine);
        }

        return playerTurnState;
    }

    private HandlePlayerCommandUseCase GetHandlePlayerCommandUseCase()
    {
        if (handlePlayerCommandUseCase == null)
        {
            handlePlayerCommandUseCase = CreateHandlePlayerCommandUseCase();
            player.ConnectPlayerCommand(
                handlePlayerCommandUseCase,
                summonSelectionController,
                this,
                battleResultController,
                plateBoardController,
                GetPlayerHudView(),
                GetManaView(),
                GetPlayerFeedbackView(),
                plateBoardController.GetPlayerPlates(),
                plateBoardController.GetEnemyPlates());
            ConnectSummonStatePanelView();
            player.ResetPlayerSetting();
        }

        return handlePlayerCommandUseCase;
    }

    private HandlePlayerCommandUseCase CreateHandlePlayerCommandUseCase()
    {
        var playerActionStateMachine = new PlayerActionStateMachine();
        var startSummonSelectionUseCase = new StartSummonSelectionUseCase(
            plateBoardController.GetFirstEmptyPlayerPlateIndex,
            () => !plateBoardController.IsPlayerPlateClear(),
            playerActionStateMachine);
        IReadOnlyList<BattleBoardInputController> playerPlates = plateBoardController.GetPlayerPlates();
        IReadOnlyList<BattleBoardInputController> enemyPlates = plateBoardController.GetEnemyPlates();
        var attackUseCase = new ExecutePlayerAttackUseCase(
            attackStateMachine,
            plateBoardController.GetBattleBoardData());

        return new HandlePlayerCommandUseCase(
            playerActionStateMachine,
            this,
            startSummonSelectionUseCase,
            attackUseCase);
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
                enemyTurnController.StartEnemyTurn,
                GetUpdateTurnSummonStateUseCase());
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

        if (battleStageData == null || stageResultData == null)
        {
            return;
        }

        resolvedClearTurn = stageResultData.GetClearTurn(battleStageData.CurrentStage, clearTurn);
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

        if (attackStateMachine == null)
        {
            Debug.LogError("AttackStateMachine 없이는 BattleSceneRuntime을 시작할 수 없습니다.");
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
