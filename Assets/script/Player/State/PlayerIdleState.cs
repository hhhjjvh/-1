public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerStateMachine playerStateMachine, Player player, string animBoolName) : base(playerStateMachine, player, animBoolName)
    {

    }


    public override void Enter()
    {
        base.Enter();
        player.ZeroVelocity();
     
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.velocity.y);
        if (xInput != 0 || yInput != 0)
        {

            if (xInput == -1 || xInput == 1)
            {
                stateMachine.ChangeState(player.moveState);
            }
        }
    }
}
