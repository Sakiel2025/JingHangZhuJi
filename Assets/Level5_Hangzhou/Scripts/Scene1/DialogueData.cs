using UnityEngine;
using System;

/// <summary>
/// 对话数据类
/// 用于存储单条对话的信息
/// </summary>
[Serializable]
public class DialogueData
{
    [Header("对话内容")]
    public string speaker;          // 说话人名字
    public string dialogue;         // 对话内容
    
    [Header("角色显示")]
    public CharacterPosition characterPosition; // 角色位置
    public CharacterExpression expression;      // 表情类型
    
    [Header("特殊效果（可选）")]
    public bool hasSpecialEffect;   // 是否有特殊效果
    public string specialEffectName; // 特殊效果名称（如"fade"、"shake"等）
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public DialogueData(string speaker, string dialogue, CharacterPosition position = CharacterPosition.None)
    {
        this.speaker = speaker;
        this.dialogue = dialogue;
        this.characterPosition = position;
        this.expression = CharacterExpression.Normal;
        this.hasSpecialEffect = false;
    }
}

/// <summary>
/// 角色位置枚举
/// </summary>
public enum CharacterPosition
{
    None,       // 不显示角色（旁白）
    Left,       // 左侧
    Right,      // 右侧
    Both        // 两侧都显示
}

/// <summary>
/// 角色表情枚举
/// </summary>
public enum CharacterExpression
{
    Normal,     // 正常
    Happy,      // 高兴
    Sad,        // 悲伤
    Angry,      // 生气
    Surprised,  // 惊讶
    Crying      // 哭泣
}

/// <summary>
/// 对话配置 - ScriptableObject版本
/// 可以在Unity Editor中创建对话资源文件
/// </summary>
[CreateAssetMenu(fileName = "DialogueConfig", menuName = "Level5/Dialogue Config", order = 1)]
public class DialogueConfig : ScriptableObject
{
    [Header("对话列表")]
    public DialogueData[] dialogues;
    
    [Header("场景信息")]
    public string sceneName;        // 场景名称
    public string sceneDescription; // 场景描述
    
    /// <summary>
    /// 获取对话数量
    /// </summary>
    public int GetDialogueCount()
    {
        return dialogues != null ? dialogues.Length : 0;
    }
    
    /// <summary>
    /// 获取指定索引的对话
    /// </summary>
    public DialogueData GetDialogue(int index)
    {
        if (dialogues != null && index >= 0 && index < dialogues.Length)
        {
            return dialogues[index];
        }
        return null;
    }
    
    /// <summary>
    /// 验证对话数据完整性
    /// </summary>
    public bool ValidateData()
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning($"对话配置 {name} 中没有对话数据！");
            return false;
        }
        
        for (int i = 0; i < dialogues.Length; i++)
        {
            if (string.IsNullOrEmpty(dialogues[i].dialogue))
            {
                Debug.LogWarning($"对话配置 {name} 的第 {i} 条对话内容为空！");
                return false;
            }
        }
        
        return true;
    }
}
