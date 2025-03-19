using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private ItemData[] itemDatas;
    [SerializeField] private int dropCount;
    private List<ItemData> dropItems= new List<ItemData>();

    [SerializeField] protected GameObject dropPrefab;
   
    //[SerializeField] private ItemData itemData;
  


    public virtual void GenerateDropItems()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
        //    if(Random.Range(0, 100) <= itemDatas[i].dropChance)
            {
                dropItems.Add(itemDatas[i]);
                //Debug.Log(itemDatas[i].itemName);
            }
        }

        for (int i = 0; i < dropCount; i++)
        {
            if(dropItems.Count<1) return;
            ItemData dropItem = dropItems[Random.Range(0, dropItems.Count-1)];
            dropItems.Remove(dropItem);
            DropItem(dropItem);
            //Debug.Log(dropItem.itemName);

        }


    }

    public void DropItem(ItemData itemData)
    {
        GameObject dropItem = PoolMgr.Instance.GetObj("Item _Sword", transform.position, Quaternion.identity);
        //Instantiate(dropPrefab, transform.position, Quaternion.identity);
        Vector2 randomDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(5f, 8f));
        dropItem.GetComponent<ItemObject>().SetUpItem(itemData, randomDirection);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
