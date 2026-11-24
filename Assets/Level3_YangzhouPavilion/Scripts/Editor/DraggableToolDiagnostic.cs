using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace JingHangZhuJi.Level3.Editor
{
    /// <summary>
    /// 拖动工具诊断器 - 检查拖动功能所需的所有配置
    /// </summary>
    public class DraggableToolDiagnostic : EditorWindow
    {
        [MenuItem("Level3/诊断拖动工具配置")]
        public static void ShowWindow()
        {
            GetWindow<DraggableToolDiagnostic>("拖动工具诊断");
        }

        private Vector2 scrollPos;

        void OnGUI()
        {
            GUILayout.Label("拖动工具配置诊断", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // 1. 检查 EventSystem
            CheckEventSystem();
            EditorGUILayout.Space();

            // 2. 检查选中的对象
            if (Selection.activeGameObject != null)
            {
                CheckSelectedObject(Selection.activeGameObject);
            }
            else
            {
                EditorGUILayout.HelpBox("请在 Hierarchy 中选择千斤顶或鲁班尺对象", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            if (GUILayout.Button("查找所有 DraggableTool 对象", GUILayout.Height(30)))
            {
                FindAllDraggableTools();
            }
        }

        private void CheckEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            
            if (eventSystem == null)
            {
                EditorGUILayout.HelpBox("❌ 场景中缺少 EventSystem！", MessageType.Error);
                if (GUILayout.Button("自动添加 EventSystem"))
                {
                    GameObject esObj = new GameObject("EventSystem");
                    esObj.AddComponent<EventSystem>();
                    esObj.AddComponent<StandaloneInputModule>();
                    Debug.Log("✅ 已添加 EventSystem");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✅ EventSystem 已存在", MessageType.Info);
                
                // 检查 InputModule
                if (eventSystem.GetComponent<StandaloneInputModule>() == null)
                {
                    EditorGUILayout.HelpBox("⚠️ 缺少 StandaloneInputModule", MessageType.Warning);
                    if (GUILayout.Button("添加 StandaloneInputModule"))
                    {
                        eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                        Debug.Log("✅ 已添加 StandaloneInputModule");
                    }
                }
            }
        }

        private void CheckSelectedObject(GameObject obj)
        {
            EditorGUILayout.LabelField("检查对象: " + obj.name, EditorStyles.boldLabel);
            
            // 检查 DraggableTool 组件
            DraggableTool draggable = obj.GetComponent<DraggableTool>();
            if (draggable == null)
            {
                EditorGUILayout.HelpBox("❌ 缺少 DraggableTool 组件", MessageType.Error);
                if (GUILayout.Button("添加 DraggableTool 组件"))
                {
                    obj.AddComponent<DraggableTool>();
                    Debug.Log($"✅ 已为 {obj.name} 添加 DraggableTool 组件");
                }
                return;
            }
            else
            {
                EditorGUILayout.HelpBox("✅ DraggableTool 组件已存在", MessageType.Info);
            }

            // 检查 Collider2D
            Collider2D collider = obj.GetComponent<Collider2D>();
            if (collider == null)
            {
                EditorGUILayout.HelpBox("❌ 缺少 Collider2D 组件（拖动必需！）", MessageType.Error);
                if (GUILayout.Button("添加 Box Collider 2D"))
                {
                    obj.AddComponent<BoxCollider2D>();
                    Debug.Log($"✅ 已为 {obj.name} 添加 BoxCollider2D");
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"✅ Collider2D 已存在: {collider.GetType().Name}", MessageType.Info);
                
                // 检查 Collider 是否禁用
                if (!collider.enabled)
                {
                    EditorGUILayout.HelpBox("⚠️ Collider2D 已禁用！", MessageType.Warning);
                    if (GUILayout.Button("启用 Collider2D"))
                    {
                        collider.enabled = true;
                        Debug.Log($"✅ 已启用 {obj.name} 的 Collider2D");
                    }
                }
            }

            // 检查 SpriteRenderer
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                EditorGUILayout.HelpBox("⚠️ 缺少 SpriteRenderer（视觉反馈不可用）", MessageType.Warning);
            }
            else
            {
                if (sr.sprite == null)
                {
                    EditorGUILayout.HelpBox("⚠️ SpriteRenderer 没有 Sprite", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox($"✅ SpriteRenderer 正常: {sr.sprite.name}", MessageType.Info);
                }
            }

            // 检查 Canvas 父级（如果有）
            Canvas parentCanvas = obj.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                EditorGUILayout.HelpBox($"⚠️ 检测到父级 Canvas: {parentCanvas.gameObject.name}", MessageType.Warning);
                EditorGUILayout.LabelField("注意: DraggableTool 设计用于世界空间对象，不是 UI 元素！");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("配置摘要:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Has DraggableTool: {draggable != null}");
            EditorGUILayout.LabelField($"Has Collider2D: {collider != null}");
            EditorGUILayout.LabelField($"Has SpriteRenderer: {sr != null}");
            EditorGUILayout.LabelField($"In Canvas: {parentCanvas != null}");
        }

        private void FindAllDraggableTools()
        {
            DraggableTool[] tools = GameObject.FindObjectsOfType<DraggableTool>();
            
            if (tools.Length == 0)
            {
                Debug.LogWarning("场景中没有找到 DraggableTool 对象");
                return;
            }

            Debug.Log($"找到 {tools.Length} 个 DraggableTool 对象:");
            foreach (var tool in tools)
            {
                bool hasCollider = tool.GetComponent<Collider2D>() != null;
                bool hasSpriteRenderer = tool.GetComponent<SpriteRenderer>() != null;
                
                string status = hasCollider ? "✅" : "❌";
                Debug.Log($"{status} {tool.gameObject.name} - Collider: {hasCollider}, SpriteRenderer: {hasSpriteRenderer}");
            }
        }
    }
}
