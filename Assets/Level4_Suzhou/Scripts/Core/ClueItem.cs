using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;  // 支持TextMeshPro

/// <summary>
/// 通用线索物品脚本（场景2-5共用）
/// 功能：点击收集线索，显示提示，切换到下一场景
/// 支持Unity UI Text 和 TextMeshPro两种文本组件
/// 新增：支持初始隐藏，点击后显现
/// </summary>
[RequireComponent(typeof(PolygonCollider2D))]
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
    
    [Header("UI引用 - Unity UI Text")]
    [Tooltip("顶部提示文字（Unity UI Text）")]
    public Text hintText;
    
    [Tooltip("发现线索时的文字（Unity UI Text）")]
    public Text foundText;
    
    [Header("UI引用 - TextMeshPro（二选一）")]
    [Tooltip("顶部提示文字（TextMeshPro）")]
    public TMP_Text hintTextTMP;
    
    [Tooltip("发现线索时的文字（TextMeshPro）")]
    public TMP_Text foundTextTMP;
    
    [Header("文字框引用")]
    [Tooltip("提示文字框背景（TiShiKuang），场景开始即显示并一直保持")]
    public GameObject tiShiKuang;

    [Tooltip("黑底文字框背景（HeiDiWenZiKuang），点击物品后与发现文字一起显示")]
    public GameObject heiDiWenZiKuang;

    [Header("按钮引用")]
    [Tooltip("继续按钮")]
    public Button nextButton;
    
    [Header("显示设置")]
    [Tooltip("初始状态是否隐藏？勾选则开始时不可见，点击后显现")]
    public bool startHidden = false;
    
    [Tooltip("显现动画时长（秒）")]
    public float appearDuration = 0.3f;
    
    [Header("动画设置")]
    [Tooltip("放大倍数")]
    public float scaleMultiplier = 1.5f;
    
    [Tooltip("放大动画时长（秒）")]
    public float animationDuration = 0.5f;
    
    [Header("音效（可选）")]
    public AudioClip collectSound;

    [Header("配音（可选）")]
    [Tooltip("弹出黑底文字框时播放的配音")]
    public AudioClip voiceClip;
    
    private SpriteRenderer sr;
    private Color originalColor;
    private bool collected = false;
    private bool isVisible = true;  // 是否已显现
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        
        // 如果设置为初始隐藏，则设置透明度为0
        if (startHidden)
        {
            isVisible = false;
            Color hiddenColor = originalColor;
            hiddenColor.a = 0f;  // 完全透明
            sr.color = hiddenColor;
            Debug.Log($"线索物品初始化（隐藏状态）：{clueName}");
        }
        else
        {
            Debug.Log($"线索物品初始化（可见状态）：{clueName}");
        }
        
        // TiShiKuang：场景开始就显示，一直保持
        if (tiShiKuang != null)
        {
            tiShiKuang.SetActive(true);
        }

        // HeiDiWenZiKuang：初始隐藏，点击后才显示
        if (heiDiWenZiKuang != null)
        {
            heiDiWenZiKuang.SetActive(false);
        }

        // 初始化UI状态（支持两种文本类型）
        if (foundText != null)
        {
            foundText.gameObject.SetActive(false);
        }
        if (foundTextTMP != null)
        {
            foundTextTMP.gameObject.SetActive(false);
        }
        
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
    }
    
    /// <summary>
    /// 鼠标悬停时高亮（仅在可见状态有效）
    /// </summary>
    void OnMouseEnter()
    {
        if (collected || !isVisible) return;
        
        sr.color = Color.yellow;
        
        // 修改鼠标样式（可选）
        // Cursor.SetCursor(handCursor, Vector2.zero, CursorMode.Auto);
    }
    
    /// <summary>
    /// 鼠标离开时恢复
    /// </summary>
    void OnMouseExit()
    {
        if (collected || !isVisible) return;
        
        sr.color = originalColor;
        
        // 恢复鼠标样式
        // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    /// <summary>
    /// 鼠标点击时：如果隐藏则显现，如果可见则收集
    /// </summary>
    void OnMouseDown()
    {
        if (collected) return;
        
        // 如果当前是隐藏状态，先显现
        if (!isVisible)
        {
            StartCoroutine(AppearThenCollect());
        }
        // 如果已经可见，直接收集
        else
        {
            collected = true;
            StartCoroutine(CollectClue());
        }
    }
    
    /// <summary>
    /// 显现协程：淡入 + 收集
    /// </summary>
    IEnumerator AppearThenCollect()
    {
        Debug.Log($"线索显现：{clueName}");
        isVisible = true;
        
        // 淡入动画
        float elapsed = 0f;
        Color startColor = sr.color;  // alpha = 0
        Color targetColor = originalColor;  // alpha = 1
        
        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            sr.color = Color.Lerp(startColor, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 确保完全显示
        sr.color = originalColor;
        
        // 短暂延迟后自动收集
        yield return new WaitForSeconds(0.2f);
        
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
        
        // 提示文字与TiShiKuang保持显示，不隐藏
        
        // 移动并放大到屏幕中央
        float elapsed = 0f;
        Vector3 originalPosition = transform.position;
        Vector3 originalScale = transform.localScale;
        
        // 目标位置：摄像机中心（世界坐标）
        Vector3 targetPosition = Camera.main.transform.position;
        targetPosition.z = originalPosition.z;  // 保持Z坐标不变
        
        // 目标缩放：放大
        Vector3 targetScale = originalScale * scaleMultiplier;
        
        // 移动+放大动画
        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            
            // 平滑移动到中央
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            
            // 平滑放大
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 确保到达目标位置和缩放
        transform.position = targetPosition;
        transform.localScale = targetScale;
        
        // 保存线索数据
        SaveClueData();
        
        // 显示黑底文字框 + 发现文字（同时出现）
        if (heiDiWenZiKuang != null)
        {
            heiDiWenZiKuang.SetActive(true);
        }

        // 播放配音
        if (voiceClip != null)
        {
            AudioSource.PlayClipAtPoint(voiceClip, Camera.main.transform.position);
        }

        // 显示发现文字（支持两种文本类型）
        string displayText = $"获得'{clueName}'字！\n{clueDescription}";
        if (foundText != null)
        {
            foundText.text = displayText;
            foundText.gameObject.SetActive(true);
        }
        if (foundTextTMP != null)
        {
            foundTextTMP.text = displayText;
            foundTextTMP.gameObject.SetActive(true);
        }
        
        // 延迟1秒后显示继续按钮
        yield return new WaitForSeconds(1f);
        
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
        
        // 禁用交互
        GetComponent<PolygonCollider2D>().enabled = false;
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
