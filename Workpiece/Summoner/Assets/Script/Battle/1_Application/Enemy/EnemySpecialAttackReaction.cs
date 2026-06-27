using System.Collections.Generic;
using UnityEngine;

// 역할: EnemySpecialAttackReaction의 책임을 정의한다.
internal class EnemySpecialAttackReaction
{
    private delegate EnemySpecialAttackPickData SpecialAttackPickStep(AttackData attackStrategy, int specialAttackIndex);

    private readonly PlateBoardView plateBoardController;

    private readonly EnemyNormalAttackReaction normalAttackReaction;

    private readonly AttackStateMachine attackStateMachine;

    private readonly EnemySpecialAttackExecution specialAttackExecution;

    public EnemySpecialAttackReaction(
        PlateBoardView plateBoardController,
        EnemyNormalAttackReaction normalAttackReaction,
        AttackStateMachine attackStateMachine)
    {
        this.plateBoardController = plateBoardController;
        IReadOnlyList<BattleBoardInputController> playerPlates = plateBoardController.GetPlayerPlates();
        IReadOnlyList<BattleBoardInputController> enemyPlates = plateBoardController.GetEnemyPlates();
        this.normalAttackReaction = normalAttackReaction;
        this.attackStateMachine = attackStateMachine;
        specialAttackExecution = new EnemySpecialAttackExecution(
            playerPlates,
            enemyPlates);
    }

    public bool TryReactToPrediction(
        Summon attacker,
        int attackerPlateIndex,
        AttackPredictionData playerPrediction)
    {
        AttackData predictedAttack = playerPrediction.GetAttackStrategy();
        if (predictedAttack == null)
        {
            Debug.Log("적 예측 공격 데이터가 없습니다.");
            return false;
        }

        if (predictedAttack.IsStrategy<AttackAllEnemiesStrategy>())
        {
            if (predictedAttack.GetStatusType() == StatusType.Poison)
            {
                return TryReactToPoisonAllAttack(attacker, attackerPlateIndex, playerPrediction);
            }

            if (predictedAttack.GetStatusType() == StatusType.None)
            {
                return TryReactToAllAttack(attacker, attackerPlateIndex, playerPrediction);
            }
        }
        else if (predictedAttack.IsStrategy<TargetedAttackStrategy>())
        {
            if (predictedAttack.GetStatusType() == StatusType.None)
            {
                return TryReactToTargetAttack(attacker, attackerPlateIndex, playerPrediction);
            }

            if (predictedAttack.GetStatusType() == StatusType.Upgrade)
            {
                return TryReactToUpgradeAttack(attacker, attackerPlateIndex, playerPrediction);
            }

            if (predictedAttack.GetStatusType() == StatusType.Heal)
            {
                return TryReactToHealAttack(attacker, attackerPlateIndex, playerPrediction);
            }
        }
        else if (predictedAttack.IsStrategy<ClosestEnemyAttackStrategy>())
        {
            return false;
        }
        else
        {
            Debug.Log("적 예측 공격 전략을 지원하지 않습니다.");
            return false;
        }

        Debug.Log("적이 일치하는 특수 반응 분기를 찾지 못했습니다.");
        return false;
    }

    private bool TryReactToPoisonAllAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPredictionData playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReaction.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()} 이 일반 공격으로 반응했습니다.");
            return true;
        }

        EnemySpecialAttackPickData pick = PickPoisonReaction(
            attacker,
            playerPrediction,
            plateBoardController.GetLowestHealthEnemyPlateIndex());
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToTargetAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPredictionData playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReaction.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()} 이 일반 공격으로 반응했습니다.");
            return true;
        }

        EnemySpecialAttackPickData pick = PickTargetNoneReaction(
            attacker,
            attackerPlateIndex,
            playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToAllAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPredictionData playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReaction.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()} 이 일반 공격으로 반응했습니다.");
            return true;
        }

        EnemySpecialAttackPickData pick = PickAllNoneReaction(
            attacker,
            attackerPlateIndex,
            playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToHealAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPredictionData playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReaction.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()} 이 일반 공격으로 반응했습니다.");
            return true;
        }

        EnemySpecialAttackPickData pick = PickHealReaction(attacker, playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool TryReactToUpgradeAttack(
        Summon attacker,
        int attackerPlateIndex,
        AttackPredictionData playerPrediction)
    {
        if (!HasSpecialAttacks(attacker))
        {
            normalAttackReaction.ExecuteNormalReaction(
                attacker,
                attackerPlateIndex,
                playerPrediction.GetAttackSummonPlateIndex());
            Debug.Log($"{attacker.GetSummonName()} 이 일반 공격으로 반응했습니다.");
            return true;
        }

        EnemySpecialAttackPickData pick = PickUpgradeReaction(attacker, playerPrediction);
        return TryExecuteSpecialAttackPick(attacker, pick);
    }

    private bool HasSpecialAttacks(Summon attacker)
    {
        return attacker.GetSpecialAttackStrategy() != null;
    }

    private EnemySpecialAttackPickData PickPoisonReaction(Summon attacker, AttackPredictionData playerPrediction, int lowestHealthEnemyPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Heal)
            {
                return EnemySpecialAttackPickData.Create(lowestHealthEnemyPlateIndex, index, $"{attacker.GetSummonName()}이 회복 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy.IsStrategy<TargetedAttackStrategy>())
            {
                return EnemySpecialAttackPickData.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}이 대상 지정 특수 공격을 선택했습니다.");
            }

            return EnemySpecialAttackPickData.None();
        });
    }

    private EnemySpecialAttackPickData PickTargetNoneReaction(Summon attacker, int attackingEnemyPlateIndex, AttackPredictionData playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Stun)
            {
                return EnemySpecialAttackPickData.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}이 기절 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Shield)
            {
                return EnemySpecialAttackPickData.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}이 보호막 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Heal)
            {
                return EnemySpecialAttackPickData.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}이 회복 특수 공격을 선택했습니다.");
            }

            return EnemySpecialAttackPickData.None();
        });
    }

    private EnemySpecialAttackPickData PickAllNoneReaction(Summon attacker, int attackingEnemyPlateIndex, AttackPredictionData playerPrediction)
    {
        return PickTargetNoneReaction(attacker, attackingEnemyPlateIndex, playerPrediction);
    }

    private EnemySpecialAttackPickData PickHealReaction(Summon attacker, AttackPredictionData playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy.IsStrategy<TargetedAttackStrategy>())
            {
                return EnemySpecialAttackPickData.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}이 대상 지정 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy.IsStrategy<AttackAllEnemiesStrategy>())
            {
                return EnemySpecialAttackPickData.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}이 전체 대상 특수 공격을 선택했습니다.");
            }

            return EnemySpecialAttackPickData.None();
        });
    }

    private EnemySpecialAttackPickData PickUpgradeReaction(Summon attacker, AttackPredictionData playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Curse)
            {
                return EnemySpecialAttackPickData.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}이 저주 특수 공격을 선택했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy.IsStrategy<TargetedAttackStrategy>())
            {
                return EnemySpecialAttackPickData.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}이 대상 지정 특수 공격을 선택했습니다.");
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
            false);
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
