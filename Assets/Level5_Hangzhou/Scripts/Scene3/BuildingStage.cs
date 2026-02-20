using UnityEngine;

/// <summary>
/// 廊桥建造阶段数据
/// 用于场景3的7个建造阶段配置
/// </summary>
[System.Serializable]
public class BuildingStage
{
    [Header("阶段信息")]
    [Tooltip("阶段编号 (0-6)")]
    public int stageNumber;
    
    [Tooltip("阶段名称（如：\"基础\"）")]
    public string stageName;
    
    [Tooltip("阶段标题（如：\"【阶段1：基础】\"）")]
    [TextArea(2, 3)]
    public string stageTitle;
    
    [Tooltip("阶段说明文字")]
    [TextArea(3, 6)]
    public string stageDescription;

    [Header("材料配置")]
    [Tooltip("材料图片")]
    public Sprite materialSprite;
    
    [Tooltip("材料尺寸（可选，如果为(0,0)则使用预制体默认值）")]
    public Vector2 materialSize = Vector2.zero;
    
    [Tooltip("材料初始位置（可选，如果为(0,0)则使用预制体默认值）")]
    public Vector2 materialInitPos = Vector2.zero;
    
    [Tooltip("吸附距离（像素）")]
    public float snapDistance = 150f;

    [Header("动画")]
    [Tooltip("动画时长（秒）")]
    public float animationDuration = 1.0f;

    [Header("完成提示")]
    [Tooltip("该阶段完成后显示的提示文字")]
    [TextArea(2, 3)]
    public string completionMessage = "小墨：这一层完成了！";
}
