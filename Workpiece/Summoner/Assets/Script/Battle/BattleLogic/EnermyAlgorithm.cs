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
    private PlateController plateController;
    private List<Plate> playerPlates;
    private PlayerAttackPrediction playerAttackPrediction;
    Dictionary<int, AttackProbability> attackProbabilityMap = new Dictionary<int, AttackProbability>();

    private void Awake()
    {
        plateController = GetComponent<PlateController>();
        playerAttackPrediction = GetComponent<PlayerAttackPrediction>();
    }

    // �˰��� ������� ����
    public void ExecuteEnermyAlgorithm(Summon attackingSummon, int targetIndex)
    {
       
        // 1. ��ȯ���� ���� üũ
        playerPlates = CheckPlayerPlateState(); // ���� playerPlates��

        // 2. ������ ���� üũ (�� ����Ʈ�� ���� ������ enermyPlates �߰�)
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

        for (int i = 0; i < playerPlates.Count; i++) //�÷��̾� �÷���Ʈ ����ŭ �˻�
        {
            // 3. ��ȯ���� ���� ���� �˰��� ���� (Ư������ Ȯ���� ����)
            AttackProbability defaultProbability = new AttackProbability(0.5f, 0.5f); //�⺻ 50% 50%
            attackProbabilityMap[targetIndex] = defaultProbability; //�÷��̾� �÷���Ʈ�� �ε����� Ű���� ��ġ�ϵ���

            // 3.2 PlayerAttackPrediction Ŭ�������� ����� ���� �����ͼ� ������Ʈ
            //AttackProbability recalculatedProbability = playerAttackPrediction.ExecutePlayerAttackPrediction(playerPlates, applyEnermyPlates, defaultProbability);
            //attackProbabilityMap[targetIndex] = recalculatedProbability;
        }
        // 4. ������ ���� �˰��� ����
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
