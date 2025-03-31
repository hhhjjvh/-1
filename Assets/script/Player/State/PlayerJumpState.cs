using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerActionState
{

    public PlayerJumpState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0.3f;
        player.isJumped = false;
        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);

    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (InputManager.Instance.canJump && !player.isJumped && stateTimer < 0)
        {
            InputManager.Instance.canJump = false;
            //Debug.Log("jump");
            rb.velocity = new Vector2(rb.velocity.x, player.jumpForce * 0.9f);
            player.isJumped = true;
        }
        if (xInput != 0)
        {
            player.SetVelocity(xInput * player.moveSpeed * .8f, rb.velocity.y);
        }
        if (player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

}
