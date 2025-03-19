using System;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(menuName ="Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea(1, 30)]
    public string contentData;
    [TextArea(1, 30)]
    public string[] content;
    public Choice[] choices;
    [Header("触发符号")]
    [Tooltip("在对话内容中插入此符号时触发选项")]
    public string triggerSymbol = "@trigger";
    public int triggerLine = 0; // 触发行号
#if UNITY_EDITOR
    void OnValidate()
    {
        //"/n"
        content = contentData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(line => line.Trim()).ToArray();
        triggerLine = -1;
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i].Contains(triggerSymbol))
            {
                triggerLine = i; // 记录触发行
                break;
            }
        }
    }
    #endif
}
