using TMPro;
using UnityEngine;




public class ItemObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] protected LayerMask groundLayer;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;

    public TextMeshProUGUI text;
    public GameObject Name;
   
    private void OnValidate()
    {
        SetupVisuals();

    }
    void OnEnable()
    {
     
    }
    void OnDisable()
    {
       // shopRoom = null;
    }
    private void SetupVisuals()
    {
        if (itemData == null) return;
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
       // gameObject.name = "Item :" + itemData.itemName;
        text.text = itemData.itemName;
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = itemData.itemIcon;
        //rb.velocity = new Vector2(0, 7);

    }

  
    public void PickupItem()
    {

        bool canPicked = Inventory.Instance.AddItem(itemData, 1);
        if (canPicked)
        { // 可被拾取时启动销毁协程
            TipsBoxManager.Instance.ShowTipsBox("拾取物品：" + itemData.itemName, 1f);

        }
        PoolMgr.Instance.Release(gameObject);
        
        AudioMgr.Instance.PlaySFX("3455_get1");
  
    }
    public void SetUpItem(ItemData itemData, Vector2 velocity)
    {
        this.itemData = itemData;
        rb.velocity = velocity;
        text.text = itemData.itemName;
      
            Name.gameObject.SetActive(false);
            Name = transform.Find("Name")?.gameObject;
        
            SetupVisuals();
    }
    public virtual bool IsGroundedDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
}
