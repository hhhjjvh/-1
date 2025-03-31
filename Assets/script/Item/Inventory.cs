using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory Instance;
    public Dictionary<int, string> slotDict; // 物品槽所含物品
    public Dictionary<string, int> itemsDict ;  // 玩家背包数据
    public Dictionary<string, ItemData> resourceDict = new Dictionary<string, ItemData>();
    private ItemData[] itemsResource;
    public SlotController[] slots;
    public static event Action OnInventoryUpdated;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
       slotDict = new Dictionary<int, string>();
      itemsDict = new Dictionary<string, int>();
    }
    private void OnEnable()
    {
        // 加载所有ScriptsObject物品数据
        itemsResource = Resources.LoadAll<ItemData>("ItemData");
        foreach (ItemData item in itemsResource)
        {
            resourceDict.Add(item.itemName, item);
        }
    }
    /// <summary>
    /// 向背包数据中添加物品，返回该物体是否可被拾取
    /// </summary>
    /// <param name="item">待添加的物品信息</param>
    /// <param name="cnt">添加数量</param>
    public bool AddItem(ItemData item, int cnt)
    {
        if (itemsDict.ContainsKey(item.itemName) && itemsDict[item.itemName] + cnt <= 99)
        { // 背包中已经存在该物品，且拾取后数量小于等于99，则数量+cnt
            itemsDict[item.itemName] += cnt;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // 背包UI打开时，更新背包显示信息
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            OnInventoryUpdated?.Invoke();
            return true;
        }
        else if (!itemsDict.ContainsKey(item.itemName) && slotDict.Count < 10 && itemsDict.Count < 10 && cnt <= 99)
        { // 背包中不含该物品，且还有空间
            for (int i = 0; i < 10; i++)
            {
                bool canUseSlot = true; // 表示该槽是否为空
                foreach (KeyValuePair<int, string> pair in slotDict)
                {
                    if (i == pair.Key)
                    { // 该槽已被占用，跳出内循环，进入下一个外循环
                        canUseSlot = false;
                        break;
                    }
                }

                if (canUseSlot)
                { // 当前i所指物品槽未被占用，跳出循环
                    slotDict.Add(i, item.itemName);
                    itemsDict.Add(item.itemName, cnt);
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
            TipsBoxManager.Instance.ShowTipsBox("无法获取物品：<color=red>" + item.itemName + "</color>，已无空位", 3f);
            return false;
        }
        TipsBoxManager.Instance.ShowTipsBox("无法获取物品：<color=red>" + item.itemName + "</color>该物品已超出上限", 3f);
        return false;
    }

    /// <summary>
    /// 将背包数据中的指定物品丢弃，使之数量-1
    /// </summary>
    /// <param name="item">待添加的物品信息</param>
    public void RemoveItem(ItemData item)
    {
        if (itemsDict.ContainsKey(item.itemName))
        { // 存在该物品
            if (itemsDict[item.itemName] > 1)
            { // 数量大于1
                itemsDict[item.itemName] -= 1;
            }
            else
            { // 数量等于1
                foreach (KeyValuePair<int, string> pair in slotDict)
                {
                    if (pair.Value == item.itemName)
                    {
                        slotDict.Remove(pair.Key);
                        break;
                    }
                }
                itemsDict.Remove(item.itemName);
            }
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // 背包UI打开时，更新背包显示信息
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            ResetItem();
        }
        else
        { // 不存在该物体
            Debug.LogError("无法丢弃不存在的物体");
        }
    }

    /// <summary>
    /// 重置物品槽脚本中的slotItem
    /// </summary>
    public void ResetItem()
    {
        if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
        {
            #region 获取背包UI中的各个物品槽脚本
            GameObject bagUI = GameObject.FindGameObjectWithTag("Bag");
            slots = new SlotController[bagUI.transform.GetChild(0).GetChild(1).childCount];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = bagUI.transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<SlotController>();
            }
            #endregion

            #region 更新各个物品槽的slotItem的值
            for (int i = 0; i < slots.Length; i++)
            {
                if (slotDict.ContainsKey(i))
                {
                    slots[i].slotItem = resourceDict[slotDict[i]];
                }
                else
                { // 槽内无物品
                    slots[i].slotItem = null;
                }
            }
            #endregion
        }
    }

    public void LoadData(GameData data)
    {
        // 清空当前数据
        slotDict.Clear();
        itemsDict.Clear();

        // 从GameData加载数据
        foreach (var entry in data.slotDict)
        {
            slotDict.Add(entry.Key, entry.Value);
        }

        foreach (var entry in data.itemsDict)
        {
            itemsDict.Add(entry.Key, entry.Value);
        }

        // 更新UI
        ResetItem();
        OnInventoryUpdated?.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        // 清空GameData的字典以写入新数据
        data.slotDict.Clear();
        data.itemsDict.Clear();

        // 将当前数据复制到GameData
        foreach (var entry in slotDict)
        {
            data.slotDict.Add(entry.Key, entry.Value);
        }

        foreach (var entry in itemsDict)
        {
            data.itemsDict.Add(entry.Key, entry.Value);
        }
    }
}
