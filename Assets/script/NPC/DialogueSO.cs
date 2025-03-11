using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    public enum SystemType { Legacy, NodeBased }

    [Header("System Settings")]
    public SystemType systemType = SystemType.NodeBased;

    [Header("Node Based System")]
    public DialogueNode[] nodes; // 对话节点数组
    [NonSerialized] public int currentNodeIndex; // 当前节点索引（运行时）

    [Header("Legacy System")]
    [TextArea(1, 50)] public string dialogueData;  // 一个长字符串，包含所有对话数据
    [HideInInspector] public string[] cachedLines;
    public DialogueEvent[] events;

    [Header("Common Settings")]
    public bool hasName; // 是否显示角色名
    public EventEffect[] EndscriptableEffects;

    void OnValidate()
    {
        // 处理旧系统数据
        if (systemType == SystemType.Legacy)
        {
            cachedLines = !string.IsNullOrEmpty(dialogueData)
                ? dialogueData.Split(new[] { "/n" }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(line => line.Trim()).ToArray()
                : Array.Empty<string>();
          
            for (int i = 0; i < events.Length; i++)
            {
                events[i].triggerLine = Array.FindIndex(cachedLines, line => line.Contains(events[i].triggerSymbol));

            }
        }
        
    }
    public void ResetProgress() => currentNodeIndex = 0;
}
[System.Serializable]
public class DialogueEvent
{
    [Tooltip("在对话内容中插入此符号时触发选项")]
    public string triggerSymbol = "@trigger";
    public int triggerLine = -1; // 触发行号
   
    public UnityEvent onReached; // 触发事件
}








