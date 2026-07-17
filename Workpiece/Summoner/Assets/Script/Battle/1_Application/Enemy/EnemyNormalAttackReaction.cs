using System.Collections.Generic;
using UnityEngine;

// 역할: EnemyNormalAttackReaction의 책임을 정의한다.
class EnemyNormalAttackReaction
{
    private delegate EnemySpecialAttackPickData SpecialAttackPickStep(AttackData attackStrategy, int specialAttackIndex);

    private readonly PlateBoardView plateBoardController;

    private readonly IReadOnlyList<BattleBoardInputController> playerPlates;

    private readonly AttackStateMachine attackStateMachine;

    private readonly SpecialAttackExecution specialAttackExecution;

    public EnemyNormalAttackReaction(
        PlateBoardView plateBoardController,
        AttackStateMachine attackStateMachine)
    {
        this.plateBoardController = plateBoardController;
        playerPlates = plateBoardController.GetPlayerPlates();
        this.attackStateMachine = attackStateMachine;
        specialAttackExecution = new SpecialAttackExecution(
            playerPlates,
            plateBoardController.GetEnemyPlates());
    }

    public void ExecuteNormalReaction(Summon attacker, int attackerPlateIndex, int targetPlateIndex)
    {
        if (!HasSpecialAttacks(attacker))
        {
            ExecutePickedNormalAttack(attacker, targetPlateIndex);
            return;
        }

        if (CountPlayerSummons() >= 2)
        {
            int highHealthPlayerIndex = PickHighHealthPlayerPlateIndexWithLowAlly();

            if (highHealthPlayerIndex != -1)
            {
                Debug.Log("적이 체력이 높은 플레이어 대상과 체력이 낮은 아군을 찾았습니다.");
                EnemySpecialAttackPickData pick = PickLifeDrainReaction(
                    attacker,
                    highHealthPlayerIndex);

                if (TryExecuteSpecialAttackPick(attacker, pick))
                {
                    return;
                }
            }
            else
            {
                EnemySpecialAttackPickData pick = PickDefaultNormalReaction(
                    attacker,
                    targetPlateIndex);

                if (TryExecuteSpecialAttackPick(attacker, pick))
                {
                    return;
                }
            }
        }

        if (HasPlayerSummonOverMediumRank())
        {
            EnemySpecialAttackPickData pick = PickMediumRankReaction(
                attacker,
                attackerPlateIndex,
                targetPlateIndex);

            if (TryExecuteSpecialAttackPick(attacker, pick))
            {
                return;
            }
        }

        ExecutePickedNormalAttack(attacker, targetPlateIndex);
    }

    private bool HasSpecialAttacks(Summon attacker)
    {
        return attacker.GetSpecialAttackStrategy() != null;
    }

    private void ExecutePickedNormalAttack(Summon attacker, int targetPlateIndex)
    {
        if (ShouldUseHeavyNormalAttack())
        {
            ExecuteHeavyNormalAttack(attacker);
            return;
        }

        ExecuteNormalAttack(attacker, playerPlates, targetPlateIndex);
        Debug.Log($"{attacker.GetSummonName()}이 일반 공격을 사용했습니다.");
    }

    private void ExecuteHeavyNormalAttack(Summon attacker)
    {
        Debug.Log($"{attacker.name} 강한 공격");
        double originPower = attacker.GetAttackPower();
        attacker.SetAttackPower(attacker.GetHeavyAttackPower());
        ExecuteNormalAttack(
            attacker,
            playerPlates,
            plateBoardController.GetClosestPlayerPlateIndexExcept(attacker));
        attacker.SetAttackPower(originPower);
    }

    private void ExecuteNormalAttack(
        Summon attackSummon,
        IReadOnlyList<BattleBoardInputController> targetPlates,
        int selectedPlateIndex)
    {
        if (attackSummon == null)
        {
            Debug.Log("공격 소환수가 없습니다.");
            return;
        }

        AttackData attackStrategy = attackSummon.GetAttackStrategy();
        if (attackStrategy == null)
        {
            Debug.Log("일반 공격 데이터가 없습니다.");
            return;
        }

        if (!attackSummon.CanUseAttackStrategy(attackStrategy))
        {
            Debug.Log("일반 공격이 쿨타임 중이거나 사용할 수 없습니다.");
            return;
        }

        List<Summon> targets = attackStrategy.SelectTargets(attackSummon, targetPlates, selectedPlateIndex);
        if (targets.Count == 0)
        {
            Debug.Log("일반 공격 대상이 없습니다.");
        }

        foreach (Summon target in targets)
        {
            ApplyNormalAttackEffect(attackSummon, attackStrategy, target);
        }

        attackSummon.PlayAttackMotion(true);
        attackSummon.ApplyAttackCooldown(attackStrategy);
        attackSummon.SetAttackAvailable(false);
    }

    private void ApplyNormalAttackEffect(Summon attacker, AttackData attackStrategy, Summon target)
    {
        if (attackStrategy.GetStatusType() != StatusType.None)
        {
            Debug.LogWarning($"일반 공격으로 적용할 수 없는 상태입니다: {attackStrategy.GetStatusType()}");
            return;
        }

        double damage = attacker.GetAttackPower();
        Debug.Log($"{attacker.GetSummonName()}이 {target.GetSummonName()}을 공격했습니다.");
        target.TakeDamage(damage);
    }

    private int CountPlayerSummons()
    {
        int count = 0;
        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (playerPlates[i].GetIsInSummon())
            {
                count++;
            }
        }

        return count;
    }

    private int PickHighHealthPlayerPlateIndexWithLowAlly()
    {
        if (playerPlates.Count < 2)
        {
            return -1;
        }

        int highestHealthIndex = -1;
        double highestHealth = double.MinValue;

        for (int i = 0; i < playerPlates.Count; i++)
        {
            Summon currentSummon = playerPlates[i].GetCurrentSummon();
            if (currentSummon == null)
            {
                continue;
            }

            if (currentSummon.GetNowHP() > highestHealth)
            {
                highestHealth = currentSummon.GetNowHP();
                highestHealthIndex = i;
            }
        }

        if (highestHealthIndex == -1)
        {
            return -1;
        }

        for (int i = 0; i < playerPlates.Count; i++)
        {
            if (i == highestHealthIndex)
            {
                continue;
            }

            Summon compareSummon = playerPlates[i].GetCurrentSummon();
            if (compareSummon == null)
            {
                continue;
            }

            if (compareSummon.GetNowHP() <= highestHealth * 0.7)
            {
                return highestHealthIndex;
            }
        }

        return -1;
    }

    private bool HasPlayerSummonOverMediumRank()
    {
        foreach (BattleBoardInputController plate in playerPlates)
        {
            Summon summon = plate.GetCurrentSummon();
            if (summon != null && (summon.GetSummonRank() == SummonRank.Medium || summon.GetSummonRank() == SummonRank.High))
            {
                return true;
            }
        }

        return false;
    }

    private bool ShouldUseHeavyNormalAttack()
    {
        return Random.Range(0f, 100f) < 30f;
    }

    private EnemySpecialAttackPickData PickLifeDrainReaction(Summon attacker, int targetPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.LifeDrain)
            {
                return EnemySpecialAttackPickData.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}이 흡혈 특수 공격을 선택했습니다.");
            }

            return EnemySpecialAttackPickData.None();
        });
    }

    private EnemySpecialAttackPickData PickDefaultNormalReaction(Summon attacker, int targetPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Upgrade)
            {
                return EnemySpecialAttackPickData.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}이 강화 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Burn)
            {
                return EnemySpecialAttackPickData.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}이 화상 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy.IsStrategy<AttackAllEnemiesStrategy>())
            {
                return EnemySpecialAttackPickData.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}이 전체 대상 특수 공격을 선택했습니다.");
            }

            return EnemySpecialAttackPickData.None();
        });
    }

    private EnemySpecialAttackPickData PickMediumRankReaction(Summon attacker, int attackingEnemyPlateIndex, int targetPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Curse)
            {
                return EnemySpecialAttackPickData.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}이 저주 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Stun)
            {
                return EnemySpecialAttackPickData.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}이 기절 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Shield)
            {
                return EnemySpecialAttackPickData.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}이 보호막 특수 공격을 선택했습니다.");
            }

            return EnemySpecialAttackPickData.None();
        });
    }

    private EnemySpecialAttackPickData PickFirstUsableSpecialAttack(
        Summon attacker,
        SpecialAttackPickStep pickAttack)
    {
        AttackData[] attackStrategies = attacker.GetSpecialAttackStrategy();
        if (attackStrategies == null)
        {
            return EnemySpecialAttackPickData.None();
        }

        for (int i = 0; i < attackStrategies.Length; i++)
        {
            AttackData attackStrategy = attackStrategies[i];
            if (!CanUseSpecialAttack(attacker, attackStrategy))
            {
                continue;
            }

            EnemySpecialAttackPickData pick = pickAttack(attackStrategy, i);
            if (pick.HasValue)
            {
                return pick;
            }
        }

        return EnemySpecialAttackPickData.None();
    }

    private bool CanUseSpecialAttack(Summon attacker, AttackData attackStrategy)
    {
        return attackStrategy != null && !attacker.IsSpecialAttackCool(attackStrategy);
    }

    private bool TryExecuteSpecialAttackPick(Summon attacker, EnemySpecialAttackPickData pick)
    {
        if (!pick.HasValue)
        {
            return false;
        }

        bool executed = specialAttackExecution.Execute(
            attacker,
            pick.TargetPlateIndex,
            pick.SpecialAttackIndex,
            isPlayerAttacker: false);
        if (executed)
        {
            attackStateMachine.CompleteAttack();
            ResetAttackState();
        }

        if (executed && !string.IsNullOrEmpty(pick.LogMessage))
        {
            Debug.Log(pick.LogMessage);
        }

        return executed;
    }

    private void ResetAttackState()
    {
        attackStateMachine.Reset();
        plateBoardController.ResetAllPlateHighlight();
    }
}
