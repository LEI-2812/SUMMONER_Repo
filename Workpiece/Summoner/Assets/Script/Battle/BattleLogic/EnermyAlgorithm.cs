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


    //�˰��� ������� ����
    public List<AttackPrediction> HandleReactPrediction(Summon attackingEnermySummon, int attackingEnermyPlateIndex, List<AttackPrediction> playerAttackPredictionsList)
    {

        if (playerAttackPredictionsList.Count == 0)
        {
            // ����Ʈ�� ��������� �Ϲ� �������� ó��
            handleReactNormalAttack(attackingEnermySummon, attackingEnermyPlateIndex, plateController.getClosestPlayerPlateIndex()); // �⺻ Ÿ�� �÷���Ʈ �ε��� ���
            Debug.Log("����Ʈ�� �� �Ϲݰ��� ����");
        }
        else
        {
            Debug.Log("�������� ��...");
            int indexToRemove = canReactWithSpecialAttack(attackingEnermySummon, attackingEnermyPlateIndex, playerAttackPredictionsList);
            if (indexToRemove != -1) //Ư���������� ������ �����Ұ��
            {
                // Ư���������� ������ ��� �ش� �׸��� ����Ʈ���� ����
                playerAttackPredictionsList.RemoveAt(indexToRemove);
                Debug.Log("Ư������ ���� �Ϸ�, ����Ʈ���� �׸� ����");
            }
            else
            {
                // Ư�������� �Ұ����� ��� �Ϲݰ������� ����
                handleReactNormalAttack(attackingEnermySummon, attackingEnermyPlateIndex, plateController.getClosestPlayerPlateIndex());
                Debug.Log("Ư�����ݿ� ���� ���������� ���ų� Ȯ���� �ɷȽ��ϴ�. �Ϲݰ������� ����");
            }
        }

        return playerAttackPredictionsList; // ����� ����Ʈ ��ȯ
    }


    // Ư���������� ������ �������� �˻��ϴ� �޼ҵ�
    private int canReactWithSpecialAttack(Summon attacker, int attackingEnermyPlateIndex, List<AttackPrediction> playerAttackPredictionsList)
    {
        int indexToRemove = -1;
        for (int i = 0; i < playerAttackPredictionsList.Count; i++)
        {
            AttackPrediction playerPrediction = playerAttackPredictionsList[i];
            AttackProbability preAttackProbability = playerPrediction.getAttackProbability(); // Ȯ��


            if (canReactSpecialAttack(preAttackProbability)) // Ư���������� ��������
            {
                bool specialAttackExecuted = handleReactSpecialAttack(attacker, attackingEnermyPlateIndex, playerPrediction);
                if (specialAttackExecuted)
                {
                    return i; // Ư�� ���� ���� �� �ش� �ε����� ��ȯ
                }
            }
        }
        return indexToRemove; // Ư�� ���ݿ� �������� ���� ���
    }



    //Ư�����ݿ� ���� ����
    private bool handleReactSpecialAttack(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        if(playerPrediction.getAttackStrategy() is AttackAllEnemiesStrategy allAttackStrategy) //������ ����Ÿ���� ��ü�������� �˻�
        {
            if(allAttackStrategy.getStatusType() == StatusType.Poison)
            {
               return reactSpecialFromPoison(attacker , attackingEnermyPlateIndex,playerPrediction); //������ ���� ����
            }
            else if(allAttackStrategy.getStatusType() == StatusType.None)
            {
                return reactSpecialFromAllNone(attacker, attackingEnermyPlateIndex, playerPrediction); //��ü���� ����
            }
        }
        else if(playerPrediction.getAttackStrategy() is TargetedAttackStrategy targetAttackStrategy) //������ ����Ÿ���� Ÿ�ٰ������� �˻�
        {
            if (targetAttackStrategy.getStatusType() == StatusType.None)
            {
                return reactSpecialFromTargetNone(attacker, attackingEnermyPlateIndex, playerPrediction); //���ݰ��� ����

            }
            else if (targetAttackStrategy.getStatusType() == StatusType.Upgrade)
            {
                return reactSpecialFromUpgrade(attacker, attackingEnermyPlateIndex, playerPrediction);

            }
            else if (targetAttackStrategy.getStatusType() == StatusType.Heal)
            {
               return reactSpecialFromHeal(attacker, attackingEnermyPlateIndex, playerPrediction);

            }
        }
        else if (playerPrediction.getAttackStrategy() is ClosestEnemyAttackStrategy)//������ ������ ���������ϰ�� (24.10.20 ���� cat��)
        { //�������� ���� else ���� elseif���
            return false; //����̴� Ư������ ������ �����Ƿ�
        }
        else
        {
            Debug.Log("�� ��ȯ���� Ư������ �������� ������ �߸� ����");
            return false;
        }


        return true;
    }



    private bool reactSpecialFromPoison(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //�ش� ��ȯ���� ��ų ��������

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getTargetPlateIndex());
            Debug.Log($"{attacker.getSummonName()}�� ��ų�� ��� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Heal) //Ÿ���÷���Ʈ�� ���� �Ǿ����
                {
                    int targetPlateIndex = plateController.getLowestHealthEnermyPlateIndex();
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� �� Ư�� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //���ݰ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i); //���ݰ���
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    return true;
                }
            }

        }
        return false;
    }

    private bool reactSpecialFromTargetNone(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //�ش� ��ȯ���� ��ų ��������

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getAttackSummonPlateIndex());
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� �� Ư�� ������ �����߽��ϴ�.");
                    return true;
                }
            }
        }
        return false;
    }

    //��ü ���� ����
    private bool reactSpecialFromAllNone(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //�ش� ��ȯ���� ��ų ��������

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex,playerPrediction.getAttackSummonPlateIndex()); //��ü������ ����� ������ �Ϲݰ��ݴ���
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i); //������ ��󿡰� ����

                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield) //���� ü���� ���� ������ ����
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i); //�ڱ� �ڽſ��� ����
                    Debug.Log($"{attacker.getSummonName()}�� ���� Ư�� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Heal)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i); //�ڱ��ڽſ��� ��
                    Debug.Log($"{attacker.getSummonName()}�� �� Ư�� ��ų�� �����߽��ϴ�.");
                    return true;
                }
            }

           }
        return false;
    }
    private bool reactSpecialFromHeal(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //�ش� ��ȯ���� ��ų ��������

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getAttackSummonPlateIndex());
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //Ÿ�ٰ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //��ü�������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ��ü ������ �����߽��ϴ�.");
                    return true;
                }
            }

        }
        return false;
    }
    private bool reactSpecialFromUpgrade(Summon attacker, int attackingEnermyPlateIndex, AttackPrediction playerPrediction)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //�ش� ��ȯ���� ��ų ��������

        if (attackStrategy == null)
        {
            handleReactNormalAttack(attacker, attackingEnermyPlateIndex, playerPrediction.getAttackSummonPlateIndex());
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
        }
        else
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Curse) //���ְ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                    //specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    return true;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is TargetedAttackStrategy) //���ݰ������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, playerPrediction.getAttackSummonPlateIndex(), i);
                   // specialAttackExecuted = true;
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    return true;
                }
            }

        }
        return true;
    }


    private void handleReactNormalAttack(Summon attacker, int attackingEnermyPlateIndex, int targetPlateIndex)
    {
        IAttackStrategy[] attackStrategy = attacker.getSpecialAttackStrategy(); //�ش� ��ȯ���� ��ų ��������
        float randomValue;
        if (attackStrategy == null)
        {
            randomValue = UnityEngine.Random.Range(0f, 100f); // 0���� 100 ������ ������ ��
            if (randomValue < 30f) //������
            {
                Debug.Log($"{attacker.name} �� ������");
                double originPower = attacker.getAttackPower();
                attacker.setAttackPower(attacker.getHeavyAttackPower()); //���ݷ��� �����ݷ����� ��ȯ
                attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlatesIndex(attacker)); //�Ϲݰ��ݼ���
                attacker.setAttackPower(originPower); //���� ���ݷ����� �ǵ�����
            }
            else //�Ϲ� ���ݷ����� ����
            {
                attacker.normalAttack(plateController.getPlayerPlates(), targetPlateIndex); //�Ϲݰ��� ����
            }
            Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");
            return;
        }

        if (plateController.getPlayerSummonCount() >= 2) //��ȯ���� 2���� �̻� �����ϴ°�?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Upgrade) //��ȭ �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ��ȭ ������ �����߽��ϴ�.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Burn) //ȭ�� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ȭ�� ������ �����߽��ϴ�.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.None && attackStrategy[i] is AttackAllEnemiesStrategy) //ȭ�� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ��ü ������ �����߽��ϴ�.");
                    return;
                }
            }
        }
        if (hasPlayerSummonOverMediumRank(plateController.getPlayerPlates())) //��ȯ�� ����� �߱� �̻��� ���� �����ϴ°�?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.Curse)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Stun)
                {
                    battleController.SpecialAttackLogic(attacker, targetPlateIndex, i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ȥ�� ������ �����߽��ϴ�.");
                    return;
                }
                else if (attackStrategy[i].getStatusType() == StatusType.Shield)
                {
                    battleController.SpecialAttackLogic(attacker, attackingEnermyPlateIndex, i); //�ڱ� �ڽſ��� ����
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ��ų�� �����߽��ϴ�.");
                    return;
                }
            }
        }
        if (get30PercentDifferentHP(attacker,plateController.getPlayerPlates()) != -1) //��ȯ���� �ٸ� ��ȯ���� ü���� 30%�̻� ���� ���� ���� �����ϴ°�?
        {
            for (int i = 0; i < attackStrategy.Length; i++)
            {
                if (attacker.isSpecialAttackCool(attackStrategy[i])) //��Ÿ���̸� ���� Ư����ų �˻�
                {
                    continue;
                }
                if (attackStrategy[i].getStatusType() == StatusType.LifeDrain) //���� �������� �˻�
                {
                    battleController.SpecialAttackLogic(attacker, get30PercentDifferentHP(attacker, plateController.getPlayerPlates()), i);
                    Debug.Log($"{attacker.getSummonName()}�� Ư�� ���� ������ �����߽��ϴ�.");
                    return;
                }
            }
        }


        randomValue = UnityEngine.Random.Range(0f, 100f); // 0���� 100 ������ ������ ��
        if (randomValue < 30f) //������
        {
            Debug.Log($"{attacker.name} �� ������");
            double originPower = attacker.getAttackPower();
            attacker.setAttackPower(attacker.getHeavyAttackPower()); //���ݷ��� �����ݷ����� ��ȯ
            attacker.normalAttack(plateController.getPlayerPlates(), plateController.getClosestPlayerPlatesIndex(attacker)); //�Ϲݰ��ݼ���
            attacker.setAttackPower(originPower); //���� ���ݷ����� �ǵ�����
        }
        else //�Ϲ� ���ݷ����� ����
        {
            attacker.normalAttack(plateController.getPlayerPlates(), targetPlateIndex); //�Ϲݰ��� ����
        }
        Debug.Log($"{attacker.getSummonName()}�� �Ϲ� ������ �����߽��ϴ�.");

    }


    private bool hasPlayerSummonOverMediumRank(List<Plate> plates)
    {
        foreach (Plate plate in plates)
        {
            Summon summon = plate.getCurrentSummon();
            if (summon != null && (summon.getSummonRank() == SummonRank.Medium || summon.getSummonRank() == SummonRank.High ))
            {
                return true; // �߱� �̻��� ��ȯ���� �����ϸ� true ��ȯ
            }
        }
        return false; // �߱� �̻��� ��ȯ���� ������ false ��ȯ
    }



    private int get30PercentDifferentHP(Summon attacker, List<Plate> targetPlates)
    {
        double attackerHealthRatio = (double)attacker.getNowHP() / attacker.getMaxHP();

        for (int i = 0; i < targetPlates.Count; i++)
        {
            Summon enermySummon = targetPlates[i].getCurrentSummon();
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
        List<Plate> applyEnermyPlates = getApplyStatusEnermyPlates();

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
    private List<Plate> getApplyStatusEnermyPlates()
    {
        List<Plate> applyEnermyPlates = new List<Plate>();

        foreach (Plate plate in plateController.getEnermyPlates())
        {
            Summon originSummon = plate.getCurrentSummon();
            if (originSummon != null)
            {
                // Summon ��ü�� �����ϰ� ���� ȿ���� ����
                Summon clonedSummon = originSummon.Clone();
                Summon adjustedSummon = ApplyEnermyStatus(clonedSummon);

                // ���� Plate�� �ӽ÷� ������ Summon ����
                plate.setCurrentSummon(adjustedSummon);
                applyEnermyPlates.Add(plate);

                // ���� Summon���� ����
                plate.setCurrentSummon(originSummon);
            }
            else
            {
                applyEnermyPlates.Add(plate); // ��ȯ���� ������ �״�� �߰�
            }
        }

        return applyEnermyPlates;
    }

    // ������ Summon�� ���� ȿ�� ����
    private Summon ApplyEnermyStatus(Summon clonedSummon)
    {
        if (clonedSummon.getAllStatusTypes().Contains(StatusType.Poison))
        {
            clonedSummon.setNowHP(clonedSummon.getNowHP() - clonedSummon.getMaxHP() * 0.1);
            if (clonedSummon.getNowHP() <= 0) return null;
        }

        if (clonedSummon.getAllStatusTypes().Contains(StatusType.Burn))
        {
            clonedSummon.setNowHP(clonedSummon.getNowHP() - clonedSummon.getMaxHP() * 0.2);
            if (clonedSummon.getNowHP() <= 0) return null;
        }

        if (clonedSummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
        {
            clonedSummon.setNowHP(clonedSummon.getNowHP() - clonedSummon.getMaxHP() * 0.2);
            if (clonedSummon.getNowHP() <= 0) return null;
        }

        return clonedSummon; // ���°� ����� ������ Summon ��ȯ
    }


    //// 2. �� ������ ���¸� �����Ͽ� ���ο� �÷���Ʈ ����Ʈ ��ȯ
    //private List<Plate> getApplyStatusEnermyPlates()
    //{
    //    List<Plate> applyEnermyPlates = new List<Plate>(); // �� ����Ʈ

    //    foreach (Plate plate in plateController.getEnermyPlates()) // enermyPlates�� �ϳ��� �����´�
    //    {
    //        Summon originSummon = plate.getCurrentSummon(); // �ش� �÷���Ʈ�� ��ȯ���� �����ͼ�
    //        if (originSummon != null)
    //        {
    //            Summon copySummon = originSummon;
    //            // ���� ���͸� ������ ���¸� ���� �� �� �÷���Ʈ ����Ʈ�� �߰�
    //            Summon applySummon = ApplyEnermyStatus(copySummon);
    //            plate.setCurrentSummon(applySummon); //null�̿��� �־���. �׾����� null�̹Ƿ� ���ݴ���� ���� �ʰ�
    //            applyEnermyPlates.Add(plate);
    //        }
    //    }

    //    return applyEnermyPlates;
    //}

    //// 2.(1) ���� ���¿� ���� ��ġ ����
    //private Summon ApplyEnermyStatus(Summon enermySummon)
    //{
    //    // ����: �ִ� ü���� 10% ������ ����
    //    if (enermySummon.getAllStatusTypes().Contains(StatusType.Poison))
    //    {
    //        enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.1); // 10% ü�� ����
    //        if (enermySummon.getNowHP() <= 0) return null; // ü���� 0 ���϶�� ����
    //    }

    //    // ȭ��: �ִ� ü���� 20% ������ ����
    //    if (enermySummon.getAllStatusTypes().Contains(StatusType.Burn))
    //    {
    //        enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% ü�� ����
    //        if (enermySummon.getNowHP() <= 0) return null; // ü���� 0 ���϶�� ����
    //    }

    //    // ����: �ִ� ü���� 20% ������ ����
    //    if (enermySummon.getAllStatusTypes().Contains(StatusType.LifeDrain))
    //    {
    //        enermySummon.setNowHP(enermySummon.getNowHP() - enermySummon.getMaxHP() * 0.2); // 20% ü�� ����
    //        if (enermySummon.getNowHP() <= 0) return null; // ü���� 0 ���϶�� ����
    //    }

    //    return enermySummon; // ���� ����� ���� ��ȯ
    //}



    public PlateController getPlateController()
    {
        return this.plateController;
    }
}
