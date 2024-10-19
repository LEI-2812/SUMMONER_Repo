using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPrediction : MonoBehaviour
{
    private PlateController plateController;


    private CatAttackPrediction catAttackPrediction;
    private FoxAttackPrediction foxAttackPrediction;
    private WolfAttackPrediction wolfAttackPrediction;
    private EagleAttackPrediction eagleAttackPrediction;
    private SnakeAttackPrediction snakeAttackPrediction;
    private RabbitAttackPrediction rabbitAttackPrediction;
    private void Awake()
    {
        plateController = GetComponent<PlateController>();
        catAttackPrediction = GetComponent<CatAttackPrediction>();
        foxAttackPrediction = GetComponent<FoxAttackPrediction>();
        wolfAttackPrediction = GetComponent<WolfAttackPrediction>();
        eagleAttackPrediction = GetComponent<EagleAttackPrediction>();
        snakeAttackPrediction = GetComponent<SnakeAttackPrediction>();
        rabbitAttackPrediction = GetComponent<RabbitAttackPrediction>();
    }

    public List<AttackPrediction> getPlayerAttackPredictionList(List<Plate>playerPlates, List<Plate>enermyPlates) //�÷��̾��� �ൿ���� ��ȯ
    {
        List<AttackPrediction> playerPrediction = new List<AttackPrediction>();

        foreach (Plate plate in playerPlates)
        {
            Summon summon = plate.getCurrentSummon();

            if (summon != null)
            {
                // ��ȯ���� Ư�� ��ų�� ��� �������� Ȯ��
                IAttackStrategy[] availableSpecialAttacks = summon.getAvailableSpecialAttacks();
                bool hasUsableSpecialAttack = (availableSpecialAttacks != null && availableSpecialAttacks.Length > 0);

                // Ư�� ��ų�� ���� ��� �Ϲ� �������� ����
                if (!hasUsableSpecialAttack)
                {
                    Debug.Log($"{summon.getSummonName()}�� ��� ������ Ư�� ��ų�� �����ϴ�. �Ϲ� �������� ó��.");
                    AttackProbability attackProbability = new AttackProbability(100f, 0f); //�Ϲݰ����� 100%
                    int attackIndex = plateController.getClosestEnermyPlatesIndex(summon);
                    if(attackIndex == -1)
                    {
                        Debug.Log("������ �ε����� ����");
                    }
                    AttackPrediction attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
             
                    playerPrediction.Add(attackPrediction);
                }
                else
                {
                    // ��ȯ���� ���� �������� ����
                    switch (summon.getSummonType())
                    {
                        case SummonType.Cat:
                            AttackPrediction catPrediction = getCatAttackPrediction(summon, enermyPlates);
                            if (catPrediction != null)
                            {
                                playerPrediction.Add(catPrediction);
                            }
                            break;
                        case SummonType.Wolf:
                            AttackPrediction wolfPrediction = getWolfAttackPrediction(summon, enermyPlates);
                            if (wolfPrediction != null)
                            {
                                playerPrediction.Add(wolfPrediction);
                            }
                            break;
                        case SummonType.Snake:
                            AttackPrediction snakePrediction = getSnakeAttackPrediction(summon, enermyPlates);
                            if (snakePrediction != null)
                            {
                                playerPrediction.Add(snakePrediction);
                            }
                            break;

                        case SummonType.Rabbit:
                            AttackPrediction rabitPrediction = getRabbitAttackPrediction(summon, playerPlates, enermyPlates);
                            if (rabitPrediction != null)
                            {
                                playerPrediction.Add(rabitPrediction);
                            }
                            break;

                        case SummonType.Eagle:
                            AttackPrediction eaglePrediction = getEagleAttackPrediction(summon, enermyPlates);
                            if (eaglePrediction != null)
                            {
                                playerPrediction.Add(eaglePrediction);
                            }
                            break;

                        case SummonType.Fox:
                            AttackPrediction foxPrediction = getFoxAttackPrediction(summon, playerPlates, enermyPlates);
                            if (foxPrediction != null)
                            {
                                playerPrediction.Add(foxPrediction);
                            }
                            break;
                    }
                }
            }
        }

        return playerPrediction;
    }


    //����� ��������
    private AttackPrediction getCatAttackPrediction(Summon cat, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = catAttackPrediction.getClosestEnermyIndex(enermyPlates);


        // �Ϲ� �������� óġ�� �����ϸ� �Ϲ� ���� Ȯ�� 10% ����
        if (catAttackPrediction.getIndexofNormalAttackCanKill(cat, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 0.1f, true);
            attackIndex = catAttackPrediction.getIndexofNormalAttackCanKill(cat, enermyPlates); //�Ϲ� �������� óġ������ �ε��� �ޱ�
        }
        else
        {
            // Ư�� �������� óġ�� �����ϸ� Ư�� ���� Ȯ�� ����
            if (catAttackPrediction.getIndexofSpecialCanKill(cat, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                attackIndex = catAttackPrediction.getIndexofSpecialCanKill(cat, enermyPlates); //Ư�� �������� óġ������ �ε��� �ޱ�
            }
            else
            { //Ư�����ݿ� +5%
                attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false);
                attackIndex = catAttackPrediction.getClosestEnermyIndex(enermyPlates); //�� �÷���Ʈ �߿��� ���� ������ �ִ� �ε��� �ޱ�
            }
        }

        AttackPrediction attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
        return attackPrediction;
    }

    //������ ��������
    private AttackPrediction getWolfAttackPrediction(Summon wolf, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = wolfAttackPrediction.getClosestEnermyIndex(enermyPlates);

        if (wolfAttackPrediction.IsEnermyCountTwoOrMore(enermyPlates)) //���� 2���� �̻��ΰ�?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            if (wolfAttackPrediction.AllEnermyHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            }
            else if(wolfAttackPrediction.AllEnermyHealthDown50(enermyPlates)) //���� ü���� ��� 50% �����ΰ�?
            {
                if (wolfAttackPrediction.HasSpecificAvailableSpecialAttack(wolf, enermyPlates))
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
            else if (wolfAttackPrediction.IsEnermyHealthDifferenceOver30(enermyPlates)) //���� ������ �ٸ� �ʿ����� ü���� 30% ������?
            {
                if (wolfAttackPrediction.IsLowestHealthEnermyClosest(wolf, enermyPlates)) //�������� �ε����� ���������ϴ� �ε����� �����Ѱ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 20f, true);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
        }
        else if(wolfAttackPrediction.IsEnermyCountOne(enermyPlates)) //���� 1���� �ΰ�?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            
            if(wolfAttackPrediction.getIndexofNormalAttackCanKill(wolf,enermyPlates) != -1) //�Ϲ� �������� ���͸� ����ĥ �� �ִ°�?
            {
                attackIndex = wolfAttackPrediction.getIndexofNormalAttackCanKill(wolf, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            }
            else
            {
                //�Ϲ� ���ݰ� Ư�������� ���ظ� �� �� �� �ִ� ������ ��ȯ���� �� �Ϲ� �����ϰ��
                if (wolfAttackPrediction.getMostDamageAttack(wolf, enermyPlates) == AttackType.NormalAttack)
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false);
                }
            }
        }
        else
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
        }

        AttackPrediction attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
        return attackPrediction;
    }


    //�䳢�� ��������
    private AttackPrediction getRabbitAttackPrediction(Summon rabbit, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = rabbitAttackPrediction.getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex); //�⺻�� ���� ������� ����

        if (rabbitAttackPrediction.GetIndexOfLowerHealthIfDifferenceOver30(playerPlates) != -1) //��ȯ�� �� ������ �ٸ� �ʰ� ü���� ������ �� 30% �̻� ������?
        {
            attackIndex = rabbitAttackPrediction.GetIndexOfLowerHealthIfDifferenceOver30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            attackPrediction = new AttackPrediction(playerPlates, attackProbability, attackIndex);
        }
        else if (rabbitAttackPrediction.getIndexOfLowerHealthIfAllDown30(playerPlates) != -1) //��ȯ�� ����� ü���� 30% �����ΰ�?
        {
            attackIndex = rabbitAttackPrediction.getIndexOfLowerHealthIfAllDown30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            attackPrediction = new AttackPrediction(playerPlates, attackProbability, attackIndex);
        }
        else if (rabbitAttackPrediction.AllPlayerSummonOver70Percent(playerPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
        }
        else if (rabbitAttackPrediction.CanNormalAttackKill(rabbit, enermyPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
        }


        return attackPrediction;
    }


    //�� ��������
    private AttackPrediction getSnakeAttackPrediction(Summon snake, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = snakeAttackPrediction.getClosestEnermyIndex(enermyPlates);


        if (snakeAttackPrediction.IsEnermyAlreadyPoisoned(enermyPlates)){ //���͵��� �̹� �ߵ� �����ΰ�?
            attackProbability = new AttackProbability(100f, 0f);
        }
        else if(!snakeAttackPrediction.canUseSpecialAttack(snake)) //Ư�������� ����� �� �ִ°�?
        {
            attackProbability = new AttackProbability(100f, 0f);
        }
        else
        {
            if (snakeAttackPrediction.isEnermyCountOverTwo(enermyPlates)) //���� 2���� �̻��ΰ�?
            {
                if (snakeAttackPrediction.AllEnermyHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
                if (snakeAttackPrediction.hasMonsterWithMoreThan4Attacks(enermyPlates)) //���� �� ������ ������ 4�� �̻��� ���� �����ϴ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
            else //1���� �϶�
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                if (snakeAttackPrediction.AllEnermyHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
                if (snakeAttackPrediction.CanNormalAttackKill(snake, enermyPlates)) //�Ϲ� ���� �� ���͸� ����ĥ �� �ִ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
            }
        }

        AttackPrediction attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
        return attackPrediction;
    }

    //���� ��������
    private AttackPrediction getFoxAttackPrediction(Summon fox, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = foxAttackPrediction.getClosestEnermyIndex(enermyPlates);
        List<Plate> targetPlate = enermyPlates;


        if (foxAttackPrediction.isAnySummonWithCurseStatus(playerPlates)) //��ȯ���� ���ֻ��¿� �ɷ��ִ� ���� �ִ°�?
        {
            attackProbability = new AttackProbability(0f, 100f); //Ư������ 100%
            targetPlate = playerPlates;
        }
        else if(foxAttackPrediction.isTwoOrMoreEnemies(enermyPlates)) //���� 2���� �̻� �����ϴ°�?
        {
            if (foxAttackPrediction.AllEnemiesHealthOver50(enermyPlates)) //���� ü���� ��� 50% �̻��ΰ�?
            {
                if (foxAttackPrediction.AllSummonsLowOrMediumRank(playerPlates)) //�Ʊ��� ����� ��� �ϱް� �߱��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                    targetPlate = playerPlates;
                }
            }
            else if (foxAttackPrediction.isAnyEnemyHealthDown30Percent(enermyPlates)) //���� ü���� �ϳ��� 30% �Ʒ��ΰ�
            {
                if (foxAttackPrediction.CanNormalAttackKill(fox, enermyPlates)) //�Ϲݰ��ݽ� óġ�� �� �ִ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
            }
        }
        else if(foxAttackPrediction.isOnlyOneEnemy(enermyPlates)) //���� 1���� �ΰ�?
        {
            if (foxAttackPrediction.isAnyEnemyHealthOver70Percent(enermyPlates)) //���� ü���� 70% �̻��ΰ�?
            {
                if (foxAttackPrediction.AllSummonsLowOrMediumRank(playerPlates)) //�Ʊ��� ����� ��� �ϱް� �߱��ΰ�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                    targetPlate = playerPlates;
                }
            }
            else if(foxAttackPrediction.CanNormalAttackKill(fox, enermyPlates)) //�Ϲݰ��ݽ� ���͸� ����ĥ �� �ִ°�?
            {
                if (foxAttackPrediction.CanNormalAttackKill(fox, enermyPlates)) //�Ϲݰ��ݽ� óġ�� �� �ִ°�?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
            }
        }

        AttackPrediction attackPrediction = new AttackPrediction(targetPlate, attackProbability, attackIndex);
        return attackPrediction;
    }


    //������ ��������
    private AttackPrediction getEagleAttackPrediction(Summon eagle, List<Plate> enermyPlates)
    {
        // �⺻�� ����: �Ϲ� ���� 50%, Ư�� ���� 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = eagleAttackPrediction.getClosestEnermyIndex(enermyPlates); //���� ��������� �ε��� �⺻��


        if (eagleAttackPrediction.isTwoOrMoreEnemies(enermyPlates)) //���� 2���� �̻��ΰ�?
        {
            if (eagleAttackPrediction.isEnermyHealthDifferenceOver30(enermyPlates)) //���� �� ���� �ٸ��ʰ� ������ �� 30% �̻� ������?
            {
                if (eagleAttackPrediction.getNormalAttackKillIndex(eagle, enermyPlates) != -1) //�Ϲ� �������� ü���� ���� ���� ������ �� �ִ°�?
                {
                    attackIndex = eagleAttackPrediction.getNormalAttackKillIndex(eagle, enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
                else
                {
                    attackIndex = eagleAttackPrediction.getSpecialAttackKillIndex(eagle, enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
            else if (eagleAttackPrediction.AreEnermyHealthWithin10Percent(enermyPlates)) //������ ü���� ���� ����Ѱ�?
            {
                if (eagleAttackPrediction.getIndexOfMostHealthEnermy(enermyPlates) != -1) //���� ü���� ���� ������ �ε���
                {
                    attackIndex = eagleAttackPrediction.getIndexOfMostHealthEnermy(enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
        }
        else if (eagleAttackPrediction.isOnlyOneEnemy(enermyPlates)) //���� 1���� �ΰ�?
        {
            if (eagleAttackPrediction.getNormalAttackKillIndex(eagle, enermyPlates) != -1)
            {
                attackIndex = eagleAttackPrediction.getNormalAttackKillIndex(eagle, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            }
            else if (eagleAttackPrediction.getSpecialAttackKillIndex(eagle, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            }
            else
            {
                if(eagleAttackPrediction.getTypeOfMoreAttackDamage(eagle,enermyPlates) == AttackType.NormalAttack)//�Ϲݰ��ݰ� Ư������ �� ���ظ� ���� �� ���ݿ� 5%���
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false);
                }
            }
        }

        AttackPrediction attackPrediction = new AttackPrediction(enermyPlates, attackProbability, attackIndex);
        return attackPrediction;
    }


        // Ȯ�� ���� �����ϰ� �����Ͽ� ��ȯ�ϴ� �޼ҵ�
        public AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack)
    {
        if (isNormalAttack)
        {
            // �Ϲ� ���� Ȯ���� ������Ű��, Ư�� ���� Ȯ���� �׸�ŭ ����
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
        }
        else
        {
            // �Ǵ�, Ư�� ���� Ȯ���� ������Ű��, �Ϲ� ���� Ȯ���� �׸�ŭ ����
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
        }
        return currentProbabilities;
    }


}
