using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : BasePanel
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private bool shouldUpdate = true; // ����Ʒ�������ֱ仯ʱ�����±���UI
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

        //��ʼ����������
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
        // ��ʼ�����ఴť
        filterButtons[0].onClick.AddListener(() => SetFilter(null)); // ȫ��
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
                { // ɾ�����в����������item
                    slots[i].transform.GetChild(1).gameObject.SetActive(false);
                   // PoolMgr.Instance.Release(slots[i].transform.GetChild(1).gameObject);
                    GameObject.Destroy(slots[i].transform.GetChild(1).gameObject); //Destroy�����ӳ٣����º󷽸�����Ʒ����ͼƬ�������⣬������
                }
            }
            foreach (KeyValuePair<int, string> pair in Inventory.Instance.slotDict)
            {
                ItemData itemData = Inventory.Instance.resourceDict[pair.Value];

                // �������
                if (currentFilter != null && itemData.itemType != currentFilter)
                {
                    // �����Ѵ��ڵ���Ʒ
                    if (slots[pair.Key].transform.childCount > 1)
                        slots[pair.Key].transform.GetChild(1).gameObject.SetActive(false);
                    continue;
                }
                // ���ɶ�Ӧ������item
                GameObject.Instantiate(item, slots[pair.Key].transform);
            // GameObject gameObject=   PoolMgr.Instance.GetObj("item", transform.position, Quaternion.identity);
               // gameObject.transform.SetParent(slots[pair.Key].transform);
                // ���±��������е��ߵĸ��ӣ���slot�����һ��������
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
