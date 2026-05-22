using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// 场景1：剧情开场管理器
/// 功能：管理对话流程、信件阅读、场景切换
/// </summary>
public class Scene1Manager : MonoBehaviour
{
    [Header("角色立绘")]
    public Image characterImage;            // 角色立绘Image组件
    public Sprite shenSunSprite;            // 沈榫立绘
    public Sprite oldWomanNormal;           // 老妇人正常表情
    public Sprite oldWomanCrying;           // 老妇人落泪表情
    
    [Header("角色位置")]
    public Vector2 leftPosition = new Vector2(-400, 0);   // 左侧位置
    public Vector2 rightPosition = new Vector2(400, 0);   // 右侧位置
    public Vector2 centerPosition = new Vector2(0, 0);    // 中间位置（可选）

    [Header("对话系统")]
    public GameObject dialoguePanel;        // 对话框面板
    public TextMeshProUGUI speakerNameText; // 说话人名字
    public TextMeshProUGUI dialogueText;    // 对话内容
    public Button nextButton;               // 继续按钮

    [Header("信件系统")]
    public GameObject letterPanel;          // 信件面板
    public TextMeshProUGUI letterContentText; // 信件内容文字（可选）
    public Button closeLetterButton;        // 关闭信件按钮
    
    [TextArea(15, 25)]
    public string letterContent = @"榫儿，

如果你拿到这封信，说明我已经走了。
也说明，你见到她了。

光绪十二年（1886年），我在断桥修桥时遇到了她。
她说想要一座廊桥，能遮风雨，能看西湖。
我答应了，画了图纸，却一直没敢回来。

不是技艺不够，是心里有愧。

榫儿，帮师父完成这座廊桥吧。

廊桥的榫卯和前面学的不一样，
图纸背面我画了四种，你先学会。

然后按照图纸，一步步建起来。

—— 师父绝笔";  // 信件内容（可在Inspector中编辑）

    [Header("音频")]
    public AudioSource bgmSource;           // 背景音乐播放器
    public AudioClip bgmClip;               // 背景音乐
    public AudioClip paperSoundClip;        // 翻纸音效
    public AudioClip buttonClickClip;       // 按钮点击音效

    [Header("对话配音（可选，与dialogues数组一一对应）")]
    [Tooltip("7句对话的配音，顺序与对话内容对应，留空则不播放")]
    public AudioClip[] dialogueVoiceClips;  // 索引0-6对应7句对话
    [Tooltip("沈榫最终台词配音：我会帮师父完成这座廊桥的。")]
    public AudioClip finalShenSunVoice;
    [Tooltip("老妇人最终台词配音：谢谢你，孩子...")]
    public AudioClip finalOldWomanVoice;

    [Header("场景切换")]
    public string nextSceneName = "Level5_Scene2A_ThroughMortise"; // 下一个场景名称
    public float sceneTransitionDelay = 1.5f; // 场景切换延迟

    // 对话数据
    private string[] dialogues;
    private string[] speakers;
    private int currentDialogueIndex = 0;
    private bool isDialogueFinished = false;

    void Start()
    {
        InitializeDialogues();
        InitializeUI();
        PlayBGM();
        StartCoroutine(StartDialogueAfterDelay(0.5f));
    }

    /// <summary>
    /// 初始化对话内容
    /// </summary>
    void InitializeDialogues()
    {
        // 配置对话内容
        dialogues = new string[]
        {
            "你是老沈的徒弟吧？",                                    // 0 - 老妇人
            "您认识我师父？",                                         // 1 - 沈榫
            "十四年前，他在这里修断桥，我每天给他送茶。",           // 2 - 老妇人
            "临走前，他说想给我建一座廊桥，还画了图纸...",          // 3 - 老妇人
            "但他再也没回来过。",                                     // 4 - 老妇人
            "前些日子，有人给我送来这封信，说你会来。",             // 5 - 老妇人
            "（老妇人递出一封信）",                                   // 6 - 旁白
        };

        speakers = new string[]
        {
            "老妇人",
            "沈榫",
            "老妇人",
            "老妇人",
            "老妇人",
            "老妇人",
            "",  // 旁白
        };

        // 验证数据长度
        if (dialogues.Length != speakers.Length)
        {
            Debug.LogError("对话和说话人数量不匹配！");
        }
    }

    /// <summary>
    /// 初始化UI状态
    /// </summary>
    void InitializeUI()
    {
        // 隐藏所有UI
        if (characterImage != null) characterImage.gameObject.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (letterPanel != null) letterPanel.SetActive(false);

        // 绑定按钮事件
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        
        if (closeLetterButton != null)
        {
            closeLetterButton.onClick.AddListener(OnCloseLetterClicked);
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
            bgmSource.volume = 0.6f;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 延迟后开始对话
    /// </summary>
    IEnumerator StartDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartDialogue();
    }

    /// <summary>
    /// 开始对话
    /// </summary>
    void StartDialogue()
    {
        currentDialogueIndex = 0;
        isDialogueFinished = false;
        ShowDialogue();
    }

    /// <summary>
    /// 显示当前对话
    /// </summary>
    void ShowDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length)
        {
            // 对话结束，打开信件
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

        // 更新对话内容
        if (speakerNameText != null)
        {
            speakerNameText.text = speakers[currentDialogueIndex];
        }

        if (dialogueText != null)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
        }

        // 播放配音
        if (dialogueVoiceClips != null && currentDialogueIndex < dialogueVoiceClips.Length
            && dialogueVoiceClips[currentDialogueIndex] != null)
        {
            AudioSource.PlayClipAtPoint(dialogueVoiceClips[currentDialogueIndex], Camera.main.transform.position);
        }
    }

    /// <summary>
    /// 显示对应角色
    /// </summary>
    void ShowCharacter(string speaker)
    {
        if (characterImage == null) return;

        if (speaker == "沈榫")
        {
            // 显示沈榫，位置在左侧
            characterImage.sprite = shenSunSprite;
            characterImage.rectTransform.anchoredPosition = leftPosition;
            characterImage.gameObject.SetActive(true);
        }
        else if (speaker == "老妇人")
        {
            // 显示老妇人，位置在右侧
            // 根据当前对话索引决定表情
            if (currentDialogueIndex >= 4 && oldWomanCrying != null)
            {
                characterImage.sprite = oldWomanCrying; // 第4句开始哭泣
            }
            else
            {
                characterImage.sprite = oldWomanNormal;
            }
            characterImage.rectTransform.anchoredPosition = rightPosition;
            characterImage.gameObject.SetActive(true);
        }
        else // 旁白，不显示角色
        {
            characterImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击继续按钮
    /// </summary>
    void OnNextButtonClicked()
    {
        PlayButtonSound();
        
        if (!isDialogueFinished)
        {
            currentDialogueIndex++;
            ShowDialogue();
        }
        else
        {
            // 进入下一个场景
            LoadNextScene();
        }
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

        // 延迟后打开信件
        StartCoroutine(OpenLetterAfterDelay(0.5f));
    }

    /// <summary>
    /// 延迟后打开信件
    /// </summary>
    IEnumerator OpenLetterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenLetter();
    }

    /// <summary>
    /// 打开信件
    /// </summary>
    void OpenLetter()
    {
        // 播放翻纸音效
        if (paperSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(paperSoundClip, Camera.main.transform.position, 0.5f);
        }

        // 显示信件
        if (letterPanel != null)
        {
            letterPanel.SetActive(true);
        }

        // 如果有信件内容文字组件，设置内容
        if (letterContentText != null)
        {
            SetLetterContent();
        }
    }

    /// <summary>
    /// 设置信件内容（从Inspector配置的letterContent字段读取）
    /// </summary>
    void SetLetterContent()
    {
        // 从 Inspector 配置的 letterContent 字段读取内容
        if (string.IsNullOrEmpty(letterContent))
        {
            Debug.LogWarning("⚠️ letterContent 为空！请在 Inspector 中填写信件内容");
            letterContentText.text = "【信件内容未配置】";
        }
        else
        {
            letterContentText.text = letterContent;
            Debug.Log($"✅ 信件内容已加载，长度: {letterContent.Length} 字符");
        }
    }

    /// <summary>
    /// 关闭信件
    /// </summary>
    void OnCloseLetterClicked()
    {
        PlayButtonSound();

        // 关闭信件
        if (letterPanel != null)
        {
            letterPanel.SetActive(false);
        }

        // 显示最终对话
        StartCoroutine(ShowFinalDialogue());
    }

    /// <summary>
    /// 显示最终对话
    /// </summary>
    IEnumerator ShowFinalDialogue()
    {
        yield return new WaitForSeconds(0.3f);

        // 显示对话框
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // 第一句：沈榫（左侧）
        if (speakerNameText != null) speakerNameText.text = "沈榫";
        if (dialogueText != null) dialogueText.text = "我会帮师父完成这座廊桥的。";

        // 播放配音
        if (finalShenSunVoice != null)
            AudioSource.PlayClipAtPoint(finalShenSunVoice, Camera.main.transform.position);
        
        // 显示沈榫立绘
        if (characterImage != null && shenSunSprite != null)
        {
            characterImage.sprite = shenSunSprite;
            characterImage.rectTransform.anchoredPosition = leftPosition;
            characterImage.gameObject.SetActive(true);
        }

        // 修改下一步按钮功能
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ShowOldWomanFinalWords);
        }
    }

    /// <summary>
    /// 显示老妇人最后的话
    /// </summary>
    void ShowOldWomanFinalWords()
    {
        PlayButtonSound();

        if (speakerNameText != null) speakerNameText.text = "老妇人";
        if (dialogueText != null) dialogueText.text = "谢谢你，孩子...";

        // 播放配音
        if (finalOldWomanVoice != null)
            AudioSource.PlayClipAtPoint(finalOldWomanVoice, Camera.main.transform.position);

        // 显示老妇人立绘（右侧，正常表情）
        if (characterImage != null && oldWomanNormal != null)
        {
            characterImage.sprite = oldWomanNormal;
            characterImage.rectTransform.anchoredPosition = rightPosition;
            characterImage.gameObject.SetActive(true);
        }

        // 修改下一步按钮功能：进入下一场景
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => {
                PlayButtonSound();
                isDialogueFinished = true;
                LoadNextScene();
            });
        }
    }

    /// <summary>
    /// 加载下一个场景
    /// </summary>
    void LoadNextScene()
    {
        StartCoroutine(LoadSceneWithDelay());
    }

    /// <summary>
    /// 延迟后加载场景
    /// </summary>
    IEnumerator LoadSceneWithDelay()
    {
        // 淡出音乐
        if (bgmSource != null)
        {
            float fadeTime = sceneTransitionDelay;
            float startVolume = bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);

        // 加载下一个场景
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("下一个场景名称未设置！");
        }
    }

    /// <summary>
    /// 播放按钮音效
    /// </summary>
    void PlayButtonSound()
    {
        if (buttonClickClip != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickClip, Camera.main.transform.position, 0.3f);
        }
    }

    /// <summary>
    /// 调试用：跳过场景
    /// </summary>
    void Update()
    {
        // 按ESC键快速跳过场景（仅开发调试用）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadNextScene();
        }
    }
}
