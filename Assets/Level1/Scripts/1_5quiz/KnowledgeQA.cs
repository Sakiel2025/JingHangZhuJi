using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic; // 用于存储选中的选项

// 定义题目类型（单选/多选）和当前题目索引
public enum QuestionState { Question1, Question2, Question3 }

public class KnowledgeQA : MonoBehaviour
{
    // UI组件（TMP版）
    [Header("UI元素（TMP版）")]
    public TextMeshProUGUI questionText;       // 题目文本框（TMP）
    public Button[] optionButtons;            // 选项按钮（4个，对应A/B/C/D）
    public TextMeshProUGUI[] optionTexts;     // 选项文字（TMP）
    public TextMeshProUGUI feedbackText;      // 正确/错误提示文本（TMP）

    // 新增：存储多选时玩家选中的选项索引
    private List<int> selectedOptions = new List<int>();
    // 题目数据
    private QuestionData[] allQuestions;
    private QuestionState currentQuestion;  
    private Color originalColor;            
    private bool isAnswering = false;       

    // 初始化
    void Start()
    {
        // 安全校验
        if (optionButtons == null || optionButtons.Length != 4)
        {
            Debug.LogError("请绑定4个选项按钮！");
            return;
        }
        if (optionTexts == null || optionTexts.Length != 4)
        {
            Debug.LogError("请绑定4个选项文本！");
            return;
        }
        
        // 保存按钮原始颜色
        originalColor = optionButtons[0].image.color;
        
        // 初始化题目数据
        InitQuestions();
        
        // 默认显示第一题
        ShowQuestion(QuestionState.Question1);
        
        // 为所有4个按钮绑定点击事件（修复前两个选项无法选中的核心）
        for (int i = 0; i < 4; i++) // 强制循环4次，确保4个按钮都绑定
        {
            int index = i;
            // 先清空原有事件，防止重复绑定
            optionButtons[i].onClick.RemoveAllListeners();
            // 绑定新的点击事件
            optionButtons[i].onClick.AddListener(() => OnOptionClick(index));
        }
    }

    // 初始化题目数据
    private void InitQuestions()
    {
        allQuestions = new QuestionData[]
        {
            // 第1题：单选 太和殿木材
            new QuestionData
            {
                question = "故宫太和殿的主要大梁使用的是哪种珍贵木材？",
                options = new string[] { "松木", "金丝楠木", "红木", "榆木" },
                correctAnswers = new int[] { 1 }, // B
                isMultiple = false,
                correctDelay = 2f,
                wrongDelay = 1f
            },
            // 第2题：多选 斗拱作用（四选三，正确A/B/C）
            new QuestionData
            {
                question = "以下哪些属于古建筑中斗拱的作用？",
                options = new string[] { "承重受力", "装饰美观", "抗震缓冲", "防火防水" },
                correctAnswers = new int[] { 0, 1, 2 }, // A/B/C
                isMultiple = true,
                correctDelay = 2f,
                wrongDelay = 1f
            },
            // 第3题：单选 斗拱构件
            new QuestionData
            {
                question = "在太和殿斗拱中，你找到师父刻字的构件是？",
                options = new string[] { "斗", "拱", "昂", "枋" },
                correctAnswers = new int[] { 2 }, // C
                isMultiple = false,
                correctDelay = 2f,
                wrongDelay = 1f
            }
        };
    }

    // 显示指定题目（重置所有状态）
    private void ShowQuestion(QuestionState state)
    {
        currentQuestion = state;
        isAnswering = false;
        selectedOptions.Clear(); // 清空选中的选项
        feedbackText.text = ""; 
        
        // 重置所有按钮：颜色、交互、选中状态
        for (int i = 0; i < 4; i++)
        {
            optionButtons[i].image.color = originalColor;
            optionButtons[i].interactable = true;
        }

        // 更新题目和选项文本
        int questionIndex = (int)currentQuestion;
        QuestionData qData = allQuestions[questionIndex];
        questionText.text = qData.question;
        for (int i = 0; i < 4; i++)
        {
            optionTexts[i].text = $"{(char)('A' + i)}. {qData.options[i]}";
        }
    }

    // 点击选项按钮（核心修复：区分单选/多选逻辑）
    private void OnOptionClick(int optionIndex)
    {
        int questionIndex = (int)currentQuestion;
        QuestionData qData = allQuestions[questionIndex];

        // 单选逻辑：只能选一个，点击即提交
        if (!qData.isMultiple)
        {
            if (isAnswering) return;
            isAnswering = true;

            // 判断是否正确
            bool isCorrect = qData.correctAnswers[0] == optionIndex;
            
            // 视觉反馈
            optionButtons[optionIndex].image.color = isCorrect ? Color.green : Color.red;
            feedbackText.text = isCorrect ? "回答正确！" : "回答错误！";
            feedbackText.color = isCorrect ? Color.green : Color.red;

            // 禁用所有按钮
            foreach (var btn in optionButtons) btn.interactable = false;

            // 延迟处理结果
            StartCoroutine(HandleAnswerResult(isCorrect, qData));
        }
        // 多选逻辑：可选中/取消选中多个选项，选满3个自动提交
        else
        {
            // 切换选中状态：选中→取消，未选中→选中
            if (selectedOptions.Contains(optionIndex))
            {
                // 取消选中：恢复原始颜色，移除索引
                selectedOptions.Remove(optionIndex);
                optionButtons[optionIndex].image.color = originalColor;
            }
            else
            {
                // 选中：标记为浅蓝色（区分选中状态），添加索引
                selectedOptions.Add(optionIndex);
                optionButtons[optionIndex].image.color = new Color(0.5f, 0.8f, 1f); // 浅蓝色
            }

            // 四选三：选中3个选项时，自动判定正误
            if (selectedOptions.Count == 3)
            {
                isAnswering = true;
                bool isCorrect = CheckMultipleAnswer(qData);

                // 视觉反馈：正确选项变绿，错误选项变红
                for (int i = 0; i < 4; i++)
                {
                    // 修复：用自定义的IsInArray方法替代Contains
                    if (IsInArray(qData.correctAnswers, i))
                    {
                        // 正确选项
                        optionButtons[i].image.color = selectedOptions.Contains(i) ? Color.green : Color.red;
                    }
                    else
                    {
                        // 错误选项（D）如果被选中，变红
                        if (selectedOptions.Contains(i))
                        {
                            optionButtons[i].image.color = Color.red;
                        }
                    }
                }

                feedbackText.text = isCorrect ? "回答正确！" : "回答错误！";
                feedbackText.color = isCorrect ? Color.green : Color.red;

                // 禁用所有按钮
                foreach (var btn in optionButtons) btn.interactable = false;

                // 延迟处理结果
                StartCoroutine(HandleAnswerResult(isCorrect, qData));
            }
        }
    }

    // 校验多选答案：必须选中所有正确选项（且未选错误选项）
    private bool CheckMultipleAnswer(QuestionData qData)
    {
        // 1. 选中的数量必须等于正确答案数量（3个）
        if (selectedOptions.Count != qData.correctAnswers.Length) return false;
        
        // 2. 选中的选项必须和正确答案完全一致
        foreach (int correctIndex in qData.correctAnswers)
        {
            // 修复：用自定义的IsInArray方法替代Contains
            if (!selectedOptions.Contains(correctIndex))
            {
                return false; // 漏选正确选项
            }
        }
        
        // 3. 不能选中错误选项（比如第四题D）
        foreach (int selectedIndex in selectedOptions)
        {
            // 修复：用自定义的IsInArray方法替代Contains
            if (!IsInArray(qData.correctAnswers, selectedIndex))
            {
                return false; // 选了错误选项
            }
        }

        return true; // 全部正确
    }

    // 处理答案结果
    private IEnumerator HandleAnswerResult(bool isCorrect, QuestionData qData)
    {
        float delay = isCorrect ? qData.correctDelay : qData.wrongDelay;
        yield return new WaitForSeconds(delay);

        if (isCorrect)
        {
            // 正确：跳下一题
            int nextQuestionIndex = (int)currentQuestion + 1;
            if (nextQuestionIndex < System.Enum.GetValues(typeof(QuestionState)).Length)
            {
                ShowQuestion((QuestionState)nextQuestionIndex);
            }
            else
            {
                // 全部完成
                questionText.text = "恭喜！所有问题回答正确！";
                feedbackText.text = "";
                foreach (var btn in optionButtons) btn.gameObject.SetActive(false);
            }
        }
        else
        {
            // 错误：刷新当前题
            ShowQuestion(currentQuestion);
        }
    }

    // 自定义辅助方法：判断整数数组是否包含某个值（替代Contains，解决语法错误）
    private bool IsInArray(int[] array, int value)
    {
        if (array == null || array.Length == 0) return false;
        foreach (int item in array)
        {
            if (item == value)
            {
                return true;
            }
        }
        return false;
    }

    // 题目数据类
    [System.Serializable]
    private class QuestionData
    {
        public string question;          
        public string[] options;         
        public int[] correctAnswers;     
        public bool isMultiple;          
        public float correctDelay;       
        public float wrongDelay;         
    }
}