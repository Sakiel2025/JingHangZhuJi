using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景4：剧情结局管理器
/// 功能：对话系统 + 蒙太奇播放 + 结束画面
/// </summary>
public class Scene4_EndingManager : MonoBehaviour
{
    [Header("角色立绘")]
    [Tooltip("角色显示Image（动态切换）")]
    public Image characterImage;
    [Tooltip("沈榫立绘")]
    public Sprite shenSunSprite;
    [Tooltip("沈榫显示位置")]
    public Vector2 shenSunPosition = new Vector2(-400, 0);
    [Tooltip("老妇人正常表情")]
    public Sprite oldWomanNormalSprite;
    [Tooltip("老妇人显示位置")]
    public Vector2 oldWomanPosition = new Vector2(400, 0);
    [Tooltip("老妇人微笑表情（可选）")]
    public Sprite oldWomanSmileSprite;

    [Header("对话系统")]
    [Tooltip("对话框面板")]
    public GameObject dialoguePanel;
    [Tooltip("说话人姓名")]
    public TextMeshProUGUI speakerNameText;
    [Tooltip("对话内容")]
    public TextMeshProUGUI dialogueText;
    [Tooltip("继续按钮")]
    public Button nextButton;

    [Header("蒙太奇系统")]
    [Tooltip("蒙太奇播放面板")]
    public GameObject montagePanel;
    [Tooltip("蒙太奇图片")]
    public Image montageImage;
    [Tooltip("字幕文字")]
    public TextMeshProUGUI subtitleText;
    [Tooltip("字幕背景")]
    public GameObject subtitleBackground;

    [Header("结束面板")]
    [Tooltip("结束画面面板")]
    public GameObject endingPanel;
    [Tooltip("结束文字")]
    public TextMeshProUGUI endingText;
    [Tooltip("继续按钮")]
    public Button continueButton;

    [Header("蒙太奇数据")]
    [Tooltip("3个蒙太奇配置")]
    public List<MontageData> montageList = new List<MontageData>();

    [Header("音频")]
    [Tooltip("背景音乐AudioSource")]
    public AudioSource bgmSource;
    [Tooltip("背景音乐Clip")]
    public AudioClip bgmClip;

    [Header("对话配音（可选，与对话内容一一对应）")]
    [Tooltip("7句对话的配音，顺序与对话内容对应，留空则不播放")]
    public AudioClip[] dialogueVoiceClips;

    [Header("场景切换")]
    [Tooltip("下一场景名称")]
    public string nextSceneName = "Success";

    // 对话数据
    private string[] dialogues;
    private string[] speakers;
    private int currentDialogueIndex = 0;

    private bool isMontageMode = false;

    void Start()
    {
        InitializeDialogues();
        InitializeUI();
        PlayBGM();
        StartDialogue();
    }

    /// <summary>
    /// 初始化对话内容
    /// </summary>
    void InitializeDialogues()
    {
        // 配置对话内容
        dialogues = new string[]
        {
            "师父的遗愿，我帮他完成了。",                             // 0
            "（环顾廊桥，抚摸栏杆）",                                   // 1 - 动作描述
            "十四年了...他画了十四年的图纸...",                      // 2
            "没想到真的建起来了。",                                     // 3
            "这座廊桥...真好。",                                       // 4
            "他若看到，一定会很欣慰。",                               // 5
            "谢谢你，也替我谢谢他。",                                 // 6
        };

        speakers = new string[]
        {
            "沈榫",
            "",  // 旁白/动作
            "老妇人",
            "老妇人",
            "老妇人",
            "老妇人",
            "老妇人",
        };
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    void InitializeUI()
    {
        // 隐藏所有UI
        if (characterImage != null) characterImage.gameObject.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (montagePanel != null) montagePanel.SetActive(false);
        if (endingPanel != null) endingPanel.SetActive(false);
        if (subtitleBackground != null) subtitleBackground.SetActive(false);

        // 绑定按钮事件
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
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
            bgmSource.loop = false;
            bgmSource.volume = 0.7f;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 开始对话
    /// </summary>
    void StartDialogue()
    {
        currentDialogueIndex = 0;
        ShowDialogue();
    }

    /// <summary>
    /// 显示当前对话
    /// </summary>
    void ShowDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length)
        {
            // 对话结束，播放蒙太奇
            EndDialogue();
            return;
        }

        // 显示对应角色
        ShowCharacter(speakers[currentDialogueIndex]);

        // 显示对话框
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = speakers[currentDialogueIndex];
        }

        if (dialogueText != null)
        {
            dialogueText.text = dialogues[currentDialogueIndex];

            // 所有对话统一使用黑色正常字体
            dialogueText.fontStyle = FontStyles.Normal;
            dialogueText.color = Color.black;
        }

        // 播放配音
        if (dialogueVoiceClips != null && currentDialogueIndex < dialogueVoiceClips.Length
            && dialogueVoiceClips[currentDialogueIndex] != null)
        {
            AudioSource.PlayClipAtPoint(dialogueVoiceClips[currentDialogueIndex], Camera.main.transform.position);
        }
    }

    /// <summary>
    /// 显示角色立绘
    /// </summary>
    void ShowCharacter(string speaker)
    {
        if (characterImage == null) return;

        if (speaker == "沈榫")
        {
            // 显示沈榫（使用配置的位置）
            characterImage.sprite = shenSunSprite;
            characterImage.rectTransform.anchoredPosition = shenSunPosition;
            characterImage.gameObject.SetActive(true);
        }
        else if (speaker == "老妇人")
        {
            // 显示老妇人（使用配置的位置）
            characterImage.sprite = oldWomanNormalSprite;
            characterImage.rectTransform.anchoredPosition = oldWomanPosition;
            characterImage.gameObject.SetActive(true);
        }
        else
        {
            // 旁白/动作描述，隐藏角色
            characterImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击"继续"按钮
    /// </summary>
    void OnNextButtonClicked()
    {
        Level5AudioManager.PlayButtonClick();
        currentDialogueIndex++;
        ShowDialogue();
    }

    /// <summary>
    /// 对话结束
    /// </summary>
    void EndDialogue()
    {
        // 隐藏对话框
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // 延迟0.5秒后播放蒙太奇
        StartCoroutine(StartMontageAfterDelay(0.5f));
    }

    /// <summary>
    /// 延迟启动蒙太奇
    /// </summary>
    IEnumerator StartMontageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(PlayMontage());
    }

    /// <summary>
    /// 播放蒙太奇序列
    /// </summary>
    IEnumerator PlayMontage()
    {
        isMontageMode = true;

        // 隐藏角色和对话框
        if (characterImage != null) characterImage.gameObject.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // 显示蒙太奇面板
        if (montagePanel != null)
        {
            montagePanel.SetActive(true);
        }

        // 逐个播放3张蒙太奇（每张停留displayDuration秒后直接切换）
        for (int i = 0; i < montageList.Count; i++)
        {
            yield return StartCoroutine(PlaySingleMontage(montageList[i]));
        }

        // 蒙太奇播放完毕，隐藏蒙太奇面板，显示结束画面
        if (montagePanel != null)
        {
            montagePanel.SetActive(false);
        }

        ShowEnding();
    }

    /// <summary>
    /// 播放单个蒙太奇
    /// </summary>
    IEnumerator PlaySingleMontage(MontageData data)
    {
        // 设置图片（直接显示，不透明度为1）
        if (montageImage != null)
        {
            montageImage.sprite = data.montageSprite;
            Color color = montageImage.color;
            montageImage.color = new Color(color.r, color.g, color.b, 1f);
        }

        // 显示字幕（如果有）
        if (data.hasSubtitle)
        {
            if (subtitleBackground != null)
            {
                subtitleBackground.SetActive(true);
            }

            if (subtitleText != null)
            {
                subtitleText.text = data.subtitleText;
                subtitleText.gameObject.SetActive(true);
            }
        }

        // 停留显示时间
        yield return new WaitForSeconds(data.displayDuration);

        // 隐藏字幕
        if (data.hasSubtitle)
        {
            if (subtitleText != null)
            {
                subtitleText.gameObject.SetActive(false);
            }

            if (subtitleBackground != null)
            {
                subtitleBackground.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 图片淡入淡出效果
    /// </summary>
    IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        if (image == null) yield break;

        Color color = image.color;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        image.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    /// <summary>
    /// 显示结束画面
    /// </summary>
    void ShowEnding()
    {
        // 显示结束画面
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }

        isMontageMode = false;
    }

    /// <summary>
    /// 点击结束画面的"继续"按钮
    /// </summary>
    void OnContinueButtonClicked()
    {
        Level5AudioManager.PlayButtonClick();
        
        // ⭐ 保存第5关进度和碎片数
        SaveLevelProgress();
        
        // 返回主菜单或进入下一关
        ReturnToMainMenu();
    }
    
    /// <summary>
    /// 保存关卡进度和碎片数（仿照第一关实现）
    /// </summary>
    void SaveLevelProgress()
    {
        // 1. 从 lastLogin 取出当前登录的用户名
        string username = PlayerPrefs.GetString("lastLogin", "");

        // 防呆：如果没有登录用户，直接报错返回
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("⚠️ 没有获取到当前登录用户，无法更新关卡和碎片！");
            return;
        }

        // 2. 读取并更新关卡数（对应 KEY: level_用户名）
        int oldLevel = PlayerPrefs.GetInt("level_" + username, 0);
        int newLevel = oldLevel + 1;
        PlayerPrefs.SetInt("level_" + username, newLevel);

        // 3. 读取并更新碎片数（对应 KEY: fragment_用户名）
        int oldFragment = PlayerPrefs.GetInt("fragment_" + username, 0);
        int newFragment = oldFragment + 1;
        PlayerPrefs.SetInt("fragment_" + username, newFragment);

        // 4. 保存到本地
        PlayerPrefs.Save();

        // 5. 打印日志，方便验证
        Debug.Log($"✅ 第5关结束：\n" +
                  $"关卡数：{oldLevel} → {newLevel}\n" +
                  $"碎片数：{oldFragment} → {newFragment}");
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    void ReturnToMainMenu()
    {
        StartCoroutine(FadeOutAndReturn());
    }

    /// <summary>
    /// 淡出并返回
    /// </summary>
    IEnumerator FadeOutAndReturn()
    {
        // 淡出BGM
        if (bgmSource != null)
        {
            float duration = 1.0f;
            float startVolume = bgmSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }

            bgmSource.Stop();
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
        if (characterImage == null) Debug.LogWarning("Character Image not assigned!");
        if (dialoguePanel == null) Debug.LogWarning("Dialogue Panel not assigned!");
        if (montagePanel == null) Debug.LogWarning("Montage Panel not assigned!");
        if (endingPanel == null) Debug.LogWarning("Ending Panel not assigned!");

        if (montageList.Count != 3)
        {
            Debug.LogWarning("Need exactly 3 montage items! Current: " + montageList.Count);
        }
    }
}
