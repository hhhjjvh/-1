using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public enum ControllerType
{
    Keyboard,
    XInputControllerWindows
}


public class Sign : MonoBehaviour
{
    public ControllerType controllerType;
    private Entity entity => GetComponentInParent<Entity>();
    private PlayerInput playerInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool canPress;
    private IInteractable interactable;
   
    private void Awake()
    {
        playerInput =new PlayerInput();
    }
    private void OnEnable()
    {
        playerInput.Enable();
        //InputSystem.onActionChanged += OnActionChanged;
        InputSystem.onActionChange+= OnActionChanged;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entity.onFlipped += FlipUI;
    }
    

    private void FlipUI()
    {
        transform.Rotate(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
       if(InputManager.Instance.canInteract)
        {
           
            if (canPress&&interactable!= null&&interactable.IsInteractable())
            {
                InputManager.Instance.canInteract = false;
                interactable.TiggerAction();
            }
        }
       if (interactable!= null&&!interactable.IsInteractable())
        {
            spriteRenderer.enabled = false;
        }

    }
    private void OnActionChanged(object arg1, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            //Debug.Log(((InputAction) arg1).activeControl.device);
            var d=((InputAction) arg1).activeControl.device;
            switch (d.device)
            {
                case Keyboard: controllerType = ControllerType.Keyboard;  break;
                case XInputControllerWindows: controllerType = ControllerType.XInputControllerWindows ;break;
                
                    default: controllerType = ControllerType.Keyboard; break;
            }
        }
    }

   
    // Start is called before the first frame update
  
    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("interacts");
        if (other.CompareTag("NPC"))
        {
           // Debug.Log("interact");
            if (other.GetComponent<IInteractable>()!= null)
            {
                interactable = other.GetComponent<IInteractable>();
            }
            if (interactable.IsInteractable())
            {
                spriteRenderer.enabled = true;
                canPress = true;
            }
            switch (controllerType)
            {
                case ControllerType.Keyboard: animator.Play("eanima"); break;
                case ControllerType.XInputControllerWindows: animator.Play("Yanima"); break;

                default: animator.Play("eanima"); break;
            }
           
            //animator.SetBool("isShow", true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            canPress = false;
            spriteRenderer.enabled = false;
        }
    }
}
