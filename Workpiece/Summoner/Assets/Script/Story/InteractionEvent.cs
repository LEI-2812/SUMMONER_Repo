using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField]
    [Header("Dialogue")]
    DialogueEvent dialogue;

    public Dialogue[] getDialogue()
    {
        dialogue.dialogues = DatabaseManager.instance.getDialogue((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }
}
