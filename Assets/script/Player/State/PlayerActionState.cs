using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionState : PlayerState
{
    public PlayerActionState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
       
        
    }
}
