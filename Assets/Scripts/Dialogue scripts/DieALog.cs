using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class DieALog : MonoBehaviour
{
    private Dialogue dialogue;
    public UnityEvent onStart, onEnd;
    public float textSpeed;

    public TMPro.TMP_Text dialogueName;
    public TMPro.TMP_Text dialogueText;
    public GameObject me;
    private int dialogueIndex;
    private bool isDialogueRunning;

    public void StartDialogue(Dialogue newDialogue)
    {
        dialogue = newDialogue;

        me.SetActive(true);
        dialogueIndex = 0;
        onStart.Invoke();
        Debug.Log("I am working. Hip hip hooray");
        //Time.timeScale = 0f;
        StartCoroutine(WriteDialoguePiece(dialogue.dialogue[0]));
    }

    public void StopDialogue()
    {
        onEnd.Invoke();
        me.SetActive(false);
        //Time.timeScale = 1f;
    }

    public void NextDialogueOrStop(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 0 || isDialogueRunning)
            return;

        ++dialogueIndex;

        if (dialogueIndex >= dialogue.dialogue.Count)
        {
            StopDialogue();
            return;
        }
        StartCoroutine(WriteDialoguePiece(dialogue.dialogue[dialogueIndex]));
    }


    public IEnumerator WriteDialoguePiece(DialoguePiece dialogue)
    {
        dialogueName.SetText(dialogue.name);
        dialogueText.text = "";

        isDialogueRunning = true;

        for (int i = 0; i < dialogue.dialogue.Length; ++i)
        {

            dialogueText.text += dialogue.dialogue[i];
            yield return new WaitForSeconds(textSpeed);
        }

        isDialogueRunning = false;
    }
}