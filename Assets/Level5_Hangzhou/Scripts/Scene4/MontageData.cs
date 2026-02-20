using UnityEngine;

/// <summary>
/// 蒙太奇数据类
/// 用于场景4的蒙太奇播放系统
/// </summary>
[System.Serializable]
public class MontageData
{
    [Header("蒙太奇图片")]
    [Tooltip("蒙太奇图片Sprite")]
    public Sprite montageSprite;      // 蒙太奇图片
    
    [Header("字幕")]
    [Tooltip("是否显示字幕")]
    public bool hasSubtitle;          // 是否有字幕
    
    [Tooltip("字幕文字内容")]
    [TextArea(2, 4)]
    public string subtitleText;       // 字幕文字
    
    [Header("时长")]
    [Tooltip("图片显示时长（秒）")]
    public float displayDuration = 2.5f;     // 显示时长（秒）
    
    [Tooltip("淡入淡出时长（秒）")]
    public float fadeDuration = 0.8f;        // 淡入淡出时长（秒）
}
