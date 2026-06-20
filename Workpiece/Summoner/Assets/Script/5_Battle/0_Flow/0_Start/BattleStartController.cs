using UnityEngine;

[RequireComponent(typeof(BattleEnemyPlacementController))]
// 역할: 전투 시작 시 적 배치와 첫 턴 초기화를 순서대로 실행한다.
public class BattleStartController : MonoBehaviour
{
    [SerializeField] private BattleEnemyPlacementController enemyPlacementController;
    [SerializeField] private TurnController turnController;

    private bool hasStartedBattle;

    private void Awake()
    {
        ReferencesEnsure();
    }

    private void Start()
    {
        StartBattleFlow();
    }

    private void StartBattleFlow()
    {
        if (hasStartedBattle)
        {
            return;
        }

        ReferencesEnsure();

        if (enemyPlacementController == null || turnController == null)
        {
            Debug.LogWarning("전투 시작에 필요한 참조가 없어 전투 시작 흐름을 실행하지 않습니다.");
            return;
        }

        hasStartedBattle = true;
        enemyPlacementController.EnemyPlacementApply();
        turnController.StartBattleTurnFlow();
    }

    private void ReferencesEnsure()
    {
        if (enemyPlacementController == null)
        {
            enemyPlacementController = GetComponent<BattleEnemyPlacementController>();
        }

        if (turnController == null)
        {
            turnController = FindObjectOfType<TurnController>();
        }
    }
}
