using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonStatePanelView : MonoBehaviour
{
    private const float SpecialAttackButtonSpacing = 70f;

    [SerializeField] private Summon stateSummon;
    [SerializeField] private Image summonImage;
    [SerializeField] private Slider HPSlider;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image statePanel;

    [Header("공격 버튼")]
    [SerializeField] private Button NormalAttackButton;
    [SerializeField] private Button SpecialAttackButton;

    private readonly List<Button> createdSpecialAttackButtons = new List<Button>();
    private Vector2 originSpecialAttackButtonPosition;
    private bool hasOriginSpecialAttackButtonPosition;
    private Action<int> specialAttackRequested;

    public void SetSpecialAttackRequestedHandler(Action<int> handler)
    {
        specialAttackRequested = handler;
    }

    public void SetStatePanel(Summon stateSummon, bool isEnemyPlate)
    {
        statePanel.gameObject.SetActive(true);
        this.stateSummon = stateSummon;
        stateSummon.AddStateChangedHandler(StateUpdate);

        if (summonImage != null && stateSummon != null && stateSummon.GetImage() != null)
        {
            summonImage.sprite = stateSummon.GetImage().sprite;
        }

        UpdateHealthSlider();

        if (isEnemyPlate)
        {
            NormalAttackButton.gameObject.SetActive(false);
            HideSpecialAttackButtons();
        }
        else
        {
            NormalAttackButton.gameObject.SetActive(true);
            NormalAttackButton.image.sprite = stateSummon.GetNormalAttackSprite();
            ShowSpecialAttackButtons(stateSummon);
        }

        StateUpdate();
    }

    public void StateUpdate()
    {
        if (shieldImage != null)
        {
            Color color = shieldImage.color;
            color.a = 0.7f;
            shieldImage.color = color;
        }

        if (stateSummon == null)
        {
            return;
        }

        if (stateSummon.GetNowHP() <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdateHealthSlider();
        UpdateShieldView();
    }

    private void UpdateHealthSlider()
    {
        if (HPSlider == null || stateSummon == null)
        {
            return;
        }

        float currentHP = (float)stateSummon.GetNowHP();
        float maxHP = (float)stateSummon.GetMaxHP();
        HPSlider.value = Mathf.Clamp(currentHP / maxHP, 0f, 1f);
    }

    private void UpdateShieldView()
    {
        if (shieldImage == null)
        {
            return;
        }

        float shieldAmount = (float)stateSummon.GetShield();
        float initialShieldAmount = (float)stateSummon.GetInitialShield();
        if (shieldAmount <= 0 || initialShieldAmount <= 0)
        {
            shieldImage.gameObject.SetActive(false);
            return;
        }

        shieldImage.gameObject.SetActive(true);
        shieldImage.fillAmount = Mathf.Clamp01(shieldAmount / initialShieldAmount);
    }

    private void ShowSpecialAttackButtons(Summon summon)
    {
        ClearCreatedSpecialAttackButtons();

        if (SpecialAttackButton == null || summon == null || summon.GetSpecialAttackCount() == 0)
        {
            if (SpecialAttackButton != null)
            {
                SpecialAttackButton.gameObject.SetActive(false);
            }

            return;
        }

        SpecialAttackButton.gameObject.SetActive(true);
        SetSpecialAttackButtonDisplay(SpecialAttackButton, summon, 0);
        ArrangeSpecialAttackButton(SpecialAttackButton, 0, summon.GetSpecialAttackCount());

        for (int i = 1; i < summon.GetSpecialAttackCount(); i++)
        {
            Button button = CreateSpecialAttackButton(i, summon.GetSpecialAttackCount());
            createdSpecialAttackButtons.Add(button);
        }
    }

    private void HideSpecialAttackButtons()
    {
        ClearCreatedSpecialAttackButtons();
        RestoreOriginSpecialAttackButtonPosition();

        if (SpecialAttackButton != null)
        {
            SpecialAttackButton.gameObject.SetActive(false);
        }
    }

    private Button CreateSpecialAttackButton(int specialAttackIndex, int specialAttackCount)
    {
        GameObject buttonObject = new GameObject("SpecialAttackButton_" + specialAttackIndex);
        buttonObject.transform.SetParent(SpecialAttackButton.transform.parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        CopyRectTransform(SpecialAttackButton.GetComponent<RectTransform>(), rectTransform);
        buttonObject.AddComponent<CanvasRenderer>();

        Image image = buttonObject.AddComponent<Image>();
        CopyImage(SpecialAttackButton.image, image);

        Button button = buttonObject.AddComponent<Button>();
        CopyButtonStyle(SpecialAttackButton, button, image);
        SetSpecialAttackButtonDisplay(button, stateSummon, specialAttackIndex);

        int capturedIndex = specialAttackIndex;
        button.onClick.AddListener(() => ExecuteSpecialAttack(capturedIndex));

        ArrangeSpecialAttackButton(button, specialAttackIndex, specialAttackCount);
        return button;
    }

    private void SetSpecialAttackButtonDisplay(Button button, Summon summon, int specialAttackIndex)
    {
        if (button == null || summon == null)
        {
            return;
        }

        if (button.image != null)
        {
            button.image.sprite = summon.GetSpecialAttackSprite(specialAttackIndex);
        }

        button.gameObject.name = "SpecialAttackButton_" + specialAttackIndex + "_" + summon.GetSpecialAttackName(specialAttackIndex);
    }

    private void ExecuteSpecialAttack(int specialAttackIndex)
    {
        if (specialAttackRequested == null)
        {
            Debug.LogWarning("SummonStatePanelView에 특수 공격 요청 핸들러가 필요합니다.");
            return;
        }

        specialAttackRequested.Invoke(specialAttackIndex);
    }

    private void ClearCreatedSpecialAttackButtons()
    {
        foreach (Button button in createdSpecialAttackButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        createdSpecialAttackButtons.Clear();
        RestoreOriginSpecialAttackButtonPosition();
    }

    private void ArrangeSpecialAttackButton(Button button, int specialAttackIndex, int specialAttackCount)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return;
        }

        Vector2 originPosition = GetOriginSpecialAttackButtonPosition();
        float startOffset = -SpecialAttackButtonSpacing * (specialAttackCount - 1) * 0.5f;
        rectTransform.anchoredPosition = originPosition + new Vector2(startOffset + SpecialAttackButtonSpacing * specialAttackIndex, 0f);
    }

    private Vector2 GetOriginSpecialAttackButtonPosition()
    {
        if (!hasOriginSpecialAttackButtonPosition && SpecialAttackButton != null)
        {
            RectTransform rectTransform = SpecialAttackButton.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                originSpecialAttackButtonPosition = rectTransform.anchoredPosition;
                hasOriginSpecialAttackButtonPosition = true;
            }
        }

        return originSpecialAttackButtonPosition;
    }

    private void RestoreOriginSpecialAttackButtonPosition()
    {
        if (!hasOriginSpecialAttackButtonPosition || SpecialAttackButton == null)
        {
            return;
        }

        RectTransform rectTransform = SpecialAttackButton.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originSpecialAttackButtonPosition;
        }
    }

    private void CopyRectTransform(RectTransform source, RectTransform target)
    {
        if (source == null || target == null)
        {
            return;
        }

        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.pivot = source.pivot;
        target.sizeDelta = source.sizeDelta;
        target.localScale = source.localScale;
        target.localRotation = source.localRotation;
    }

    private void CopyImage(Image source, Image target)
    {
        if (source == null || target == null)
        {
            return;
        }

        target.sprite = source.sprite;
        target.color = source.color;
        target.material = source.material;
        target.type = source.type;
        target.preserveAspect = source.preserveAspect;
        target.raycastTarget = source.raycastTarget;
    }

    private void CopyButtonStyle(Button source, Button target, Image targetImage)
    {
        target.transition = source.transition;
        target.colors = source.colors;
        target.spriteState = source.spriteState;
        target.animationTriggers = source.animationTriggers;
        target.navigation = source.navigation;
        target.targetGraphic = targetImage;
        target.interactable = source.interactable;
    }

}
