using System.Collections.Generic;
using UnityEngine;

public class Enermy : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private PlateController plateController;
    private EnermyAttackController enermyAttackController;
    private void Awake()
    {
        enermyAttackController = GetComponent<EnermyAttackController>();
    }

    public void EnermyTurnStart()
    {
        Debug.Log("적 턴 시작");
        EnermyActionTake();
    }

    private void EnermyActionTake()
    {
        List<AttackPrediction> playerAttackPredictionsList = enermyAttackController.GetEnermyAlgorithmController().GetPlayerAttackPredictionsList();
        if(playerAttackPredictionsList.Count == 0)
        {
            Debug.Log("예측 리스트가 비어있습니다.");
        }
        Debug.Log("리스트를 가져와서 적 대응시작");
        enermyAttackController.EnermyAttackStart(playerAttackPredictionsList);

        EnermyTurnEnd();
    }

    private void EnermyTurnEnd()
    {
        Debug.Log("적 턴 종료");
        turnController.EndTurn();
    }


    public EnermyAttackController GetEnermyAttackController()
    {
        return enermyAttackController;
    }
}
