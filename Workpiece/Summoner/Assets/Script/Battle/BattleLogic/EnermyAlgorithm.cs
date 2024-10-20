using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] private BattleController battleController;

    private List<AttackPrediction> playerAttackPredictionsList;


    // �˰��� ������� ����
    public List<AttackPrediction> HandleReactPrediction(Summon attackingEnermySummon, List<AttackPrediction> playerAttackPredictionsList)
    {
        if (playerAttackPredictionsList.Count == 0)
        {
            // ����Ʈ�� ��������� �Ϲ� �������� ó��
            handleReactNormalAttack(attackingEnermySummon, plateController.getClosestPlayerPlateIndex()); // �⺻ Ÿ�� �÷���Ʈ �ε��� ���
        }
        else
        {
            for (int i = 0; i < playerAttackPredictionsList.Count; i++)
            {
                AttackPrediction playerPrediction = playerAttackPredictionsList[i];

                AttackProbability preAttackProbability = playerPrediction.getAttackProbability(); // Ȯ��
                Summon preAttackingPlayerSummon = playerPrediction.getAttackSummon(); // ������ �÷��̾��� ���� ��ȯ��
                int preSummonPlateIndex = playerPrediction.getAttackSummonPlateIndex(); // ������ �÷��̾� ���� ��ȯ���� �÷���Ʈ �ǳ�
                IAttackStrategy preAttackStrategy = playerPrediction.getAttackStrategy(); // ������ ���ݹ��
                List<Plate> targetPlate = playerPrediction.getTargetPlate(); // ������ Ÿ���� �÷���Ʈ (�÷��̾� Ȥ�� ��)
                int targetPlateIndex = playerPrediction.getTargetPlateIndex(); // ���� Ÿ�� �÷���Ʈ ��ȣ

                if (canReactSpecialAttack(preAttackProbability)) // Ư���������� ��������
                {
                    handleReactSpecialAttack(attackingEnermySummon, preAttackStrategy, preSummonPlateIndex, targetPlate, targetPlateIndex);
                }
                else // �Ϲݰ������� ����
                {
                    handleReactNormalAttack(attackingEnermySummon, targetPlateIndex);
                }

                // ������ ������ ����Ʈ���� ����
                playerAttackPredictionsList.RemoveAt(i);
                i--; // �ε��� ����
            }
        }

        return playerAttackPredictionsList; // ����� ����Ʈ ��ȯ
    }



    //Ư�����ݿ� ���� ����
    private void handleReactSpecialAttack(Summon attacker, IAttackStrategy preAttackStrategy, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        if(preAttackStrategy is AttackAllEnemiesStrategy allAttackStrategy) //������ ����Ÿ���� ��ü�������� �˻�
        {
            if(allAttackStrategy.getStatusType() == StatusType.Poison)
            {
                reactSpecialFromPoison(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
        }
        else if(preAttackStrategy is TargetedAttackStrategy targetAttackStrategy) //������ ����Ÿ���� Ÿ�ٰ������� �˻�
        {
            if (targetAttackStrategy.getStatusType() == StatusType.None)
            {
                reactSpecialFromTargetNone(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
            else if (targetAttackStrategy.getStatusType() == StatusType.None)
            {
                reactSpecialFromUpgrade(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
            else if (targetAttackStrategy.getStatusType() == StatusType.Heal)
            {
                reactSpecialFromHeal(attacker, preAttackerIndex, targetPlate, targetPlateIndex);
            }
        }
        else
        {
            Debug.Log("���������� �߸��޾ƿ�����");
        }
    }



    private void reactSpecialFromPoison(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // ��� ������ ��ų�� ��������
        bool specialAttackExecuted = false; // Ư�� ���� ���� ���θ� ����

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� �� Ư�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //Ÿ�ٰ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    break;
                }
            }

            // Ư�� ������ �������� ���� ���, �Ϲ� ���� ����
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            }
        }
    }

    private void reactSpecialFromTargetNone(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // ��� ������ ��ų�� ��������
        bool specialAttackExecuted = false; // Ư�� ���� ���� ���θ� ����

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� �� Ư�� ������ �����߽��ϴ�.");
                    break;
                }
            }

            // Ư�� ������ �������� ���� ���, �Ϲ� ���� ����
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�."); 
            }
        }
    }
    private void reactSpecialFromAllNone(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // ��� ������ ��ų�� ��������
        bool specialAttackExecuted = false; // Ư�� ���� ���� ���θ� ����

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� �� Ư�� ��ų�� �����߽��ϴ�.");
                    break;
                }
            }

            // Ư�� ������ �������� ���� ���, �Ϲ� ���� ����
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            }
        }
    }
    private void reactSpecialFromHeal(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // ��� ������ ��ų�� ��������
        bool specialAttackExecuted = false; // Ư�� ���� ���� ���θ� ����

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //Ÿ�ٰ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //��ü�������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ��ü ������ �����߽��ϴ�.");
                    break;
                }
            }

            // Ư�� ������ �������� ���� ���, �Ϲ� ���� ����
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker, targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            }
        }
    }
    private void reactSpecialFromUpgrade(Summon attacker, int preAttackerIndex, List<Plate> targetPlate, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // ��� ������ ��ų�� ��������
        bool specialAttackExecuted = false; // Ư�� ���� ���� ���θ� ����

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, targetPlateIndex);
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Curse) //���ְ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //���ݰ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    break;
                }
            }

            // Ư�� ������ �������� ���� ���, �Ϲ� ���� ����
            if (!specialAttackExecuted)
            {
                handleReactNormalAttack(attacker,targetPlateIndex);
                Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            }
        }
    }


    private void handleReactNormalAttack(Summon attacker, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getAvailableSpecialAttacks(); // ��� ������ ��ų�� ��������

        if (attackStrategy == null)
        {
            attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlatesIndex(attacker)); //�Ϲݰ��ݼ���
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            return;
        }
        if (plateController.getPlayerPlates().Count >= 2) //��ȯ���� 2���� �̻� �����ϴ°�?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Upgrade) //��ȭ �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ��ȭ ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Burn) //ȭ�� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ȭ�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //ȭ�� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ��ü ������ �����߽��ϴ�.");
                    break;
                }
            }
        }
        else if (hasPlayerSummonOverMediumRank(plateController.getPlayerPlates())) //��ȯ�� ����� �߱� �̻��� ���� �����ϴ°�?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.Curse) //���� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Stun) //ȥ�� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ȥ�� ������ �����߽��ϴ�.");
                    break;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield) //���� ���� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ��ų�� �����߽��ϴ�.");
                    break;
                }
            }
        }
        else if (get30PercentDifferentHP(attacker,plateController.getPlayerPlates()) != -1) //��ȯ���� �ٸ� ��ȯ���� ü���� 30%�̻� ���� ���� ���� �����ϴ°�?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attackStrategy[i].getStatusType() == StatusType.LifeDrain) //���� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, get30PercentDifferentHP(attacker, plateController.getPlayerPlates()), i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    break;
                }
            }
        }
        else
        {
            attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlateIndex()); //�Ϲݰ��ݼ���
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            return;
        }
    }







    private bool hasPlayerSummonOverMediumRank(List<Plate> plates)
    {
        foreach (Plate plate in plates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null && (summon.getSummonRank() == SummonRank.Medium || summon.getSummonRank() == SummonRank.High || summon.getSummonRank() == SummonRank.Boss))
            {
                return true; // �߱� �̻��� ��ȯ���� �����ϸ� true ��ȯ
            }
        }
        return false; // �߱� �̻��� ��ȯ���� ������ false ��ȯ
    }



    private int get30PercentDifferentHP(Summon attacker, List<Plate> enermyPlates)
    {
        double attackerHealthRatio = (double)attacker.getNowHP() / attacker.getMaxHP();

        for (int i = 0; i < enermyPlates.Count; i++)
        {
            Summon enermySummon = enermyPlates[i].getCurrentSummon();
            if (enermySummon != null)
            {
                double enermyHealthRatio = (double)enermySummon.getNowHP() / enermySummon.getMaxHP();

                // ü�� ���̰� 30% �̻��� ��� �ε��� ��ȯ
                if (Math.Abs(attackerHealthRatio - enermyHealthRatio) >= 0.3)
                {
                    return i;
                }
            }
        }

        return -1; // ������ �����ϴ� ��ȯ���� ������ -1 ��ȯ
    }









    private bool canReactSpecialAttack(AttackProbability attackProbability)
    {
        // 0���� 100 ������ ���� ���� ����
        float randomValue = UnityEngine.Random.Range(0f, 100f);

        // Ư�� ���� Ȯ���� ���� ������ ũ�� true ��ȯ (Ư�� ���� ����)
        return randomValue < attackProbability.specialAttackProbability;
    }




    //�÷��̾��� ���������� ����Ʈ ��������

    public List<AttackPrediction> getPlayerAttackPredictionsList()
    {
        // 1. ��ȯ���� ���� üũ
        List<Plate> playerPlates = CheckPlayerPlateState(); // ���� playerPlates��

        // 2. ������ ���� üũ (�� ����Ʈ�� ���� ������ enermyPlates �߰�)
        List<Plate> applyEnermyPlates = GetApplyStatusEnermyPlates();

        //3. ��ȯ���� �������� ����Ʈ�� �޾ƿ´�.
        playerAttackPredictionsList = playerAttackPrediction.getPlayerAttackPredictionList(playerPlates, applyEnermyPlates);

        return playerAttackPredictionsList;
    }



    // 1. ���� playerPlate���� �����̳� ������ ������ �ȵǰ� �� ����Ʈ�� �����´�.
    public List<Plate> CheckPlayerPlateState()
    {
        List<Plate> playerPlateStates = new List<Plate>();
        List<Plate> playerPlates = plateController.getPlayerPlates();

        for (int i = 0; i < playerPlates.Count; i++) // �ε����� �̿��� ��ȸ
        {
            Plate plate = playerPlates[i];
            Summon summon = plate.getCurrentSummon();

            if (summon != null)
            {
                Debug.Log($"{summon.getSummonName()}�� ����Ʈ�� ��");
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
