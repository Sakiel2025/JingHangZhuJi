using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// 师父第三封信场景管理器
/// 负责管理黄老爷对话和信件显示
/// </summary>
public class Level3_LetterManager : MonoBehaviour
{
    [Header("UI面板引用")]
    public GameObject dialoguePanel;      // 对话面板
    public GameObject letterPanel;        // 信件面板
    
    [Header("对话组件")]
    public Image characterImage;          // 左侧人物立绘
    public TextMeshProUGUI speakerName;   // 说话者名字
    public TextMeshProUGUI dialogueText;  // 对话文字
    public Button dialogueContinueButton; // 对话继续按钮
    
    [Header("信件组件")]
    public Image letterImage;             // 信纸背景图
    public TextMeshProUGUI letterText;    // 信件文字
    public Button letterContinueButton;   // 信件继续按钮
    
    [Header("剧情配置")]
    public DialogueLine[] dialogueLines;  // 对话内容数组
    [TextArea(10, 20)]
    public string letterContent;          // 师父信件完整内容
    
    [Header("场景切换")]
    public string nextSceneName = "Level3_end"; // 下一个场景名称
    
    private int currentDialogueIndex = 0;
    
    void Start()
    {
        // === 初始化检查 ===
        Debug.Log("=== Level3_LetterManager 初始化开始 ===");
        
        // 检查必需组件
        if (dialoguePanel == null) Debug.LogError("❌ dialoguePanel 未赋值！");
        if (letterPanel == null) Debug.LogError("❌ letterPanel 未赋值！");
        if (letterText == null) Debug.LogError("❌ letterText 未赋值！");
        if (letterImage == null) Debug.LogWarning("⚠️ letterImage 未赋值（可选）");
        if (string.IsNullOrEmpty(letterContent)) Debug.LogWarning("⚠️ letterContent 为空！");
        
        // 初始化：显示对话面板，隐藏信件面板
        dialoguePanel.SetActive(true);
        letterPanel.SetActive(false);
        
        // 绑定按钮事件
        dialogueContinueButton.onClick.AddListener(OnDialogueContinue);
        letterContinueButton.onClick.AddListener(OnLetterContinue);
        
        // 显示第一句对话
        ShowDialogue(0);
        
        Debug.Log("=== Level3_LetterManager 初始化完成 ===");
    }
    
    /// <summary>
    /// 显示对话
    /// </summary>
    void ShowDialogue(int index)
    {
        if (index >= dialogueLines.Length)
        {
            // 对话结束，显示信件
            ShowLetter();
            return;
        }
        
        currentDialogueIndex = index;
        DialogueLine line = dialogueLines[index];
        
        // 更新人物立绘（如果有配置）
        if (line.characterSprite != null)
        {
            characterImage.sprite = line.characterSprite;
            characterImage.gameObject.SetActive(true);
        }
        
        speakerName.text = line.speaker;
        dialogueText.text = line.content;
        
        Debug.Log($"显示对话 {index + 1}/{dialogueLines.Length}: {line.speaker}");
    }
    
    /// <summary>
    /// 对话"继续"按钮点击
    /// </summary>
    void OnDialogueContinue()
    {
        ShowDialogue(currentDialogueIndex + 1);
    }
    
    /// <summary>
    /// 显示信件
    /// </summary>
    void ShowLetter()
    {
        Debug.Log("对话结束，显示师父第三封信");
        
        // 隐藏对话面板
        dialoguePanel.SetActive(false);
        
        // 显示信件面板
        letterPanel.SetActive(true);
        
        // === 调试信息：检查信件组件 ===
        if (letterText == null)
        {
            Debug.LogError("❌ letterText 组件未赋值！请在 Inspector 中拖拽 LetterText 对象到 Letter Text 字段");
            return;
        }
        
        if (string.IsNullOrEmpty(letterContent))
        {
            Debug.LogWarning("⚠️ letterContent 内容为空！请在 Inspector 中填写信件内容");
        }
        
        // 设置信件文字
        letterText.text = letterContent;
        
        Debug.Log($"✅ 信件文字已设置，内容长度: {letterContent.Length} 字符");
        Debug.Log($"✅ LetterText 激活状态: {letterText.gameObject.activeSelf}");
        Debug.Log($"✅ LetterText 颜色: {letterText.color}");
        Debug.Log($"✅ LetterPanel 激活状态: {letterPanel.activeSelf}");
    }
    
    /// <summary>
    /// 信件"继续"按钮点击
    /// </summary>
    void OnLetterContinue()
    {
        Debug.Log("信件阅读完毕，准备切换场景");
        SceneManager.LoadScene(nextSceneName);
    }
}

/// <summary>
/// 对话数据结构
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [Header("对话内容")]
    public string speaker;          // 说话者名字
    
    [TextArea(2, 4)]
    public string content;          // 对话内容
    
    [Header("可选：人物立绘")]
    public Sprite characterSprite;  // 人物立绘（可选，不同表情/姿态）
}
