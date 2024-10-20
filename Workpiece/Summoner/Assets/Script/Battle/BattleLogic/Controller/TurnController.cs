using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    
    [SerializeField]private Player player;
    [SerializeField] private Enermy enermy;
    public enum Turn { PlayerTurn, EnermyTurn }
    private Turn currentTurn; // 현재 턴을 나타내는 변수
    private int turnCount;

    [SerializeField] private TextMeshProUGUI turnCountText;
                            
    void Start()
    {
        currentTurn = Turn.PlayerTurn; // 첫 번째 턴은 플레이어 턴으로 시작
        turnCount = 1;
        UpdateTurnCountUI();
        StartTurn();
    }

    public void StartTurn() //해당 플레이어의 턴 시작
    {
        if (currentTurn == Turn.PlayerTurn)  // 플레이어 턴일 경우
        {
            player.startTurn();

            foreach (var summon in enermy.getEnermyAttackController().getPlateController().getEnermySummons()) //플레이어 턴 시작시 적 플레이트의 상태이상 데미지 적용
            {
                summon.UpdateStatusEffectsAndCooldowns(); // 상태이상 업데이트
                summon.getAttackStrategy().ReduceCooldown(); // 일반 공격 쿨타임 감소
            }
        }
        else if (currentTurn == Turn.EnermyTurn)  // 적의 턴일 경우
        {
            enermy.startTurn();

            // 적 턴 종료 시, 적 소환수의 상태이상 및 쿨타임 업데이트
            foreach (var summon in player.getPlateController().getPlayerSummons())
            {
                summon.UpdateStatusEffectsAndCooldowns(); // 상태이상 업데이트
                summon.getAttackStrategy().ReduceCooldown(); // 일반 공격 쿨타임 감소
            }
        }
    }

    public void EndTurn()
    {
        if (currentTurn == Turn.PlayerTurn)
        {
            // 다음 턴을 적의 턴으로 설정
            currentTurn = Turn.EnermyTurn;

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
            Debug.Log("현재 턴: " + turnCount);

            StartTurn();
        }
    }
    private void UpdateTurnCountUI()
    {
        turnCountText.text = $"Current Turn : {turnCount}";
    }

    public Turn getCurrentTurn()
    {
        return currentTurn;
    }
} 
