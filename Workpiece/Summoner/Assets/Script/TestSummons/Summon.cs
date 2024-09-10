using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SummonRank
{
    Low, Medium, High
}

public class Summon : MonoBehaviour
{
    public string summonName;
    public int health;
    public double attackPower; //�Ϲݰ���
    public double SpecialPower;  //Ư������
    public SummonRank summonRank;

    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(int damage) //������ �Ա�
    {
        health -= damage;
        Debug.Log($"{summonName} takes {damage} damage. Remaining health: {health}");
        if (health <= 0)
        {
            die();
        }
    }

    public virtual void takeSkill() //��ų���
    {
        Debug.Log($"{summonName} takes skill.");
        
    }

    public virtual void die()
    {
        Debug.Log($"{summonName} has died.");
        Destroy(gameObject); // ��ȯ�� ������Ʈ ����
    }

}
