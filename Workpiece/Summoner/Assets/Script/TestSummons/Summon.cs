using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, //아군 소환수
    Normal, Special, Boss //적 소환수
}

public class Summon : MonoBehaviour
{
    public Image image; //이미지
    protected string summonName; //이름
    protected double maxHP; //최대체력
    protected double nowHP; //현재 체력
    protected double attackPower; //일반공격
    protected double specialPower;  //특수공격
    protected SummonRank summonRank; //등급

    protected IAttackStrategy AttackStrategy { get; set; } // 일반 공격 전략
    protected IAttackStrategy SpecialAttackStrategy { get; set; } // 특수 공격 전략

    public void normalAttack(List<Plate> enemyPlates, int selectedPlateIndex)
    {
        AttackStrategy?.Attack(this, enemyPlates, selectedPlateIndex);
    }
    public void SpecialAttack(List<Plate> enemyPlates, int selectedPlateIndex)
    {
        SpecialAttackStrategy?.Attack(this, enemyPlates, selectedPlateIndex);
    }

    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(double damage) //데미지 입기
    {
        nowHP = nowHP - damage;
        Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
        if (nowHP <= 0)
        {
            die();
        }
    }

    public virtual void takeSkill() //스킬사용
    {
        Debug.Log($"{summonName} takes skill.");
        
    }

    public virtual void die()
    {
        Debug.Log($"{summonName} has died.");
        Destroy(gameObject); // 소환수 오브젝트 삭제
    }

    public string SummonName
    {
        get { return summonName; }
        set { summonName = value; }
    }

    public double MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public double NowHP
    {
        get { return nowHP; }
        set { nowHP = value; }
    }

    public double AttackPower
    {
        get { return attackPower; }
        set { attackPower = value; }
    }

    public double SpecialPower
    {
        get { return specialPower; }
        set { specialPower = value; }
    }

    public SummonRank SummonRank
    {
        get { return summonRank; }
        set { summonRank = value; }
    }

}
