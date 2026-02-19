using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// 场景2D：馒头榫学习管理器（最后一种榫卯）
/// 功能：知识卡片展示、2个构件拼接、成功反馈、场景切换
/// </summary>
public class Scene2D_RoundTenonManager : MonoBehaviour
{
    [Header("UI面板")]
    [Tooltip("知识卡片面板")]
    public GameObject knowledgeCardPanel;
    [Tooltip("拼接练习区域")]
    public GameObject puzzleArea;
    [Tooltip("成功提示面板")]
    public GameObject successPanel;

    [Header("知识卡片UI")]
    [Tooltip("卡片描述文字")]
    public TextMeshProUGUI cardDescription;
    [Tooltip("师父笔记文字")]
    public TextMeshProUGUI masterNote;
    [Tooltip("开始练习按钮")]
    public Button startButton;

    [Header("知识卡片内容")]
    [Tooltip("榫卯类型描述（显示在CardDescription中）")]
    [TextArea(3, 6)]
    public string mortiseDescription = "【馒头榫】用途：柱与屋架、圆形构件连接 特点：圆形榫头，可360度旋转调整";
    
    [Tooltip("师父笔记（显示在MasterNote中）")]
    public string masterNoteText = "师父笔记：\"馒头榫圆润，廊桥顶上用\"";

    [Header("拼接区域 - 2个构件")]
    [Tooltip("柱子（Piece1）的Image组件")]
    public Image piece1Image;
    [Tooltip("屋架（Piece2）的Image组件")]
    public Image piece2Image;
    [Tooltip("柱子的拖拽脚本")]
    public DraggablePiece piece1Draggable;
    [Tooltip("屋架的拖拽脚本")]
    public DraggablePiece piece2Draggable;

    [Header("成功提示UI")]
    [Tooltip("成功提示文字")]
    public TextMeshProUGUI successText;
    [Tooltip("继续按钮")]
    public Button continueButton;

    [Header("成功提示内容")]
    [Tooltip("拼接成功时显示的文字")]
    public string successMessage = "小墨：馒头榫拼好了！四种榫卯都学会了！";

    [Header("音频")]
    [Tooltip("背景音乐")]
    public AudioClip bgmClip;
    [Tooltip("拼接音效")]
    public AudioClip snapSoundClip;
    [Tooltip("成功音效")]
    public AudioClip successSoundClip;

    [Header("场景切换")]
    [Tooltip("下一场景名称")]
    public string nextSceneName = "Level5_Scene3_Building";
    [Tooltip("场景切换延迟（秒）")]
    public float transitionDelay = 2.0f;

    // 内部状态
    private int snappedCount = 0; // 已吸附的构件数量
    private bool puzzleCompleted = false;

    void Start()
    {
        InitializeUI();
        SetupKnowledgeCard();
        PlayBGM();
        ShowKnowledgeCard();
    }

    /// <summary>
    /// 初始化UI状态
    /// </summary>
    void InitializeUI()
    {
        // 隐藏所有面板
        if (knowledgeCardPanel != null) knowledgeCardPanel.SetActive(false);
        if (puzzleArea != null) puzzleArea.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);

        // 绑定按钮事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
    }

    /// <summary>
    /// 设置知识卡片内容
    /// </summary>
    void SetupKnowledgeCard()
    {
        if (cardDescription != null)
        {
            cardDescription.text = mortiseDescription;
        }

        if (masterNote != null)
        {
            masterNote.text = masterNoteText;
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    void PlayBGM()
    {
        if (bgmClip != null)
        {
            Level5AudioManager.PlayBGM(bgmClip);
        }
    }

    /// <summary>
    /// 显示知识卡片
    /// </summary>
    void ShowKnowledgeCard()
    {
        if (knowledgeCardPanel != null)
        {
            knowledgeCardPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 点击"开始练习"按钮
    /// </summary>
    void OnStartButtonClicked()
    {
        // 隐藏知识卡片
        if (knowledgeCardPanel != null)
        {
            knowledgeCardPanel.SetActive(false);
        }

        // 显示拼接区域
        if (puzzleArea != null)
        {
            puzzleArea.SetActive(true);
        }

        // 设置拖拽脚本的回调
        SetupDraggableCallbacks();

        // 播放按钮音效
        PlayButtonClickSound();
    }

    /// <summary>
    /// 设置拖拽脚本的回调
    /// </summary>
    void SetupDraggableCallbacks()
    {
        if (piece1Draggable != null)
        {
            piece1Draggable.onSnapSuccess = OnPieceSnapped;
        }

        if (piece2Draggable != null)
        {
            piece2Draggable.onSnapSuccess = OnPieceSnapped;
        }
    }

    /// <summary>
    /// 当某个构件吸附成功时调用
    /// </summary>
    void OnPieceSnapped()
    {
        snappedCount++;

        // 播放吸附音效
        PlaySnapSound();

        // 检查是否所有构件都已吸附（2个构件）
        if (snappedCount >= 2 && !puzzleCompleted)
        {
            puzzleCompleted = true;
            OnPuzzleCompleted();
        }
    }

    /// <summary>
    /// 拼接完成
    /// </summary>
    void OnPuzzleCompleted()
    {
        // 延迟显示成功面板
        StartCoroutine(ShowSuccessPanelDelayed(0.5f));
    }

    /// <summary>
    /// 延迟显示成功面板
    /// </summary>
    IEnumerator ShowSuccessPanelDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 播放成功音效
        PlaySuccessSound();

        // 显示成功面板
        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }

        if (successText != null)
        {
            successText.text = successMessage;
        }
    }

    /// <summary>
    /// 点击"继续"按钮
    /// </summary>
    void OnContinueButtonClicked()
    {
        PlayButtonClickSound();
        LoadNextScene();
    }

    /// <summary>
    /// 加载下一场景
    /// </summary>
    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
        else
        {
            Debug.LogWarning("Next scene name is not set!");
        }
    }

    /// <summary>
    /// 淡出并加载场景
    /// </summary>
    IEnumerator FadeOutAndLoadScene()
    {
        // 可以添加淡出效果
        yield return new WaitForSeconds(0.3f);

        // 加载下一场景
        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// 播放按钮点击音效
    /// </summary>
    void PlayButtonClickSound()
    {
        Level5AudioManager.PlayButtonClick();
    }

    /// <summary>
    /// 播放吸附音效
    /// </summary>
    void PlaySnapSound()
    {
        if (snapSoundClip != null)
        {
            Level5AudioManager.PlaySFX(snapSoundClip);
        }
    }

    /// <summary>
    /// 播放成功音效
    /// </summary>
    void PlaySuccessSound()
    {
        if (successSoundClip != null)
        {
            Level5AudioManager.PlaySFX(successSoundClip);
        }
    }

    /// <summary>
    /// 重置拼接（用于测试）
    /// </summary>
    public void ResetPuzzle()
    {
        snappedCount = 0;
        puzzleCompleted = false;

        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }

        // 重置拖拽脚本
        if (piece1Draggable != null) piece1Draggable.ResetPosition();
        if (piece2Draggable != null) piece2Draggable.ResetPosition();
    }

    // 用于Inspector调试
    void OnValidate()
    {
        // 检查必要组件是否配置
        if (knowledgeCardPanel == null) Debug.LogWarning("Knowledge Card Panel not assigned!");
        if (puzzleArea == null) Debug.LogWarning("Puzzle Area not assigned!");
        if (successPanel == null) Debug.LogWarning("Success Panel not assigned!");
        
        if (piece1Image == null || piece2Image == null)
        {
            Debug.LogWarning("Piece Images not fully assigned! Need 2 pieces for Round Tenon.");
        }

        if (piece1Draggable == null || piece2Draggable == null)
        {
            Debug.LogWarning("Draggable scripts not fully assigned! Need 2 pieces for Round Tenon.");
        }
    }
}
