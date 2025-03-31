using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory Instance;
    public Dictionary<int, string> slotDict; // ��Ʒ��������Ʒ
    public Dictionary<string, int> itemsDict ;  // ��ұ�������
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
        // ��������ScriptsObject��Ʒ����
        itemsResource = Resources.LoadAll<ItemData>("ItemData");
        foreach (ItemData item in itemsResource)
        {
            resourceDict.Add(item.itemName, item);
        }
    }
    /// <summary>
    /// �򱳰������������Ʒ�����ظ������Ƿ�ɱ�ʰȡ
    /// </summary>
    /// <param name="item">����ӵ���Ʒ��Ϣ</param>
    /// <param name="cnt">�������</param>
    public bool AddItem(ItemData item, int cnt)
    {
        if (itemsDict.ContainsKey(item.itemName) && itemsDict[item.itemName] + cnt <= 99)
        { // �������Ѿ����ڸ���Ʒ����ʰȡ������С�ڵ���99��������+cnt
            itemsDict[item.itemName] += cnt;
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
            { // ����UI��ʱ�����±�����ʾ��Ϣ
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            OnInventoryUpdated?.Invoke();
            return true;
        }
        else if (!itemsDict.ContainsKey(item.itemName) && slotDict.Count < 10 && itemsDict.Count < 10 && cnt <= 99)
        { // �����в�������Ʒ���һ��пռ�
            for (int i = 0; i < 10; i++)
            {
                bool canUseSlot = true; // ��ʾ�ò��Ƿ�Ϊ��
                foreach (KeyValuePair<int, string> pair in slotDict)
                {
                    if (i == pair.Key)
                    { // �ò��ѱ�ռ�ã�������ѭ����������һ����ѭ��
                        canUseSlot = false;
                        break;
                    }
                }

                if (canUseSlot)
                { // ��ǰi��ָ��Ʒ��δ��ռ�ã�����ѭ��
                    slotDict.Add(i, item.itemName);
                    itemsDict.Add(item.itemName, cnt);
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
            TipsBoxManager.Instance.ShowTipsBox("�޷���ȡ��Ʒ��<color=red>" + item.itemName + "</color>�����޿�λ", 3f);
            return false;
        }
        TipsBoxManager.Instance.ShowTipsBox("�޷���ȡ��Ʒ��<color=red>" + item.itemName + "</color>����Ʒ�ѳ�������", 3f);
        return false;
    }

    /// <summary>
    /// �����������е�ָ����Ʒ������ʹ֮����-1
    /// </summary>
    /// <param name="item">����ӵ���Ʒ��Ϣ</param>
    public void RemoveItem(ItemData item)
    {
        if (itemsDict.ContainsKey(item.itemName))
        { // ���ڸ���Ʒ
            if (itemsDict[item.itemName] > 1)
            { // ��������1
                itemsDict[item.itemName] -= 1;
            }
            else
            { // ��������1
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
            { // ����UI��ʱ�����±�����ʾ��Ϣ
                GameObject.FindGameObjectWithTag("Bag").GetComponent<BagUI>().UpdateBagUI();
            }
            ResetItem();
        }
        else
        { // �����ڸ�����
            Debug.LogError("�޷����������ڵ�����");
        }
    }

    /// <summary>
    /// ������Ʒ�۽ű��е�slotItem
    /// </summary>
    public void ResetItem()
    {
        if (UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag))
        {
            #region ��ȡ����UI�еĸ�����Ʒ�۽ű�
            GameObject bagUI = GameObject.FindGameObjectWithTag("Bag");
            slots = new SlotController[bagUI.transform.GetChild(0).GetChild(1).childCount];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = bagUI.transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<SlotController>();
            }
            #endregion

            #region ���¸�����Ʒ�۵�slotItem��ֵ
            for (int i = 0; i < slots.Length; i++)
            {
                if (slotDict.ContainsKey(i))
                {
                    slots[i].slotItem = resourceDict[slotDict[i]];
                }
                else
                { // ��������Ʒ
                    slots[i].slotItem = null;
                }
            }
            #endregion
        }
    }

    public void LoadData(GameData data)
    {
        // ��յ�ǰ����
        slotDict.Clear();
        itemsDict.Clear();

        // ��GameData��������
        foreach (var entry in data.slotDict)
        {
            slotDict.Add(entry.Key, entry.Value);
        }

        foreach (var entry in data.itemsDict)
        {
            itemsDict.Add(entry.Key, entry.Value);
        }

        // ����UI
        ResetItem();
        OnInventoryUpdated?.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        // ���GameData���ֵ���д��������
        data.slotDict.Clear();
        data.itemsDict.Clear();

        // ����ǰ���ݸ��Ƶ�GameData
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
