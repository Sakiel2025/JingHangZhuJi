using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Level3.UI; // 添加UI命名空间引用

namespace Level3_YangzhouPavilion
{
    /// <summary>
    /// 扬州关卡的离线剧情对话。
    /// </summary>
    public class AIDialogueManager_Enhanced : MonoBehaviour
    {
        #region 数据结构定义
        
        [System.Serializable]
        public class Question
        {
            [Header("问题基础信息")]
            public string questionText;

            [Tooltip("提问时的配音音频（留空则不播放）")]
            public AudioClip voiceClip;
            
            [Header("问题背景")]
            [TextArea(3, 5)]
            public string context;
            
            [Header("预设回答")]
            [TextArea(2, 4)]
            public string shallowAnswer;
            [TextArea(2, 4)]
            public string mediumAnswer;
            [TextArea(2, 4)]
            public string deepAnswer;
            [TextArea(2, 4)]
            public string profoundAnswer;
        }
        
        public enum ResponseLevel
        {
            Shallow,    // 肤浅
            Medium,     // 中等
            Deep,       // 深刻
            Profound    // 深邃
        }
        
        #endregion
        
        #region 配置参数
        
        [Header("=== 问题列表 ===")]
        public List<Question> questions = new List<Question>();
        
        [Header("=== 奖励配置 ===")]
        public int shallowReward = 50;
        public int mediumReward = 100;
        public int deepReward = 200;
        public int profoundReward = 300;
        
        [Header("=== UI组件引用（可选）===")]
        [Tooltip("如果有自定义UI可以关联，留空则通过事件系统通知")]
        public DialogueBoxUI dialogueUI;
        
        [Header("=== 事件系统 ===")]
        [Tooltip("当提出问题时触发，参数：问题文本")]
        public UnityEvent<string> OnQuestionAsked;
        
        [Tooltip("当回答被评价后触发，参数：等级, 反馈文本")]
        public UnityEvent<ResponseLevel, string> OnResponseEvaluated;
        
        [Tooltip("当发放奖励时触发，参数：银两, 声望, 匠心值")]
        public UnityEvent<int, int, int> OnRewardGranted;
        
        [Tooltip("当对话结束时触发")]
        public UnityEvent OnDialogueComplete;
        
        [Tooltip("当显示最终评价时触发，参数：评价文本")]
        public UnityEvent<string> OnShowFinalFeedback;
        
        #endregion
        
        #region 状态变量
        
        private int currentQuestionIndex = 0;
        private bool isProcessing = false;
        
        private int profoundAnswerCount = 0;
        private int deepAnswerCount = 0;
        
        #endregion
        
        #region 初始化
        
        private void Start()
        {
            
            // 可选的UI引用（如果场景中有DialogueBoxUI）
            if (dialogueUI == null)
            {
                dialogueUI = FindObjectOfType<DialogueBoxUI>();
            }
        }
        
        #endregion
        
        #region 对话流程控制
        
        public void StartDialogue()
        {
            currentQuestionIndex = 0;
            profoundAnswerCount = 0;
            deepAnswerCount = 0;
            
            ShowNextQuestion();
        }
        
        private void ShowNextQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                EndDialogue();
                return;
            }
            
            Question currentQuestion = questions[currentQuestionIndex];
            
            Debug.Log($"📢 问题 {currentQuestionIndex + 1}/{questions.Count}: {currentQuestion.questionText}");
            
            // 通过UIManager显示问题（需要输入）
            JingHangZhuJi.UIManager uiManager = JingHangZhuJi.UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.ShowDialogue("黄老爷", currentQuestion.questionText, true, null, currentQuestion.voiceClip); // needInput = true
            }
            else
            {
                Debug.LogWarning("⚠️ UIManager not found!");
            }
            
            // 触发事件
            OnQuestionAsked?.Invoke(currentQuestion.questionText);
            
            // 如果有直接引用的UI，也可以调用
            if (dialogueUI != null)
            {
                dialogueUI.Show("黄老爷", currentQuestion.questionText);
            }
        }
        
        /// <summary>
        /// 供外部调用的方法：提交玩家回答
        /// </summary>
        public void SubmitAnswer(string answer)
        {

            if (isProcessing || currentQuestionIndex >= questions.Count)
                return;
            
            StartCoroutine(ShowPresetFeedback());
        }
        
        private IEnumerator ShowPresetFeedback()
        {
            isProcessing = true;
            Question currentQuestion = questions[currentQuestionIndex];
            
            const ResponseLevel level = ResponseLevel.Medium;
            string feedback = string.IsNullOrWhiteSpace(currentQuestion.mediumAnswer)
                ? "老夫记下了。我们接着聊吧。"
                : currentQuestion.mediumAnswer;

            // 显示反馈（不需要输入，显示继续按钮）
            JingHangZhuJi.UIManager uiManager = JingHangZhuJi.UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.ShowDialogue("黄老爷", feedback, false, () => {
                    // 玩家点击"继续"后，进入下一题
                    Debug.Log("✅ 玩家点击了继续按钮，准备进入下一题");
                    currentQuestionIndex++;
                    isProcessing = false;
                    ShowNextQuestion();
                });
            }
            else
            {
                Debug.LogWarning("⚠️ UIManager not found!");
                // 没有UIManager的情况下，自动等待3秒进入下一题
                yield return new WaitForSeconds(3f);
                currentQuestionIndex++;
                isProcessing = false;
                ShowNextQuestion();
                yield break;
            }
            
            OnResponseEvaluated?.Invoke(level, feedback);
            
            if (dialogueUI != null)
            {
                dialogueUI.Show("黄老爷", feedback);
            }
            
            Debug.Log($"💬 黄老爷反馈: {feedback}");
            
            // 发放奖励
            int reward = GetReward(level);
            GrantReward(reward, level);
            
            // ⚠️ 注意：不再自动等待，而是等待玩家点击"继续"按钮
            // 继续逻辑已移到上面的回调函数中
        }
        
        private void EndDialogue()
        {
            string finalFeedback = GenerateFinalFeedback();
            
            Debug.Log($"🎊 对话结束！最终评价:\n{finalFeedback}");
            
            // 通过UIManager显示最终评价（不需要输入，显示继续按钮）
            JingHangZhuJi.UIManager uiManager = JingHangZhuJi.UIManager.Instance;
            if (uiManager != null)
            {
                uiManager.ShowDialogue("黄老爷", finalFeedback, false, () => {
                    // 玩家点击"继续"后，关闭对话框并跳转到下一个场景
                    Debug.Log("✅ 玩家查看完最终评价，准备跳转到 Level3_Letter 场景");
                    uiManager.HideDialogue();
                    
                    // 跳转到下一个场景：Level3_Letter
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Level3_Letter");
                    
                    // 触发完成事件（在场景切换前）
                    OnDialogueComplete?.Invoke();
                });
            }
            else
            {
                Debug.LogWarning("⚠️ UIManager not found! 直接跳转场景");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Level3_Letter");
                OnDialogueComplete?.Invoke();
            }
            
            // 通过事件系统通知
            OnShowFinalFeedback?.Invoke(finalFeedback);
            
            if (dialogueUI != null)
            {
                dialogueUI.Show("黄老爷", finalFeedback);
            }
        }
        
        #endregion
        
        #region 奖励系统
        
        private int GetReward(ResponseLevel level)
        {
            switch (level)
            {
                case ResponseLevel.Profound: return profoundReward;
                case ResponseLevel.Deep: return deepReward;
                case ResponseLevel.Medium: return mediumReward;
                default: return shallowReward;
            }
        }
        
        private void GrantReward(int baseReward, ResponseLevel level)
        {
            int silver = baseReward;
            int reputation = baseReward / 10;
            int craftValue = (int)level * 10;
            
            // 这里可以调用实际的游戏货币系统
            // GameManager.Instance.AddSilver(silver);
            // GameManager.Instance.AddReputation(reputation);
            // GameManager.Instance.AddCraftValue(craftValue);
            
            OnRewardGranted?.Invoke(silver, reputation, craftValue);
            
            Debug.Log($"奖励发放: 银两+{silver}, 声望+{reputation}, 匠心值+{craftValue}");
        }
        
        #endregion
        
        #region 最终评价生成
        
        private string GenerateFinalFeedback()
        {
            StringBuilder feedback = new StringBuilder();
            
            // 基于回答质量的评价
            if (profoundAnswerCount >= 3)
            {
                feedback.Append("你对园林建造有着极为深刻的见解，让老夫刮目相看！");
            }
            else if (deepAnswerCount >= 3)
            {
                feedback.Append("你的思考很深入，假以时日定能成为一代名匠。");
            }
            else
            {
                feedback.Append("你的想法还需要更多磨练，多看多想多实践。");
            }
            
            return feedback.ToString();
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 获取统计数据
        /// </summary>
        public (int profound, int deep) GetStatistics()
        {
            return (profoundAnswerCount, deepAnswerCount);
        }
        
        /// <summary>
        /// 手动触发下一个问题（用于调试）
        /// </summary>
        public void DebugNextQuestion()
        {
            if (!isProcessing)
            {
                ShowNextQuestion();
            }
        }
        
        #endregion
    }
}
