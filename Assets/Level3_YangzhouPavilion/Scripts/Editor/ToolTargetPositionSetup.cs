using UnityEngine;
using UnityEditor;

namespace JingHangZhuJi.Level3.Editor
{
    /// <summary>
    /// 自动创建和配置工具目标位置
    /// </summary>
    public class ToolTargetPositionSetup : EditorWindow
    {
        [MenuItem("Level3/创建工具目标位置")]
        public static void ShowWindow()
        {
            GetWindow<ToolTargetPositionSetup>("工具目标位置配置");
        }

        private Transform foundationTransform;
        private Transform woodPilesTransform;
        private float jackYOffset = -0.5f; // 千斤顶在台基下方0.5米
        private float rulerYOffset = 0f;   // 鲁班尺在木桩中心

        void OnGUI()
        {
            GUILayout.Label("工具目标位置自动配置", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "此工具会自动创建千斤顶和鲁班尺的目标位置对象，" +
                "并根据台基和木桩的位置自动计算正确坐标。",
                MessageType.Info
            );

            EditorGUILayout.Space();

            // 输入参考对象
            GUILayout.Label("参考对象（用于计算位置）:", EditorStyles.boldLabel);
            foundationTransform = (Transform)EditorGUILayout.ObjectField(
                "台基对象 (Foundation)", 
                foundationTransform, 
                typeof(Transform), 
                true
            );

            woodPilesTransform = (Transform)EditorGUILayout.ObjectField(
                "木桩对象 (WoodPiles)", 
                woodPilesTransform, 
                typeof(Transform), 
                true
            );

            EditorGUILayout.Space();

            // 偏移设置
            GUILayout.Label("偏移设置:", EditorStyles.boldLabel);
            jackYOffset = EditorGUILayout.FloatField("千斤顶 Y 偏移", jackYOffset);
            EditorGUILayout.LabelField("", "(相对台基底部，负值表示下方)", EditorStyles.miniLabel);

            rulerYOffset = EditorGUILayout.FloatField("鲁班尺 Y 偏移", rulerYOffset);
            EditorGUILayout.LabelField("", "(相对木桩中心)", EditorStyles.miniLabel);

            EditorGUILayout.Space();

            // 如果未找到对象，尝试自动搜索
            if (foundationTransform == null || woodPilesTransform == null)
            {
                EditorGUILayout.HelpBox("未指定参考对象，将尝试自动搜索场景中的对象", MessageType.Warning);
                if (GUILayout.Button("自动搜索场景对象", GUILayout.Height(30)))
                {
                    AutoFindObjects();
                }
            }

            EditorGUILayout.Space();

            // 创建按钮
            GUI.enabled = foundationTransform != null && woodPilesTransform != null;
            if (GUILayout.Button("创建目标位置", GUILayout.Height(40)))
            {
                CreateTargetPositions();
            }
            GUI.enabled = true;

            EditorGUILayout.Space();

            // 手动创建选项
            EditorGUILayout.HelpBox("或者手动创建（使用固定坐标）:", MessageType.Info);
            if (GUILayout.Button("手动创建（使用默认坐标）", GUILayout.Height(30)))
            {
                CreateTargetPositionsManual();
            }
        }

        private void AutoFindObjects()
        {
            // 搜索 Foundation
            GameObject foundation = GameObject.Find("Foundation");
            if (foundation != null)
            {
                foundationTransform = foundation.transform;
                Debug.Log("✅ 找到 Foundation 对象");
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到 Foundation 对象");
            }

            // 搜索 WoodPiles
            GameObject woodPiles = GameObject.Find("WoodPiles");
            if (woodPiles != null)
            {
                woodPilesTransform = woodPiles.transform;
                Debug.Log("✅ 找到 WoodPiles 对象");
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到 WoodPiles 对象");
            }
        }

        private void CreateTargetPositions()
        {
            // 计算千斤顶目标位置（台基下方）
            Vector3 jackPosition = foundationTransform.position;
            jackPosition.y += jackYOffset;

            // 计算鲁班尺目标位置（木桩中心）
            Vector3 rulerPosition = woodPilesTransform.position;
            rulerPosition.y += rulerYOffset;

            // 创建或更新千斤顶目标位置
            GameObject jackTarget = GameObject.Find("JackTargetPosition");
            if (jackTarget == null)
            {
                jackTarget = new GameObject("JackTargetPosition");
                Debug.Log("✅ 创建 JackTargetPosition");
            }
            else
            {
                Debug.Log("📝 更新 JackTargetPosition 位置");
            }
            jackTarget.transform.position = jackPosition;

            // 创建或更新鲁班尺目标位置
            GameObject rulerTarget = GameObject.Find("RulerTargetPosition");
            if (rulerTarget == null)
            {
                rulerTarget = new GameObject("RulerTargetPosition");
                Debug.Log("✅ 创建 RulerTargetPosition");
            }
            else
            {
                Debug.Log("📝 更新 RulerTargetPosition 位置");
            }
            rulerTarget.transform.position = rulerPosition;

            // 创建可视化区域
            CreateVisualArea(jackTarget, "千斤顶目标区域", Color.yellow);
            CreateVisualArea(rulerTarget, "鲁班尺目标区域", Color.cyan);

            Debug.Log($"✅ 目标位置配置完成！");
            Debug.Log($"千斤顶目标: {jackPosition}");
            Debug.Log($"鲁班尺目标: {rulerPosition}");

            EditorUtility.DisplayDialog(
                "配置完成",
                $"目标位置已创建/更新：\n\n" +
                $"千斤顶目标: {jackPosition}\n" +
                $"鲁班尺目标: {rulerPosition}\n\n" +
                $"请在 Inspector 中将这些对象拖入工具的 Target Position 字段！",
                "确定"
            );
        }

        private void CreateTargetPositionsManual()
        {
            // 使用默认坐标（根据设计文档）
            Vector3 jackPosition = new Vector3(0, -2.5f, 0);  // 台基下方
            Vector3 rulerPosition = new Vector3(0, -2.5f, 0);  // 木桩位置（与台基同高度）

            // 创建目标位置
            GameObject jackTarget = GameObject.Find("JackTargetPosition");
            if (jackTarget == null)
            {
                jackTarget = new GameObject("JackTargetPosition");
            }
            jackTarget.transform.position = jackPosition;

            GameObject rulerTarget = GameObject.Find("RulerTargetPosition");
            if (rulerTarget == null)
            {
                rulerTarget = new GameObject("RulerTargetPosition");
            }
            rulerTarget.transform.position = rulerPosition;

            // 创建可视化区域
            CreateVisualArea(jackTarget, "千斤顶目标区域", Color.yellow);
            CreateVisualArea(rulerTarget, "鲁班尺目标区域", Color.cyan);

            Debug.Log($"✅ 使用默认坐标创建目标位置");
            Debug.Log($"千斤顶目标: {jackPosition}");
            Debug.Log($"鲁班尺目标: {rulerPosition}");

            EditorUtility.DisplayDialog(
                "配置完成",
                "已使用默认坐标创建目标位置！\n\n" +
                "如果位置不对，请手动调整或使用参考对象重新创建。",
                "确定"
            );
        }

        private void CreateVisualArea(GameObject parent, string name, Color color)
        {
            // 检查是否已有可视化区域
            Transform existing = parent.transform.Find("TargetAreaVisual");
            if (existing != null)
            {
                Debug.Log($"📝 可视化区域已存在: {name}");
                return;
            }

            // 创建可视化子对象
            GameObject visual = new GameObject("TargetAreaVisual");
            visual.transform.SetParent(parent.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

            // 添加 SpriteRenderer
            SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
            
            // 尝试找到 Unity 内置的 Knob sprite
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            
            // 设置颜色（半透明）
            color.a = 0.3f;
            sr.color = color;
            
            // 设置排序层级（在工具下方）
            sr.sortingOrder = -1;

            Debug.Log($"✅ 创建可视化区域: {name}");
        }
    }
}
