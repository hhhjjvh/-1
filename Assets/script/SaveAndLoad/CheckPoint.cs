using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour,IInteractable
{
    private Animator anim;
    public string checkPointID;
    public bool actived;
   private GameObject Name;
  
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Name = transform.Find("Name")?.gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!actived)
        {
          Name.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Name.SetActive(false);
        }
    }
  
    [ContextMenu("Generate ID")]
    private void GenerateID()
    {
        checkPointID = System.Guid.NewGuid().ToString();
    }
    public void ActivateCheckPoint()
    {
        
        actived = true;
        anim.SetBool("Active", true);
    }

    public void TiggerAction()
    {
        ActivateCheckPoint();
        SaveManager.Instance.SaveGame();
    }

    public bool IsInteractable()
    {
        return !actived;
    }
}
