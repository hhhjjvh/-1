using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player :Entity
{
    public ItemData item;
    [Header("Move info")]
    public float moveSpeed=8;
    public float jumpForce;
    public bool isJumped;
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerRunState runState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(stateMachine, this, "Idle");
        moveState = new PlayerMoveState(stateMachine, this, "Run");
        jumpState= new PlayerJumpState(stateMachine, this, "Run");
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool canPicked = Inventory.Instance.AddItem(item, 1);
            if (canPicked)
            { // 可被拾取时启动销毁协程
                TipsBoxManager.Instance.ShowTipsBox("拾取物品：" + item.itemName, 1f);

            }
        }
    }
}
