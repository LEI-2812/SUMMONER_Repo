using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High
}

public class Summon : MonoBehaviour
{
    public Image image;
    public string summonName;
    public double maxHP;
    public double nowHP;
    public double attackPower; //일반공격
    public double SpecialPower;  //특수공격
    public SummonRank summonRank;

    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(int damage) //데미지 입기
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

}
