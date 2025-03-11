using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Task/TaskSO")]
public class TaskSO : ScriptableObject
{
    public string taskID;          // 任务唯一标识
    public string taskName;        // 任务名称
    [TextArea] public string description; // 任务描述
    public TaskStatus status = TaskStatus.NotStarted; // 任务状态
    public TaskSO[] prerequisites; // 前置任务（依赖任务）
    [Header("任务条件")]
    public TaskCondition[] conditions; // 支持多个条件
    [Header("脚本化事件")]
    public EventEffect[] onAcceptedEffects; // 接取任务时触发的事件
    public EventEffect[] onCompletedEffects; // 完成任务时触发的事件

    public enum TaskStatus
    {
        NotStarted,   // 未接取
        InProgress,   // 进行中
        Completed     // 已完成
    }
    private void OnValidate()
    {


#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            string path = AssetDatabase.GetAssetPath(this);
            taskID = AssetDatabase.AssetPathToGUID(path);
        }
#endif
    }
    public void ResetProgress()
    {
        foreach (var condition in conditions)
        {
            condition.ResetProgress();
        }
        status = TaskStatus.NotStarted;
    }
}