using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
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

    public Dialogue[] getDialogue()
    {
        storyStage.checkStage();
        dialogue.line.x = storyStage.getX();
        dialogue.line.y = storyStage.getY();
        dialogue.dialogues = DatabaseManager.instance.getDialogue((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }
}
