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
            // 监听回车键提交答案
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("=== 检测到 Enter 键按下 ===");
                Debug.Log($"answerInputField 是否为空: {answerInputField == null}");
                if (answerInputField != null)
                {
                    Debug.Log($"输入框是否激活: {answerInputField.gameObject.activeInHierarchy}");
                    Debug.Log($"输入框是否有焦点: {answerInputField.isFocused}");
                    Debug.Log($"输入框内容: '{answerInputField.text}'");
                    
                    // 只要输入框激活且有内容，就允许提交（不检查焦点）
                    if (answerInputField.gameObject.activeInHierarchy && !string.IsNullOrEmpty(answerInputField.text.Trim()))
                    {
                        Debug.Log("✅ 满足提交条件，调用 OnSubmitAnswer");
                        OnSubmitAnswer();
                    }
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
            Debug.Log($"=== UIManager.ShowDialogue 被调用 ===");
            Debug.Log($"speakerName: {speakerName}");
            Debug.Log($"dialogue: {dialogue}");
            Debug.Log($"needInput: {needInput}");
            Debug.Log($"dialoguePanel 是否为空: {dialoguePanel == null}");
            Debug.Log($"inputPanel 是否为空: {inputPanel == null}");
            Debug.Log($"inputPanel 和 dialoguePanel 是否同一对象: {inputPanel == dialoguePanel}");
            if (inputPanel != null && dialoguePanel != null)
            {
                Debug.Log($"inputPanel 是否为 dialoguePanel 的子对象: {inputPanel.transform.IsChildOf(dialoguePanel.transform)}");
            }
            
            // 设置回调
            currentDialogueCallback = onContinue;
            
            if (dialoguePanel == null)
            {
                Debug.LogError("❌ dialoguePanel 为空！请在 UIManager 中配置 Dialogue Panel！");
                return;
            }

            dialoguePanel.SetActive(true);
            Debug.Log($"dialoguePanel 已激活");
            Debug.Log($"dialoguePanel 当前激活状态: {dialoguePanel.activeSelf}");
            Debug.Log($"dialoguePanel 在层级中是否激活: {dialoguePanel.activeInHierarchy}");
            
            // 临时调试：打印 DialogueBox 的信息
            RectTransform rect = dialoguePanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                Debug.Log($"DialogueBox 位置: {rect.anchoredPosition}, 大小: {rect.sizeDelta}");
                Debug.Log($"DialogueBox 缩放: {rect.localScale}, 父对象: {dialoguePanel.transform.parent.name}");
            }
            
            // 检查 CanvasGroup（可能导致透明）
            CanvasGroup cg = dialoguePanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                Debug.Log($"CanvasGroup alpha: {cg.alpha}, interactable: {cg.interactable}");
                cg.alpha = 1f; // 强制不透明
                cg.interactable = true; // 确保可交互
                cg.blocksRaycasts = true; // 确保可以接收点击
            }

            if (speakerNameText != null)
                speakerNameText.text = speakerName;
            else
                Debug.LogWarning("⚠️ speakerNameText 为空");

            if (dialogueText != null)
                dialogueText.text = dialogue;
            else
                Debug.LogWarning("⚠️ dialogueText 为空");

            // 根据是否需要输入显示不同的UI
            // 特别处理：如果inputPanel是dialoguePanel的子对象，使用CanvasGroup控制可见性
            if (inputPanel != null && inputPanel != dialoguePanel)
            {
                bool isChild = inputPanel.transform.IsChildOf(dialoguePanel.transform);
                
                if (isChild)
                {
                    // ⚠️ 重要：子对象必须先SetActive(true)，否则CanvasGroup无效
                    if (!inputPanel.activeSelf)
                    {
                        inputPanel.SetActive(true);
                        Debug.Log("inputPanel (子对象) 首次激活");
                    }
                    
                    // 使用CanvasGroup控制可见性（避免频繁SetActive导致的布局问题）
                    CanvasGroup inputCG = inputPanel.GetComponent<CanvasGroup>();
                    if (inputCG == null)
                        inputCG = inputPanel.AddComponent<CanvasGroup>();
                    
                    inputCG.alpha = needInput ? 1f : 0f;
                    inputCG.interactable = needInput;
                    inputCG.blocksRaycasts = needInput;
                    Debug.Log($"inputPanel (子对象) 使用CanvasGroup控制，alpha={inputCG.alpha}");
                }
                else
                {
                    // 如果是独立对象，直接控制激活状态
                    inputPanel.SetActive(needInput);
                    Debug.Log($"inputPanel (独立对象) 设置为: {needInput}");
                }
            }
            else if (answerInputField != null)
            {
                // 如果 inputPanel 就是 dialoguePanel 或为空，直接控制输入框
                answerInputField.gameObject.SetActive(needInput);
                Debug.Log($"直接控制 answerInputField: {needInput}");
            }
            else
            {
                Debug.LogWarning("⚠️ inputPanel 和 answerInputField 都无法使用");
            }
            
            // 如果需要输入，自动聚焦到输入框并清空内容
            if (needInput && answerInputField != null)
            {
                // ⚠️ 确保输入框本身也是激活的
                if (!answerInputField.gameObject.activeSelf)
                {
                    answerInputField.gameObject.SetActive(true);
                    Debug.Log("answerInputField 已激活");
                }
                
                answerInputField.text = ""; // 清空之前的内容
                answerInputField.ActivateInputField(); // 激活并聚焦
                Debug.Log("✅ 输入框已激活并聚焦");
            }
            
            // 控制按钮显示：
            // - needInput = true (需要输入)：隐藏所有按钮，玩家按 Enter 键提交
            // - needInput = false (显示反馈)：显示"点击此处继续"按钮
            if (submitButton != null)
            {
                submitButton.gameObject.SetActive(false); // 不使用提交按钮，按回车提交
            }
                
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(!needInput); // 只在反馈时显示继续按钮
                Debug.Log($"continueButton 显示状态: {!needInput}");
                
                // 修改继续按钮的文本为"点击此处继续"
                TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "点击此处继续";
                }
            }
            else
            {
                Debug.LogError("❌ continueButton 为空！请在 UIManager 中配置 Continue Button！");
            }
                
            Debug.Log("=== ShowDialogue 完成 ===");
            
            // ⚠️ 重要调试：延迟检查对话框是否仍然激活
            StartCoroutine(CheckDialoguePanelStatusAfterDelay());
        }
        
        private IEnumerator CheckDialoguePanelStatusAfterDelay()
        {
            yield return new WaitForSeconds(0.1f); // 等待一帧后检查
            Debug.Log($"[延迟检查] dialoguePanel 激活状态: {dialoguePanel != null && dialoguePanel.activeSelf}");
            if (dialoguePanel != null && !dialoguePanel.activeSelf)
            {
                Debug.LogError("❌❌❌ 对话框在显示后被关闭了！请检查是否有其他代码调用了 HideDialogue()");
            }
        }

        /// <summary>
        /// 隐藏对话
        /// </summary>
        public void HideDialogue()
        {
            Debug.LogError("=== ❌❌❌ HideDialogue 被调用 ===");
            Debug.LogError($"调用堆栈:\n{System.Environment.StackTrace}");
            
            if (dialoguePanel != null)
            {
                Debug.LogError($"即将关闭 dialoguePanel，当前状态: {dialoguePanel.activeSelf}");
                dialoguePanel.SetActive(false);
            }
        }

        /// <summary>
        /// 提交答案（按回车键触发）
        /// </summary>
        private void OnSubmitAnswer()
        {
            Debug.Log("=== OnSubmitAnswer 被调用 ===");
            
            if (answerInputField == null)
            {
                Debug.LogError("❌ answerInputField 为空！");
                return;
            }
            
            string answer = answerInputField.text.Trim();
            Debug.Log($"输入的答案: '{answer}' (长度: {answer.Length})");
            
            if (string.IsNullOrEmpty(answer))
            {
                Debug.LogWarning("⚠️ 答案为空，显示提示");
                ShowMessage("请输入你的想法");
                return;
            }
            
            Debug.Log($"✅ 答案有效，准备提交给 AIDialogueManager");
            
            // 通知对话管理器
            Level3.AIDialogueManager dialogueManager = FindObjectOfType<Level3.AIDialogueManager>();
            if (dialogueManager != null)
            {
                Debug.Log("找到 AIDialogueManager，调用 SubmitAnswer");
                dialogueManager.SubmitAnswer(answer);
            }
            else
            {
                Debug.LogError("❌ 找不到 AIDialogueManager！");
            }

            // 清空输入
            answerInputField.text = "";
        }

        /// <summary>
        /// 继续对话
        /// </summary>
        private void OnContinueDialogue()
        {
            Debug.Log("=== OnContinueDialogue 被调用 ===");
            Debug.Log($"currentDialogueCallback 是否为空: {currentDialogueCallback == null}");
            
            // ⭐ 优先级1: 如果有回调，先执行回调（用于开场对话等特殊对话）
            if (currentDialogueCallback != null)
            {
                Debug.Log("✅ 检测到回调，准备执行回调");
                Debug.Log("⚠️ 注意：不关闭对话框，让回调内部的 ShowDialogue() 处理");
                // ⚠️ 重要：不要先调用 HideDialogue()，让回调内部的 ShowDialogue() 自动处理
                // 因为 ShowDialogue() 会先激活 dialoguePanel，不会导致闪现
                System.Action callback = currentDialogueCallback;
                currentDialogueCallback = null; // 清空回调
                Debug.Log("📞 即将调用回调函数");
                callback.Invoke();
                Debug.Log("✅ 回调函数执行完毕");
                return; // ← 重要：执行完回调后立即返回，不继续执行后面的代码
            }
            
            // ⭐ 优先级2: 如果没有回调，查找 AIDialogueManager 并继续下一个问题
            Level3.AIDialogueManager dialogueManager = FindObjectOfType<Level3.AIDialogueManager>();
            if (dialogueManager != null)
            {
                Debug.Log("✅ 找到 AIDialogueManager，调用 AskNextQuestion");
                dialogueManager.AskNextQuestion();
            }
            else
            {
                Debug.Log("⚠️ 没有回调也没有 AIDialogueManager，关闭对话框");
                HideDialogue();
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
