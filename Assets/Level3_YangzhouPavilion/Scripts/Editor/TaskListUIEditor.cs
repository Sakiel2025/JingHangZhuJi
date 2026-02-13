using UnityEngine;
using UnityEditor;
using Level3.UI;

namespace Level3.Editor
{
    [CustomEditor(typeof(TaskListUI))]
    public class TaskListUIEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认的Inspector
            DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("任务列表工具", EditorStyles.boldLabel);
        
        TaskListUI taskListUI = (TaskListUI)target;

        // 批量调整按钮
        EditorGUILayout.HelpBox("调整Inspector中的参数后,点击此按钮应用到所有任务项", MessageType.Info);
        if (GUILayout.Button("应用参数到所有任务项", GUILayout.Height(35)))
        {
            taskListUI.SendMessage("AdjustExistingTaskItems");
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space(5);

        // 还原按钮
        if (GUILayout.Button("还原任务列表 (4个任务)", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("还原任务列表", 
                "这将清除现有任务项并重新生成4个默认任务。\n\n确定要继续吗?", 
                "确定", "取消"))
            {
                taskListUI.SendMessage("RestoreTaskList");
                EditorUtility.SetDirty(target);
            }
        }

        // 清理按钮
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUILayout.Button("清空所有任务", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("清空任务列表", 
                "这将删除所有任务项。\n\n确定要继续吗?", 
                "确定", "取消"))
            {
                taskListUI.SendMessage("ClearTaskList");
                EditorUtility.SetDirty(target);
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("使用方法:\n" +
            "1. 在Inspector中调整参数\n" +
            "2. 点击'应用参数到所有任务项'\n" +
            "3. 保存场景(Ctrl+S)", MessageType.Info);
        }
    }
}
