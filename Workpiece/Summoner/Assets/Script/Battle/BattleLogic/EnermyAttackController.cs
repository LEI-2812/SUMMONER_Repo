using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("��Ʈ�ѷ�")]
    [SerializeField] private BattleController battleController;
    
    public void EnermyAttackLogic(Summon attakingSummon)
    {
        if (attakingSummon == null)
        {
            Debug.LogError("������ ��ȯ���� �����ϴ�.");
            return;
        }


    }

    //private void normalAttackLogic()
    //{
    //    double attackDamage = getNormalAttackDamage(attackingSummon);
    //    Plate targetPlate = battleController.GetRandomEnemyPlate(); // ���� �� ����

    //    if (targetPlate != null && targetPlate.getSummon() != null)
    //    {
    //        targetPlate.getSummon().takeDamage(attackDamage);
    //        Debug.Log($"{attackingSummon.getSummonName()}��(��) {attackDamage} �������� ���� �����߽��ϴ�.");
    //    }
    //}

    //private double getNormalAttackDamage(Summon summon)
    //{
    //    int randomValue = Random.Range(1, 11); //1~11
    //    if(randomValue < 7) //�Ϲݰ��ݷ�
    //    {
    //        Debug.Log($"{attackingSummon.getSummonName()}��(��) �Ϲ� ������ �����մϴ�.");
    //        return summon.AttackPower;
    //    }
    //    else //�����ݷ�
    //    {

    //    }
    //    return summon.getHeavyPower();
    //}
}
