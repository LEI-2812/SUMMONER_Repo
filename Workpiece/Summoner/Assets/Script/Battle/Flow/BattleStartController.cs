using UnityEngine;

[RequireComponent(typeof(BattleEnemyPlacementController))]
public class BattleStartController : MonoBehaviour
{
    [SerializeField] private BattleEnemyPlacementController enemyPlacementController;
    [SerializeField] private TurnController turnController;

    private void Awake()
    {
        ReferencesEnsure();
    }

    private void Start()
    {
        BattleStartInitialize();
    }

    public void BattleStartInitialize()
    {
        ReferencesEnsure();

        if (enemyPlacementController == null || turnController == null)
        {
            Debug.LogWarning("전투 시작에 필요한 참조가 없어 전투 시작 흐름을 실행하지 않습니다.");
            return;
        }

        enemyPlacementController.EnemyPlacementApply();
        turnController.TurnStartInitialize();
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
