using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    private List<Plate> enermyPlates; // 적이 사용할 플레이트 목록

    [Header("컨트롤러")]
    [SerializeField] private TurnController turnController;
    private EnermyAttackController enermyAttackController;
    //private EnermyAlgorithm enermyAlgorithm;

    private void Awake()
    {
        enermyAttackController = GetComponent<EnermyAttackController>();
        //enermyAlgorithm = GetComponent<EnermyAlgorithm>();
    }

    public  void startTurn()
    {
        Debug.Log("적 턴 시작");
        // 적의 행동을 자동으로 결정함 (예: 플레이어를 공격)
        takeAction();
    }

    public void takeAction() //여기에 AI로직 작성
    {
        //플레이어의 예측공격 리스트를 가져오고
        List<AttackPrediction> playerAttackPredictionsList = enermyAttackController.getEnermyAlgorithmController().getPlayerAttackPredictionsList();
        if(playerAttackPredictionsList.Count == 0)
        {
            Debug.Log("예측 리스트가 비어있습니다.");
        }
        Debug.Log("리스트를 가져와서 적 대응시작");
        //적의 공격 시작
        enermyAttackController.EnermyAttackStart(playerAttackPredictionsList);



        EndTurn();
    }

    public void EndTurn()
    {
        Debug.Log("적 턴 종료");
        turnController.EndTurn();
    }


    public EnermyAttackController getEnermyAttackController()
    {
        return enermyAttackController;
    }
}
