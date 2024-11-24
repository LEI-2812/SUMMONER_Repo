using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enermy : Character
{
    private List<Plate> enermyPlates; // ���� ����� �÷���Ʈ ���

    [Header("��Ʈ�ѷ�")]
    [SerializeField] private TurnController turnController;
    [SerializeField] private PlateController plateController;
    private EnermyAttackController enermyAttackController;
    //private EnermyAlgorithm enermyAlgorithm;

    BattleAlert battleAlert;

    //private int stageNum;
    //private int currentTurn;

    private void Awake()
    {
        enermyAttackController = GetComponent<EnermyAttackController>();
        //enermyAlgorithm = GetComponent<EnermyAlgorithm>();
    }

    private void Start()
    {
        battleAlert = GetComponent<BattleAlert>();
    }

    public  void startTurn()
    {
        Debug.Log("�� �� ����");
        // ���� �ൿ�� �ڵ����� ������ (��: �÷��̾ ����)
        takeAction();
    }

    public void takeAction() //���⿡ AI���� �ۼ�
    {
        //�÷��̾��� �������� ����Ʈ�� ��������
        List<AttackPrediction> playerAttackPredictionsList = enermyAttackController.getEnermyAlgorithmController().getPlayerAttackPredictionsList();
        if(playerAttackPredictionsList.Count == 0)
        {
            Debug.Log("���� ����Ʈ�� ����ֽ��ϴ�.");
        }
        Debug.Log("����Ʈ�� �����ͼ� �� ��������");
        //���� ���� ����
        enermyAttackController.EnermyAttackStart(playerAttackPredictionsList);

        EndTurn();
    }

    public void EndTurn()
    {
        Debug.Log("�� �� ����");
        turnController.EndTurn();
    }


    public EnermyAttackController getEnermyAttackController()
    {
        return enermyAttackController;
    }
}
