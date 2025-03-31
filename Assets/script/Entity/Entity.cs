using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int facingDirection { get; protected set; } = 1;
    protected bool facingRight = true;
    public bool canMove = true;
    public System.Action onFlipped { get; set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX entityFX { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        entityFX = GetComponent<EntityFX>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();   
        cd = GetComponent<CapsuleCollider2D>();     
    }
    protected virtual void Start()
    {
       
    }
    protected virtual void Update()
    {
      
    }
    public void SetVelocity(float x, float y)
    {
        
        rb.velocity = new Vector2(x, y);
        FlipControllers(x);
    }
    public void ZeroVelocity()
    {
       
        rb.velocity = Vector2.zero;
    }
    public virtual void Flip()
    {
        facingDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        if (onFlipped != null)
        {
            onFlipped();
        }
    }
    public virtual void FlipControllers(float x)
    {
        if (facingRight && x < 0 || !facingRight && x > 0)
        {
            Flip();
        }

    }
    public virtual bool IsGroundedDetected()
    {
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer))
        {
            return true;
        }

        return false;
    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
    }
    }
