// 역할: 공격 흐름의 현재 상태를 표현한다.
public enum AttackState
{
    Idle,
    SourceSelected,
    TargetSelecting,
    Executing,
    Completed,
    Canceled
}
