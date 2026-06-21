using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 역할: Unity 이벤트와 씬 연결을 유지하고 플레이어 행동, UI, 피드백 책임을 연결한다.
public class PlayerController : MonoBehaviour
{
    [Header("마나UI")]
    [SerializeField] private List<RawImage> manaList;

    [Header("마나텍스쳐")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;

    [Header("버튼 UI")]
    [SerializeField] private Button summonButton;
    [SerializeField] private Button reSummonButton;

    [Header("상태창 패널")]
    [SerializeField] private Image statePanel;

    [Header("효과음")]
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource failSound;

    [Header("컨트롤러")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private TurnController turnController;
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;
    [SerializeField] private BattleResultController battleResultController;

    private PlayerTurnFlow playerTurnFlow;
    private PlayerView playerView;
    private PlayerFeedbackView feedbackView;

    public int currentTurn;
    public int clearTurn;

    private void Awake()
    {
        EnsureBattleResultController();

        playerTurnFlow = CreatePlayerTurnFlow();
        clearTurn = playerTurnFlow.GetClearTurn();
        playerTurnFlow.ResetPlayerSetting();
    }

    private PlayerTurnFlow CreatePlayerTurnFlow()
    {
        playerView = new PlayerView(
            manaList,
            notHaveTexture,
            haveTexture,
            summonButton,
            reSummonButton,
            statePanel);

        feedbackView = new PlayerFeedbackView(clickSound, failSound);
        var resourceState = new PlayerTurnResourceState();
        var turnProgressState = new PlayerTurnProgressState();
        var turnActions = new PlayerTurnActions(
            turnController,
            battleResultController);
        var actionExecutor = new PlayerActionExecutor();
        var summonActions = new PlayerSummonActions(
            summonController,
            plateController,
            resourceState,
            actionExecutor,
            playerView,
            feedbackView);
        var attackActions = new PlayerAttackActions(
            this,
            summonController,
            battleController,
            plateController,
            turnProgressState,
            turnActions,
            playerView,
            feedbackView);

        return new PlayerTurnFlow(
            this,
            summonController,
            battleController,
            resourceState,
            turnProgressState,
            turnActions,
            summonActions,
            attackActions,
            playerView,
            feedbackView);
    }

    private void Update()
    {
        playerTurnFlow.UpdateRedrawButtonState();
    }

    public void PlayerTurnStart()
    {
        playerTurnFlow.StartPlayerTurn();
    }

    public void OnSummonBtnClick()
    {
        playerTurnFlow.TryStartSummon();
    }

    public void PlayerTurnOverBtn()
    {
        playerTurnFlow.TryEndPlayerTurn();
    }

    public void OnRedrawButtonClick()
    {
        playerTurnFlow.TryStartRedraw();
    }

    public void OnAttackBtnClick()
    {
        playerTurnFlow.TryExecuteNormalAttack();
    }

    public void OnSpecialAttackBtnClick()
    {
        playerTurnFlow.TryExecuteSpecialAttack();
    }

    public void SetHasSummonedThisTurn(bool value)
    {
        playerTurnFlow.SetHasSummonedThisTurn(value);
    }

    public bool HasSummonedThisTurn()
    {
        return playerTurnFlow.HasSummonedThisTurn();
    }

    public void UpdateManaUI()
    {
        playerTurnFlow.UpdateManaUI();
    }

    public void AddMana()
    {
        playerTurnFlow.AddMana();
    }

    private void EnsureBattleResultController()
    {
        if (battleResultController != null)
        {
            return;
        }

        battleResultController = GetComponent<BattleResultController>();
        if (battleResultController == null)
        {
            battleResultController = FindObjectOfType<BattleResultController>();
        }

        if (battleResultController == null)
        {
            Debug.LogError("PlayerController needs BattleResultController.");
        }
    }
}
