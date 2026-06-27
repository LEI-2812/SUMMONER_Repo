using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
// 역할: SummonStatusView의 책임을 정의한다.
public class SummonStatusView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float blinkInterval = 1f;

    private int currentEffectIndex;
    private int activeEffectCount;
    private float blinkTimer;
    private bool colorFeedbackPlaying;
    private Coroutine colorFeedbackCoroutine;
    private Color baseColor = Color.white;
    private bool baseColorCaptured;

    private void Awake()
    {
        ImageEnsure();
        BaseColorCapture();
    }

    public void ShowStatuses(IReadOnlyList<StatusData> activeStatusDatas)
    {
        ShowStatuses(activeStatusDatas, false);
    }

    public void ShowStatuses(IReadOnlyList<StatusData> activeStatusDatas, bool isPaused)
    {
        if (isPaused || colorFeedbackPlaying)
        {
            return;
        }

        ImageEnsure();
        BaseColorCapture();

        if (activeStatusDatas == null || activeStatusDatas.Count == 0)
        {
            StatusColorReset();
            return;
        }

        if (activeStatusDatas.Count == 1)
        {
            activeEffectCount = 1;
            currentEffectIndex = 0;
            blinkTimer = 0f;
            SetColorByStatus(activeStatusDatas[0].statusType);
            return;
        }

        MultipleStatusColorBlink(activeStatusDatas);
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
        BaseColorCapture();
        image.color = baseColor;
        blinkTimer = 0f;
        currentEffectIndex = 0;
        activeEffectCount = 0;
    }

    private void BaseColorCapture()
    {
        if (baseColorCaptured || image == null)
        {
            return;
        }

        baseColor = image.color;
        baseColorCaptured = true;
    }

    private void MultipleStatusColorBlink(IReadOnlyList<StatusData> activeStatusDatas)
    {
        if (activeEffectCount != activeStatusDatas.Count)
        {
            activeEffectCount = activeStatusDatas.Count;
            currentEffectIndex = 0;
            blinkTimer = 0f;
            SetColorByStatus(activeStatusDatas[currentEffectIndex].statusType);
            return;
        }

        blinkTimer += Time.deltaTime;

        if (blinkTimer < blinkInterval)
        {
            return;
        }

        blinkTimer = 0f;
        currentEffectIndex = (currentEffectIndex + 1) % activeStatusDatas.Count;
        SetColorByStatus(activeStatusDatas[currentEffectIndex].statusType);
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
        BaseColorCapture();

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
