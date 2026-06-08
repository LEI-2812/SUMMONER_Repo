using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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
    private Color originalColor;

    [Header("컨트롤러들")]
    [SerializeField] private SummonController summonController;
    [SerializeField] private BattleController battleController;

    [Header("효과음")]
    [SerializeField] private AudioSource clickSound;

    private List<stateObserver> observers = new List<stateObserver>();

    void Start()
    {
        statePanel.SetActive(false);
        plateImage = GetComponent<Image>(); // 자신의 Image 컴포넌트 가져오기
        originalColor = plateImage.color; // 원래 색상 저장
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
        currentSummon.transform.SetParent(this.transform, false);
        currentSummon.transform.localPosition = Vector3.zero;

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

        currentSummon.RemoveObserver(statePanelScript);
        Destroy(currentSummon.gameObject);
        SetCurrentSummon(null);
        Debug.Log("기존 소환수가 파괴되었습니다.");
    }

    private Summon CreateSummonInstance(Summon summon)
    {
        Summon summonClone = Instantiate(summon, spawnTransform.localPosition, spawnTransform.rotation);
        summonClone.transform.SetParent(transform, false);
        return summonClone;
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
            statePanelScript.SetStatePanel(currentSummon, false);
        }
    }


    // 플레이트 강조 (색상 변경)
    public void Highlight()
    {
        plateImage.color = Color.yellow; // 이미지의 색상을 노란색으로 변경
    }

    // 강조 해제
    public void Unhighlight()
    {
        plateImage.color = originalColor; // 이미지의 색상을 원래대로 복원
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowSummonHoverEnter();
        ShowSummonSelectionHoverEnter();
        ShowAttackTargetHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowSummonSelectionHoverExit();
        ShowAttackTargetHoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TrySelectRedrawPlate())
        {
            PlayClickSound();
            return;
        }

        if (TrySelectAttackTargetPlate())
        {
            PlayClickSound();
            return;
        }

        TryOpenSummonStatePanel();
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            clickSound.Play();
        }
    }

    private void ShowSummonHoverEnter()
    {
        if (currentSummon == null)
        {
            return;
        }

        SetSummonImageTransparency(1.0f);
    }

    private void ShowSummonSelectionHoverEnter()
    {
        if (!isInSummon || !IsSummonSelectionActive())
        {
            return;
        }

        Highlight();
        SetSummonImageTransparency(1.0f);
    }

    private void ShowAttackTargetHoverEnter()
    {
        if (!BattleAttackActiveOnPlate())
        {
            return;
        }

        if (TryGetAttackTargetPlate(out _, out _))
        {
            Highlight();
            return;
        }

        SetSummonImageTransparency(0.5f);
    }

    private void ShowSummonSelectionHoverExit()
    {
        if (currentSummon == null || !IsSummonSelectionActive())
        {
            return;
        }

        Unhighlight();
        SetSummonImageTransparency(0.5f);
    }

    private void ShowAttackTargetHoverExit()
    {
        if (!BattleAttackActiveOnPlate())
        {
            return;
        }

        Unhighlight();
    }

    private bool TrySelectRedrawPlate()
    {
        if (!IsSummonSelectionActive() || !isInSummon)
        {
            return false;
        }

        summonController.SelectPlate(this);
        Unhighlight();
        SetSummonImageTransparency(1.0f);
        return true;
    }

    private void TryOpenSummonStatePanel()
    {
        if (currentSummon == null || IsSummonSelectionActive() || IsBattleAttacking())
        {
            return;
        }

        Debug.Log("클릭된 플레이트의 소환수:" + currentSummon.GetSummonName());
        statePanel.SetActive(true);

        int plateIndex = GetPlayerPlateIndex();
        summonController.SetPlayerSelectedIndex(plateIndex);
        statePanelScript.SetStatePanel(currentSummon, IsCurrentEnermyPlate());
    }

    private bool TrySelectAttackTargetPlate()
    {
        if (!BattleAttackActiveOnPlate())
        {
            return false;
        }

        if (!TryGetAttackTargetPlate(out int plateIndex, out string plateName))
        {
            Debug.Log("유효한 플레이트가 선택되지 않았습니다.");
            return false;
        }

        summonController.SetPlayerSelectedIndex(plateIndex);
        Debug.Log($"{plateName}의 플레이트 {plateIndex}가 선택되었습니다.");
        Unhighlight();
        return true;
    }

    private bool TryGetAttackTargetPlate(out int plateIndex, out string plateName)
    {
        plateIndex = -1;
        plateName = "알 수 없음";

        PlateController plateController = GetPlateController();
        if (plateController == null)
        {
            return false;
        }

        bool targetsPlayerPlate = AttackingSummonTargetsPlayerPlate();
        return plateController.TryGetAttackTargetPlate(this, targetsPlayerPlate, out plateIndex, out plateName);
    }

    private bool BattleAttackActiveOnPlate()
    {
        return isInSummon && IsBattleAttacking();
    }

    private bool AttackingSummonTargetsPlayerPlate()
    {
        return battleController != null && battleController.DoesCurrentSpecialAttackTargetPlayerPlate();
    }

    private bool IsSummonSelectionActive()
    {
        return summonController != null && summonController.IsSummoning();
    }

    private bool IsBattleAttacking()
    {
        return battleController != null && battleController.GetIsAttacking();
    }

    private int GetPlayerPlateIndex()
    {
        PlateController plateController = GetPlateController();
        return plateController == null ? -1 : plateController.GetPlayerPlateIndex(this);
    }

    // 소환수 이미지 투명도 설정
    public void SetSummonImageTransparency(float alpha)
    {
        if (summonImg != null)
        {
            Color color = summonImg.color;
            color.a = alpha; // 투명도 설정
            summonImg.color = color;
        }
    }

    // 현재 플레이트가 적의 플레이트인지 검사하는 메소드
    private bool IsCurrentEnermyPlate()
    {
        PlateController plateController = GetPlateController();
        return plateController != null && plateController.ContainsEnermyPlate(this);
    }

    private PlateController GetPlateController()
    {
        return battleController == null ? null : battleController.GetPlateController();
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
