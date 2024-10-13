using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private BattleController battleController;
    
    public void EnermyAttackLogic(Summon attakingSummon)
    {
        if (attakingSummon == null)
        {
            Debug.LogError("공격할 소환수가 없습니다.");
            return;
        }


    }

    //private void normalAttackLogic()
    //{
    //    double attackDamage = getNormalAttackDamage(attackingSummon);
    //    Plate targetPlate = battleController.GetRandomEnemyPlate(); // 랜덤 적 선택

    //    if (targetPlate != null && targetPlate.getSummon() != null)
    //    {
    //        targetPlate.getSummon().takeDamage(attackDamage);
    //        Debug.Log($"{attackingSummon.getSummonName()}이(가) {attackDamage} 데미지로 적을 공격했습니다.");
    //    }
    //}

    //private double getNormalAttackDamage(Summon summon)
    //{
    //    int randomValue = Random.Range(1, 11); //1~11
    //    if(randomValue < 7) //일반공격력
    //    {
    //        Debug.Log($"{attackingSummon.getSummonName()}이(가) 일반 공격을 수행합니다.");
    //        return summon.AttackPower;
    //    }
    //    else //강공격력
    //    {

    //    }
    //    return summon.getHeavyPower();
    //}
}
