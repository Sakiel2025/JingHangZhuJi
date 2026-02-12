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
        /// 初始化问题库
        /// </summary>
        private void InitializeQuestions()
        {
            // 第一个问题：何为"好"
            questions.Add(new Question
            {
                questionText = "小墨匠人，你觉得什么样的建筑，才算是'好'的建筑？",
                keywords = new string[] { "人", "百姓", "生活", "实用", "文化", "传承", "精神", "意义" },
                minWords = 20,
                shallowResponse = "黄老爷微微摇头，继续品茶。",
                mediumResponse = "黄老爷点点头：'这样的理解，倒也不差。'",
                deepResponse = "黄老爷眼中露出赞许：'有见地。建筑确实应当服务于人。'",
                profoundResponse = "黄老爷起身拱手：'匠人能有此见识，实属难得！你不仅懂建筑，更懂百姓之心。'"
            });

            // 第二个问题：盐商之殇
            questions.Add(new Question
            {
                questionText = "这园子，我花了十年光景建造。如今却要倾塌...你说，这究竟是天意，还是人祸？",
                keywords = new string[] { "自然", "规律", "基础", "地基", "维护", "预防", "技术", "科学" },
                minWords = 25,
                shallowResponse = "黄老爷苦笑：'年轻人，话不能这么简单。'",
                mediumResponse = "黄老爷叹气：'技术固然重要，可又岂是全部？'",
                deepResponse = "黄老爷若有所思：'你说得对，许多灾祸本可避免。'",
                profoundResponse = "黄老爷眼眶湿润：'先生一语点醒梦中人！我这些年，只顾追求奢华，却忘了敬畏自然...'"
            });

            // 第三个问题：工匠与士人
            questions.Add(new Question
            {
                questionText = "你觉得，我们盐商与你们工匠，是否也如这亭子与地基，缺一不可？",
                keywords = new string[] { "相互", "依存", "尊重", "合作", "社会", "分工", "平等", "价值" },
                minWords = 30,
                shallowResponse = "黄老爷皱眉不语。",
                mediumResponse = "黄老爷点头：'确实如此。'",
                deepResponse = "黄老爷动容：'你这番话，让我重新思考士农工商的关系。'",
                profoundResponse = "黄老爷起身深鞠一躬：'今日一席话，胜读十年书！我辈商人虽有财，却缺了你这份见识。从今往后，我当更敬重匠人！'"
            });

            totalQuestions = questions.Count;
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
            currentQuestionIndex = 0;
            AskNextQuestion();
        }

        /// <summary>
        /// 询问下一个问题
        /// </summary>
        public void AskNextQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                CompleteDialogue();
                return;
            }

            Question currentQuestion = questions[currentQuestionIndex];
            OnQuestionAsked?.Invoke(currentQuestion);
            
            UIManager.Instance?.ShowDialogue(
                "黄老爷", 
                currentQuestion.questionText, 
                true // 需要玩家输入
            );
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

            // 显示反馈
            UIManager.Instance?.ShowDialogue("黄老爷", feedback, false);

            // 给予奖励
            GiveReward(level);

            // 等待玩家点击后继续
            StartCoroutine(WaitAndNextQuestion());
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
                finalFeedback = "与小墨匠人一席话，让老夫对匠人之道有了新的认识。";
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
