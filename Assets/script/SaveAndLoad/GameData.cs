using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public int currency;
    public int coin;

    public int experience;
    public int needExperience;
    public int level;

    public string saveTime ="";
    public float _totalPlaytimeSeconds;

    public Vector3 position;

    // 任务状态
    public SerializableDictionary<string, TaskSO.TaskStatus> taskStatuses = new();

    // 条件进度（Key格式：TaskID_ConditionGUID）
    public SerializableDictionary<string, int> conditionProgress = new();
    //public SerializableDictionary<string, int,int> inventory;
    public SerializableDictionary<string, int> inventory;
    public SerializableDictionary<string, int> inventorySlot;
    public SerializableDictionary<string, bool> skillTree;

    public List<string> equipmentID;

    public SerializableDictionary<string, bool> checkPoints;
    public string closestCheckPoint;

    public SerializableDictionary<string, float> volumeSetting;

  

    public string scenceName;

    public GameData()
    {
        currency = 0;
        coin = 0;
        experience = 0;
        needExperience = 0;
        level = 1;
        saveTime = System.DateTime.Now.ToString();
        _totalPlaytimeSeconds= 0;
        position = Vector3.zero;
        taskStatuses = new SerializableDictionary<string, TaskSO.TaskStatus>();
        conditionProgress = new SerializableDictionary<string, int>();
     
    inventory = new SerializableDictionary<string, int>();
        inventorySlot = new SerializableDictionary<string, int>();
        skillTree = new SerializableDictionary<string, bool>();
        equipmentID = new List<string>();
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
