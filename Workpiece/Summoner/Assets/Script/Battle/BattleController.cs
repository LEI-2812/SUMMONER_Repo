using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("아군 및 적 플레이트")]
    [SerializeField] private List<Plate> playerPlates; //플레이어 플레이트
    [SerializeField] private List<Plate> enermyPlates; //적 플레이트

    [SerializeField] private StatePanel statePanel;

    private bool isAttacking = false; //공격중인이 판별

    public Summon attackStart()
    {
        isAttacking = true; //공격시작
        return statePanel.getStatePanelSummon(); //상태창에 있는 소환수를 반환
    }




    public bool getIsAttaking()
    {
        return isAttacking;
    }
    public void setIsAttaking(bool isAttacking)
    {
        this.isAttacking= isAttacking;
    }


}
