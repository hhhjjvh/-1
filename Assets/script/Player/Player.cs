using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :Entity
{
    [Header("Move info")]
    public float moveSpeed=8;
    public float jumpForce;
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerRunState runState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(stateMachine, this, "Idle");
        moveState = new PlayerMoveState(stateMachine, this, "Move");
        runState = new PlayerRunState(stateMachine, this, "Run");
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        if (Time.timeScale == 0 || !canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        base.Update();
        stateMachine.currentState.Update();
    }
}
