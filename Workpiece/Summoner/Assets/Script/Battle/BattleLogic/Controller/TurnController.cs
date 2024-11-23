using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    
    [SerializeField] private Player player;
    [SerializeField] private Enermy enermy;
    public enum Turn { PlayerTurn, EnermyTurn }
    private Turn currentTurn; // 현재 턴을 나타내는 변수
    private int turnCount;
    [SerializeField] private int clearTurn;

    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI turnClearText;

    StageController stageController;

    void Start()
    {
        stageController = FindAnyObjectByType<StageController>();

        currentTurn = Turn.PlayerTurn; // 첫 번째 턴은 플레이어 턴으로 시작
        turnCount = 1;
        UpdateTurnCountUI();
        SetClearCountUI();
        StartTurn();
    }

    public void StartTurn() //해당 플레이어의 턴 시작
    {
        if (currentTurn == Turn.PlayerTurn)  // 플레이어 턴일 경우
        {
            // 적 소환수 상태 업데이트 및 데미지 처리
            var enermyPlates = enermy.getEnermyAttackController().getPlateController().getEnermySummons();

            // 상태 업데이트를 먼저 진행
            foreach (var summon in enermyPlates)
            {
                summon.UpdateDamageStatusEffects(); // 데미지를 주는 상태이상 업데이트
                summon.UpdateStunAndCurseStatus();  // 스턴 및 저주 상태 업데이트
                summon.getAttackStrategy().ReduceCooldown(); // 일반 공격 쿨타임 감소
            }

            player.getPlateController().CompactEnermyPlates(); //앞당기기

            // 승리 조건
            if (enermy.getEnermyAttackController().getPlateController().IsEnermyPlateClear() && (player.clearTurn >= player.currentTurn))
            {
                Debug.Log("승리!");
                player.battleAlert.clearAlert(stageController.stageNum);
            }


            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateSpecialAttackCooldowns(); // 특수 공격 쿨타임 업데이트
            }

            player.startTurn();
        }
        else if (currentTurn == Turn.EnermyTurn)  // 적의 턴일 경우
        {
            // 적 턴 시작시 아군 소환수 상태이상 및 쿨타임 업데이트
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateDamageStatusEffects(); // 데미지를 주는 상태이상 업데이트
                summon.UpdateStunAndCurseStatus(); // 스턴 및 저주 상태 업데이트
                summon.getAttackStrategy().ReduceCooldown(); // 일반 공격 쿨타임 감소
            }

            foreach (var summon in enermy.getEnermyAttackController().getPlateController().getEnermySummons())
            {
                summon.UpdateSpecialAttackCooldowns(); // 특수 공격 쿨타임 업데이트
            }

            enermy.startTurn();
        }
    }

    public void EndTurn()
    {
        if (currentTurn == Turn.PlayerTurn)
        {
            // 다음 턴을 적의 턴으로 설정
            currentTurn = Turn.EnermyTurn;

            // 적 턴 시작시 아군 소환수 상태이상 및 쿨타임 업데이트
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateUpgradeStatus(); //강화 상태 업데이트
            }

            // 플레이어 턴이 끝나면 턴 카운트를 증가시키지 않고 바로 적 턴 시작
            StartTurn();
        }
        else if (currentTurn == Turn.EnermyTurn)
        {

            // 적 턴이 끝난 후 턴 카운트를 증가시키고 플레이어 턴 시작
            currentTurn = Turn.PlayerTurn;
            turnCount++; // 적 턴이 끝나면 턴 카운트를 증가시킴
            UpdateTurnCountUI();
            player.AddMana();

            //적 턴 끝날때 적 플레이트의 강화를 품
            foreach (var summon in enermy.getEnermyAttackController().getPlateController().getEnermySummons())
            {
                summon.UpdateUpgradeStatus(); //강화 상태 업데이트
            }

            // 플레이어 턴이 끝날 때 혼란 상태가 아닌 소환수들의 공격 가능 여부를 설정
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                if (!summon.IsStun()) // 스턴 상태가 아닌 경우
                {
                    summon.setIsAttack(true); // 공격 가능하게 설정
                }
            }

            Debug.Log("현재 턴: " + turnCount);

            StartTurn();
        }
    }
    private void UpdateTurnCountUI()
    {
        turnCountText.text = $"Current Turn : {turnCount}";
    }

    private void SetClearCountUI()
    {
        turnClearText.text = $"Clear Turn : {clearTurn}";
    }

    public Turn getCurrentTurn()
    {
        return currentTurn;
    }

    public int GetTurnCount()
    {
        return turnCount;
    }

    public int GetClearTurn()
    {
        return clearTurn;
    }
} 
