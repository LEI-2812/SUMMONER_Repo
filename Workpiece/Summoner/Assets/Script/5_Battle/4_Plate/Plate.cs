using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 역할: 플레이어와 적 소환수가 올라가는 칸이며 소환수 배치 상태와 Unity 입력 진입점을 관리한다.
public class Plate : MonoBehaviour,
    IPointerEnterHandler, //플레이트에 마우스 올렸을때 이벤트 인터페이스
    IPointerExitHandler,  //플레이트에 마우스가 벗어낫을때 이벤트 인터페이스
    IPointerClickHandler, //플레이트 클릭시 상태창
    UpdateStateObserver
{
    //plate를 프리팹시켜서 넣을것.
    private bool isInSummon = false; // 현재 소환수가 있는지 여부
    [SerializeField] private Summon currentSummon;   // 플레이트 위에 있는 소환수
    [SerializeField] private GameObject statePanel;  // 상태 패널 (On/Off)
    [SerializeField] private SummonStatePanelView statePanelScript; // 상태 패널에 소환수 정보를 업데이트하는 스크립트
    [SerializeField] private Image summonImg;
    [SerializeField] private Transform spawnTransform;

    private Image plateImage; // 자기 자신의 Image 컴포넌트
    private PlateVisualView visualView;
    private PlateInputActions inputActions;

    [Header("컨트롤러들")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlateController plateController;

    [Header("효과음")]
    [SerializeField] private AudioSource clickSound;

    private readonly PlateSummonExecutor summonExecutor = new PlateSummonExecutor();
    private List<stateObserver> observers = new List<stateObserver>();

    void Start()
    {
        statePanel.SetActive(false);
        EnsurePlateController();
        plateImage = GetComponent<Image>(); // 자신의 Image 컴포넌트 가져오기
        visualView = new PlateVisualView(plateImage, summonImg);
        inputActions = new PlateInputActions(
            this,
            summonController,
            battleController,
            plateController,
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
        Debug.Log($"소환수 {summonClone.GetSummonName()} 을 {(isResummon ? "재소환" : "소환")}했습니다.");
    }

    public void RemoveSummon()
    {
        SetCurrentSummon(null);
        Debug.Log("소환수 제거.");
    }

    public void DirectMoveSummon(Summon summon)
    {
        if (summon == null) return;

        SetCurrentSummon(summon);
        summonExecutor.MoveSummonToPlate(currentSummon, transform);

        Debug.Log($"소환수 {summon.GetSummonName()} 이(가) 새 위치로 이동했습니다.");
    }

    private bool CanPlaceSummon(bool isResummon)
    {
        if (!isInSummon || isResummon)
        {
            return true;
        }

        Debug.Log("이미 이 플레이트에 소환수가 있습니다.");
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

        summonExecutor.DestroySummonInstance(currentSummon, statePanelScript);
        SetCurrentSummon(null);
        Debug.Log("기존 소환수가 파괴되었습니다.");
    }

    private Summon CreateSummonInstance(Summon summon)
    {
        return summonExecutor.CreateSummonInstance(summon, spawnTransform, transform);
    }

    private void ConnectCurrentSummonStatePanel()
    {
        if (statePanelScript == null)
        {
            Debug.LogError("statePanelScript가 null임");
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
            int playerPlateIndex = plateController == null ? -1 : plateController.GetPlayerPlateIndex(this);
            if (battleController != null)
            {
                battleController.SelectPlayerAttackSource(currentSummon, playerPlateIndex);
            }

            statePanelScript.SetStatePanel(currentSummon, false);
        }
    }


    // 플레이트 강조 (색상 변경)
    public void Highlight()
    {
        visualView.ShowHighlight();
    }

    // 강조 해제
    public void Unhighlight()
    {
        visualView.HideHighlight();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        inputActions.ShowPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inputActions.ShowPointerExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inputActions.TryHandleClick();
    }

    // 소환수 이미지 투명도 설정
    public void SetSummonImageTransparency(float alpha)
    {
        visualView.SetSummonImageTransparency(alpha);
    }

    private void EnsurePlateController()
    {
        if (plateController == null)
        {
            plateController = FindObjectOfType<PlateController>();
        }

        if (plateController == null)
        {
            Debug.LogError("Plate needs PlateController.");
        }
    }


    public Summon GetCurrentSummon()
    {
        return currentSummon;
    }

    public void SetCurrentSummon(Summon currentSummon)
    {
        this.currentSummon = currentSummon;
        isInSummon = currentSummon != null;
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
        //Debug.Log("호출");
        foreach (var observer in observers)
        {
            observer.StateUpdate();
        }
    }
}
