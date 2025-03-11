using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGTrigger : MonoBehaviour
{
    public DialogueSO dialogueSO;
    public bool isTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered&&collision.CompareTag("Player"))
        {
            DialogueManager.Instance.StartDialogue(dialogueSO);
            isTriggered = true;
        }
    }
}
