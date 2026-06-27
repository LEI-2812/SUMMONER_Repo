using UnityEngine;

// 역할: BattleRuntimeData의 책임을 정의한다.
public class BattleRuntimeData : MonoBehaviour
{
    [SerializeField] private int defaultStage = 1;

    private BattleStageData stageData;
    private BattleAttackData attackData;

    public BattleStageData StageData => GetStageData();
    public BattleAttackData AttackData => GetAttackData();
    public int CurrentStage => StageData.CurrentStage;
    public bool IsSpecialAttackTargetSelectionActive => AttackData.IsSpecialAttackTargetSelectionActive;
    public Summon AttackingSummon => AttackData.AttackingSummon;
    public SpecialAttackInfo CurrentSpecialAttackInfo => AttackData.CurrentSpecialAttackInfo;
    public int AttackingPlateIndex => AttackData.AttackingPlateIndex;
    public int SelectedSpecialAttackTargetPlateIndex => AttackData.SelectedSpecialAttackTargetPlateIndex;

    private void Awake()
    {
        StageData.SetStage(defaultStage);
    }

    public void SelectPlayerAttackSource(Summon summon, int plateIndex)
    {
        AttackData.SelectPlayerAttackSource(summon, plateIndex);
    }

    public void ClearPlayerAttackSource()
    {
        AttackData.ClearPlayerAttackSource();
    }

    public void StartAttackFromSelectedSource()
    {
        AttackData.StartAttackFromSelectedSource();
    }

    public void SetCurrentSpecialAttackInfo(SpecialAttackInfo specialAttackInfo)
    {
        AttackData.SetCurrentSpecialAttackInfo(specialAttackInfo);
    }

    public void StartSpecialAttackTargetSelection()
    {
        AttackData.StartSpecialAttackTargetSelection();
    }

    public void CancelSpecialAttackTargetSelection()
    {
        AttackData.CancelSpecialAttackTargetSelection();
    }

    public void SelectSpecialAttackTargetPlate(int plateIndex)
    {
        AttackData.SelectSpecialAttackTargetPlate(plateIndex);
    }

    public void ClearSpecialAttackTargetSelection()
    {
        AttackData.ClearSpecialAttackTargetSelection();
    }

    public bool HasCurrentSpecialAttackInfo()
    {
        return AttackData.HasCurrentSpecialAttackInfo();
    }

    public int GetCurrentSpecialAttackInfoIndex()
    {
        return AttackData.GetCurrentSpecialAttackInfoIndex();
    }

    public bool DoesCurrentSpecialAttackTargetPlayerPlate()
    {
        return AttackData.DoesCurrentSpecialAttackTargetPlayerPlate();
    }

    public void ResetAttackSelection()
    {
        AttackData.ResetAttackSelection();
    }

    private BattleStageData GetStageData()
    {
        if (stageData == null)
        {
            stageData = new BattleStageData(defaultStage);
        }

        return stageData;
    }

    private BattleAttackData GetAttackData()
    {
        if (attackData == null)
        {
            attackData = new BattleAttackData();
        }

        return attackData;
    }
}
