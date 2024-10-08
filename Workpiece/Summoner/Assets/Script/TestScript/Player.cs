using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : Character
{
    [Header("마나UI")]
    [SerializeField] private List<RawImage> manaList;
    [Header("마나텍스쳐")]
    [SerializeField] private Texture notHaveTexture;
    [SerializeField] private Texture haveTexture;
    private int mana;
    private int usedMana;

    [Header("컨트롤러")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private TurnController turnController;
    
    private bool hasSummonedThisTurn;

    private void Start()
    {
        ResetPlayerSetting();
    }

    public override void startTurn()
    {
        base.startTurn();
        Debug.Log($"{gameObject.name} 의 마나: {mana}");
        hasSummonedThisTurn = false;
    }

    public void OnSummonBtnClick()
    {
        if (hasSummonedThisTurn)
        {
            Debug.Log("이 턴에서는 이미 소환을 했습니다. 다음 턴에 소환할 수 있습니다.");
            return;
        }

        if (mana > 0)
        {
            for (int i = 0; i < summonController.getPlayerPlate().Count; i++)
            {
                if (!summonController.getPlayerPlate()[i].isInSummon)
                {
                    Debug.Log(i + "번째 플레이트에 소환 예정");
                    summonController.StartSummon(i, false);
                    mana -= 1;
                    hasSummonedThisTurn = true;
                    UpdateManaUI();
                    return;
                }
            }
            Debug.Log("모든 플레이트에 소환수가 있습니다.");
        }
        else
        {
            takeAction();
            Debug.Log("마나가 부족하여 소환 불가능");
        }
    }

    //플레이어는 버튼 클릭을 통해서만 턴종료를 시킨다.
    public void PlayerTurnOverBtn() //버튼에 넣을 메소드
    {
        // 플레이어 턴일 때만 턴 종료 가능
        if (turnController.currentTurn == TurnController.Turn.PlayerTurn)
        {
            Debug.Log("플레이어 턴 종료");
            turnController.EndTurn();
        }
        else
        {
            Debug.Log("플레이어 턴이 아닙니다.");
        }
    }

    public void OnReSummonBtnClick() //재소환 버튼 클릭
    {
        if (mana >= usedMana) {
            if (summonController.StartResummon())
            { //재소환 시작
              //마나 차감
                mana -= usedMana;
                usedMana += 1;
                UpdateManaUI();
            }
        }
        else
        {
            Debug.Log("재소환시 필요한 마나가 모자랍니다.");
        }
    }

    public void SetHasSummonedThisTurn(bool value) //이번턴에 소환했는지 여부
    {
        hasSummonedThisTurn = value;
    }

    public bool HasSummonedThisTurn()
    {
        return hasSummonedThisTurn;
    }

    private void ResetPlayerSetting()
    {
        mana = 10;
        usedMana = 1;
        UpdateManaUI();
    }

    public void UpdateManaUI()
    {
        for (int i = 0; i < manaList.Count; i++)
        {
            manaList[i].texture = (i < mana) ? haveTexture : notHaveTexture;
        }
    }
}
