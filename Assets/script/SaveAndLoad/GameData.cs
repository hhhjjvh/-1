using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currency;
    public int coin;

    public string saveTime ="";
    public float _totalPlaytimeSeconds;

    public Vector3 position;

    // ����״̬
    public SerializableDictionary<string, TaskSO.TaskStatus> taskStatuses = new();

    // �������ȣ�Key��ʽ��TaskID_ConditionGUID��
    public SerializableDictionary<string, int> conditionProgress = new();
   
    public SerializableDictionary<int, string> slotDict; // ��Ʒ��������Ʒ
    public SerializableDictionary<string, int> itemsDict;  // ��ұ�������

    public SerializableDictionary<string, bool> checkPoints;
    public string closestCheckPoint;

    public SerializableDictionary<string, float> volumeSetting;

    public SerializableDictionary<string, ItemSaveInfo> slotInfo = new();

    public string scenceName;

    public GameData()
    {
        currency = 0;
        coin = 0;
        
        saveTime = System.DateTime.Now.ToString();
        _totalPlaytimeSeconds= 0;
        position = Vector3.zero;
        taskStatuses = new SerializableDictionary<string, TaskSO.TaskStatus>();
        conditionProgress = new SerializableDictionary<string, int>();

        slotDict = new SerializableDictionary<int, string>();
        itemsDict = new SerializableDictionary<string, int>();
        checkPoints = new SerializableDictionary<string, bool>();
        closestCheckPoint = string.Empty;
        volumeSetting = new SerializableDictionary<string, float>();
       // inventorys = new Inventory();
    }
    public void SaveGameScene(GameSceneSO gameScenceSo)
    {
        scenceName =JsonUtility.ToJson(gameScenceSo);
    }
    public GameSceneSO LoadGameScene()
    {
        if (scenceName == null) return null;
        var newGameScenceSo = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(scenceName, newGameScenceSo);
        return newGameScenceSo;


        //JsonUtility.FromJson<GameScenceSo>(scenceName);
       
    }
   
}
[System.Serializable]
public class ItemSaveInfo
{
    public string itemID;       // ��ƷΨһ��ʶ
    public int stackSize;       // �ѵ�����
    public ItemType itemType;   // ��Ʒ���ͣ��������� inventory/stash��
}