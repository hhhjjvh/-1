using UnityEngine;


public class InteractCG : MonoBehaviour, IInteractable
{
    public bool isEntered;
    public DialogueSO dialogueSO;
    private GameObject Name;

    void Start()
    {
        Name = transform.Find("Name")?.gameObject;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isEntered = true;
            Name?.SetActive(true);
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isEntered = false;
            Name?.SetActive(false);
        }

    }

    public void TiggerAction()
    {
        if (isEntered && DialogueManager.Instance.dialogueBox.activeInHierarchy == false)
        {
            InputManager.Instance.canInteract = false;
            DialogueManager.Instance.StartDialogue(dialogueSO);
            isEntered = false;
        }
    }

    public bool IsInteractable()
    {
        return isEntered && DialogueManager.Instance.dialogueBox.activeInHierarchy == false;
    }
}

