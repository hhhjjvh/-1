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
        GetItem(); // ������ȡ��ǰ������Ʒ��Ϣ
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
    /// ���ݸñ������ӣ���ѯ�ֵ��иñ������ӵ���Ʒ
    /// </summary>
    /// <returns>�ñ�������������Ʒ��Items</returns>
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
        transform.GetChild(0).gameObject.SetActive(true); // ѡ��ͼƬ��ʾ

        if (GetItem() != null)
        { // �ñ������ڴ�����Ʒ
            itemTipUI.ShowItemTip();
            itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.sellPrice);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Bag").transform as RectTransform, Input.mousePosition, null, out mousePos);
            itemTipUI.SetTipPosition(mousePos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false); // ѡ��ͼƬ�ر�

        itemTipUI.HideItemTip();
    }

    public void OnDrop(PointerEventData eventData)
    { // �˴���transformΪ�ɿ�����ʱ����������Ϸ����
        if (!item)
        { // ��ǰ��Ϊ��
            ItemDrag.itemBeginDragged.transform.SetParent(transform);
            if (!Inventory.Instance.slotDict.ContainsKey(slotID))
            {
                Inventory.Instance.slotDict.Add(slotID, Inventory.Instance.slotDict[ItemDrag.startID]);
                Inventory.Instance.slotDict.Remove(ItemDrag.startID);
            }
        }
        else
        { // ��ǰ�۲�Ϊ��
            Transform temp = ItemDrag.startParent; // ��¼������ԭ���Ĳ�
            item.transform.SetParent(temp); // �ò۵�ԭ�����û����϶�����ԭ���Ĳ�
            ItemDrag.itemBeginDragged.transform.SetParent(transform); // ���϶��������û����µĲ�

            // �ֵ�ֵ����
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
