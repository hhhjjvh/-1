using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    public GameObject taskPanel;
    public TextMeshProUGUI taskDescriptionText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            taskPanel.SetActive(!taskPanel.activeSelf);
            UpdateTaskUI();
        }
    }

    private void UpdateTaskUI()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var task in TaskManager.Instance.activeTasks)
        {
            sb.AppendLine($"- {task.taskName}: {task.description} ({task.status})");
        }
        taskDescriptionText.text = sb.ToString();
    }
}
