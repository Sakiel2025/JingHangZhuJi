using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace Level4.Suzhou
{
    /// <summary>
    /// 场景7 - 信件场景控制器
    /// 展示师父的第四封信，并提供继续功能
    /// </summary>
    public class LetterSceneController : MonoBehaviour
    {
        [Header("UI引用")]
        [Tooltip("信件面板")]
        public GameObject letterPanel;
        
        [Tooltip("继续按钮")]
        public Button continueButton;
        
        [Header("打字机效果（可选）")]
        [Tooltip("信件文本组件（用于逐字显示）")]
        public TextMeshProUGUI letterText;
        
        [Tooltip("每个字的显示间隔（秒）")]
        [Range(0.01f, 0.2f)]
        public float typeSpeed = 0.05f;
        
        [Tooltip("是否启用打字机效果")]
        public bool enableTypewriter = false;
        
        [Header("音效（可选）")]
        [Tooltip("展开信纸音效")]
        public AudioClip paperUnfoldSound;
        
        [Tooltip("背景音乐")]
        public AudioClip bgMusic;
        
        [Tooltip("背景音乐音量")]
        [Range(0f, 1f)]
        public float bgMusicVolume = 0.3f;
        
        private AudioSource audioSource;
        private string fullText;
        private bool isTyping = false;
        
        void Start()
        {
            // 初始化音频
            audioSource = gameObject.AddComponent<AudioSource>();
            
            // 播放背景音乐
            if (bgMusic != null)
            {
                audioSource.clip = bgMusic;
                audioSource.loop = true;
                audioSource.volume = bgMusicVolume;
                audioSource.Play();
            }
            
            // 播放展开信纸音效
            if (paperUnfoldSound != null)
            {
                AudioSource.PlayClipAtPoint(paperUnfoldSound, Camera.main.transform.position);
            }
            
            // 绑定按钮事件
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }
            
            // 启动显示效果
            if (enableTypewriter && letterText != null)
            {
                fullText = letterText.text;
                StartCoroutine(TypewriterAndFadeIn());
            }
            else
            {
                StartCoroutine(FadeInLetter());
            }
            
            Debug.Log("场景7 - 信件场景加载完成");
        }
        
        void Update()
        {
            // 按空格键跳过打字机动画
            if (isTyping && Input.GetKeyDown(KeyCode.Space))
            {
                SkipTypewriter();
            }
        }
        
        /// <summary>
        /// 打字机效果 + 淡入动画
        /// </summary>
        IEnumerator TypewriterAndFadeIn()
        {
            // 先淡入面板
            yield return StartCoroutine(FadeInLetter());
            
            // 再启动打字机效果
            isTyping = true;
            letterText.text = "";
            
            foreach (char c in fullText)
            {
                letterText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            
            isTyping = false;
        }
        
        /// <summary>
        /// 跳过打字机动画
        /// </summary>
        void SkipTypewriter()
        {
            StopAllCoroutines();
            isTyping = false;
            
            if (letterText != null && !string.IsNullOrEmpty(fullText))
            {
                letterText.text = fullText;
            }
            
            // 确保面板完全显示
            if (letterPanel != null)
            {
                CanvasGroup canvasGroup = letterPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
            }
        }
        
        /// <summary>
        /// 信件淡入动画
        /// </summary>
        IEnumerator FadeInLetter()
        {
            if (letterPanel == null) yield break;
            
            CanvasGroup canvasGroup = letterPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = letterPanel.AddComponent<CanvasGroup>();
            }
            
            // 从透明到不透明
            canvasGroup.alpha = 0f;
            float duration = 1f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        /// <summary>
        /// 点击继续按钮
        /// </summary>
        void OnContinueClicked()
        {
            Debug.Log("信件阅读完成，切换到建造场景");
            
            // 如果还在播放动画，先跳过
            if (isTyping)
            {
                SkipTypewriter();
                return; // 第一次点击跳过动画，第二次才切换场景
            }
            
            // 保存进度
            if (Level4GameData.Instance != null)
            {
                Level4GameData.Instance.scene7_Building = true; // 使用现有字段
            }
            
            // 淡出音乐
            if (audioSource != null && audioSource.isPlaying)
            {
                StartCoroutine(FadeOutMusic(0.5f));
            }
            
            // 直接切换到场景8（建造场景）
            SceneManager.LoadScene("Level4_Building");
        }
        
        /// <summary>
        /// 淡出背景音乐
        /// </summary>
        IEnumerator FadeOutMusic(float duration)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }
            
            audioSource.Stop();
        }
    }
}
