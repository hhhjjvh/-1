using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class TaskManager : MonoBehaviour, ISaveManager
{
    public static TaskManager Instance;
    public List<TaskSO> activeTasks = new List<TaskSO>(); // 当前活跃任务
    public List<TaskSO> allTasks;
    [Header("UI References")]
    public GameObject taskCompletedPrefab;
   // public Transform uiParent;
    // 新增事件
    public event System.Action OnTaskUpdated;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
#if UNITY_EDITOR
        FillUpTaskDataBase();
#endif
    }
    void Start()
    {
        TriggerTaskUpdate();
    }
    private void Update()
    {
        foreach (var task in activeTasks.ToArray()) // 使用ToArray避免迭代时修改集合
        {
            bool allConditionsMet = true;
            foreach (var condition in task.conditions)
            {
                if (!condition.IsConditionMet())
                {
                    allConditionsMet = false;
                    break;
                }
            }

            if (allConditionsMet)
            {
                CompleteTask(task);
            }
        }
    }
    private void TriggerTaskUpdate()
    {
        OnTaskUpdated?.Invoke();
    }

    public bool TryAcceptTask(TaskSO task)
    {

        if (task.status != TaskSO.TaskStatus.NotStarted) return false;

        // 检查前置任务
        foreach (var prerequisite in task.prerequisites)
        {
            if (prerequisite.status != TaskSO.TaskStatus.Completed) return false;
        }

        task.status = TaskSO.TaskStatus.InProgress;
        activeTasks.Add(task);

        // 注册所有条件的监听
        foreach (var condition in task.conditions)
        {
            condition.RegisterListeners();
        }

        // 触发接取任务事件
        foreach (var effect in task.onAcceptedEffects)
        {
            if (effect != null && effect.ApplyTrigger())
            {
                effect.ApplyEffect(PlayerManager.instance.player.gameObject);
            }
        }
        TriggerTaskUpdate(); // 在任务状态变化后触发事件
        return true;
    }

    public void CompleteTask(TaskSO task)
    {
        if (task.status == TaskSO.TaskStatus.InProgress)
        {
            task.status = TaskSO.TaskStatus.Completed;
            activeTasks.Remove(task);
            // 显示完成UI
            if (taskCompletedPrefab)
            {
                taskCompletedPrefab.SetActive(true);
                taskCompletedPrefab.GetComponent<TaskCompletedUI>().Show(task);
            }
            foreach (var condition in task.conditions)
            {
                condition.UnregisterListeners();
            }
            // 触发完成任务事件
            foreach (var effect in task.onCompletedEffects)
            {
                if (effect != null && effect.ApplyTrigger())
                {
                    effect.ApplyEffect(PlayerManager.instance.player.gameObject);
                }
            }
        }
        TriggerTaskUpdate(); // 在任务状态变化后触发事件
    }

    public void ReloadActiveTasks()
    {
        activeTasks.Clear();
        foreach (var task in allTasks)
        {
            if (task.status == TaskSO.TaskStatus.InProgress)
            {
                activeTasks.Add(task);
                foreach (var condition in task.conditions)
                {
                    condition.RegisterListeners();
                }
            }
        }
    }
    public void LoadData(GameData data)
    {
        foreach (var task in allTasks)
        {
            if (data.taskStatuses.TryGetValue(task.taskID, out var status))
            {
                task.status = status;
            }

            // 恢复条件进度
            foreach (var condition in task.conditions)
            {
                string key = $"{task.taskID}_{condition.guid}";
                if (data.conditionProgress.TryGetValue(key, out int progress))
                {
                    condition.currentAmount = progress;
                }
            }
        }

        // 重新激活任务
        ReloadActiveTasks();
        // StartCoroutine(LoadGameData(data));
    }


    public void SaveData(ref GameData data)
    {
        foreach (var task in allTasks)
        {
            data.taskStatuses[task.taskID] = task.status;

            // 保存条件进度
            foreach (var condition in task.conditions)
            {
                string key = $"{task.taskID}_{condition.guid}";
                data.conditionProgress[key] = condition.currentAmount;
            }
        }
    }
    [ContextMenu("ResetProgress")]
    public void ResetProgress()
    {
        foreach (var task in allTasks)
        {
            task.ResetProgress();
        }
    }
#if UNITY_EDITOR

    [ContextMenu("Fill Up TaskDataBase")]
    private void FillUpTaskDataBase() => allTasks = new List<TaskSO>(GetTaskDataBase());
    private List<TaskSO> GetTaskDataBase()
    {
        List<TaskSO> taskDataBase = new List<TaskSO>();
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Resources_moved/Data/Task" });
        foreach (string assetName in assetNames)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetName);
            var taskData = AssetDatabase.LoadAssetAtPath<TaskSO>(path);
            if (taskData != null)
            {
                taskDataBase.Add(taskData);
            }
        }
        return taskDataBase;
    }
#endif
}
