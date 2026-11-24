using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 场景1：前置剧情控制器
/// 功能：进场自动依次播放4句配音，全部播完后可点击继续切换场景
/// </summary>
public class IntroController : MonoBehaviour
{
    [Header("UI引用（必需）")]
    [Tooltip("继续按钮")]
    public Button nextButton;

    [Header("对话设置（在这里直接编辑对话内容）")]
    [TextArea(3, 10)]
    [Tooltip("对话文本内容（仅用于显示，配音按下方顺序播放）")]
    public string dialogueContent = "小墨：\"师父的第四封信在苏州拙政园。\"\n沈榫：\"听说那里藏着'借山水意'的秘密。\"\n小墨：\"我们一起去探索吧！\"";

    [Header("配音（按顺序1→2→3→4自动播放）")]
    [Tooltip("第1句配音")]
    public AudioClip voice1;
    [Tooltip("第2句配音")]
    public AudioClip voice2;
    [Tooltip("第3句配音")]
    public AudioClip voice3;
    [Tooltip("第4句配音")]
    public AudioClip voice4;

    [Header("场景设置")]
    [Tooltip("下一个场景的名称")]
    public string nextSceneName = "Level4_LotusPool";

    private AudioSource audioSource;

    void Start()
    {
        // 确保 Level4GameData 存在
        if (Level4GameData.Instance == null)
        {
            GameObject dataObj = new GameObject("Level4GameData");
            dataObj.AddComponent<Level4GameData>();
            Debug.Log("自动创建 Level4GameData");
        }

        // 绑定按钮点击事件
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextClicked);
        }
        else
        {
            Debug.LogError("NextButton 未关联！请在 Inspector 中拖入按钮");
        }

        // 自动查找并设置对话文字
        Text dialogueText = FindDialogueText();
        if (dialogueText != null)
        {
            dialogueText.text = dialogueContent;
            Debug.Log("对话内容已设置");
        }

        // 获取或添加 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // 自动开始依次播放配音
        StartCoroutine(PlayVoicesInOrder());

        Debug.Log("场景1：前置剧情 - 已加载");
    }

    /// <summary>
    /// 依次播放4段配音，每段播完后自动播下一段
    /// </summary>
    IEnumerator PlayVoicesInOrder()
    {
        AudioClip[] clips = { voice1, voice2, voice3, voice4 };

        foreach (AudioClip clip in clips)
        {
            if (clip == null) continue;

            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log($"播放配音：{clip.name}");

            // 等待当前配音播完
            yield return new WaitForSeconds(clip.length);
        }

        Debug.Log("所有配音播放完毕");
    }

    /// <summary>
    /// 自动查找对话文字组件
    /// </summary>
    Text FindDialogueText()
    {
        // 尝试查找名为 "DialogueText" 的Text组件
        GameObject dialogueObj = GameObject.Find("DialogueText");
        if (dialogueObj != null)
        {
            return dialogueObj.GetComponent<Text>();
        }

        // 如果找不到，尝试查找 DialoguePanel 下的第一个 Text
        GameObject panel = GameObject.Find("DialoguePanel");
        if (panel != null)
        {
            Text[] texts = panel.GetComponentsInChildren<Text>();
            if (texts.Length > 0)
            {
                return texts[0];
            }
        }

        Debug.LogWarning("未找到 DialogueText 组件，对话将不会显示");
        return null;
    }

    /// <summary>
    /// 点击"继续"按钮
    /// </summary>
    void OnNextClicked()
    {
        Debug.Log($"切换到下一场景：{nextSceneName}");

        // 停止正在播放的配音
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 标记场景1完成
        if (Level4GameData.Instance != null)
        {
            Level4GameData.Instance.scene1_Intro = true;
        }

        // 切换到荷花池场景
        SceneManager.LoadScene(nextSceneName);
    }
}
