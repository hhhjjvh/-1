using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Dialogue/Choice")]
public class Choice : ScriptableObject
{
    public string text;
    public DialogueSO nextDialogue; // 分支对话引用
    public UnityEvent onSelected;
    [Header("脚本化事件")]
    public EventEffect[] scriptableEffects;
    public int nextDialogueIndex; // 分支对话索引
    public int EndingDialogueIndex; // 结束对话索引
    [Header("显示条件")]
    public string requiredItem; // 需要持有道具
    public int minLevel = 1;    // 最低等级
    [Header("任务触发")]
    public TaskSO taskToTrigger;    // 选择此选项时触发的任务
    public bool requireTaskCompletion; // 是否需要前置任务完成才能显示此选项
}

