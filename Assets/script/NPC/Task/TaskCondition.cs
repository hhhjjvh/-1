using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public abstract class TaskCondition : ScriptableObject
{
    [Header("基础配置")]
    public string guid; // 新增唯一标识符
    public string conditionDescription; // 条件描述（如“收集5个苹果”）
    public int requiredAmount = 1;      // 需要完成的数量
    [NonSerialized] public int currentAmount; // 当前进度（运行时）
    public System.Action OnProgressUpdated;// 新增事件

   

    // 初始化时生成GUID（在CreateAssetMenu创建时自动生成）

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        // 仅在编辑器生成时分配 GUID
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
    public virtual void ResetProgress()
    {
        OnProgressUpdated?.Invoke();
        currentAmount = 0;
    }
    public abstract bool IsConditionMet();    // 检查条件是否满足
    public abstract void RegisterListeners(); // 注册事件监听
    public abstract void UnregisterListeners(); // 注销事件监听
}
