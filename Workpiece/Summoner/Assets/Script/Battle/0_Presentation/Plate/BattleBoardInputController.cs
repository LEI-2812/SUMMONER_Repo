using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 역할: BattleBoardInputController의 책임을 정의한다.
public class BattleBoardInputController : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    UpdateStateObserver
{
    private bool isInSummon = false;
    [SerializeField] private Summon currentSummon;
    [SerializeField] private GameObject statePanel;
    [SerializeField] private SummonStatePanelView statePanelScript;
    [SerializeField] private Image summonImg;
    [SerializeField] private Transform spawnTransform;

    private Image plateImage;
    private PlateVisualView visualView;
    private SelectBoardTargetUseCase selectBoardTargetUseCase;
    private AttackStateMachine attackStateMachine;

    [Header("전투 연결")]
    [UnityEngine.Serialization.FormerlySerializedAs("summonController")]
    [SerializeField] private SummonSelectionController summonSelectionController;
    [UnityEngine.Serialization.FormerlySerializedAs("battleController")]
    [UnityEngine.Serialization.FormerlySerializedAs("attackStateMachineProvider")]
    [SerializeField] private AttackStateMachineHost attackStateMachineHost;
    [UnityEngine.Serialization.FormerlySerializedAs("plateController")]
    [SerializeField] private PlateBoardView plateBoardController;

    [Header("사운드")]
    [SerializeField] private AudioSource clickSound;

    private List<stateObserver> observers = new List<stateObserver>();

    void Start()
    {
        statePanel.SetActive(false);
        EnsurePlateBoardView();
        EnsureAttackStateMachine();
        plateImage = GetComponent<Image>();
        visualView = new PlateVisualView(plateImage, summonImg);
        selectBoardTargetUseCase = new SelectBoardTargetUseCase(
            this,
            summonSelectionController,
            attackStateMachine,
            plateBoardController,
            statePanel,
            statePanelScript,
            clickSound);
    }

    public void SummonPlaceOnPlate(Summon summon, bool isResummon = false)
    {
        if (!CanPlaceSummon(isResummon))
        {
            return;
        }

        bool previousAttackState = GetPreviousAttackState();
        RemoveCurrentSummonForRedraw(isResummon);

        Summon summonClone = CreateSummonInstance(summon);
        SetCurrentSummon(summonClone);
        ConnectCurrentSummonStatePanel();
        summonClone.SummonInitialize();

        RestoreRedrawSummonState(isResummon, previousAttackState);
    }

    public void RemoveSummon()
    {
        SetCurrentSummon(null);
        Debug.Log("플레이트에서 소환수를 제거했습니다.");
    }

    public void DirectMoveSummon(Summon summon)
    {
        if (summon == null) return;

        SetCurrentSummon(summon);
        MoveSummonToPlate(currentSummon);

        Debug.Log($"{summon.GetSummonName()}을 플레이트로 이동했습니다.");
    }

    private bool CanPlaceSummon(bool isResummon)
    {
        if (!isInSummon || isResummon)
        {
            return true;
        }

        Debug.Log("이미 소환수가 있는 플레이트입니다.");
        return false;
    }

    private bool GetPreviousAttackState()
    {
        if (currentSummon == null)
        {
            return true;
        }

        return currentSummon.GetIsAttack();
    }

    private void RemoveCurrentSummonForRedraw(bool isResummon)
    {
        if (!isResummon || currentSummon == null)
        {
            return;
        }

        DestroySummonInstance(currentSummon, statePanelScript);
        SetCurrentSummon(null);
        Debug.Log("다시 뽑기로 기존 소환수를 제거했습니다.");
    }

    private Summon CreateSummonInstance(Summon summon)
    {
        Summon summonClone = Object.Instantiate(
            summon,
            spawnTransform.localPosition,
            spawnTransform.rotation);
        MoveSummonToPlate(summonClone);
        return summonClone;
    }


    private void DestroySummonInstance(Summon summon, stateObserver statePanelObserver)
    {
        if (summon == null)
        {
            return;
        }

        summon.RemoveObserver(statePanelObserver);
        Object.Destroy(summon.gameObject);
    }

    private void MoveSummonToPlate(Summon summon)
    {
        if (summon == null)
        {
            return;
        }

        summon.transform.SetParent(transform, false);
        summon.transform.localPosition = Vector3.zero;
    }
    private void ConnectCurrentSummonStatePanel()
    {
        if (statePanelScript == null)
        {
            Debug.LogError("BattleBoardInputController에 SummonStatePanelView가 필요합니다.");
            return;
        }

        currentSummon.AddObserver(statePanelScript);
        NotifyObservers();
    }

    private void RestoreRedrawSummonState(bool isResummon, bool previousAttackState)
    {
        if (!isResummon)
        {
            return;
        }

        currentSummon.SetIsAttack(previousAttackState);
        if (statePanelScript != null)
        {
            int playerPlateIndex = plateBoardController == null ? -1 : plateBoardController.GetPlayerPlateIndex(this);
            if (attackStateMachine != null)
            {
                attackStateMachine.SelectPlayerAttackSource(currentSummon, playerPlateIndex);
            }

            statePanelScript.SetStatePanel(currentSummon, false);
        }
    }


    public void Highlight()
    {
        visualView.ShowHighlight();
    }

    public void Unhighlight()
    {
        visualView.HideHighlight();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        selectBoardTargetUseCase.ExecutePointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectBoardTargetUseCase.ExecutePointerExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectBoardTargetUseCase.Execute();
    }

    public void SetSummonImageTransparency(float alpha)
    {
        visualView.SetSummonImageTransparency(alpha);
    }

    private void EnsurePlateBoardView()
    {
        if (plateBoardController == null)
        {
            plateBoardController = FindObjectOfType<PlateBoardView>();
        }

        if (plateBoardController == null)
        {
            Debug.LogError("BattleBoardInputController에 PlateBoardView가 필요합니다.");
        }
    }

    private void EnsureAttackStateMachine()
    {
        if (attackStateMachine != null)
        {
            return;
        }

        if (attackStateMachineHost == null)
        {
            return;
        }

        attackStateMachine = attackStateMachineHost.GetAttackStateMachine();
    }


    public Summon GetCurrentSummon()
    {
        return currentSummon;
    }

    public void SetCurrentSummon(Summon currentSummon)
    {
        if (this.currentSummon != null && this.currentSummon != currentSummon)
        {
            this.currentSummon.ClearDeathHandler();
        }

        this.currentSummon = currentSummon;
        isInSummon = currentSummon != null;

        if (this.currentSummon != null)
        {
            this.currentSummon.SetDeathHandler(RemoveDefeatedSummon);
        }
    }

    private void RemoveDefeatedSummon(Summon defeatedSummon)
    {
        if (currentSummon != defeatedSummon)
        {
            return;
        }

        RemoveSummon();
    }

    public bool GetIsInSummon()
    {
        return isInSummon;
    }

    public void AddObserver(stateObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(stateObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.StateUpdate();
        }
    }
}
