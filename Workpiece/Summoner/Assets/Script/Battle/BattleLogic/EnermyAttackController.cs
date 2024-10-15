using System.Collections.Generic;
using UnityEngine;


public class EnermyAttackController : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;
    private enum AttackType{ NormalAttack, SpecialAttack};
    
    public void EnermyAttackLogic(Summon attakingSummon)
    {
        if (attakingSummon == null)
        {
            Debug.LogError("공격할 소환수가 없습니다.");
            return;
        }

        if (!attakingSummon.IsCooltime()) //쿨타임중인 스킬이 없을경우
        {
            AttackType selectedAttakType = SelectAttackType(); //일반공격과 특수공격을 랜덤으로 받아옴
            if(selectedAttakType == AttackType.SpecialAttack)
            {
                //쿨타임이 없는 특수스킬을 사용하게 한다. (2개 이상이면 랜덤으로)
                // 쿨타임이 없는 특수 스킬 목록을 가져옴
                List<int> availableSpecialAttacks = new List<int>();
            }
            else
            {
                normalAttackLogic(attakingSummon); //평타
            }
        }
        else //스킬들이 쿨타임이여서 평타만 공격
        {
            normalAttackLogic(attakingSummon);
        }





    }

    private void normalAttackLogic(Summon attackingSummon)
    {
        int selectAttackIndex = plateController.getClosestPlayerPlatesIndex(); //플레이어 플레이트에서 가장 가까운 소환수의 인덱스를 받아온다.
        if (selectAttackIndex < 0)
        {
            Debug.Log("공격할 소환수가 없습니다."); return;
        }

        attackingSummon.normalAttack(plateController.getPlayerPlates(), selectAttackIndex); //일반공격 수행
    }




    //50% 50% 일반공격과 특수공격을 받아온다.
    private AttackType SelectAttackType()
    {
        float randomValue = Random.Range(0f, 100f); // 0에서 100 사이의 무작위 값

        if (randomValue <= 50)
        {
            return AttackType.NormalAttack;
        }
        else // High 등급 (15%)
        {
            return AttackType.SpecialAttack;
        }
    }



}
