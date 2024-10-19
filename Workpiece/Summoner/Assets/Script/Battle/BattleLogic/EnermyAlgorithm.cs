using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackProbability
{
    public float normalAttackProbability;
    public float specialAttackProbability;

    // �����ڸ� �߰��Ͽ� �ʱ�ȭ�� �� �ֵ��� ��
    public AttackProbability(float normalProb, float specialProb)
    {
        normalAttackProbability = normalProb;
        specialAttackProbability = specialProb;
    }
}


public class EnermyAlgorithm : MonoBehaviour
{
    [SerializeField] private PlateController plateController;
    [SerializeField] private PlayerAttackPrediction playerAttackPrediction;
     private List<AttackPrediction> playerAttackPredictionsList;


    // �˰��� ������� ����
    public void ExecuteEnermyAlgorithm(Summon attackingSummon, int targetIndex)
    {

        // 1. ��ȯ���� ���� üũ
        List<Plate> playerPlates = CheckPlayerPlateState(); // ���� playerPlates��

        // 2. ������ ���� üũ (�� ����Ʈ�� ���� ������ enermyPlates �߰�)
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

        //3. ��ȯ���� �������� ����Ʈ�� �޾ƿ´�.
        playerAttackPredictionsList = playerAttackPrediction.getPlayerAttackPredictionList(playerPlates, applyEnermyPlates);

        // 4. ������ ���� �˰��� ����
        //���� �߰�����
    }










    // 1. ���� playerPlate���� �����̳� ������ ������ �ȵǰ� �� ����Ʈ�� �����´�.
    public List<Plate> CheckPlayerPlateState()
    {
        List<Plate> playerPlateStates = new List<Plate>();

        foreach (Plate plate in plateController.getPlayerPlates()) // playerPlates�� ����Ʈ�� ��ȸ
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null)
            {
                // ���� �÷���Ʈ�� �� ����Ʈ�� �߰� (���¸� ����)
                playerPlateStates.Add(plate);
            }
        }

        return playerPlateStates;
    }

    // 2. �� ������ ���¸� �����Ͽ� ���ο� �÷���Ʈ ����Ʈ ��ȯ
    private List<Plate> GetApplyStatusEnermyPlates()
    {
        List<Plate> applyEnermyPlates = new List<Plate>(); // �� ����Ʈ

        foreach (Plate plate in plateController.getEnermyPlates()) // enermyPlates�� �ϳ��� �����´�
        {
            Summon originalSummon = plate.getCurrentSummon(); // �ش� �÷���Ʈ�� ��ȯ���� �����ͼ�
            if (originalSummon != null)
            {
                // ���� ���͸� ������ ���¸� ���� �� �� �÷���Ʈ ����Ʈ�� �߰�
                Summon applySummon = ApplyEnermyStatus(originalSummon);
                plate.setCurrentSummon(applySummon); //null�̿��� �־���. �׾����� null�̹Ƿ� ���ݴ���� ���� �ʰ�
                applyEnermyPlates.Add(plate);
            }
        }

        return applyEnermyPlates;
    }

    // 2.(1) ���� ���¿� ���� ��ġ ����
    private Summon ApplyEnermyStatus(Summon enermySummon)
    {
        // ����: �ִ� ü���� 10% ������ ����
        if (enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.1); // 10% ü�� ����
            if (enermySummon.getNowHP() <= 0) return null; // ü���� 0 ���϶�� ����
        }

        // ȭ��: �ִ� ü���� 20% ������ ����
        if (enermySummon.getAllStatusTypes().Contains(StatusType.Burn))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% ü�� ����
            if (enermySummon.getNowHP() <= 0) return null; // ü���� 0 ���϶�� ����
        }

        // ����: �ִ� ü���� 20% ������ ����
        if (enermySummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
        {
            enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% ü�� ����
            if (enermySummon.getNowHP() <= 0) return null; // ü���� 0 ���϶�� ����
        }

        return enermySummon; // ���� ����� ���� ��ȯ
    }
}
