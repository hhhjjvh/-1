using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbout : MonoBehaviour, IInteractable
{
   public bool isEntered;
    [TextArea(1,3)]
    public string[] lines;
    [SerializeField]private bool hasName;
    public int facingDirection = 1;
    public bool facingRight = true;
    public bool canflip = true;
    public bool isShop;
    public DialogueSO dialogueSO;

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.x<PlayerManager.instance.player.transform.position.x&&!facingRight||
            transform.position.x > PlayerManager.instance.player.transform.position.x && facingRight)&& canflip)
        {
            Flip();
        }

        
    }
   // OnTriggerEnter2D(Collider2D other)
   public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isEntered = true;
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isEntered = false;
        }
          
    }
    public virtual void Flip()
    {

        facingDirection *= -1;

        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
       

    }

    public void TiggerAction()
    {
        if (isEntered && DialogueManager.Instance.dialogueBox.activeInHierarchy == false)
        {
           // Debug.Log("Interact with NPC");
            InputManager.Instance.canInteract = false;
            DialogueManager.Instance.StartDialogue(dialogueSO, this);
            isEntered = false;
        }
    }

    public bool IsInteractable()
    {
        return isEntered && DialogueManager.Instance.dialogueBox.activeInHierarchy == false;
    }
}
