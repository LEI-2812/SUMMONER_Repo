// 역할: 적이 예측 공격에 대응할 때 사용할 특수공격 후보를 선택한다.
// 책임 아님: 공격 실행, 예측 목록 제거, 확률 판정.
internal class EnemySpecialAttackPicker
{
    // 역할: 각 대응 상황마다 "이 스킬을 고를 수 있는가"만 다르게 판단하게 만드는 규칙 함수다.
    private delegate EnemySpecialAttackPick SpecialAttackPickRule(IAttackStrategy attackStrategy, int specialAttackIndex);

    public EnemySpecialAttackPick PickPoisonReaction(Summon attacker, AttackPrediction playerPrediction, int lowestHealthEnemyPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Heal)
            {
                return EnemySpecialAttackPick.Create(lowestHealthEnemyPlateIndex, index, $"{attacker.GetSummonName()}가 힐 특수 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy is TargetedAttackStrategy)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 특수 저격 공격을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickTargetNoneReaction(Summon attacker, int attackingEnemyPlateIndex, AttackPrediction playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Stun)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 스턴 특수 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Shield)
            {
                return EnemySpecialAttackPick.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}가 쉴드 특수 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Heal)
            {
                return EnemySpecialAttackPick.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}가 힐 특수 공격을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickAllNoneReaction(Summon attacker, int attackingEnemyPlateIndex, AttackPrediction playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Stun)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 스턴 특수 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Shield)
            {
                return EnemySpecialAttackPick.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}가 쉴드 특수 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Heal)
            {
                return EnemySpecialAttackPick.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}가 힐 특수 스킬을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickHealReaction(Summon attacker, AttackPrediction playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy is TargetedAttackStrategy)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 특수 저격 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy is AttackAllEnemiesStrategy)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 특수 전체 공격을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickUpgradeReaction(Summon attacker, AttackPrediction playerPrediction)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Curse)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 특수 저주 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy is TargetedAttackStrategy)
            {
                return EnemySpecialAttackPick.Create(playerPrediction.GetAttackSummonPlateIndex(), index, $"{attacker.GetSummonName()}가 특수 저격 공격을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickLifeDrainReaction(Summon attacker, int targetPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.LifeDrain)
            {
                return EnemySpecialAttackPick.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}가 특수 흡혈 공격을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickDefaultNormalReaction(Summon attacker, int targetPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Upgrade)
            {
                return EnemySpecialAttackPick.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}가 특수 강화 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Burn)
            {
                return EnemySpecialAttackPick.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}가 특수 화상 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.None && attackStrategy is AttackAllEnemiesStrategy)
            {
                return EnemySpecialAttackPick.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}가 특수 전체 공격을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    public EnemySpecialAttackPick PickMediumRankReaction(Summon attacker, int attackingEnemyPlateIndex, int targetPlateIndex)
    {
        return PickFirstUsableSpecialAttack(attacker, (attackStrategy, index) =>
        {
            if (attackStrategy.GetStatusType() == StatusType.Curse)
            {
                return EnemySpecialAttackPick.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}가 특수 저주 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Stun)
            {
                return EnemySpecialAttackPick.Create(targetPlateIndex, index, $"{attacker.GetSummonName()}가 특수 혼란 공격을 실행했습니다.");
            }

            if (attackStrategy.GetStatusType() == StatusType.Shield)
            {
                return EnemySpecialAttackPick.Create(attackingEnemyPlateIndex, index, $"{attacker.GetSummonName()}가 특수 쉴드 스킬을 실행했습니다.");
            }

            return EnemySpecialAttackPick.None();
        });
    }

    private EnemySpecialAttackPick PickFirstUsableSpecialAttack(
        Summon attacker,
        SpecialAttackPickRule pickAttack)
    {
        // 스킬 배열 순서를 유지해야 기존 대응 우선순위가 바뀌지 않는다.
        IAttackStrategy[] attackStrategies = attacker.GetSpecialAttackStrategy();
        if (attackStrategies == null)
        {
            return EnemySpecialAttackPick.None();
        }

        for (int i = 0; i < attackStrategies.Length; i++)
        {
            IAttackStrategy attackStrategy = attackStrategies[i];
            if (!CanUseSpecialAttack(attacker, attackStrategy))
            {
                continue;
            }

            EnemySpecialAttackPick pick = pickAttack(attackStrategy, i);
            if (pick.HasValue)
            {
                return pick;
            }
        }

        return EnemySpecialAttackPick.None();
    }

    private bool CanUseSpecialAttack(Summon attacker, IAttackStrategy attackStrategy)
    {
        // 쿨타임 중인 스킬은 후보에서 제외한다.
        return attackStrategy != null && !attacker.IsSpecialAttackCool(attackStrategy);
    }
}

// 역할: 선택된 특수공격의 대상, 스킬 인덱스, 실행 로그를 함께 전달한다.
internal struct EnemySpecialAttackPick
{
    // 역할: 사용할 수 있는 특수 공격 후보가 실제로 선택됐는지 저장한다.
    public bool HasValue { get; }

    // 역할: 선택된 특수 공격이 겨냥할 플레이트 위치를 저장한다.
    public int TargetPlateIndex { get; }

    // 역할: 소환수의 특수 공격 배열에서 몇 번째 스킬을 쓸지 저장한다.
    public int SpecialAttackIndex { get; }

    // 역할: 공격 실행 후 출력할 디버그 로그 문장을 저장한다.
    public string LogMessage { get; }

    private EnemySpecialAttackPick(bool hasValue, int targetPlateIndex, int specialAttackIndex, string logMessage)
    {
        HasValue = hasValue;
        TargetPlateIndex = targetPlateIndex;
        SpecialAttackIndex = specialAttackIndex;
        LogMessage = logMessage;
    }

    public static EnemySpecialAttackPick Create(int targetPlateIndex, int specialAttackIndex, string logMessage)
    {
        return new EnemySpecialAttackPick(true, targetPlateIndex, specialAttackIndex, logMessage);
    }

    public static EnemySpecialAttackPick None()
    {
        return new EnemySpecialAttackPick(false, -1, -1, string.Empty);
    }
}
