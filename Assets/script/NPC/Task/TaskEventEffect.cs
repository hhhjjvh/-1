using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


[CreateAssetMenu(menuName = "Event/Effects/TaskEvent")]
public class TaskEventEffect : EventEffect
{
    [Header("任务事件类型")]
    public TaskEventType eventType = TaskEventType.OnAccepted;

    [Header("效果配置")]
    public GameObject spawnPrefab;   // 生成对象（如奖励道具）
    public AudioClip soundEffect;    // 播放音效
    public string uiMessage;         // 显示的UI文本

    public enum TaskEventType
    {
        OnAccepted,  // 接取任务时触发
        OnCompleted  // 完成任务时触发
    }

    public override void ApplyEffect(GameObject target)
    {
        // 实现具体效果
        if (soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(soundEffect, target.transform.position);
        }

        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, target.transform.position, Quaternion.identity);
        }

        //if (!string.IsNullOrEmpty(uiMessage))
        //{
        //    UIManager.Instance.ShowMessage(uiMessage);
        //}
    }
}
