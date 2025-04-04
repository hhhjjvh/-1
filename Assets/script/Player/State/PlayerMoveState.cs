using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
       
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();         
        player.SetVelocity(xInput * player.moveSpeed, player.rb.velocity.y);
        if(xInput==0)
        {
            stateMachine.ChangeState(player.idleState);
        }
       
    }
}
