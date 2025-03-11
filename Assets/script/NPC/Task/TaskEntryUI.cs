using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    using TMPro;
using NUnit.Framework.Interfaces;
using System.Drawing;

public class TaskEntryUI : MonoBehaviour
{
    public TMP_Text taskNameText;
    public TMP_Text descriptionText;
    public TMP_Text progressText;
   // public TMP_Text rewardsText; // 新增奖励文本
    private TaskSO boundTask;
    public void BindData(TaskSO task)
    {
        // 新增奖励显示
        string rewards = GetRewardsDescription(task);
        taskNameText.text = task.taskName;
        descriptionText.text = $"{task.description}\n<size=30><b>奖励: {rewards}</b></size>";
      
        // 更新条件进度
        string progress = "";
        foreach (var condition in task.conditions)
        {
            progress += $"{condition.conditionDescription}: {condition.currentAmount}/{condition.requiredAmount}\n";
        }
        progressText.text = progress;

        boundTask = task;
       // UpdateUI();
        foreach (var condition in task.conditions)
        {
            condition.OnProgressUpdated += UpdateUI;
        }
        
        
    }
    private string GetRewardsDescription(TaskSO task)
    {
        List<string> rewardDescriptions = new List<string>();
        foreach (var effect in task.onCompletedEffects)
        {
            if (effect is EventEffect eventEffect)
            {
                rewardDescriptions.Add(eventEffect.GetEffectDescription());
            }
        }
        return rewardDescriptions.Count > 0 ? string.Join("     ", rewardDescriptions) : "无";
    }

    private void OnDestroy()
    {
        if (boundTask != null)
        {
            foreach (var condition in boundTask.conditions)
            {
                condition.OnProgressUpdated -= UpdateUI;
            }
        }
    }
    private float lastUpdateTime;
    private void UpdateUI()
    {
        if (Time.time - lastUpdateTime < 0.5f) return; // 每 0.5 秒最多刷新一次
        lastUpdateTime = Time.time;
        // 重新绑定数据
        BindData(boundTask);
    }
}
