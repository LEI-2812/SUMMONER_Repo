// 역할: 예측된 일반 공격과 특수 공격 선택 확률을 담는다.
public struct AttackProbability
{
    // 역할: 일반 공격을 선택할 가능성을 퍼센트 값으로 저장한다.
    public float normalAttackProbability;

    // 역할: 특수 공격을 선택할 가능성을 퍼센트 값으로 저장한다.
    public float specialAttackProbability;

    public AttackProbability(float normalProb, float specialProb)
    {
        normalAttackProbability = normalProb;
        specialAttackProbability = specialProb;
    }
}
