using UnityEngine;

namespace Level4.Suzhou
{
    /// <summary>
    /// 装饰风格配置：包含一套完整的装饰物
    /// </summary>
    [System.Serializable]
    public class DecorationSet
    {
        [Header("=== 风格信息 ===")]
        public string styleID;           // 风格ID（如 "Style_Palace"）
        public string styleName;         // 风格名称（如 "宫廷风"）
        
        [Header("=== 装饰物图片 ===")]
        public Sprite leftHangingSprite;    // 左侧悬挂物（如宫灯）
        public Sprite rightHangingSprite;   // 右侧悬挂物（如宫灯）
        public Sprite groundSprite;         // 地面装饰（如石狮）
        
        [Header("=== 位置偏移（相对屋顶区域）===")]
        public Vector2 leftOffset = new Vector2(-200f, -50f);    // 左侧悬挂位置
        public Vector2 rightOffset = new Vector2(200f, -50f);    // 右侧悬挂位置
        public Vector2 groundOffset = new Vector2(0f, -470f);    // 地面位置（相对屋顶）
        
        [Header("=== 尺寸 ===")]
        public Vector2 hangingSize = new Vector2(120f, 120f);    // 悬挂物尺寸
        public Vector2 groundSize = new Vector2(150f, 100f);     // 地面装饰尺寸
    }
}
