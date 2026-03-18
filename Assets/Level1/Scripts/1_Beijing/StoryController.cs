using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 前置剧情逐句播放控制器
/// 挂载到StoryPanel上
/// </summary>
public class StoryController : MonoBehaviour
{
    // 【在Inspector面板赋值】
    [Header("旁白UI组件（原有）")]
    public TextMeshProUGUI storyText; // 旁白文本框
    public Button continueBtn;        // 旁白的继续按钮
    public GameObject storyPanel;     // 旁白整个面板（用来隐藏旁白）

    [Header("小墨UI组件（新增+完善）")]
    public GameObject xiaoMoPanel;    // 小墨总面板（父容器）
    public Image xiaoMoImage;         // 小墨立绘Image
    public TextMeshProUGUI xiaoMoNameText; // 小墨角色名字框（新增！）
    public TextMeshProUGUI xiaoMoDialogText; // 小墨对话框文本框
    public Button xiaoMoContinueBtn;  // 小墨面板内的继续按钮

    [Header("剧情配置")]
    [TextArea(2, 5)] 
    public string[] storyLines; // 逐句旁白文本
    public string xiaoMoName = "小墨"; // 小墨的角色名（可在面板改）

    // 私有变量
    private int currentLineIndex = 0; // 当前播放的剧情索引

    void Start()
    {
        // 初始化：空值检查 + 隐藏小墨UI + 绑定按钮
        CheckReferences();
        HideXiaoMoUI();
        continueBtn.onClick.AddListener(OnContinueClick);
        ShowCurrentStoryLine();
    }

    /// <summary>
    /// 空值检查（避免赋值遗漏报错）
    /// </summary>
    private void CheckReferences()
    {
        if (storyText == null) Debug.LogError("storyText 未赋值！");
        if (continueBtn == null) Debug.LogError("continueBtn 未赋值！");
        if (storyPanel == null) Debug.LogError("storyPanel 未赋值！");
        if (xiaoMoPanel == null) Debug.LogError("xiaoMoPanel 未赋值！");
        if (xiaoMoImage == null) Debug.LogError("xiaoMoImage 未赋值！");
        if (xiaoMoNameText == null) Debug.LogError("xiaoMoNameText 未赋值！"); // 检查名字框
        if (xiaoMoDialogText == null) Debug.LogError("xiaoMoDialogText 未赋值！");
        if (xiaoMoContinueBtn == null) Debug.LogError("xiaoMoContinueBtn 未赋值！");
    }

    /// <summary>
    /// 点击“继续”按钮触发（旁白的继续按钮）
    /// </summary>
    private void OnContinueClick()
    {
        currentLineIndex++;
        if (currentLineIndex < storyLines.Length)
        {
            ShowCurrentStoryLine();
        }
        else
        {
            EndStory(); // 旁白播完，显示小墨UI（含名字框）
        }
    }

    /// <summary>
    /// 显示当前索引的旁白文本
    /// </summary>
    private void ShowCurrentStoryLine()
    {
        if (currentLineIndex >= 0 && currentLineIndex < storyLines.Length)
        {
            storyText.text = storyLines[currentLineIndex];
        }
    }

    /// <summary>
    /// 旁白播放完毕，切换到小墨UI（核心：名字框和立绘同步显示）
    /// </summary>
    private void EndStory()
    {
        // 隐藏旁白所有UI
        storyPanel.SetActive(false);
        // 显示小墨所有UI（立绘+名字框+对话框+按钮）
        ShowXiaoMoUI();
    }

    /// <summary>
    /// 隐藏小墨所有UI（初始状态调用）
    /// </summary>
    private void HideXiaoMoUI()
    {
        if (xiaoMoPanel != null) xiaoMoPanel.SetActive(false);
        if (xiaoMoImage != null) xiaoMoImage.gameObject.SetActive(false);
        if (xiaoMoNameText != null) xiaoMoNameText.gameObject.SetActive(false); // 隐藏名字框
        if (xiaoMoDialogText != null) xiaoMoDialogText.gameObject.SetActive(false);
        if (xiaoMoContinueBtn != null) xiaoMoContinueBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示小墨所有UI（旁白结束后调用，名字框和立绘同步显示）
    /// </summary>
    private void ShowXiaoMoUI()
    {
        if (xiaoMoPanel != null) xiaoMoPanel.SetActive(true);
        if (xiaoMoImage != null) xiaoMoImage.gameObject.SetActive(true);
        
        // 显示名字框并赋值（可在面板改名字）
        if (xiaoMoNameText != null)
        {
            xiaoMoNameText.text = xiaoMoName;
            xiaoMoNameText.gameObject.SetActive(true);
        }
        
        if (xiaoMoDialogText != null) xiaoMoDialogText.gameObject.SetActive(true);
        if (xiaoMoContinueBtn != null) xiaoMoContinueBtn.gameObject.SetActive(true);
    }

    // 可选：重置剧情（如需重新播放剧情时调用）
    public void ResetStory()
    {
        currentLineIndex = 0;
        // 显示旁白，隐藏小墨
        storyPanel.SetActive(true);
        HideXiaoMoUI();
        // 重新显示第一句旁白
        ShowCurrentStoryLine();
    }
}