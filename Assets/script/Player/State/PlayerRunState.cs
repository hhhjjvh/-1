using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
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
        player.SetVelocity(xInput * player.moveSpeed*2, player.rb.velocity.y);
        if (xInput == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
        player.entityFX.CreatAfterImage();
    }
}
