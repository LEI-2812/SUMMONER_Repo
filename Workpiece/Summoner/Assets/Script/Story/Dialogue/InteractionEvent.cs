using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField]
    [Header("Dialogue")]
    DialogueEvent dialogue;

    private CheckStage checkStage;
    private void Start()
    {
        checkStage = GetComponent<CheckStage>();
    }

    public Dialogue[] getDialogue()
    {
        checkStage.checkStage();
        dialogue.line.x = CheckStage.x;
        dialogue.line.y = CheckStage.y;
        dialogue.dialogues = DatabaseManager.instance.getDialogue((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }
}
