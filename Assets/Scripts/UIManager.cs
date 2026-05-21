using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace JingHangZhuJi
{
    /// <summary>
    /// UI管理器 - 统一管理所有UI显示
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("通用UI")]
        [SerializeField] private GameObject messagePanel; // 消息面板
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float messageDuration = 3f;

        [Header("对话UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private GameObject inputPanel; // 输入面板
        [SerializeField] private TMP_InputField answerInputField;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button continueButton;











        [Header("进度UI")]
        [SerializeField] private Slider stabilityBar; // 稳定性进度条
        [SerializeField] private TextMeshProUGUI stabilityText;
        [SerializeField] private Slider progressBar; // 通用进度条
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("工具栏UI")]
        [SerializeField] private GameObject toolbarPanel;
        [SerializeField] private Button rulerButton; // 鲁班尺按钮
        [SerializeField] private Button craftsmanEyeButton; // 匠人之眼按钮
        [SerializeField] private Button jackButton; // 千斤顶按钮
        [SerializeField] private GameObject supportControlPanel; // 支撑控制面板

        [Header("支撑控制UI")]
        [SerializeField] private Button[] supportUpButtons = new Button[4];
        [SerializeField] private Button[] supportDownButtons = new Button[4];
        [SerializeField] private TextMeshProUGUI[] supportHeightTexts = new TextMeshProUGUI[4];

        [Header("关卡完成UI")]
        [SerializeField] private GameObject levelCompletePanel;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;

        [Header("奖励UI")]
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private TextMeshProUGUI rewardText;

        private Coroutine messageCoroutine;
        private System.Action currentDialogueCallback;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 设置为全局单例，切换场景时不销毁
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (answerInputField != null
                    && answerInputField.gameObject.activeInHierarchy
                    && !string.IsNullOrEmpty(answerInputField.text.Trim()))
                {
                    OnSubmitAnswer();
                }
            }
        }


        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            // 隐藏所有面板
            if (messagePanel != null) messagePanel.SetActive(false);
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (inputPanel != null) inputPanel.SetActive(false);
            if (supportControlPanel != null) supportControlPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (rewardPanel != null) rewardPanel.SetActive(false);

            // 绑定按钮事件
            if (submitButton != null)
                submitButton.onClick.AddListener(OnSubmitAnswer);
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueDialogue);
        }

        #region 消息显示

        /// <summary>
        /// 显示临时消息
        /// </summary>
        public void ShowMessage(string message, float duration = -1f)
        {
            if (messagePanel == null || messageText == null) return;

            if (messageCoroutine != null)
                StopCoroutine(messageCoroutine);

            messageText.text = message;
            messagePanel.SetActive(true);

            float actualDuration = duration > 0 ? duration : messageDuration;
            messageCoroutine = StartCoroutine(HideMessageAfterDelay(actualDuration));
        }

        private IEnumerator HideMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (messagePanel != null)
                messagePanel.SetActive(false);
        }

        #endregion

        #region 对话系统

        /// <summary>
        /// 显示对话
        /// </summary>
        public void ShowDialogue(string speakerName, string dialogue, bool needInput, System.Action onContinue = null)
        {
            currentDialogueCallback = onContinue;

            if (dialoguePanel == null) return;
            dialoguePanel.SetActive(true);

            // 强制 CanvasGroup 可见（如有）
            CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
            if (cg != null) { cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true; }

            if (speakerNameText != null) speakerNameText.text = speakerName;
            if (dialogueText != null) dialogueText.text = dialogue;

            // 控制 inputPanel 显示
            if (inputPanel != null && inputPanel != dialoguePanel)
            {
                if (inputPanel.transform.IsChildOf(dialoguePanel.transform))
                {
                    if (!inputPanel.activeSelf) inputPanel.SetActive(true);
                    CanvasGroup inputCG = inputPanel.GetComponent<CanvasGroup>();
                    if (inputCG == null) inputCG = inputPanel.AddComponent<CanvasGroup>();
                    inputCG.alpha = needInput ? 1f : 0f;
                    inputCG.interactable = needInput;
                    inputCG.blocksRaycasts = needInput;
                }
                else
                {
                    inputPanel.SetActive(needInput);
                }
            }
            else if (answerInputField != null)
            {
                answerInputField.gameObject.SetActive(needInput);
            }

            // 激活输入框
            if (needInput && answerInputField != null)
            {
                if (!answerInputField.gameObject.activeSelf)
                    answerInputField.gameObject.SetActive(true);

                answerInputField.text = "";
                answerInputField.interactable = true;
                if (submitButton != null) submitButton.interactable = true;

                answerInputField.ActivateInputField();
            }

            // 控制按钮
            if (submitButton != null) submitButton.gameObject.SetActive(needInput);
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(!needInput);
                TextMeshProUGUI btnText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = "点击此处继续";
            }
        }

        public void HideDialogue()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            currentDialogueCallback = null;
        }

        /// <summary>
        /// 仅更新对话框文字，不改变输入框/按钮状态（用于等待AI期间轮播提示）
        /// </summary>
        public void UpdateDialogueText(string speakerName, string text)
        {
            if (speakerNameText != null) speakerNameText.text = speakerName;
            if (dialogueText != null) dialogueText.text = text;
        }

        /// <summary>
        /// 提交答案（按回车键触发）
        /// </summary>
        private void OnSubmitAnswer()
        {
            if (answerInputField == null) return;

            string answer = answerInputField.text.Trim();
            if (string.IsNullOrEmpty(answer))
            {
                ShowMessage("请输入你的想法");
                return;
            }

            // 立即禁用输入框和提交按钮，防止重复提交
            answerInputField.interactable = false;
            if (submitButton != null) submitButton.interactable = false;

            // 优先查找新版增强对话管理器
            Level3_YangzhouPavilion.AIDialogueManager_Enhanced enhancedManager =
                FindObjectOfType<Level3_YangzhouPavilion.AIDialogueManager_Enhanced>();

            if (enhancedManager != null)
            {
                enhancedManager.SubmitAnswer(answer);
            }
            else
            {
                JingHangZhuJi.Level3.AIDialogueManager dialogueManager =
                    FindObjectOfType<JingHangZhuJi.Level3.AIDialogueManager>();
                if (dialogueManager != null)
                    dialogueManager.SubmitAnswer(answer);
            }

            answerInputField.text = "";
        }

        /// <summary>
        /// 继续对话
        /// </summary>
        private void OnContinueDialogue()
        {
            if (currentDialogueCallback != null)
            {
                System.Action callback = currentDialogueCallback;
                currentDialogueCallback = null;
                callback.Invoke();
            }
        }

        #endregion

        #region 进度显示

        /// <summary>
        /// 更新稳定性进度条
        /// </summary>
        public void UpdateStabilityBar(float value)
        {
            if (stabilityBar != null)
            {
                stabilityBar.value = value / 100f;
            }

            if (stabilityText != null)
            {
                stabilityText.text = $"稳定性: {value:F1}%";
                
                // 根据稳定性改变颜色
                if (value >= 95f)
                    stabilityText.color = Color.green;
                else if (value >= 70f)
                    stabilityText.color = Color.yellow;
                else
                    stabilityText.color = Color.red;
            }
        }

        /// <summary>
        /// 更新通用进度
        /// </summary>
        public void UpdateProgress(string label, float progress)
        {
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{label}: {(progress * 100f):F0}%";
            }
        }

        /// <summary>
        /// 隐藏进度条
        /// </summary>
        public void HideProgress()
        {
            if (progressBar != null)
                progressBar.gameObject.SetActive(false);
        }

        #endregion

        #region 支撑控制UI

        /// <summary>
        /// 显示支撑控制面板
        /// </summary>
        public void ShowSupportControlPanel(GameObject[] jacks, System.Action<int, bool> onAdjust)
        {
            if (supportControlPanel == null) return;

            supportControlPanel.SetActive(true);

            // 绑定按钮
            for (int i = 0; i < 4; i++)
            {
                int index = i; // 闭包捕获
                
                if (supportUpButtons[i] != null)
                {
                    supportUpButtons[i].onClick.RemoveAllListeners();
                    supportUpButtons[i].onClick.AddListener(() => onAdjust?.Invoke(index, true));
                }

                if (supportDownButtons[i] != null)
                {
                    supportDownButtons[i].onClick.RemoveAllListeners();
                    supportDownButtons[i].onClick.AddListener(() => onAdjust?.Invoke(index, false));
                }
            }
        }

        /// <summary>
        /// 更新支撑点高度显示
        /// </summary>
        public void UpdateSupportHeight(int index, float height)
        {
            if (index >= 0 && index < 4 && supportHeightTexts[index] != null)
            {
                supportHeightTexts[index].text = $"{height:F2}m";
            }
        }

        /// <summary>
        /// 隐藏支撑控制面板
        /// </summary>
        public void HideSupportControlPanel()
        {
            if (supportControlPanel != null)
                supportControlPanel.SetActive(false);
        }

        #endregion

        #region 工具栏

        /// <summary>
        /// 显示操作按钮
        /// </summary>
        public void ShowActionButton(string label, System.Action onClick)
        {
            // 创建临时按钮
            GameObject buttonObj = new GameObject("ActionButton");
            buttonObj.transform.SetParent(toolbarPanel.transform, false);
            
            Button button = buttonObj.AddComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = label;
            buttonText.fontSize = 24;
            buttonText.alignment = TextAlignmentOptions.Center;

            button.onClick.AddListener(() => {
                onClick?.Invoke();
                Destroy(buttonObj);
            });
        }

        /// <summary>
        /// 设置工具按钮状态（解锁/锁定）
        /// </summary>
        public void SetToolUnlocked(string toolName, bool unlocked)
        {
            Button targetButton = null;

            switch (toolName.ToLower())
            {
                case "craftsmaneye":
                case "匠人之眼":
                    targetButton = craftsmanEyeButton;
                    break;
                case "ruler":
                case "鲁班尺":
                    targetButton = rulerButton;
                    break;
                case "jack":
                case "千斤顶":
                    targetButton = jackButton;
                    break;
            }

            if (targetButton != null)
            {
                targetButton.interactable = unlocked;
                // 可以添加视觉效果，比如灰度化
                var image = targetButton.GetComponent<Image>();
                if (image != null)
                {
                    image.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
                }
            }
        }

        /// <summary>
        /// 绑定匠人之眼按钮
        /// </summary>
        public void SetupCraftsmanEyeButton(System.Action onClicked)
        {
            if (craftsmanEyeButton != null)
            {
                craftsmanEyeButton.onClick.RemoveAllListeners();
                craftsmanEyeButton.onClick.AddListener(() => onClicked?.Invoke());
            }
        }

        /// <summary>
        /// 绑定鲁班尺按钮
        /// </summary>
        public void SetupRulerButton(System.Action onClicked)
        {
            if (rulerButton != null)
            {
                rulerButton.onClick.RemoveAllListeners();
                rulerButton.onClick.AddListener(() => onClicked?.Invoke());
            }
        }

        /// <summary>
        /// 绑定千斤顶按钮
        /// </summary>
        public void SetupJackButton(System.Action onClicked)
        {
            if (jackButton != null)
            {
                jackButton.onClick.RemoveAllListeners();
                jackButton.onClick.AddListener(() => onClicked?.Invoke());
            }
        }

        /// <summary>
        /// 高亮显示对象
        /// </summary>
        public void HighlightObject(GameObject obj)
        {
            // 添加高亮效果（可以使用Outline组件或粒子特效）
            Debug.Log($"高亮对象: {obj.name}");
            // TODO: 实现高亮效果
        }

        /// <summary>
        /// 添加交互按钮（3D世界空间）
        /// </summary>
        public void AddInteractionButton(Transform target, string label, System.Action onClick)
        {
            // 创建世界空间UI按钮
            Debug.Log($"为 {target.name} 添加交互按钮: {label}");
            // TODO: 实现3D交互按钮
        }

        #endregion

        #region 奖励显示













        /// <summary>
        /// 显示奖励
        /// </summary>
        public void ShowReward(string reward)
        {
            if (rewardPanel == null || rewardText == null) return;

            rewardText.text = reward;
            rewardPanel.SetActive(true);

            StartCoroutine(HideRewardAfterDelay(2f));
        }

        private IEnumerator HideRewardAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
        }

        #endregion

        #region 关卡完成

        /// <summary>
        /// 显示关卡完成界面
        /// </summary>
        public void ShowLevelComplete(string levelName, string grade, int score, float time)
        {
            if (levelCompletePanel == null) return;

            levelCompletePanel.SetActive(true);

            if (levelNameText != null)
                levelNameText.text = levelName;

            if (gradeText != null)
            {
                gradeText.text = $"评级: {grade}";
                
                // 根据评级设置颜色
                if (grade.Contains("S"))
                    gradeText.color = new Color(1f, 0.84f, 0f); // 金色
                else if (grade.Contains("A"))
                    gradeText.color = new Color(0.75f, 0.75f, 0.75f); // 银色
                else
                    gradeText.color = Color.white;
            }

            if (scoreText != null)
                scoreText.text = $"得分: {score}";

            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"用时: {minutes:00}:{seconds:00}";
            }

            // 绑定按钮
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(OnNextLevel);
            }

            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(OnRetry);
            }
        }

        private void OnNextLevel()
        {
            // 加载下一关
            GameManager.Instance?.LoadLevel(4);
        }

        private void OnRetry()
        {
            // 重新开始当前关卡
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        #endregion
    }
}
