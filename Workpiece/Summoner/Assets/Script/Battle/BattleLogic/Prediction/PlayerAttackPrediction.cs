using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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

    public List<AttackPrediction> getPlayerAttackPredictionList(List<Plate>playerPlates, List<Plate>enermyPlates) //플레이어의 행동예측 반환
    {
        List<AttackPrediction> playerPrediction = new List<AttackPrediction>();

        foreach (Plate plate in playerPlates)
        {
            Summon summon = plate.getCurrentSummon();

            if (summon != null)
            {
                // 소환수의 특수 스킬이 사용 가능한지 확인
                IAttackStrategy[] availableSpecialAttacks = summon.getAvailableSpecialAttacks();
                bool hasUsableSpecialAttack = (availableSpecialAttacks != null && availableSpecialAttacks.Length > 0);
                int attackIndex = plateController.getClosestPlayerPlateIndex();
                if (attackIndex == -1)
                {
                    Debug.Log("공격할 인덱스가 없음");
                }
                int attackSummonPlateIndex = plateController.GetPlateIndex(plate); //자기 자신의 플레이트 번호

                // 특수 스킬이 없는 경우 일반 공격으로 예측
                if (!hasUsableSpecialAttack)
                {
                    Debug.Log($"{summon.getSummonName()}는 사용 가능한 특수 스킬이 없습니다. 일반 공격으로 예측합니다.");
                    AttackProbability attackProbability = new AttackProbability(100f, 0f); //일반공격을 100%

                    AttackPrediction attackPrediction = new AttackPrediction(
                        summon, //공격
                        plateController.GetPlateIndex(plate), //자기 자신의 플레이트 번호
                        summon.getAttackStrategy(), //일반공격
                        0, //특수공격 번호
                        enermyPlates, //타겟 플레이트
                        attackIndex, ///타겟 번호
                        attackProbability //확률
                        );
             
                    playerPrediction.Add(attackPrediction);
                }
                else
                {
                    // 소환수에 따라 예측공격 수행
                    switch (summon.getSummonType())
                    {
                        case SummonType.Cat:
                            AttackPrediction catPrediction = getCatAttackPrediction(summon, attackSummonPlateIndex, enermyPlates);
                            if (catPrediction != null)
                            {
                                playerPrediction.Add(catPrediction);
                            }
                            break;
                        case SummonType.Wolf:
                            AttackPrediction wolfPrediction = getWolfAttackPrediction(summon, attackSummonPlateIndex, enermyPlates);
                            if (wolfPrediction != null)
                            {
                                playerPrediction.Add(wolfPrediction);
                            }
                            break;
                        case SummonType.Snake:
                            AttackPrediction snakePrediction = getSnakeAttackPrediction(summon, attackSummonPlateIndex, enermyPlates);
                            if (snakePrediction != null)
                            {
                                playerPrediction.Add(snakePrediction);
                            }
                            break;

                        case SummonType.Rabbit:
                            AttackPrediction rabitPrediction = getRabbitAttackPrediction(summon, attackSummonPlateIndex, playerPlates, enermyPlates);
                            if (rabitPrediction != null)
                            {
                                playerPrediction.Add(rabitPrediction);
                            }
                            break;

                        case SummonType.Eagle:
                            AttackPrediction eaglePrediction = getEagleAttackPrediction(summon, attackSummonPlateIndex, enermyPlates);
                            if (eaglePrediction != null)
                            {
                                playerPrediction.Add(eaglePrediction);
                            }
                            break;

                        case SummonType.Fox:
                            AttackPrediction foxPrediction = getFoxAttackPrediction(summon, attackSummonPlateIndex,  playerPlates, enermyPlates);
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


    //고양이 예측공격
    private AttackPrediction getCatAttackPrediction(Summon cat, int catPlateIndex ,List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = catAttackPrediction.getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        // 일반 공격으로 처치가 가능하면 일반 공격 확률 10% 증가
        if (catAttackPrediction.getIndexofNormalAttackCanKill(cat, enermyPlates) != -1)
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 0.1f, true);
            attackIndex = catAttackPrediction.getIndexofNormalAttackCanKill(cat, enermyPlates); //일반 공격으로 처치가능한 인덱스 받기
        }
        else
        {
            // 특수 공격으로 처치가 가능하면 특수 공격 확률 증가
            if (catAttackPrediction.getIndexofSpecialCanKill(cat, enermyPlates) != -1)
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                attackIndex = catAttackPrediction.getIndexofSpecialCanKill(cat, enermyPlates); //특수 공격으로 처치가능한 인덱스 받기
            }
            else
            { //특수공격에 +5%
                attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false);
                attackIndex = catAttackPrediction.getClosestEnermyIndex(enermyPlates); //적 플레이트 중에서 가장 가까이 있는 인덱스 받기
            }
        }

        attackPrediction = new AttackPrediction(cat, catPlateIndex, cat.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }

    //늑대의 예측공격
    private AttackPrediction getWolfAttackPrediction(Summon wolf, int wolfPlateIndex, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = wolfAttackPrediction.getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(wolf, wolfPlateIndex, wolf.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (wolfAttackPrediction.IsEnermyCountTwoOrMore(enermyPlates)) //적이 2마리 이상인가?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            if (wolfAttackPrediction.AllEnermyHealthOver50(enermyPlates)) //몬스터 체력이 모두 50% 이상인가?
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            }
            else if(wolfAttackPrediction.AllEnermyHealthDown50(enermyPlates)) //몬스터 체력이 모두 50% 이하인가?
            {
                if (wolfAttackPrediction.HasSpecificAvailableSpecialAttack(wolf, enermyPlates))
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
            else if (wolfAttackPrediction.IsEnermyHealthDifferenceOver30(enermyPlates)) //몬스터 한쪽이 다른 쪽에비해 체력이 30% 높은가?
            {
                if (wolfAttackPrediction.IsLowestHealthEnermyClosest(wolf, enermyPlates)) //낮은쪽의 인덱스가 근접공격하는 인덱스와 동일한가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 20f, true);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
        }
        else if(wolfAttackPrediction.IsEnermyCountOne(enermyPlates)) //적이 1마리 인가?
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            
            if(wolfAttackPrediction.getIndexofNormalAttackCanKill(wolf,enermyPlates) != -1) //일반 공격으로 몬스터를 물리칠 수 있는가?
            {
                attackIndex = wolfAttackPrediction.getIndexofNormalAttackCanKill(wolf, enermyPlates);
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            }
            else
            {
                //일반 공격과 특수공격중 피해를 더 줄 수 있는 공격을 반환했을 때 일반 공격일경우
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

        attackPrediction = new AttackPrediction(wolf, wolfPlateIndex, wolf.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }


    //토끼의 예측공격
    private AttackPrediction getRabbitAttackPrediction(Summon rabbit, int rabbitPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = rabbitAttackPrediction.getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (rabbitAttackPrediction.GetIndexOfLowerHealthIfDifferenceOver30(playerPlates) != -1) //소환수 중 한쪽이 다른 쪽과 체력을 비교했을 때 30% 이상 낮은가?
        {
            attackIndex = rabbitAttackPrediction.GetIndexOfLowerHealthIfDifferenceOver30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (rabbitAttackPrediction.getIndexOfLowerHealthIfAllDown30(playerPlates) != -1) //소환수 모두의 체력이 30% 이하인가?
        {
            attackIndex = rabbitAttackPrediction.getIndexOfLowerHealthIfAllDown30(playerPlates);
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (rabbitAttackPrediction.AllPlayerSummonOver70Percent(playerPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            attackIndex = rabbitAttackPrediction.getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if (rabbitAttackPrediction.CanNormalAttackKill(rabbit, enermyPlates))
        {
            attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
            attackIndex = rabbitAttackPrediction.getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else// 모두 조건이 안맞으면 가장 낮은 체력 아군 힐
        {
            attackIndex = rabbitAttackPrediction.getIndexOfLowestHealthSummon(playerPlates);
            attackPrediction = new AttackPrediction(rabbit, rabbitPlateIndex, rabbit.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }


        return attackPrediction;
    }


    //뱀 예측공격
    private AttackPrediction getSnakeAttackPrediction(Summon snake, int snakePlateIndex, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = snakeAttackPrediction.getClosestEnermyIndex(enermyPlates);
        AttackPrediction attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);

        if (snakeAttackPrediction.IsEnermyAlreadyPoisoned(enermyPlates)){ //몬스터들이 이미 중독 상태인가?
            attackProbability = new AttackProbability(100f, 0f);
            attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability); //일반공격으로
        }
        else if(!snakeAttackPrediction.canUseSpecialAttack(snake)) //특수공격을 사용할 수 있는가?
        {
            attackProbability = new AttackProbability(100f, 0f);
            attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getAttackStrategy(), 0, enermyPlates, attackIndex, attackProbability); //일반공격으로
        }
        else
        {
            if (snakeAttackPrediction.isEnermyCountOverTwo(enermyPlates)) //적이 2마리 이상인가?
            {
                if (snakeAttackPrediction.AllEnermyHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
                if (snakeAttackPrediction.hasMonsterWithMoreThan4Attacks(enermyPlates)) //몬스터 중 공격의 개수가 4개 이상인 몹이 존재하는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
            else //1마리 일때
            {
                attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                if (snakeAttackPrediction.AllEnermyHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
                if (snakeAttackPrediction.CanNormalAttackKill(snake, enermyPlates)) //일반 공격 시 몬스터를 물리칠 수 있는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
            }
            attackPrediction = new AttackPrediction(snake, snakePlateIndex, snake.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        }

        return attackPrediction;
    }

    //여우 예측공격
    private AttackPrediction getFoxAttackPrediction(Summon fox, int foxPlateIndex, List<Plate> playerPlates, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = foxAttackPrediction.getClosestEnermyIndex(enermyPlates);
        List<Plate> targetPlate = enermyPlates;

        //소환수, 소환수의 플레이트 번호, 소환수의 특수공격첫번째, 특수공격배열 인덱스번호, 타겟플레이트, 타겟플레이트 변호, 확률
        AttackPrediction attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);


        if (foxAttackPrediction.getIndexOfSummonWithCurseStatus(playerPlates) != -1) //소환수중 저주상태에 걸려있는 몹이 있는가?
        {
            attackProbability = new AttackProbability(0f, 100f); //특수공격 100%
            attackIndex = foxAttackPrediction.getIndexOfSummonWithCurseStatus(playerPlates);
            attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }
        else if(foxAttackPrediction.isTwoOrMoreEnemies(enermyPlates)) //적이 2마리 이상 존재하는가?
        {
            if (foxAttackPrediction.AllEnemiesHealthOver50(enermyPlates)) //적의 체력이 모두 50% 이상인가?
            {
                if (foxAttackPrediction.AllSummonsLowOrMediumRank(playerPlates)) //아군의 등급이 모두 하급과 중급인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                    targetPlate = playerPlates;
                }
            }
            else if (foxAttackPrediction.isAnyEnemyHealthDown30Percent(enermyPlates)) //적의 체력이 하나만 30% 아래인가
            {
                if (foxAttackPrediction.CanNormalAttackKill(fox, enermyPlates)) //일반공격시 처치할 수 있는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
            }
        }
        else if(foxAttackPrediction.isOnlyOneEnemy(enermyPlates)) //적이 1마리 인가?
        {
            if (foxAttackPrediction.isAnyEnemyHealthOver70Percent(enermyPlates)) //적의 체력이 70% 이상인가?
            {
                if (foxAttackPrediction.AllSummonsLowOrMediumRank(playerPlates)) //아군의 등급이 모두 하급과 중급인가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                    targetPlate = playerPlates;
                }
            }
            else if(foxAttackPrediction.CanNormalAttackKill(fox, enermyPlates)) //일반공격시 몬스터를 물리칠 수 있는가?
            {
                if (foxAttackPrediction.CanNormalAttackKill(fox, enermyPlates)) //일반공격시 처치할 수 있는가?
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, true);
                }
            }
        }
        else
        {
            attackIndex = foxAttackPrediction.getIndexOfHighestAttackPower(playerPlates);
            attackPrediction = new AttackPrediction(fox, foxPlateIndex, fox.getSpecialAttackStrategy()[0], 0, playerPlates, attackIndex, attackProbability);
        }

        return attackPrediction;
    }


    //독수리 예측공격
    private AttackPrediction getEagleAttackPrediction(Summon eagle,int eaglePlateIndex, List<Plate> enermyPlates)
    {
        // 기본값 설정: 일반 공격 50%, 특수 공격 50%
        AttackProbability attackProbability = new AttackProbability(50f, 50f);
        int attackIndex = eagleAttackPrediction.getClosestEnermyIndex(enermyPlates); //가장 가까운적의 인덱스 기본값


        if (eagleAttackPrediction.isTwoOrMoreEnemies(enermyPlates)) //적이 2마리 이상인가?
        {
            if (eagleAttackPrediction.isEnermyHealthDifferenceOver30(enermyPlates)) //몬스터 한 쪽이 다른쪽과 비교했을 때 30% 이상 낮은가?
            {
                if (eagleAttackPrediction.getNormalAttackKillIndex(eagle, enermyPlates) != -1) //일반 공격으로 체력이 낮은 쪽을 공격할 수 있는가?
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
            else if (eagleAttackPrediction.AreEnermyHealthWithin10Percent(enermyPlates)) //몬스터의 체력이 서로 비슷한가?
            {
                if (eagleAttackPrediction.getIndexOfMostHealthEnermy(enermyPlates) != -1) //가장 체력이 많은 몬스터의 인덱스
                {
                    attackIndex = eagleAttackPrediction.getIndexOfMostHealthEnermy(enermyPlates);
                    attackProbability = AdjustAttackProbabilities(attackProbability, 10f, false);
                }
            }
        }
        else if (eagleAttackPrediction.isOnlyOneEnemy(enermyPlates)) //적이 1마리 인가?
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
                if(eagleAttackPrediction.getTypeOfMoreAttackDamage(eagle,enermyPlates) == AttackType.NormalAttack)//일반공격과 특수공격 중 피해를 많이 줄 공격에 5%상승
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, true);
                }
                else
                {
                    attackProbability = AdjustAttackProbabilities(attackProbability, 5f, false);
                }
            }
        }

        //소환수, 소환수의 플레이트 번호, 소환수의 특수공격첫번째, 특수공격배열 인덱스번호, 타겟플레이트, 타겟플레이트 변호, 확률
        AttackPrediction attackPrediction = new AttackPrediction(eagle, eaglePlateIndex, eagle.getSpecialAttackStrategy()[0], 0, enermyPlates, attackIndex, attackProbability);
        return attackPrediction;
    }


        // 확률 값을 설정하고 조정하여 반환하는 메소드
        public AttackProbability AdjustAttackProbabilities(AttackProbability currentProbabilities, float AttackChange, bool isNormalAttack)
    {
        if (isNormalAttack)
        {
            // 일반 공격 확률을 증가시키고, 특수 공격 확률을 그만큼 감소
            currentProbabilities.normalAttackProbability += AttackChange;
            currentProbabilities.specialAttackProbability -= AttackChange;
        }
        else
        {
            // 또는, 특수 공격 확률을 증가시키고, 일반 공격 확률을 그만큼 감소
            currentProbabilities.specialAttackProbability += AttackChange;
            currentProbabilities.normalAttackProbability -= AttackChange;
        }
        return currentProbabilities;
    }

 
}
