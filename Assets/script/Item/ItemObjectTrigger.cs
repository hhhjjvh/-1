using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour, IInteractable
{
    private ItemObject myitemObject => GetComponentInParent<ItemObject>();
   

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (myitemObject != null)
            {
                myitemObject.Name.SetActive(true);
                //myitemObject.PickupItem();
            }

        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (myitemObject != null)
                myitemObject.Name.SetActive(false);
        }
    }


    public void TiggerAction()
    {  
        myitemObject.PickupItem();
    }

    public bool IsInteractable()
    {
        return true;
    }
}
