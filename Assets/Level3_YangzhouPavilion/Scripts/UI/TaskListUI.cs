using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Level3.UI
{
    [System.Serializable]
    public class TaskItem
    {
        public string taskName;
        public bool isCompleted;
        public TextMeshProUGUI textComponent;
    }

    public class TaskListUI : MonoBehaviour
    {
    [Header("任务列表")]
    [SerializeField] private List<TaskItem> tasks = new List<TaskItem>();

    [Header("UI引用")]
    [SerializeField] private Transform taskListContainer;
    [SerializeField] private GameObject taskItemPrefab;

    [Header("文字样式设置")]
    [SerializeField] private float fontSize = 24f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private TextAlignmentOptions textAlignment = TextAlignmentOptions.Left;
    
    [Header("布局参数")]
    [SerializeField] private float itemHeight = 40f;
    [SerializeField] private Vector4 textMargin = new Vector4(10, 5, 10, 5); // 左,上,右,下
    [SerializeField] private Vector2 anchorMin = new Vector2(0, 1);
    [SerializeField] private Vector2 anchorMax = new Vector2(1, 1);
    [SerializeField] private Vector2 pivot = new Vector2(0, 1);

    [Header("运行时选项")]
    [SerializeField] private bool useExistingTaskItems = true; // 使用Scene中已有的任务项
    
    // 预览模式备份
    [SerializeField] private List<TaskItemBackup> previewBackup = new List<TaskItemBackup>();
    [SerializeField] private bool isInPreviewMode = false;

    [System.Serializable]
    private class TaskItemBackup
    {
        public GameObject gameObject;
        public bool wasActive;
    }

    void Start()
    {
        // Debug输出当前参数值
        Debug.Log($"[TaskListUI] 参数检查 - Font Size: {fontSize}, Item Height: {itemHeight}, Text Margin: {textMargin}");
        
        if (useExistingTaskItems)
        {
            // 检查TaskListContainer是否有子对象
            if (taskListContainer != null && taskListContainer.childCount > 0)
            {
                // 使用Scene中已有的任务项
                InitializeExistingTasks();
            }
            else
            {
                // TaskListContainer为空,自动生成任务
                Debug.LogWarning("[TaskListUI] TaskListContainer为空,自动生成任务列表");
                InitializeTasks();
            }
        }
        else
        {
            // 动态生成新的任务项
            InitializeTasks();
        }
    }

    // 初始化Scene中已有的任务项(不重新创建)
    void InitializeExistingTasks()
    {
        if (taskListContainer == null)
        {
            Debug.LogError("TaskListContainer 未设置!");
            return;
        }

        tasks.Clear();

        // 遍历现有的子对象,添加到任务列表
        foreach (Transform child in taskListContainer)
        {
            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                TaskItem taskItem = new TaskItem
                {
                    taskName = child.name,
                    isCompleted = false,
                    textComponent = textComponent
                };
                tasks.Add(taskItem);
            }
        }

        Debug.Log($"[TaskListUI] 已初始化 {tasks.Count} 个现有任务项");
    }

    // 在Editor模式下预览任务列表(右键菜单)
    [ContextMenu("预览任务列表")]
    void PreviewTaskList()
    {
        // 备份现有任务项
        BackupExistingTasks();
        
        // 生成预览
        InitializeTasks();
        isInPreviewMode = true;
        Debug.Log("已进入预览模式。使用'退出预览'可以恢复原有任务项。");
    }

    // 应用预览到场景(保存预览的任务)(右键菜单)
    [ContextMenu("应用预览到场景")]
    void ApplyPreview()
    {
        if (!isInPreviewMode)
        {
            Debug.LogWarning("当前不在预览模式中");
            return;
        }

        // 删除备份(不再恢复)
        foreach (var backup in previewBackup)
        {
            if (backup.gameObject != null)
            {
                DestroyImmediate(backup.gameObject);
            }
        }
        previewBackup.Clear();
        
        isInPreviewMode = false;
        Debug.Log("预览已应用到场景,任务项已保存");
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    // 检查当前状态
    [ContextMenu("检查当前状态")]
    void CheckCurrentState()
    {
        int taskCount = taskListContainer != null ? taskListContainer.childCount : 0;
        Debug.Log($"=== TaskListUI 状态检查 ===\n" +
                  $"预览模式: {(isInPreviewMode ? "是" : "否")}\n" +
                  $"备份数量: {previewBackup.Count}\n" +
                  $"TaskListContainer子对象数: {taskCount}\n" +
                  $"tasks列表数: {tasks.Count}\n" +
                  $"========================");
    }

    // 重置所有状态(清除标志但不删除任务)
    [ContextMenu("重置状态(保留任务)")]
    void ResetState()
    {
        previewBackup.Clear();
        isInPreviewMode = false;
        Debug.Log("状态已重置,任务项已保留");
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    // 强制退出预览模式(无需检查状态)
    [ContextMenu("强制退出预览")]
    void ForceExitPreview()
    {
        // 清理所有子对象
        if (taskListContainer != null)
        {
            int childCount = taskListContainer.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(taskListContainer.GetChild(i).gameObject);
            }
        }

        // 清空备份和状态
        previewBackup.Clear();
        isInPreviewMode = false;
        tasks.Clear();
        
        Debug.Log("已强制退出预览模式,所有任务已清空");
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    // 退出预览模式,恢复原有任务项(右键菜单)
    [ContextMenu("退出预览(放弃修改)")]
    void ExitPreview()
    {
        // 检查是否有备份可恢复
        if (previewBackup.Count == 0)
        {
            Debug.LogWarning("没有可恢复的备份。如需清空任务,请使用'强制退出预览'或'清理任务列表'");
            return;
        }

        // 删除预览生成的任务项
        if (taskListContainer != null)
        {
            int childCount = taskListContainer.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                // 只删除不在备份列表中的对象
                Transform child = taskListContainer.GetChild(i);
                bool isBackup = previewBackup.Exists(b => b.gameObject == child.gameObject);
                if (!isBackup && child.gameObject.activeSelf)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        // 恢复备份的任务项
        RestoreBackup();
        isInPreviewMode = false;
        Debug.Log("已退出预览模式,恢复原有任务项");
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    // 备份现有任务项
    void BackupExistingTasks()
    {
        previewBackup.Clear();
        
        if (taskListContainer != null)
        {
            foreach (Transform child in taskListContainer)
            {
                TaskItemBackup backup = new TaskItemBackup
                {
                    gameObject = child.gameObject,
                    wasActive = child.gameObject.activeSelf
                };
                previewBackup.Add(backup);
                
                // 隐藏原有任务项
                child.gameObject.SetActive(false);
            }
        }
        
        Debug.Log($"已备份 {previewBackup.Count} 个任务项");
    }

    // 恢复备份的任务项
    void RestoreBackup()
    {
        foreach (var backup in previewBackup)
        {
            if (backup.gameObject != null)
            {
                backup.gameObject.SetActive(backup.wasActive);
            }
        }
        
        previewBackup.Clear();
        
        // 重新初始化现有任务
        if (useExistingTaskItems)
        {
            InitializeExistingTasks();
        }
    }

    // 调整现有任务项的布局参数(右键菜单)
    [ContextMenu("批量调整现有任务项")]
    void AdjustExistingTaskItems()
    {
        if (taskListContainer == null)
        {
            Debug.LogError("TaskListContainer 未设置!");
            return;
        }

        int adjustedCount = 0;
        foreach (Transform child in taskListContainer)
        {
            // 调整RectTransform
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, itemHeight);
            }

            // 调整TextMeshProUGUI
            TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.fontSize = fontSize;
                text.color = textColor;
                text.alignment = textAlignment;
                text.margin = textMargin;
                adjustedCount++;
            }
        }

        Debug.Log($"已调整 {adjustedCount} 个任务项的参数");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    // 还原任务列表(右键菜单) - 重新生成4个任务
    [ContextMenu("还原任务列表(4个任务)")]
    void RestoreTaskList()
    {
        ClearTaskList();
        InitializeTasks();
        Debug.Log("任务列表已还原为4个任务");
    }

    // 清理任务列表(右键菜单)
    [ContextMenu("清理任务列表")]
    void ClearTaskList()
    {
        foreach (var task in tasks)
        {
            if (task.textComponent != null)
            {
                DestroyImmediate(task.textComponent.gameObject);
            }
        }
        tasks.Clear();

        if (taskListContainer != null)
        {
            int childCount = taskListContainer.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(taskListContainer.GetChild(i).gameObject);
            }
        }
    }

    void InitializeTasks()
    {
        // 清理现有任务项(避免重复创建)
        foreach (var task in tasks)
        {
            if (task.textComponent != null)
            {
                Destroy(task.textComponent.gameObject);
            }
        }
        tasks.Clear();

        // 清理容器中的所有子对象
        if (taskListContainer != null)
        {
            foreach (Transform child in taskListContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // 第三关任务清单(简化版 - 4个任务)
        string[] taskNames = {
            "1. 使用匠人之眼扫描亭子",
            "2. 安装千斤顶支撑结构",
            "3. 更换腐朽木桩",
            "4. 重铺石质台基"
        };

        foreach (var taskName in taskNames)
        {
            CreateTaskItem(taskName);
        }
    }

    void CreateTaskItem(string taskName)
    {
        // 动态创建任务文本
        GameObject taskObj = new GameObject(taskName);
        taskObj.transform.SetParent(taskListContainer, false);

        // 添加并配置RectTransform (使用Inspector中的参数)
        RectTransform rectTransform = taskObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.sizeDelta = new Vector2(0, itemHeight);
        
        var text = taskObj.AddComponent<TextMeshProUGUI>();
        text.text = $"[ ] {taskName}";
        text.fontSize = fontSize;
        text.color = textColor;
        text.alignment = textAlignment;
        text.margin = textMargin;
        
        // 禁用下划线和其他特殊样式（避免警告）
        text.fontStyle = FontStyles.Normal;
        text.enableAutoSizing = false;

        TaskItem newTask = new TaskItem
        {
            taskName = taskName,
            isCompleted = false,
            textComponent = text
        };

        tasks.Add(newTask);
    }

        public void CompleteTask(int taskIndex)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                tasks[taskIndex].isCompleted = true;
                tasks[taskIndex].textComponent.text = 
                    $"<color=#4CAF50>[√] {tasks[taskIndex].taskName}</color>";  // 用 [√] 替代 ☑
                
                Debug.Log($"任务完成: {tasks[taskIndex].taskName}");
            }
        }

        public bool AreAllTasksCompleted()
        {
            return tasks.TrueForAll(task => task.isCompleted);
        }
    }
}
