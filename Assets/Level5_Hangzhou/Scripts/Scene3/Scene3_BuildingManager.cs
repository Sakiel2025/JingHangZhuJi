using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景3：廊桥建造管理器
/// 功能：7个阶段逐层建造廊桥，拖拽材料到指定位置
/// </summary>
public class Scene3_BuildingManager : MonoBehaviour
{
    [Header("UI面板")]
    [Tooltip("阶段说明弹窗")]
    public GameObject stageInfoPanel;
    [Tooltip("完成提示弹窗")]
    public GameObject completionPanel;

    [Header("阶段说明UI")]
    [Tooltip("阶段标题")]
    public TextMeshProUGUI stageTitle;
    [Tooltip("阶段说明")]
    public TextMeshProUGUI stageDescription;
    [Tooltip("开始建造按钮")]
    public Button startButton;

    [Header("完成提示UI")]
    [Tooltip("完成提示文字")]
    public TextMeshProUGUI completionText;
    [Tooltip("继续按钮")]
    public Button continueButton;

    [Header("廊桥区域")]
    [Tooltip("7个廊桥图层Image组件（初始半透明作为凹槽，吸附后变不透明）")]
    public List<Image> layerImages = new List<Image>();

    [Header("材料区域")]
    [Tooltip("7个材料对象")]
    public List<GameObject> materialObjects = new List<GameObject>();

    [Header("阶段数据")]
    [Tooltip("7个建造阶段的配置")]
    public List<BuildingStage> stages = new List<BuildingStage>();

    [Header("音频")]
    [Tooltip("背景音乐AudioSource")]
    public AudioSource bgmSource;
    [Tooltip("背景音乐Clip")]
    public AudioClip bgmClip;
    [Tooltip("吸附音效")]
    public AudioClip snapSound;
    [Tooltip("成功音效")]
    public AudioClip successSound;
    [Tooltip("完工音效")]
    public AudioClip completionSound;

    [Header("场景切换")]
    [Tooltip("下一场景名称")]
    public string nextSceneName = "Level5_Scene4_Ending";

    // 内部状态
    private int currentStageIndex = 0;

    void Start()
    {
        InitializeUI();
        PlayBGM();
        StartStage(0);
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    void InitializeUI()
    {
        // 隐藏所有面板
        if (stageInfoPanel != null) stageInfoPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);

        // 初始化所有廊桥层为半透明（凹槽效果）
        for (int i = 0; i < layerImages.Count; i++)
        {
            if (layerImages[i] != null)
            {
                Color color = layerImages[i].color;
                color.a = 0.3f; // 半透明作为凹槽
                layerImages[i].color = color;
                layerImages[i].gameObject.SetActive(false); // 初始隐藏
            }
        }

        // 隐藏所有材料
        foreach (GameObject material in materialObjects)
        {
            if (material != null) material.SetActive(false);
        }

        // 绑定按钮
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
    /// 播放背景音乐
    /// </summary>
    void PlayBGM()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 开始某个阶段
    /// </summary>
    void StartStage(int stageIndex)
    {
        currentStageIndex = stageIndex;
        BuildingStage stage = stages[currentStageIndex];

        // 显示阶段说明弹窗
        ShowStageInfo(stage);
    }

    /// <summary>
    /// 显示阶段说明
    /// </summary>
    void ShowStageInfo(BuildingStage stage)
    {
        if (stageTitle != null)
        {
            stageTitle.text = stage.stageTitle;
        }

        if (stageDescription != null)
        {
            stageDescription.text = stage.stageDescription;
        }

        if (stageInfoPanel != null)
        {
            stageInfoPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 点击"开始建造"按钮
    /// </summary>
    void OnStartButtonClicked()
    {
        // 隐藏弹窗
        if (stageInfoPanel != null)
        {
            stageInfoPanel.SetActive(false);
        }

        // 显示材料
        ShowStageMaterial();

        // 播放按钮音效
        Level5AudioManager.PlayButtonClick();
    }

    /// <summary>
    /// 显示当前阶段的材料
    /// </summary>
    void ShowStageMaterial()
    {
        BuildingStage stage = stages[currentStageIndex];
        GameObject material = materialObjects[currentStageIndex];

        // 配置材料图片
        Image materialImage = material.GetComponent<Image>();
        if (materialImage != null && stage.materialSprite != null)
        {
            materialImage.sprite = stage.materialSprite;
        }

        // 配置材料位置和大小（可选，如果为零则使用预制体默认值）
        RectTransform materialRect = material.GetComponent<RectTransform>();
        if (materialRect != null)
        {
            // 只有设置了非零值才覆盖
            if (stage.materialSize != Vector2.zero)
            {
                materialRect.sizeDelta = stage.materialSize;
            }
            
            if (stage.materialInitPos != Vector2.zero)
            {
                materialRect.anchoredPosition = stage.materialInitPos;
            }
        }

        // 配置拖拽脚本
        DraggableMaterial draggable = material.GetComponent<DraggableMaterial>();
        if (draggable != null && currentStageIndex < layerImages.Count)
        {
            draggable.targetSlot = layerImages[currentStageIndex].GetComponent<RectTransform>();
            draggable.snapDistance = stage.snapDistance;
            draggable.OnMaterialSnapped = OnMaterialSnapped;
            draggable.ResetMaterial();
        }

        // 显示材料
        material.SetActive(true);

        // 显示当前阶段的图层（半透明作为凹槽引导）
        if (currentStageIndex < layerImages.Count && layerImages[currentStageIndex] != null)
        {
            layerImages[currentStageIndex].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 材料吸附成功回调
    /// </summary>
    void OnMaterialSnapped()
    {
        // 播放吸附音效
        if (snapSound != null)
        {
            Level5AudioManager.PlaySFX(snapSound);
        }

        // 隐藏材料
        materialObjects[currentStageIndex].SetActive(false);

        // 播放搭建动画
        StartCoroutine(PlayBuildingAnimation());
    }

    /// <summary>
    /// 播放搭建动画
    /// </summary>
    IEnumerator PlayBuildingAnimation()
    {
        BuildingStage stage = stages[currentStageIndex];

        // 当前图层渐变为不透明（从0.3到1.0）
        if (currentStageIndex < layerImages.Count && layerImages[currentStageIndex] != null)
        {
            Image currentLayer = layerImages[currentStageIndex];
            float elapsed = 0f;
            
            while (elapsed < stage.animationDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.3f, 1.0f, elapsed / stage.animationDuration);
                Color color = currentLayer.color;
                color.a = alpha;
                currentLayer.color = color;
                yield return null;
            }
            
            // 确保最终为不透明
            Color finalColor = currentLayer.color;
            finalColor.a = 1.0f;
            currentLayer.color = finalColor;
        }

        // 播放成功音效
        if (successSound != null)
        {
            Level5AudioManager.PlaySFX(successSound);
        }

        // 显示完成提示
        ShowCompletionMessage();
    }

    /// <summary>
    /// 显示完成提示
    /// </summary>
    void ShowCompletionMessage()
    {
        BuildingStage stage = stages[currentStageIndex];

        // 显示完成文字
        if (completionText != null)
        {
            completionText.text = stage.completionMessage;
        }

        // 更新按钮文字
        if (continueButton != null)
        {
            TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (currentStageIndex < stages.Count - 1)
                {
                    buttonText.text = "继续";
                }
                else
                {
                    buttonText.text = "前往下一场景";
                }
            }
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 点击"继续"按钮
    /// </summary>
    void OnContinueButtonClicked()
    {
        // 隐藏提示
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        // 播放按钮音效
        Level5AudioManager.PlayButtonClick();

        // 进入下一阶段
        currentStageIndex++;

        if (currentStageIndex < stages.Count)
        {
            StartStage(currentStageIndex);
        }
        else
        {
            // 全部完成，进入结局
            CompleteBridge();
        }
    }

    /// <summary>
    /// 廊桥完工
    /// </summary>
    void CompleteBridge()
    {
        // 播放完工音效
        if (completionSound != null)
        {
            Level5AudioManager.PlaySFX(completionSound);
        }

        // 播放完工动画
        StartCoroutine(PlayCompletionAnimation());
    }

    /// <summary>
    /// 播放完工动画（镜头拉远效果）
    /// </summary>
    IEnumerator PlayCompletionAnimation()
    {
        // 镜头拉远效果（调整BridgeArea的Scale）
        if (layerImages.Count > 0 && layerImages[0] != null)
        {
            // 获取Layer的父对象（BridgeArea）
            RectTransform bridgeRect = layerImages[0].transform.parent.GetComponent<RectTransform>();
            if (bridgeRect != null)
            {
                Vector3 startScale = bridgeRect.localScale;
                Vector3 endScale = startScale * 0.85f;

                float duration = 2.0f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    bridgeRect.localScale = Vector3.Lerp(startScale, endScale, t);
                    yield return null;
                }
            }
        }

        // 显示完工提示
        if (completionText != null)
        {
            completionText.text = "小墨：廊桥建好了！师父一定会很高兴的！";
        }

        // 更新按钮文字为"前往下一场景"
        if (continueButton != null)
        {
            TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "前往下一场景";
            }

            // 修改按钮事件为加载场景
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(LoadEndingScene);
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 加载结局场景
    /// </summary>
    void LoadEndingScene()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    /// <summary>
    /// 淡出并加载场景
    /// </summary>
    IEnumerator FadeOutAndLoadScene()
    {
        // 淡出BGM
        if (bgmSource != null)
        {
            float duration = 1.0f;
            float startVolume = bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
        }

        // 加载下一场景
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set!");
        }
    }

    // 用于Inspector调试
    void OnValidate()
    {
        // 检查必要组件
        if (stageInfoPanel == null) Debug.LogWarning("Stage Info Panel not assigned!");
        if (completionPanel == null) Debug.LogWarning("Completion Panel not assigned!");
        
        if (layerImages.Count != 7)
        {
            Debug.LogWarning("Need exactly 7 layer images! Current: " + layerImages.Count);
        }

        if (materialObjects.Count != 7)
        {
            Debug.LogWarning("Need exactly 7 material objects! Current: " + materialObjects.Count);
        }

        if (stages.Count != 7)
        {
            Debug.LogWarning("Need exactly 7 building stages! Current: " + stages.Count);
        }
    }
}
