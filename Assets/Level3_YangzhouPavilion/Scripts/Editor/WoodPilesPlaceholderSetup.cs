using UnityEngine;
using UnityEditor;

namespace JingHangZhuJi.Level3.Editor
{
    /// <summary>
    /// 木桩占位符自动配置工具
    /// 用于在没有美术资源时,快速设置临时的彩色占位符
    /// </summary>
    public class WoodPilesPlaceholderSetup : EditorWindow
    {
        [MenuItem("Level3/Setup WoodPiles Placeholders")]
        static void SetupPlaceholders()
        {
            // 查找 WoodPilesManager
            WoodPilesManager manager = FindObjectOfType<WoodPilesManager>();
            
            if (manager == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中未找到 WoodPilesManager！", "确定");
                return;
            }

            // 获取所有子对象
            Transform parent = manager.transform;
            
            // 查找或创建子对象
            GameObject normalObj = FindOrCreateChild(parent, "WoodPiles_Normal");
            GameObject rottenObj = FindOrCreateChild(parent, "WoodPiles_Rotten");
            GameObject measuringObj = FindOrCreateChild(parent, "WoodPiles_Measuring");
            GameObject qualifiedObj = FindOrCreateChild(parent, "WoodPiles_Qualified");

            // 配置每个对象的 SpriteRenderer (木桩在台基下方,Y偏移-1)
            SetupSpriteRenderer(normalObj, new Color(0.8f, 0.6f, 0.4f), "正常", -1f);      // 棕色
            SetupSpriteRenderer(rottenObj, new Color(0.3f, 0.2f, 0.1f), "腐朽", -1f);      // 深棕
            SetupSpriteRenderer(measuringObj, Color.magenta, "测量中", -1f);                // 品红
            SetupSpriteRenderer(qualifiedObj, Color.green, "合格", -1f);                    // 绿色

            EditorUtility.DisplayDialog("完成", 
                "木桩占位符配置完成！\n\n" +
                "浅棕色 = 正常\n" +
                "深棕色 = 腐朽\n" +
                "品红色 = 测量中\n" +
                "绿色 = 合格\n\n" +
                "运行游戏或按 Q/W/E/R 键测试！", 
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
                obj.transform.localScale = new Vector3(4, 1.5f, 1); // 木桩比台基小一点
                return obj;
            }
            
            return child.gameObject;
        }

        static void SetupSpriteRenderer(GameObject obj, Color color, string stateName, float yOffset)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            
            if (sr == null)
            {
                sr = obj.AddComponent<SpriteRenderer>();
            }

            // 使用 Unity 内置的白色方块 Sprite
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sr.color = color;
            sr.sortingOrder = 1; // 木桩在台基上方

            // 调整位置(木桩在台基下方一点)
            obj.transform.localPosition = new Vector3(0, yOffset, 0);

            Debug.Log($"[占位符配置] {obj.name} 设置为 {stateName} 状态 ({color})");
        }
    }
}
