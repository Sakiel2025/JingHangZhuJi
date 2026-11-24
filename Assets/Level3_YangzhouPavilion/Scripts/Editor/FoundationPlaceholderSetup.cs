using UnityEngine;
using UnityEditor;

namespace JingHangZhuJi.Level3.Editor
{
    /// <summary>
    /// 台基占位符自动配置工具
    /// 用于在没有美术资源时,快速设置临时的彩色占位符
    /// </summary>
    public class FoundationPlaceholderSetup : EditorWindow
    {
        [MenuItem("Level3/Setup Foundation Placeholders")]
        static void SetupPlaceholders()
        {
            // 查找 FoundationStateManager
            FoundationStateManager manager = FindObjectOfType<FoundationStateManager>();
            
            if (manager == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中未找到 FoundationStateManager！", "确定");
                return;
            }

            // 获取所有子对象
            Transform parent = manager.transform;
            
            // 查找或创建子对象
            GameObject normalObj = FindOrCreateChild(parent, "foundation_normal");
            GameObject tiltedObj = FindOrCreateChild(parent, "foundation_tilted");
            GameObject openedObj = FindOrCreateChild(parent, "foundation_opened");
            GameObject repairedObj = FindOrCreateChild(parent, "foundation_repaired");

            // 配置每个对象的 SpriteRenderer
            SetupSpriteRenderer(normalObj, Color.white, "正常");
            SetupSpriteRenderer(tiltedObj, Color.yellow, "倾斜");
            SetupSpriteRenderer(openedObj, Color.cyan, "施工");
            SetupSpriteRenderer(repairedObj, Color.green, "完成");

            EditorUtility.DisplayDialog("完成", 
                "台基占位符配置完成！\n\n" +
                "白色 = 正常\n" +
                "黄色 = 倾斜\n" +
                "青色 = 施工\n" +
                "绿色 = 完成\n\n" +
                "运行游戏或按 1/2/3/4 键测试！", 
                "确定");
        }

        static GameObject FindOrCreateChild(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            
            if (child == null)
            {
                GameObject obj = new GameObject(name);
                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector3(5, 2, 1); // 默认缩放
                return obj;
            }
            
            return child.gameObject;
        }

        static void SetupSpriteRenderer(GameObject obj, Color color, string stateName)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            
            if (sr == null)
            {
                sr = obj.AddComponent<SpriteRenderer>();
            }

            // 使用 Unity 内置的白色方块 Sprite
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sr.color = color;
            sr.sortingOrder = 0;

            Debug.Log($"[占位符配置] {obj.name} 设置为 {stateName} 状态 ({color})");
        }
    }
}
