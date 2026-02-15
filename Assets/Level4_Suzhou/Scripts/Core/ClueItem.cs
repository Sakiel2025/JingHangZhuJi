using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 通用线索物品脚本（场景2-5共用）
/// 功能：点击收集线索，显示提示，切换到下一场景
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ClueItem : MonoBehaviour
{
    [Header("线索配置")]
    [Tooltip("线索名称：借、山、水、意")]
    public string clueName = "借";
    
    [Tooltip("线索描述")]
    public string clueDescription = "借景的智慧";
    
    [Tooltip("下一个场景名称")]
    public string nextSceneName = "Level4_MountainTower";
    
    [Header("UI引用")]
    [Tooltip("顶部提示文字")]
    public Text hintText;
    
    [Tooltip("发现线索时的文字")]
    public Text foundText;
    
    [Tooltip("继续按钮")]
    public Button nextButton;
    
    [Header("动画设置")]
    [Tooltip("放大倍数")]
    public float scaleMultiplier = 1.5f;
    
    [Tooltip("放大动画时长（秒）")]
    public float animationDuration = 0.5f;
    
    [Header("音效（可选）")]
    public AudioClip collectSound;
    
    private SpriteRenderer sr;
    private Color originalColor;
    private bool collected = false;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        
        // 初始化UI状态
        if (foundText != null)
        {
            foundText.gameObject.SetActive(false);
        }
        
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        
        Debug.Log($"线索物品初始化：{clueName}");
    }
    
    /// <summary>
    /// 鼠标悬停时高亮
    /// </summary>
    void OnMouseEnter()
    {
        if (collected) return;
        
        sr.color = Color.yellow;
        
        // 修改鼠标样式（可选）
        // Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
    }
    
    /// <summary>
    /// 鼠标离开时恢复
    /// </summary>
    void OnMouseExit()
    {
        if (collected) return;
        
        sr.color = originalColor;
        
        // 恢复鼠标样式
        // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    /// <summary>
    /// 鼠标点击时收集线索
    /// </summary>
    void OnMouseDown()
    {
        if (collected) return;
        
        collected = true;
        StartCoroutine(CollectClue());
    }
    
    /// <summary>
    /// 收集线索的协程（带动画效果）
    /// </summary>
    IEnumerator CollectClue()
    {
        Debug.Log($"收集线索：{clueName}");
        
        // 播放音效
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        // 放大动画
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;
        
        while (elapsed < animationDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = targetScale;
        
        // 保存线索数据
        SaveClueData();
        
        // 隐藏提示文字
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
        
        // 显示发现文字
        if (foundText != null)
        {
            foundText.text = $"获得'{clueName}'字！\n{clueDescription}";
            foundText.gameObject.SetActive(true);
        }
        
        // 延迟1秒后显示继续按钮
        yield return new WaitForSeconds(1f);
        
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
        
        // 禁用交互
        GetComponent<BoxCollider2D>().enabled = false;
    }
    
    /// <summary>
    /// 保存线索数据到 Level4GameData
    /// </summary>
    void SaveClueData()
    {
        if (Level4GameData.Instance == null)
        {
            Debug.LogWarning("Level4GameData.Instance 为空！");
            return;
        }
        
        Level4GameData data = Level4GameData.Instance;
        
        switch (clueName)
        {
            case "借":
                data.clue_Jie = true;
                data.scene2_LotusPool = true;
                break;
                
            case "山":
                data.clue_Shan = true;
                data.scene3_MountainTower = true;
                break;
                
            case "水":
                data.clue_Shui = true;
                data.scene4_Corridor = true;
                break;
                
            case "意":
                data.clue_Yi = true;
                data.scene5_RainPavilion = true;
                break;
                
            default:
                Debug.LogWarning($"未知的线索名称：{clueName}");
                break;
        }
        
        Debug.Log($"线索已保存：{clueName}，已收集 {data.GetCollectedClueCount()}/4");
    }
    
    /// <summary>
    /// 点击继续按钮，切换到下一场景
    /// </summary>
    void OnNextButtonClicked()
    {
        Debug.Log($"切换到下一场景：{nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
