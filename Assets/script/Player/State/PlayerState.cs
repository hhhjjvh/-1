using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    
    protected Rigidbody2D rb;

   protected PlayerStateMachine stateMachine;
   protected Player player;

    private string animBoolName;
    protected float xInput{get; set; }
    protected float yInput{ get; set; }

    protected float stateTimer;
    protected bool triggerCalled;

   // protected float animSpeed;
    public PlayerState(PlayerStateMachine playerStateMachine, Player player, string animBoolName)
    {
        this.stateMachine = playerStateMachine;
        this.player = player;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
       // player.anim.SetBool(animBoolName, true);
       player.anim.Play(animBoolName);
        rb = player.rb;
        triggerCalled = false;
    }
    public virtual void Exit()
    {
       // player.anim.SetBool(animBoolName, false);

    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        // xInput = Input.GetAxisRaw("Horizontal");
        xInput = InputManager.Instance.moveInput.x;
      
        //yInput = Input.GetAxisRaw("Vertical");
        yInput = InputManager.Instance.moveInput.y;
        player.anim.SetFloat("yVelocity", rb.velocity.y);
        //player.entityFX.CreatAfterImage();
        
    }
    public virtual void AnimationTrigger()
    {
        triggerCalled = true;
    }
   
}
