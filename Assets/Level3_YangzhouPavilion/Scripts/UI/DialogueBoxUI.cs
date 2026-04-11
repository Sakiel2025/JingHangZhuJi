using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Level3.UI
{
    public class DialogueBoxUI : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private Image characterAvatar;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("打字机效果")]
        [SerializeField] private float typingSpeed = 0.05f;

        private Coroutine typingCoroutine;
        private Coroutine fadeCoroutine;
        private string fullText;

        void Awake()
        {
            // 自动获取CanvasGroup
            if (canvasGroup == null)
            {
                canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = dialogueBox.AddComponent<CanvasGroup>();
                }
            }
        }

        void Start()
        {
            Hide(); // 初始隐藏
        }

        public void Show(string speakerName, string dialogue, Sprite avatar = null)
        {
            dialogueBox.SetActive(true);
            speakerNameText.text = speakerName;
            fullText = dialogue;

            if (avatar != null && characterAvatar != null)
            {
                characterAvatar.sprite = avatar;
            }

            // 停止之前的打字机效果
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            // 直接显示完整文本，不使用打字机效果
            dialogueText.text = dialogue;

            // 淡入动画
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeIn());
        }

        IEnumerator TypeText(string text)
        {
            dialogueText.text = "";
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            typingCoroutine = null;
        }

        public void ShowInstant(string speakerName, string dialogue, Sprite avatar = null)
        {
            dialogueBox.SetActive(true);
            speakerNameText.text = speakerName;
            dialogueText.text = dialogue;
            fullText = dialogue;

            if (avatar != null && characterAvatar != null)
            {
                characterAvatar.sprite = avatar;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        public void Hide()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOut());
        }

        // 点击对话框加速或跳过
        public void OnDialogueBoxClicked()
        {
            if (typingCoroutine != null)
            {
                // 正在打字，立即显示完整文本
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                dialogueText.text = fullText;
            }
        }

        private IEnumerator FadeIn()
        {
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                }
                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private IEnumerator FadeOut()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                }
                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            dialogueBox.SetActive(false);
        }

        // 检查是否正在打字
        public bool IsTyping()
        {
            return typingCoroutine != null;
        }
    }
}
