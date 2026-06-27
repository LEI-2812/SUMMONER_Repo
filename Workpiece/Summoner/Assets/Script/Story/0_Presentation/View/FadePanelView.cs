using System;
using System.Collections;
using UnityEngine;

// 역할: FadePanelView의 책임을 정의한다.
public class FadePanelView : MonoBehaviour
{
    public bool isFadeIn; // true=FadeIn, false=FadeOut
    public GameObject panel;
    private Action onCompleteCallback;

    void Start()
    {
        if (!panel)
        {
            Debug.LogError("Panel 오브젝트를 찾을 수 없습니다.");
            throw new MissingComponentException();
        }

        if (isFadeIn)
        {
            panel.SetActive(true);
            StartCoroutine(CoFadeIn());
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void FadeOut()
    {
        panel.SetActive(true);
        Debug.Log("FadeCanvasController Fade Out 시작");
        StartCoroutine(CoFadeOut());
    }

    IEnumerator CoFadeIn()
    {
        float elapsedTime = 0f;
        float fadedTime = 0.5f;

        while (elapsedTime <= fadedTime)
        {
            panel.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
        Debug.Log("Fade In 중...");
            yield return null;
        }
        panel.SetActive(false);
        onCompleteCallback?.Invoke();
        yield break;
    }

    IEnumerator CoFadeOut()
    {
        float elapsedTime = 0f;
        float fadedTime = 0.5f;

        while (elapsedTime <= fadedTime)
        {
            panel.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(0f, 1f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
        Debug.Log("Fade Out 중...");
            yield return null;
        }

        onCompleteCallback?.Invoke();
        yield break;
    }

    public void RegisterCallback(Action callback)
    {
        onCompleteCallback = callback;
    }
}
