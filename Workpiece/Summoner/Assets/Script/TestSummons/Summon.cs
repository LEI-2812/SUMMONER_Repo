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
    public string summonName; //�̸�
    public double maxHP; //�ִ�ü��
    public double nowHP; //���� ü��
    public double attackPower; //�Ϲݰ���
    public double SpecialPower;  //Ư������
    public SummonRank summonRank; //���

    public virtual void attack()
    {
        Debug.Log($"{summonName} attacks with {attackPower} power!");
    }

    public virtual void takeDamage(int damage) //������ �Ա�
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

}
