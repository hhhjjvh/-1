using UnityEngine;

public class PlayerGroundedState : PlayerActionState
{
  
    public PlayerGroundedState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
     //  attackPressTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        if (InputManager.Instance.canJump && player.IsGroundedDetected())
        {
            stateMachine.ChangeState(player.jumpState);
            InputManager.Instance.canJump = false;
        }
    }

}
