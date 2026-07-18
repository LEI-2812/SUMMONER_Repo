using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 역할: 스토리 대사와 이미지를 화면에 표시한다.
public class StoryDialogueView : MonoBehaviour, IPointerClickHandler
{
    [Header("참조")]
    public Text characterName;
    public Text dialogueContext;

    [SerializeField] private FadePanelView fadePanelView;

    private StoryImageView storyImageView;

    private void Awake()
    {
        storyImageView = GetComponent<StoryImageView>();
    }

    public void Clear()
    {
        characterName.text = "";
        dialogueContext.text = "";
    }

    public void ShowDialogue(Dialogue dialogue, int dialogueLineIndex)
    {
        DialogueLineShow.Show(characterName, dialogueContext, dialogue, dialogueLineIndex);
    }

    public void ShowImage(int dialogueIndex)
    {
        if (storyImageView != null)
        {
            storyImageView.ShowImage(dialogueIndex);
        }
    }

    public void FadeOut(Action callback)
    {
        fadePanelView.RegisterCallback(() =>
        {
            callback?.Invoke();
        });

        fadePanelView.FadeOut();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
