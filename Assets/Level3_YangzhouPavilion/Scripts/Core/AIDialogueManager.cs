using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// AI对话系统 - 与黄老爷的深夜长谈
    /// </summary>
    public class AIDialogueManager : MonoBehaviour
    {
        [System.Serializable]
        public class Question
        {
            public string questionText;
            public string[] keywords; // 关键词列表
            public int minWords = 20; // 最少字数要求
            public string shallowResponse; // 浅层回答的反馈
            public string mediumResponse; // 中层回答的反馈
            public string deepResponse; // 深层回答的反馈
            public string profoundResponse; // 升华回答的反馈
        }

        public enum ResponseLevel
        {
            Shallow,    // 浅层（敷衍）
            Medium,     // 中层（基础理解）
            Deep,       // 深层（有思考）
            Profound    // 升华（见众生）
        }

        [Header("对话配置")]
        [SerializeField] private List<Question> questions = new List<Question>();
        [SerializeField] private int currentQuestionIndex = 0;

        [Header("奖励配置")]
        [SerializeField] private int shallowReward = 10; // 银两
        [SerializeField] private int mediumReward = 30;
        [SerializeField] private int deepReward = 50;
        [SerializeField] private int profoundReward = 100;
        
        // 声望奖励常量
        private const int PROFOUND_PRESTIGE_REWARD = 20;

        [Header("统计数据")]
        private Dictionary<ResponseLevel, int> responseStats = new Dictionary<ResponseLevel, int>();
        private int totalQuestions = 0;
        private bool hasUnlockedSpecialItem = false;

        // 事件
        public System.Action<Question> OnQuestionAsked;
        public System.Action<ResponseLevel, string> OnResponseEvaluated;
        public System.Action<int, int> OnRewardGiven; // 银两, 声望
        public System.Action OnDialogueComplete;

        private void Start()
        {
            InitializeQuestions();
            InitializeStats();
        }

        /// <summary>
        /// 初始化问题库 - 使用 Inspector 中配置的问题
        /// </summary>
        private void InitializeQuestions()
        {
            // 直接使用 Inspector 中配置的问题列表
            totalQuestions = questions.Count;
            
            Debug.Log($"[AIDialogueManager] 问题库初始化完成，共 {totalQuestions} 个问题");
            
            // 输出所有问题用于调试
            for (int i = 0; i < questions.Count; i++)
            {
                Debug.Log($"  问题 {i + 1}: {questions[i].questionText}");
            }
            
            // 检查是否有配置问题
            if (totalQuestions == 0)
            {
                Debug.LogError("[AIDialogueManager] ⚠️ 警告：没有配置任何问题！请在 Inspector 中的 Questions 列表添加问题。");
            }
        }

        /// <summary>
        /// 初始化统计
        /// </summary>
        private void InitializeStats()
        {
            responseStats[ResponseLevel.Shallow] = 0;
            responseStats[ResponseLevel.Medium] = 0;
            responseStats[ResponseLevel.Deep] = 0;
            responseStats[ResponseLevel.Profound] = 0;
        }

        /// <summary>
        /// 开始对话
        /// </summary>
        public void StartDialogue()
        {
            Debug.Log("=== AIDialogueManager.StartDialogue 被调用 ===");
            Debug.Log($"questions.Count = {questions.Count}");
            
            currentQuestionIndex = 0;
            AskNextQuestion();
        }

        /// <summary>
        /// 询问下一个问题
        /// </summary>
        public void AskNextQuestion()
        {
            Debug.Log($"=== AskNextQuestion 被调用，currentQuestionIndex = {currentQuestionIndex} ===");
            
            if (currentQuestionIndex >= questions.Count)
            {
                Debug.Log("所有问题已完成，调用 CompleteDialogue");
                CompleteDialogue();
                return;
            }

            Question currentQuestion = questions[currentQuestionIndex];
            Debug.Log($"当前问题: {currentQuestion.questionText}");
            
            OnQuestionAsked?.Invoke(currentQuestion);
            
            Debug.Log($"准备调用 UIManager.ShowDialogue");
            UIManager.Instance?.ShowDialogue(
                "黄老爷", 
                currentQuestion.questionText, 
                true // 需要玩家输入
            );
            Debug.Log("UIManager.ShowDialogue 调用完成");
        }

        /// <summary>
        /// 提交玩家的回答
        /// </summary>
        public void SubmitAnswer(string answerText)
        {
            if (string.IsNullOrEmpty(answerText))
            {
                UIManager.Instance?.ShowMessage("请输入你的想法");
                return;
            }

            Question currentQuestion = questions[currentQuestionIndex];
            ResponseLevel level = AnalyzeAnswer(answerText, currentQuestion);
            
            // 记录统计
            responseStats[level]++;

            // 获取反馈文本
            string feedback = GetFeedbackText(currentQuestion, level);
            
            // 触发事件
            OnResponseEvaluated?.Invoke(level, feedback);

            // 显示反馈（不需要输入，显示继续按钮）
            UIManager.Instance?.ShowDialogue("黄老爷", feedback, false);

            // 给予奖励
            GiveReward(level);

            // ⚠️ 修改：不自动等待，而是让玩家点击"继续"按钮来触发下一个问题
            // 需要在 UIManager 的 OnContinueDialogue 中调用 AskNextQuestion
            currentQuestionIndex++;
        }

        /// <summary>
        /// 分析玩家回答的深度
        /// </summary>
        private ResponseLevel AnalyzeAnswer(string answer, Question question)
        {
            // 1. 字数检查
            int wordCount = answer.Length;
            
            // 2. 关键词匹配
            int keywordMatchCount = 0;
            foreach (string keyword in question.keywords)
            {
                if (answer.Contains(keyword))
                {
                    keywordMatchCount++;
                }
            }

            // 3. 评估逻辑
            if (wordCount < question.minWords)
            {
                return ResponseLevel.Shallow; // 字数不足，敷衍
            }
            else if (keywordMatchCount == 0)
            {
                return ResponseLevel.Shallow; // 没有关键词，答非所问
            }
            else if (keywordMatchCount == 1 && wordCount < question.minWords * 1.5f)
            {
                return ResponseLevel.Medium; // 基础理解
            }
            else if (keywordMatchCount >= 2 && wordCount >= question.minWords * 1.5f)
            {
                return ResponseLevel.Deep; // 有深度思考
            }
            else if (keywordMatchCount >= 3 && wordCount >= question.minWords * 2f)
            {
                return ResponseLevel.Profound; // 升华
            }
            else
            {
                return ResponseLevel.Medium; // 默认中等
            }
        }

        /// <summary>
        /// 获取反馈文本
        /// </summary>
        private string GetFeedbackText(Question question, ResponseLevel level)
        {
            switch (level)
            {
                case ResponseLevel.Shallow:
                    return question.shallowResponse;
                case ResponseLevel.Medium:
                    return question.mediumResponse;
                case ResponseLevel.Deep:
                    return question.deepResponse;
                case ResponseLevel.Profound:
                    return question.profoundResponse;
                default:
                    return question.mediumResponse;
            }
        }

        /// <summary>
        /// 给予奖励
        /// </summary>
        private void GiveReward(ResponseLevel level)
        {
            int silver = 0;
            int prestige = 0;
            int craftHeart = 0; // 匠心值

            switch (level)
            {
                case ResponseLevel.Shallow:
                    silver = shallowReward;
                    break;
                case ResponseLevel.Medium:
                    silver = mediumReward;
                    prestige = 5;
                    craftHeart = 1;
                    break;
                case ResponseLevel.Deep:
                    silver = deepReward;
                    prestige = 10;
                    craftHeart = 3;
                    break;
                case ResponseLevel.Profound:
                    silver = profoundReward;
                    prestige = PROFOUND_PRESTIGE_REWARD;
                    craftHeart = 5;
                    // 解锁特殊物品
                    if (!hasUnlockedSpecialItem && responseStats[ResponseLevel.Profound] >= 2)
                    {
                        UnlockSpecialItem();
                    }
                    break;
            }

            // 通知奖励系统
            OnRewardGiven?.Invoke(silver, prestige);
            
            // 更新玩家资源
            GameManager.Instance?.AddSilver(silver);
            GameManager.Instance?.AddPrestige(prestige);
            GameManager.Instance?.AddCraftHeart(craftHeart);

            // 显示奖励UI
            string rewardText = $"获得：银两 +{silver}";
            if (prestige > 0) rewardText += $"，声望 +{prestige}";
            if (craftHeart > 0) rewardText += $"，匠心 +{craftHeart}";
            
            UIManager.Instance?.ShowReward(rewardText);
        }

        /// <summary>
        /// 解锁特殊物品
        /// </summary>
        private void UnlockSpecialItem()
        {
            hasUnlockedSpecialItem = true;
            UIManager.Instance?.ShowMessage("黄老爷赠予了你《黄氏藏书》！");
            GameManager.Instance?.UnlockItem("黄氏藏书");
            GameManager.Instance?.UnlockAchievement("见众生"); // 成就
            
            Debug.Log("解锁特殊物品：黄氏藏书");
        }

        /// <summary>
        /// 等待并继续下一个问题
        /// </summary>
        private IEnumerator WaitAndNextQuestion()
        {
            yield return new WaitForSeconds(2f);
            
            currentQuestionIndex++;
            AskNextQuestion();
        }

        /// <summary>
        /// 完成对话
        /// </summary>
        private void CompleteDialogue()
        {
            // 计算总体表现
            int profoundCount = responseStats[ResponseLevel.Profound];
            int deepCount = responseStats[ResponseLevel.Deep];

            string finalFeedback = "";
            if (profoundCount >= 2)
            {
                finalFeedback = "今夜与先生畅谈，如醍醐灌顶！老夫受益匪浅，这份谢礼，还请收下。";
                GameManager.Instance?.AddSilver(500);
            }
            else if (profoundCount + deepCount >= 2)
            {
                finalFeedback = "与匠人一席话，让老夫对匠人之道有了新的认识。";
                GameManager.Instance?.AddSilver(200);
            }
            else
            {
                finalFeedback = "今夜一谈，也算有所收获。时候不早了，各自安歇吧。";
                GameManager.Instance?.AddSilver(100);
            }

            UIManager.Instance?.ShowDialogue("黄老爷", finalFeedback, false);
            
            OnDialogueComplete?.Invoke();
            
            // 解锁下一阶段
            StartCoroutine(CompleteDialoguePhase());
        }

        private IEnumerator CompleteDialoguePhase()
        {
            yield return new WaitForSeconds(3f);
            
            Level3GameController.Instance?.OnDialoguePhaseComplete();
        }

        #region 公共接口

        public int GetCurrentQuestionIndex() => currentQuestionIndex;
        public int GetTotalQuestions() => totalQuestions;
        public Dictionary<ResponseLevel, int> GetResponseStats() => responseStats;
        public bool HasUnlockedSpecialItem() => hasUnlockedSpecialItem;

        #endregion

        #region 扩展：对接GPT API (可选)

        /// <summary>
        /// 使用GPT API进行高级分析（可选功能）
        /// </summary>
        /*
        private async Task<ResponseLevel> AnalyzeAnswerWithGPT(string answer, Question question)
        {
            // TODO: 调用GPT API
            string prompt = $@"
                问题：{question.questionText}
                玩家回答：{answer}
                
                请评估回答的深度（Shallow/Medium/Deep/Profound），
                考虑因素：是否有人文关怀、思考深度、文化理解。
            ";
            
            // 调用API并返回结果
            // ...
            
            return ResponseLevel.Medium;
        }
        */

        #endregion
    }
}
