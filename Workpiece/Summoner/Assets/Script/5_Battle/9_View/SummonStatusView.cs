using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
// 역할: 소환수 HP, 상태이상 아이콘, 피격 색상 같은 상태 표시를 담당한다.
public class SummonStatusView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float blinkInterval = 1f;

    private int currentEffectIndex;
    private int activeEffectCount;
    private float blinkTimer;
    private bool colorFeedbackPlaying;
    private Coroutine colorFeedbackCoroutine;

    private void Awake()
    {
        ImageEnsure();
    }

    public void StatusEffectsShow(IReadOnlyList<StatusEffect> activeStatusEffects)
    {
        StatusEffectsShow(activeStatusEffects, false);
    }

    public void StatusEffectsShow(IReadOnlyList<StatusEffect> activeStatusEffects, bool isPaused)
    {
        if (isPaused || colorFeedbackPlaying)
        {
            return;
        }

        ImageEnsure();

        if (activeStatusEffects == null || activeStatusEffects.Count == 0)
        {
            StatusColorReset();
            return;
        }

        if (activeStatusEffects.Count == 1)
        {
            activeEffectCount = 1;
            currentEffectIndex = 0;
            blinkTimer = 0f;
            SetColorByStatus(activeStatusEffects[0].statusType);
            return;
        }

        MultipleStatusColorBlink(activeStatusEffects);
    }

    private void ImageEnsure()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    private void StatusColorReset()
    {
        image.color = Color.white;
        blinkTimer = 0f;
        currentEffectIndex = 0;
        activeEffectCount = 0;
    }

    private void MultipleStatusColorBlink(IReadOnlyList<StatusEffect> activeStatusEffects)
    {
        if (activeEffectCount != activeStatusEffects.Count)
        {
            activeEffectCount = activeStatusEffects.Count;
            currentEffectIndex = 0;
            blinkTimer = 0f;
            SetColorByStatus(activeStatusEffects[currentEffectIndex].statusType);
            return;
        }

        blinkTimer += Time.deltaTime;

        if (blinkTimer < blinkInterval)
        {
            return;
        }

        blinkTimer = 0f;
        currentEffectIndex = (currentEffectIndex + 1) % activeStatusEffects.Count;
        SetColorByStatus(activeStatusEffects[currentEffectIndex].statusType);
    }

    private void SetColorByStatus(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.Burn:
                image.color = new Color(1f, 0.35f, 0.15f);
                break;
            case StatusType.Poison:
                image.color = new Color(0.3f, 1f, 0.3f);
                break;
            case StatusType.Stun:
                image.color = new Color(0.2f, 0.2f, 0.2f);
                break;
            case StatusType.Curse:
                image.color = new Color(0.64f, 0.19f, 0.84f);
                break;
            case StatusType.LifeDrain:
                image.color = new Color(1f, 1f, 0.5f);
                break;
            case StatusType.Shield:
                image.color = new Color(0.35f, 0.75f, 1f);
                break;
            case StatusType.Upgrade:
                image.color = new Color(0.35f, 0.55f, 1f);
                break;
            case StatusType.OnceInvincibility:
                image.color = new Color(1f, 0.85f, 0.25f);
                break;
            case StatusType.Heal:
                image.color = new Color(0.2f, 0.85f, 0.35f);
                break;
            default:
                image.color = Color.white;
                break;
        }
    }

    public void AttackColorShow()
    {
        ColorFeedbackShow(new Color(0f, 0f, 0f));
    }

    public void DamageColorShow()
    {
        ColorFeedbackShow(new Color(1f, 0.431f, 0.431f));
    }

    public void StatusHitColorShow()
    {
        ColorFeedbackShow(new Color(0.639f, 0.192f, 0.839f));
    }

    public void HealColorShow()
    {
        ColorFeedbackShow(new Color(0.192f, 0.835f, 0.318f));
    }

    private void ColorFeedbackShow(Color color)
    {
        ImageEnsure();

        if (colorFeedbackCoroutine != null)
        {
            StopCoroutine(colorFeedbackCoroutine);
        }

        colorFeedbackCoroutine = StartCoroutine(ColorFeedback(color));
    }

    private IEnumerator ColorFeedback(Color color)
    {
        colorFeedbackPlaying = true;
        image.color = color;

        yield return new WaitForSeconds(1f);

        colorFeedbackPlaying = false;
        colorFeedbackCoroutine = null;
        StatusColorReset();
    }
}
