using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

namespace Level4.Suzhou
{
    /// <summary>
    /// 场景6解谜管理器
    /// 管理解谜逻辑、检查答案、显示结果
    /// </summary>
    public class PuzzleManager : MonoBehaviour
    {
        [Header("UI引用")]
        [Tooltip("提示文本")]
        public TextMeshProUGUI hintText;
    
    [Tooltip("成功对话面板")]
    public GameObject successPanel;
    
    [Tooltip("继续按钮")]
    public Button continueButton;
    
    [Header("槽位引用（按顺序 0-3）")]
    [Tooltip("4个槽位，按顺序拖入：借、山、水、意")]
    public DropSlot[] slots = new DropSlot[4];
    
    [Header("正确答案")]
    [Tooltip("正确的字顺序")]
    private string[] correctOrder = {"借", "山", "水", "意"};
    
    [Header("音效（可选）")]
    public AudioClip successSound;
    public AudioClip wrongSound;
    private AudioSource audioSource;
    
    [Header("设置")]
    [Tooltip("显示成功面板前的延迟时间（秒）")]
    public float successDelay = 0.5f;
    
    [Tooltip("错误提示显示时间（秒）")]
    public float wrongHintDuration = 2f;
    
    private bool isPuzzleCompleted = false;
    
    void Start()
    {
        // 初始化
        successPanel.SetActive(false);
        
        // 绑定继续按钮
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        // 获取音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (successSound != null || wrongSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 验证槽位设置
        ValidateSlots();
        
        Debug.Log("场景6解谜管理器初始化完成");
    }
    
    /// <summary>
    /// 验证槽位配置
    /// </summary>
    void ValidateSlots()
    {
        if (slots.Length != 4)
        {
            Debug.LogError("槽位数量不正确！应该是4个");
            return;
        }
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogError($"槽位 {i} 未分配！");
            }
            else if (slots[i].correctCharacter != correctOrder[i])
            {
                Debug.LogWarning($"槽位 {i} 的正确字符应该是 '{correctOrder[i]}'，当前是 '{slots[i].correctCharacter}'");
            }
        }
    }
    
    /// <summary>
    /// 检查解谜是否完成
    /// 由 DropSlot 在字放入后调用
    /// </summary>
    public void CheckPuzzleCompletion()
    {
        if (isPuzzleCompleted) return;
        
        Debug.Log("检查解谜完成度...");
        
        // 验证槽位数组
        if (slots == null || slots.Length != 4)
        {
            Debug.LogError("槽位数组配置错误！请在Inspector中分配4个槽位。");
            return;
        }
        
        // 1. 检查是否所有槽位都被填满
        foreach (var slot in slots)
        {
            if (slot == null)
            {
                Debug.LogError("槽位数组中有null元素！请在PuzzleManager的Inspector中分配所有槽位（Slot_0到Slot_3）。");
                return;
            }
            
            if (!slot.isOccupied)
            {
                Debug.Log("还有空槽位，未完成");
                return;
            }
        }
        
        Debug.Log("所有槽位已填满，检查顺序...");
        
        // 2. 检查顺序是否正确
        bool allCorrect = true;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentCharacter == null)
            {
                allCorrect = false;
                break;
            }
            
            DraggableCharacter character = slots[i].currentCharacter.GetComponent<DraggableCharacter>();
            if (character == null || character.characterName != correctOrder[i])
            {
                allCorrect = false;
                Debug.Log($"槽位 {i} 不正确：期望 '{correctOrder[i]}'，实际 '{(character != null ? character.characterName : "null")}'");
                break;
            }
        }
        
        if (allCorrect)
        {
            Debug.Log("解谜成功！");
            StartCoroutine(ShowSuccessPanel());
        }
        else
        {
            Debug.Log("顺序错误！");
            StartCoroutine(ShowWrongHint());
        }
    }
    
    /// <summary>
    /// 显示成功面板
    /// </summary>
    IEnumerator ShowSuccessPanel()
    {
        isPuzzleCompleted = true;
        
        // 等待一小段时间
        yield return new WaitForSeconds(successDelay);
        
        // 播放成功音效
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
        
        // 显示成功对话面板
        successPanel.SetActive(true);
        
        Debug.Log("显示成功面板");
    }
    
    /// <summary>
    /// 显示错误提示
    /// </summary>
    IEnumerator ShowWrongHint()
    {
        // 播放错误音效
        if (audioSource != null && wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }
        
        // 显示错误提示
        string originalText = hintText.text;
        Color originalColor = hintText.color;
        
        hintText.text = "顺序不对...再想想园林造景的顺序";
        hintText.color = Color.red;
        
        yield return new WaitForSeconds(wrongHintDuration);
        
        // 恢复原始提示
        hintText.text = originalText;
        hintText.color = originalColor;
        
        // 清空所有槽位，让玩家重新放置
        ResetAllSlots();
    }
    
    /// <summary>
    /// 重置所有槽位
    /// </summary>
    void ResetAllSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.isOccupied && slot.currentCharacter != null)
            {
                DraggableCharacter character = slot.currentCharacter.GetComponent<DraggableCharacter>();
                if (character != null)
                {
                    character.ResetState();
                }
                slot.RemoveCharacter();
            }
        }
        
        Debug.Log("已重置所有槽位");
    }
    
    /// <summary>
    /// 继续按钮点击事件
    /// </summary>
    void OnContinueClicked()
    {
        Debug.Log("点击继续按钮，准备切换到场景7");
        
        // 保存进度（如果有进度系统）
        if (Level4GameData.Instance != null)
        {
            Level4GameData.Instance.scene6_CentralPavilion = true;
        }
        
        // 切换到场景7（读信场景）
        SceneManager.LoadScene("Level4_Letter");
    }
    
    /// <summary>
    /// 手动检查按钮（可选，用于调试）
    /// </summary>
    public void OnCheckButtonClicked()
    {
        CheckPuzzleCompletion();
    }
    
    /// <summary>
    /// 重置按钮（可选，用于测试）
    /// </summary>
    public void OnResetButtonClicked()
    {
        isPuzzleCompleted = false;
        successPanel.SetActive(false);
        ResetAllSlots();
        
        if (hintText != null)
        {
            hintText.text = "将四个字按正确顺序放入凹槽";
            hintText.color = Color.black;
        }
        
        Debug.Log("解谜已重置");
    }
    }
}
