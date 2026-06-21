using System.Collections.Generic;
using UnityEngine;

// 역할: 적 소환수들을 순서대로 찾아 한 적의 행동 실행 흐름에 넘긴다.
public class EnermyAttackController : MonoBehaviour
{
    // 역할: 적 플레이트 순회와 적 소환수 수 조회에 사용한다.
    [SerializeField] private PlateController plateController;

    // 역할: 현재 플레이어 소환수들의 다음 공격 예측 목록을 만든다.
    [SerializeField] private PlayerAttackPrediction playerAttackPrediction;

    // 역할: 적 대응 특수 공격을 전투 실행 흐름으로 연결한다.
    [SerializeField] private BattleController battleController;

    // 역할: 적 소환수 1명 단위의 실제 턴 행동 실행 객체를 저장해 재사용한다.
    private EnemyTurnActionRunner enemyTurnActionRunner;
    private PlayerAttackPredictionListBuilder playerAttackPredictionListBuilder;

    private void Awake()
    {
        if (plateController == null)
        {
            Debug.LogError("EnermyAttackController needs PlateController. Assign it in the Inspector.");
        }

        if (playerAttackPrediction == null)
        {
            Debug.LogError("EnermyAttackController needs PlayerAttackPrediction. Assign it in the Inspector.");
        }

        if (battleController == null)
        {
            Debug.LogError("EnermyAttackController needs BattleController. Assign it in the Inspector.");
        }
    }

    public void EnermyAttackStart()
    {
        if (!CanStartEnemyAttack())
        {
            return;
        }

        List<AttackPrediction> playerAttackPredictionsList = BuildPlayerAttackPredictions();
        RunEnemyActions(playerAttackPredictionsList);
    }

    private List<AttackPrediction> BuildPlayerAttackPredictions()
    {
        List<AttackPrediction> playerAttackPredictionsList = GetPlayerAttackPredictionListBuilder().Build();
        if (playerAttackPredictionsList.Count == 0)
        {
            Debug.Log("예측 리스트가 비어있습니다.");
        }

        return playerAttackPredictionsList;
    }

    private void RunEnemyActions(List<AttackPrediction> playerAttackPredictionsList)
    {
        IReadOnlyList<Plate> enermyPlates = plateController.GetEnermyPlates();
        for (int index = 0; index < plateController.GetEnermySummonCount(); index++) //적이 순차적으로 공격준비
        {
            playerAttackPredictionsList = GetEnemyTurnActionRunner().RunEnemyAction(
                enermyPlates[index].GetCurrentSummon(),
                enermyPlates,
                index,
                playerAttackPredictionsList);
        }
    }

    private EnemyTurnActionRunner GetEnemyTurnActionRunner()
    {
        if (enemyTurnActionRunner == null)
        {
            enemyTurnActionRunner = new EnemyTurnActionRunner(
                new EnemyPredictionReactionService(plateController, battleController),
                plateController);
        }

        return enemyTurnActionRunner;
    }

    private PlayerAttackPredictionListBuilder GetPlayerAttackPredictionListBuilder()
    {
        if (playerAttackPredictionListBuilder == null)
        {
            playerAttackPredictionListBuilder = new PlayerAttackPredictionListBuilder(
                plateController,
                playerAttackPrediction);
        }

        return playerAttackPredictionListBuilder;
    }

    private bool CanStartEnemyAttack()
    {
        if (plateController == null)
        {
            Debug.LogError("EnermyAttackController cannot start without PlateController.");
            return false;
        }

        if (playerAttackPrediction == null)
        {
            Debug.LogError("EnermyAttackController cannot start without PlayerAttackPrediction.");
            return false;
        }

        if (battleController == null)
        {
            Debug.LogError("EnermyAttackController cannot start without BattleController.");
            return false;
        }

        return true;
    }

}
