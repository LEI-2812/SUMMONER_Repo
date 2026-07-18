using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 역할: BattleBoardInputController의 책임을 정의한다.
public class BattleBoardInputController : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
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
    private PlateData plateData;

    [Header("전투 연결")]
    [SerializeField] private SummonSelectionController summonSelectionController;
    [SerializeField] private PlateBoardView plateBoardController;

    [Header("사운드")]
    [SerializeField] private AudioSource clickSound;

    void Start()
    {
        statePanel.SetActive(false);
        EnsurePlateBoardView();
        plateImage = GetComponent<Image>();
        visualView = new PlateVisualView(plateImage, summonImg);
        if (selectBoardTargetUseCase == null)
        {
            selectBoardTargetUseCase = new SelectBoardTargetUseCase(attackStateMachine);
        }
    }

    public void ConnectAttackState(AttackStateMachine attackStateMachine)
    {
        this.attackStateMachine = attackStateMachine;
        selectBoardTargetUseCase = new SelectBoardTargetUseCase(attackStateMachine);
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


    private void DestroySummonInstance(Summon summon, SummonStatePanelView statePanelView)
    {
        if (summon == null)
        {
            return;
        }

        if (statePanelView != null)
        {
            summon.RemoveStateChangedHandler(statePanelView.StateUpdate);
        }

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

        currentSummon.AddStateChangedHandler(statePanelScript.StateUpdate);
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
        BoardTargetInput input = CreateBoardTargetInput();
        if (input.CurrentSummon == null)
        {
            return;
        }

        bool showHighlight = input.IsSummonSelectionActive
            || (input.IsAttackTargetSelectionActive && input.CanSelectAttackTarget);
        if (showHighlight)
        {
            Highlight();
        }

        float transparency = input.IsAttackTargetSelectionActive && !input.CanSelectAttackTarget
            ? 0.5f
            : 1.0f;
        SetSummonImageTransparency(transparency);
    }

    public void ConnectPlateData(PlateData connectedPlateData)
    {
        plateData = connectedPlateData;
        plateData?.SetCurrentSummon(currentSummon);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BoardTargetInput input = CreateBoardTargetInput();
        if (input.CurrentSummon == null)
        {
            return;
        }

        if (input.IsSummonSelectionActive || input.IsAttackTargetSelectionActive)
        {
            Unhighlight();
        }

        if (input.IsSummonSelectionActive)
        {
            SetSummonImageTransparency(0.5f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BoardTargetInput input = CreateBoardTargetInput();
        ApplyClickAction(selectBoardTargetUseCase.Execute(input), input);
        PlayClickSound();
    }

    private BoardTargetInput CreateBoardTargetInput()
    {
        bool isSummonSelectionActive = summonSelectionController != null
            && summonSelectionController.IsSummoning();
        bool isAttackTargetSelectionActive = attackStateMachine != null
            && attackStateMachine.IsSpecialAttackTargetSelectionActive();
        int attackTargetPlateIndex = -1;
        string attackTargetPlateName = "none";
        bool canSelectAttackTarget = false;

        if (isAttackTargetSelectionActive && plateBoardController != null)
        {
            bool targetsPlayerPlate = attackStateMachine.DoesCurrentSpecialAttackTargetPlayerPlate();
            canSelectAttackTarget = plateBoardController.TryGetAttackTargetPlate(
                this,
                targetsPlayerPlate,
                out attackTargetPlateIndex,
                out attackTargetPlateName);
        }

        int playerPlateIndex = plateBoardController == null
            ? -1
            : plateBoardController.GetPlayerPlateIndex(this);
        bool isEnemyPlate = plateBoardController != null
            && plateBoardController.GetEnemyPlateIndex(this) >= 0;

        return new BoardTargetInput(
            currentSummon,
            isSummonSelectionActive,
            isAttackTargetSelectionActive,
            canSelectAttackTarget,
            attackTargetPlateIndex,
            attackTargetPlateName,
            playerPlateIndex,
            isEnemyPlate,
            statePanel != null && statePanelScript != null);
    }

    private void ApplyClickAction(BoardClickAction action, BoardTargetInput input)
    {
        switch (action)
        {
            case BoardClickAction.SelectRedrawPlate:
                summonSelectionController.SelectPlate(this);
                Unhighlight();
                SetSummonImageTransparency(1.0f);
                break;

            case BoardClickAction.SelectAttackTarget:
                Debug.Log($"{input.AttackTargetPlateName} 플레이트 {input.AttackTargetPlateIndex}번을 선택했습니다.");
                Unhighlight();
                break;

            case BoardClickAction.InvalidAttackTarget:
                Debug.Log("유효하지 않은 공격 대상 플레이트입니다.");
                break;

            case BoardClickAction.ShowSummonStatus:
                Debug.Log("소환수 상태 패널 열기: " + input.CurrentSummon.GetSummonName());
                statePanel.SetActive(true);
                statePanelScript.SetStatePanel(input.CurrentSummon, input.IsEnemyPlate);
                break;
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && clickSound.isActiveAndEnabled && clickSound.gameObject.activeInHierarchy)
        {
            clickSound.Play();
        }
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
        plateData?.SetCurrentSummon(currentSummon);

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

}
