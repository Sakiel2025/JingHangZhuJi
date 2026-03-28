using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum QuestionState { Question1, Question2, Question3 }

public class KnowledgeQA : MonoBehaviour
{
    [Header("UI元素（TMP版）")]
    public TextMeshProUGUI questionText;       
    public Button[] optionButtons;            
    public TextMeshProUGUI[] optionTexts;     
    public TextMeshProUGUI feedbackText;     

    // ========== 新增1：场景切换变量 ==========
    [Header("场景切换")]
    public GameObject nextSceneBtn; // 前往下一场景的按钮
    public string nextSceneName;    // 目标场景名
    // ========================================

    private List<int> selectedOptions = new List<int>();
    private QuestionData[] allQuestions;
    private QuestionState currentQuestion;  
    private Color originalColor;            
    private bool isAnswering = false;       

    void Start()
    {
        // 你的原有初始化逻辑（完全不变）
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
        
        originalColor = optionButtons[0].image.color;
        InitQuestions();
        ShowQuestion(QuestionState.Question1);
        
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionClick(index));
        }
    }

    // 你的原有InitQuestions方法（完全不变）
    private void InitQuestions()
    {
        allQuestions = new QuestionData[]
        {
            new QuestionData
            {
                question = "故宫太和殿的主要大梁使用的是哪种珍贵木材？（单选）",
                options = new string[] { "松木", "金丝楠木", "红木", "榆木" },
                correctAnswers = new int[] { 1 },
                isMultiple = false,
                correctDelay = 2f,
                wrongDelay = 1f
            },
            new QuestionData
            {
                question = "以下哪些属于古建筑中斗拱的作用？（三选）",
                options = new string[] { "承重受力", "装饰美观", "抗震缓冲", "防火防水" },
                correctAnswers = new int[] { 0, 1, 2 },
                isMultiple = true,
                correctDelay = 2f,
                wrongDelay = 1f
            },
            new QuestionData
            {
                question = "在太和殿斗拱中，你找到师父刻字的构件是？（单选）",
                options = new string[] { "斗", "拱", "昂", "枋" },
                correctAnswers = new int[] { 2 },
                isMultiple = false,
                correctDelay = 2f,
                wrongDelay = 1f
            }
        };
    }

    // 你的原有ShowQuestion方法（完全不变）
    private void ShowQuestion(QuestionState state)
    {
        currentQuestion = state;
        isAnswering = false;
        selectedOptions.Clear();
        feedbackText.text = ""; 
        
        for (int i = 0; i < 4; i++)
        {
            optionButtons[i].image.color = originalColor;
            optionButtons[i].interactable = true;
        }

        int questionIndex = (int)currentQuestion;
        QuestionData qData = allQuestions[questionIndex];
        questionText.text = qData.question;
        for (int i = 0; i < 4; i++)
        {
            optionTexts[i].text = $"{(char)('A' + i)}. {qData.options[i]}";
        }
    }

    // 你的原有OnOptionClick方法（完全不变）
    private void OnOptionClick(int optionIndex)
    {
        int questionIndex = (int)currentQuestion;
        QuestionData qData = allQuestions[questionIndex];

        if (!qData.isMultiple)
        {
            if (isAnswering) return;
            isAnswering = true;

            bool isCorrect = qData.correctAnswers[0] == optionIndex;
            
            optionButtons[optionIndex].image.color = isCorrect ? Color.green : Color.red;
            feedbackText.text = isCorrect ? "回答正确！" : "回答错误！";
            feedbackText.color = isCorrect ? Color.green : Color.red;

            foreach (var btn in optionButtons) btn.interactable = false;

            StartCoroutine(HandleAnswerResult(isCorrect, qData));
        }
        else
        {
            if (selectedOptions.Contains(optionIndex))
            {
                selectedOptions.Remove(optionIndex);
                optionButtons[optionIndex].image.color = originalColor;
            }
            else
            {
                selectedOptions.Add(optionIndex);
                optionButtons[optionIndex].image.color = new Color(0.5f, 0.8f, 1f);
            }

            if (selectedOptions.Count == 3)
            {
                isAnswering = true;
                bool isCorrect = CheckMultipleAnswer(qData);

                for (int i = 0; i < 4; i++)
                {
                    if (IsInArray(qData.correctAnswers, i))
                    {
                        optionButtons[i].image.color = selectedOptions.Contains(i) ? Color.green : Color.red;
                    }
                    else
                    {
                        if (selectedOptions.Contains(i))
                        {
                            optionButtons[i].image.color = Color.red;
                        }
                    }
                }

                feedbackText.text = isCorrect ? "回答正确！" : "回答错误！";
                feedbackText.color = isCorrect ? Color.green : Color.red;

                foreach (var btn in optionButtons) btn.interactable = false;

                StartCoroutine(HandleAnswerResult(isCorrect, qData));
            }
        }
    }

    // 你的原有CheckMultipleAnswer方法（完全不变）
    private bool CheckMultipleAnswer(QuestionData qData)
    {
        if (selectedOptions.Count != qData.correctAnswers.Length) return false;
        
        foreach (int correctIndex in qData.correctAnswers)
        {
            if (!selectedOptions.Contains(correctIndex))
            {
                return false;
            }
        }
        
        foreach (int selectedIndex in selectedOptions)
        {
            if (!IsInArray(qData.correctAnswers, selectedIndex))
            {
                return false;
            }
        }

        return true;
    }

    // 你的原有HandleAnswerResult方法（仅改“全部完成”部分）
    private IEnumerator HandleAnswerResult(bool isCorrect, QuestionData qData)
    {
        float delay = isCorrect ? qData.correctDelay : qData.wrongDelay;
        yield return new WaitForSeconds(delay);

        if (isCorrect)
        {
            int nextQuestionIndex = (int)currentQuestion + 1;
            if (nextQuestionIndex < System.Enum.GetValues(typeof(QuestionState)).Length)
            {
                ShowQuestion((QuestionState)nextQuestionIndex);
            }
            else
            {
                // ========== 新增2：显示按钮 ==========
                questionText.text = "恭喜！所有问题回答正确！";
                feedbackText.text = "";
                foreach (var btn in optionButtons) btn.gameObject.SetActive(false);
                // 显示前往下一场景按钮
                if (nextSceneBtn != null)
                {
                    nextSceneBtn.SetActive(true);
                }
                // =====================================
            }
        }
        else
        {
            ShowQuestion(currentQuestion);
        }
    }

    // 你的原有IsInArray方法（完全不变）
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

    // ========== 新增3：场景切换方法 ==========
    // 点击按钮跳转到下一场景
    public void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("请填写nextSceneName！");
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
    // ========================================

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