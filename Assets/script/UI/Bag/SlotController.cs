using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public int slotID;
    public ItemData slotItem;
    [SerializeField] private ItemTipUI itemTipUI;
    [SerializeField] private GameObject optionPanel;
    private Vector2 mousePos;
    private void OnEnable()
    {
        Inventory.OnInventoryUpdated += UpdateSlot;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryUpdated -= UpdateSlot;
    }

    private void UpdateSlot()
    {
        slotItem = GetItem();
    }
    private void Update()
    {
        GetItem(); // 持续获取当前槽内物品信息
    }

    public GameObject item
    {
        get
        {
            if (transform.childCount > 1)
            {
                return transform.GetChild(1).gameObject;
            }
            return null;
        }
    }

    public void SetUp(int id, ItemTipUI itemTipUI, GameObject optionPanel)
    {
        slotID = id;
        this.itemTipUI = itemTipUI;
        this.optionPanel = optionPanel;
       
    }

    /// <summary>
    /// 根据该背包格子，查询字典中该背包格子的物品
    /// </summary>
    /// <returns>该背包格子所含物品的Items</returns>
    private ItemData GetItem()
    {
        if (Inventory.Instance == null || Inventory.Instance.slotDict == null)
            return null;

        if (Inventory.Instance.slotDict.ContainsKey(slotID))
        {
            slotItem = Inventory.Instance.resourceDict[Inventory.Instance.slotDict[slotID]];
            return slotItem;
        }
        slotItem = null;
        return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(true); // 选中图片显示

        if (GetItem() != null)
        { // 该背包格内存在物品
            itemTipUI.ShowItemTip();
            itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.sellPrice);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Bag").transform as RectTransform, Input.mousePosition, null, out mousePos);
            itemTipUI.SetTipPosition(mousePos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false); // 选中图片关闭

        itemTipUI.HideItemTip();
    }

    public void OnDrop(PointerEventData eventData)
    { // 此处的transform为松开键鼠时的所处的游戏对象
        if (!item)
        { // 当前槽为空
            ItemDrag.itemBeginDragged.transform.SetParent(transform);
            if (!Inventory.Instance.slotDict.ContainsKey(slotID))
            {
                Inventory.Instance.slotDict.Add(slotID, Inventory.Instance.slotDict[ItemDrag.startID]);
                Inventory.Instance.slotDict.Remove(ItemDrag.startID);
            }
        }
        else
        { // 当前槽不为空
            Transform temp = ItemDrag.startParent; // 记录该物体原来的槽
            item.transform.SetParent(temp); // 该槽的原物体置换到拖动物体原来的槽
            ItemDrag.itemBeginDragged.transform.SetParent(transform); // 被拖动的物体置换到新的槽

            // 字典值交换
            string tempStr = Inventory.Instance.slotDict[ItemDrag.startID];
            Inventory.Instance.slotDict[ItemDrag.startID] = Inventory.Instance.slotDict[slotID];
            Inventory.Instance.slotDict[slotID] = tempStr;
        }
    }

    public void ShowOptionPanel()
    {
        if (slotItem != null)
        {
            OptionPanel.currItem = slotItem;
            Vector2 mouse;
            optionPanel.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Bag").transform
                as RectTransform, Input.mousePosition, null, out mouse);
            optionPanel.transform.localPosition = mouse;
        }
    }
}
