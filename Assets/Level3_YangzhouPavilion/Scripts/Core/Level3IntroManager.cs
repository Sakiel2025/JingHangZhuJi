using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// Level3 前置剧情场景管理器 - 视觉小说风格
    /// 负责播放黄府门口的开场剧情，然后切换到主场景
    /// </summary>
    public class Level3IntroManager : MonoBehaviour
    {
        [Header("UI 引用")]
        [Tooltip("角色立绘图片")]
        public Image characterImage;
        
        [Tooltip("角色名文本")]
        public TextMeshProUGUI speakerNameText;
        
        [Tooltip("对话内容文本")]
        public TextMeshProUGUI dialogueText;
        
        [Tooltip("继续按钮")]
        public Button continueButton;

        [Header("配音")]
        [Tooltip("对话配音播放器（不填则自动添加）")]
        public AudioSource voiceAudioSource;

        [Header("场景切换设置")]
        [Tooltip("主游戏场景名称")]
        public string mainSceneName = "Level3_Main";

        [Header("对话设置")]
        [Tooltip("所有对话内容")]
        public DialogueLine[] dialogueLines;
        
        [Tooltip("是否启用打字机效果")]
        public bool useTypewriterEffect = false;
        
        [Tooltip("打字机速度（每个字的间隔秒数）")]
        public float typewriterSpeed = 0.05f;

        // 内部状态
        private int currentLineIndex = 0;
        private bool isTyping = false;
        private bool isDialogueFinished = false;

        private void Start()
        {
            Debug.Log("[Level3Intro] 前置剧情场景开始");
            useTypewriterEffect = false; // 强制关闭打字机效果
            
            // 自动补充 AudioSource
            if (voiceAudioSource == null)
                voiceAudioSource = gameObject.AddComponent<AudioSource>();
            
            // 隐藏继续按钮
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
                // 绑定按钮点击事件
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
            
            // 显示第一句对话
            ShowCurrentLine();
        }

        private void Update()
        {
            // 也可以按空格继续（备用）
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnContinueButtonClicked();
            }
        }

        /// <summary>
        /// 继续按钮点击事件
        /// </summary>
        private void OnContinueButtonClicked()
        {
            if (isTyping)
            {
                // 如果正在打字，跳过打字效果，立即显示完整文本
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLineIndex].dialogueText;
                isTyping = false;
                ShowContinueButton();
            }
            else if (!isDialogueFinished)
            {
                // 显示下一句对话
                NextLine();
            }
        }

        /// <summary>
        /// 显示当前对话行
        /// </summary>
        private void ShowCurrentLine()
        {
            if (currentLineIndex >= dialogueLines.Length)
            {
                // 所有对话播放完毕
                FinishDialogue();
                return;
            }

            DialogueLine line = dialogueLines[currentLineIndex];
            
            // 更新角色名
            if (speakerNameText != null)
            {
                speakerNameText.text = line.speakerName;
            }
            
            // 更新角色立绘
            if (characterImage != null)
            {
                if (line.showCharacter && line.characterSprite != null)
                {
                    characterImage.sprite = line.characterSprite;
                    characterImage.gameObject.SetActive(true);
                }
                else
                {
                    characterImage.gameObject.SetActive(false);
                }
            }

            // 播放配音（旁白不播放）
            if (voiceAudioSource != null && line.speakerName != "旁白")
            {
                voiceAudioSource.Stop();
                if (line.voiceClip != null)
                {
                    voiceAudioSource.clip = line.voiceClip;
                    voiceAudioSource.Play();
                }
            }
            
            // 显示对话文本
            if (useTypewriterEffect)
            {
                StartCoroutine(TypewriterEffect(line.dialogueText));
            }
            else
            {
                dialogueText.text = line.dialogueText;
                ShowContinueButton();
            }
        }

        /// <summary>
        /// 打字机效果协程
        /// </summary>
        private IEnumerator TypewriterEffect(string text)
        {
            isTyping = true;
            dialogueText.text = "";
            
            // 隐藏继续按钮
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }
            
            // 逐字显示
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typewriterSpeed);
            }
            
            isTyping = false;
            ShowContinueButton();
        }

        /// <summary>
        /// 显示继续按钮
        /// </summary>
        private void ShowContinueButton()
        {
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 切换到下一句对话
        /// </summary>
        private void NextLine()
        {
            currentLineIndex++;
            ShowCurrentLine();
        }

        /// <summary>
        /// 对话结束，切换到主场景
        /// </summary>
        private void FinishDialogue()
        {
            isDialogueFinished = true;
            Debug.Log($"[Level3Intro] 剧情播放完毕，切换到主场景: {mainSceneName}");
            
            // 延迟加载（可选）
            StartCoroutine(LoadMainSceneWithDelay(1f));
        }

        /// <summary>
        /// 延迟加载主场景
        /// </summary>
        private IEnumerator LoadMainSceneWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(mainSceneName);
        }
    }

    /// <summary>
    /// 对话行数据结构
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        [Tooltip("说话人名字")]
        public string speakerName;
        
        [Tooltip("对话内容")]
        [TextArea(2, 5)]
        public string dialogueText;
        
        [Tooltip("角色立绘（可选）")]
        public Sprite characterSprite;
        
        [Tooltip("是否显示角色立绘")]
        public bool showCharacter = true;

        [Tooltip("配音音频（旁白留空）")]
        public AudioClip voiceClip;
    }
}
