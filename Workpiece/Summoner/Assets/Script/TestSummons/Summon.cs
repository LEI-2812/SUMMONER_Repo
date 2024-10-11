using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SummonRank
{
    Low, Medium, High, //�Ʊ� ��ȯ��
    Normal, Special, Boss //�� ��ȯ��
}

public class Summon : MonoBehaviour
{
    public Image image; //�̹���
    protected string summonName; //�̸�
    protected double maxHP; //�ִ�ü��
    protected double nowHP; //���� ü��
    protected double attackPower; //�Ϲݰ���
    protected double specialPower;  //Ư������
    protected SummonRank summonRank; //���

    protected IAttackStrategy AttackStrategy { get; set; } // �Ϲ� ���� ����
    protected IAttackStrategy SpecialAttackStrategy { get; set; } // Ư�� ���� ����

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

    public virtual void takeDamage(double damage) //������ �Ա�
    {
        nowHP = nowHP - damage;
        Debug.Log($"{summonName} takes {damage} damage. Remaining health: {nowHP}");
        if (nowHP <= 0)
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
