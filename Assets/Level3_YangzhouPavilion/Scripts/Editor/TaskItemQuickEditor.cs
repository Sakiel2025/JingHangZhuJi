using UnityEngine;
using UnityEditor;
using TMPro;

namespace Level3.Editor
{
    /// <summary>
    /// 为TaskListContainer下的每个任务项添加快速调整工具
    /// </summary>
    public class TaskItemQuickEditor : EditorWindow
    {
        private GameObject taskListContainer;
        private Vector2 scrollPosition;
        
        [MenuItem("Tools/Level3/任务项快速调整工具")]
        public static void ShowWindow()
        {
            var window = GetWindow<TaskItemQuickEditor>("任务项调整");
            window.minSize = new Vector2(400, 500);
        }

        void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("任务项快速调整工具", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("选择TaskListContainer,然后单独调整每个任务项的参数", MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            // 选择TaskListContainer
            taskListContainer = (GameObject)EditorGUILayout.ObjectField(
                "TaskListContainer", 
                taskListContainer, 
                typeof(GameObject), 
                true
            );

            if (taskListContainer == null)
            {
                EditorGUILayout.HelpBox("请拖入TaskListContainer对象", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("任务项列表:", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // 遍历所有子对象
            for (int i = 0; i < taskListContainer.transform.childCount; i++)
            {
                Transform child = taskListContainer.transform.GetChild(i);
                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                RectTransform rectTransform = child.GetComponent<RectTransform>();

                if (text == null) continue;

                // 绘制分隔线
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginVertical("box");
                
                // 显示任务名称(可点击选中)
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(child.name, EditorStyles.label))
                {
                    Selection.activeGameObject = child.gameObject;
                    EditorGUIUtility.PingObject(child.gameObject);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                // === RectTransform 参数 ===
                if (rectTransform != null)
                {
                    EditorGUILayout.LabelField("RectTransform", EditorStyles.boldLabel);

                    // Position (Pos X, Y, Z)
                    Vector3 newPos = EditorGUILayout.Vector3Field("Position", rectTransform.anchoredPosition3D);
                    if (newPos != rectTransform.anchoredPosition3D)
                    {
                        Undo.RecordObject(rectTransform, "Change Position");
                        rectTransform.anchoredPosition3D = newPos;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    // Anchors (Min & Max)
                    Vector2 newAnchorMin = EditorGUILayout.Vector2Field("Anchor Min", rectTransform.anchorMin);
                    if (newAnchorMin != rectTransform.anchorMin)
                    {
                        Undo.RecordObject(rectTransform, "Change Anchor Min");
                        rectTransform.anchorMin = newAnchorMin;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    Vector2 newAnchorMax = EditorGUILayout.Vector2Field("Anchor Max", rectTransform.anchorMax);
                    if (newAnchorMax != rectTransform.anchorMax)
                    {
                        Undo.RecordObject(rectTransform, "Change Anchor Max");
                        rectTransform.anchorMax = newAnchorMax;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    // Pivot
                    Vector2 newPivot = EditorGUILayout.Vector2Field("Pivot", rectTransform.pivot);
                    if (newPivot != rectTransform.pivot)
                    {
                        Undo.RecordObject(rectTransform, "Change Pivot");
                        rectTransform.pivot = newPivot;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    // Size Delta (Width & Height)
                    Vector2 newSizeDelta = EditorGUILayout.Vector2Field("Size Delta", rectTransform.sizeDelta);
                    if (newSizeDelta != rectTransform.sizeDelta)
                    {
                        Undo.RecordObject(rectTransform, "Change Size Delta");
                        rectTransform.sizeDelta = newSizeDelta;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    // Rotation
                    Vector3 newRotation = EditorGUILayout.Vector3Field("Rotation", rectTransform.localEulerAngles);
                    if (newRotation != rectTransform.localEulerAngles)
                    {
                        Undo.RecordObject(rectTransform, "Change Rotation");
                        rectTransform.localEulerAngles = newRotation;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    // Scale
                    Vector3 newScale = EditorGUILayout.Vector3Field("Scale", rectTransform.localScale);
                    if (newScale != rectTransform.localScale)
                    {
                        Undo.RecordObject(rectTransform, "Change Scale");
                        rectTransform.localScale = newScale;
                        EditorUtility.SetDirty(rectTransform);
                    }

                    EditorGUILayout.Space(5);
                }

                // === TextMeshPro 参数 ===
                EditorGUILayout.LabelField("TextMeshPro", EditorStyles.boldLabel);

                // 字体大小
                float newFontSize = EditorGUILayout.FloatField("Font Size", text.fontSize);
                if (newFontSize != text.fontSize)
                {
                    Undo.RecordObject(text, "Change Font Size");
                    text.fontSize = newFontSize;
                    EditorUtility.SetDirty(text);
                }

                // 文字颜色
                Color newColor = EditorGUILayout.ColorField("Text Color", text.color);
                if (newColor != text.color)
                {
                    Undo.RecordObject(text, "Change Text Color");
                    text.color = newColor;
                    EditorUtility.SetDirty(text);
                }

                // 对齐方式
                TextAlignmentOptions newAlignment = (TextAlignmentOptions)EditorGUILayout.EnumPopup("Alignment", text.alignment);
                if (newAlignment != text.alignment)
                {
                    Undo.RecordObject(text, "Change Alignment");
                    text.alignment = newAlignment;
                    EditorUtility.SetDirty(text);
                }

                // 边距
                Vector4 newMargin = EditorGUILayout.Vector4Field("Margin (L,T,R,B)", text.margin);
                if (newMargin != text.margin)
                {
                    Undo.RecordObject(text, "Change Margin");
                    text.margin = newMargin;
                    EditorUtility.SetDirty(text);
                }

                EditorGUI.indentLevel--;
                
                // 快速定位按钮
                if (GUILayout.Button("在Scene中定位", GUILayout.Height(25)))
                {
                    Selection.activeGameObject = child.gameObject;
                    SceneView.FrameLastActiveSceneView();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            // 批量操作
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("批量操作:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("应用统一字体大小", GUILayout.Height(30)))
            {
                float uniformSize = EditorUtility.DisplayDialog("设置统一字体大小", 
                    "输入字体大小", "确定", "取消") 
                    ? 24f : 0f;
                
                if (uniformSize > 0)
                {
                    ApplyUniformFontSize(uniformSize);
                }
            }
        }

        void ApplyUniformFontSize(float size)
        {
            for (int i = 0; i < taskListContainer.transform.childCount; i++)
            {
                Transform child = taskListContainer.transform.GetChild(i);
                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    Undo.RecordObject(text, "Set Uniform Font Size");
                    text.fontSize = size;
                    EditorUtility.SetDirty(text);
                }
            }
        }
    }
}
