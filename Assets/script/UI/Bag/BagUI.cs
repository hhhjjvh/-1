using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : BasePanel
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private bool shouldUpdate = true; // 当物品数量出现变化时，更新背包UI
    [SerializeField] private GameObject item;
    [SerializeField] private ItemTipUI itemTipUI;
    [SerializeField] private GameObject optionPanel;
    public int slotCount = 18;
    public Transform itemParent;
    [SerializeField] private Button[] filterButtons;
    private ItemType? currentFilter = null;

    private void Awake()
    {
        OpenPanel(UIConst.PlayerBag);
        GameManager.Instance.PauseGame(true);
        Time.timeScale = 0f;

        //初始化背包格子
      //  ClearSlots(itemParent);
       // GenerateSlots(itemParent, slotCount);


    }
    void Start()
    {
        slots = new GameObject[transform.GetChild(0).GetChild(1).childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = transform.GetChild(0).GetChild(1).GetChild(i).gameObject;
        }
        // 初始化分类按钮
        filterButtons[0].onClick.AddListener(() => SetFilter(null)); // 全部
        filterButtons[1].onClick.AddListener(() => SetFilter(ItemType.Material));
        filterButtons[2].onClick.AddListener(() => SetFilter(ItemType.Equipment));
        filterButtons[3].onClick.AddListener(() => SetFilter(ItemType.food));
    }
    public void SetFilter(ItemType? type)
    {
        currentFilter = type;
        shouldUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldUpdate)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].transform.childCount == 2)
                { // 删除所有槽内无物体的item
                    slots[i].transform.GetChild(1).gameObject.SetActive(false);
                   // PoolMgr.Instance.Release(slots[i].transform.GetChild(1).gameObject);
                    GameObject.Destroy(slots[i].transform.GetChild(1).gameObject); //Destroy存在延迟，导致后方更新物品数量图片出现问题，先隐藏
                }
            }
            foreach (KeyValuePair<int, string> pair in Inventory.Instance.slotDict)
            {
                ItemData itemData = Inventory.Instance.resourceDict[pair.Value];

                // 分类过滤
                if (currentFilter != null && itemData.itemType != currentFilter)
                {
                    // 隐藏已存在的物品
                    if (slots[pair.Key].transform.childCount > 1)
                        slots[pair.Key].transform.GetChild(1).gameObject.SetActive(false);
                    continue;
                }
                // 生成对应数量的item
                GameObject.Instantiate(item, slots[pair.Key].transform);
            // GameObject gameObject=   PoolMgr.Instance.GetObj("item", transform.position, Quaternion.identity);
               // gameObject.transform.SetParent(slots[pair.Key].transform);
                // 更新背包中已有道具的格子，即slot下最后一个子物体
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount-1).
                    GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount - 1).
                    GetChild(0).GetComponent<Image>().sprite = Inventory.Instance.resourceDict[pair.Value].icon;
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount - 1).
                    GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
                slots[pair.Key].transform.GetChild(slots[pair.Key].transform.childCount - 1).
                    GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = Inventory.Instance.itemsDict[pair.Value].ToString();
            }
            shouldUpdate = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            CloseBag();
        }
    }
    private void GenerateSlots(Transform parent, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = PoolMgr.Instance.GetObj("slot", transform.position, Quaternion.identity);
            SlotController slotID = obj.GetComponent<SlotController>();
            slotID.SetUp(i, itemTipUI, optionPanel);
            obj.transform.SetParent(parent);
        }
    }
    private void ClearSlots(Transform parent)
    {
        foreach (Transform child in parent)
        {
            PoolMgr.Instance.Release(child.gameObject);
        }
    }
    private void CloseBag()
    {
        if (!isRemove)
        {
            ClosePanel();
            GameManager.Instance.PauseGame(false);
            Time.timeScale = 1f;
        }
    }

    public void UpdateBagUI()
    {
        shouldUpdate = true;
    }
}
