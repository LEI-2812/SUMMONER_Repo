using System;
using System.Collections;
using UnityEngine;
public class FadeController : MonoBehaviour // Panel ������ ������ ���̵��� or ���̵�ƿ�
{
    public bool isFadeIn; // true=FadeIn, false=FadeOut
    public GameObject panel; // �������� ������ Panel ������Ʈ
    private Action onCompleteCallback; // FadeIn �Ǵ� FadeOut ������ ������ �Լ�

    void Start()
    {
        if (!panel)
        {
            Debug.LogError("Panel ������Ʈ�� ã�� �� �����ϴ�.");
            throw new MissingComponentException();
        }

        if (isFadeIn) // Fade In Mode -> �ٷ� �ڷ�ƾ ����
        {
            panel.SetActive(true); // Panel Ȱ��ȭ
            StartCoroutine(CoFadeIn());
        }
        else
        {
            panel.SetActive(false); // Panel ��Ȱ��ȭ
        }
    }

    public void FadeOut()
    {
        panel.SetActive(true); // Panel Ȱ��ȭ
        Debug.Log("FadeCanvasController_ Fade Out ����");
        StartCoroutine(CoFadeOut());
        Debug.Log("FadeCanvasController_ Fade Out ��");
    }

    IEnumerator CoFadeIn()
    {
        float elapsedTime = 0f; // ���� ��� �ð�
        float fadedTime = 0.5f; // �� �ҿ� �ð�

        while (elapsedTime <= fadedTime)
        {
            panel.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
            Debug.Log("Fade In ��...");
            yield return null;
        }
        Debug.Log("Fade In ��");
        panel.SetActive(false); // Panel�� ��Ȱ��ȭ
        onCompleteCallback?.Invoke(); // ���Ŀ� �ؾ� �ϴ� �ٸ� �׼��� �ִ� ���(null�� �ƴ�) �����Ѵ�
        yield break;
    }

    IEnumerator CoFadeOut()
    {
        float elapsedTime = 0f; // ���� ��� �ð�
        float fadedTime = 0.5f; // �� �ҿ� �ð�

        while (elapsedTime <= fadedTime)
        {
            panel.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(0f, 1f, elapsedTime / fadedTime));

            elapsedTime += Time.deltaTime;
            Debug.Log("Fade Out ��...");
            yield return null;
        }

        Debug.Log("Fade Out ��");
        onCompleteCallback?.Invoke(); // ���Ŀ� �ؾ� �ϴ� �ٸ� �׼��� �ִ� ���(null�� �ƴ�) �����Ѵ�
        yield break;
    }

    public void RegisterCallback(Action callback) // �ٸ� ��ũ��Ʈ���� �ݹ� �׼� ����ϱ� ���� ���
    {
        onCompleteCallback = callback;
    }
}