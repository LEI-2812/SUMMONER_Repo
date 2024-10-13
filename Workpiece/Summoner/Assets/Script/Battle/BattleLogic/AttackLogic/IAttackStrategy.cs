using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackStrategy
{
   void Attack(Summon attacker, List<Plate> enemyPlates, int selectedPlateIndex, int SpecialAttackarrayIndex);

    StatusType getStatusType(); // ���� Ÿ�� ��ȯ �޼��� �߰�
    double getSpecialDamage(); //Ư�����ݷ°� ��ȯ
    int getCooltime(); // ��Ÿ�� �� ��ȯ
    int getCurrentCooldown(); // ���� ��Ÿ�� ���� ��ȯ
    void ApplyCooldown(); // ��Ÿ�� ����
    void ReduceCooldown(); // �� ���� �� ��Ÿ�� ����
}
