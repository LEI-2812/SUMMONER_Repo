using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField]
    [Header("Dialogue")]
    DialogueEvent dialogue;

    private StoryStage storyStage;
    private void Start()
    {
        storyStage = GetComponent<StoryStage>();
    }

    public Dialogue[] GetDialogue()
    {
        storyStage.CheckStage();
        dialogue.line.x = storyStage.GetX();
        dialogue.line.y = storyStage.GetY();
        dialogue.dialogues = DatabaseManager.instance.GetDialogue((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }
}
