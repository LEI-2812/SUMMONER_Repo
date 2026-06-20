using UnityEngine;

// 역할: 플레이어 턴에서 사용하는 마나, 재소환 비용, 턴당 소환 여부를 저장한다.
public class PlayerTurnResourceState
{
    public int Mana { get; private set; }
    public int RedrawCost { get; private set; }
    public bool HasSummonedThisTurn { get; private set; }

    public void Reset()
    {
        Mana = 10;
        RedrawCost = 1;
        HasSummonedThisTurn = false;
    }

    public void StartTurn()
    {
        HasSummonedThisTurn = false;
    }

    public bool CanUseSummon()
    {
        return Mana > 0 && !HasSummonedThisTurn;
    }

    public bool CanShowSummonAvailable()
    {
        return CanUseSummon();
    }

    public void UseSummonMana()
    {
        Mana -= 1;
        HasSummonedThisTurn = true;
    }

    public bool CanRedraw()
    {
        return Mana >= RedrawCost;
    }

    public void UseRedrawMana()
    {
        Mana -= RedrawCost;
        RedrawCost += 1;
    }

    public void AddMana(int maxMana)
    {
        Mana = Mathf.Min(Mana + 1, maxMana);
    }

    public void SetHasSummonedThisTurn(bool value)
    {
        HasSummonedThisTurn = value;
    }
}
