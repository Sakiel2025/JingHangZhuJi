// 保存路径：Scripts/RainEventSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RainEventSystem : MonoBehaviour
{
    [Header("选择题核心组件")]
    public TextMeshProUGUI countdownTMP;    // 倒计时文本
    public TextMeshProUGUI rainTipTMP;      // 提示/结果文本
    public Button optionABtn;               // 选项A按钮
    public Button optionBBtn;               // 选项B按钮
    public Button optionCBtn;               // 选项C按钮
    public TextMeshProUGUI optionATMP;      // 选项A文本
    public TextMeshProUGUI optionBTMP;      // 选项B文本
    public TextMeshProUGUI optionCTMP;      // 选项C文本

    // 核心变量（仅保留选择题相关）
    private float remainingTime = 30f;      // 倒计时时长
    private bool isDecisionMade = false;    // 是否已选择选项

    void Start()
    {
        // 空值检查（仅检查选择题核心组件）
        if (!CheckReferences())
        {
            Debug.LogError("[暴雨选择题系统] 关键组件未赋值！");
            return;
        }

        // 初始隐藏面板（可选，根据你的需求调整）
        gameObject.SetActive(false);

        // 绑定选项点击事件
        optionABtn.onClick.AddListener(OnOptionAClick);
        optionBBtn.onClick.AddListener(OnOptionBClick);
        optionCBtn.onClick.AddListener(OnOptionCClick);

        // 初始化选项文本
        optionATMP.text = "方案A：油布遮挡工地";
        optionBTMP.text = "方案B：加速完成安装";
        optionCTMP.text = "方案C：先转移粮食";
    }

    // 简化版：仅检查选择题核心组件
    private bool CheckReferences()
    {
        bool isAllValid = true;
        if (countdownTMP == null) { Debug.LogError("countdownTMP 未赋值！"); isAllValid = false; }
        if (rainTipTMP == null) { Debug.LogError("rainTipTMP 未赋值！"); isAllValid = false; }
        if (optionABtn == null) { Debug.LogError("optionABtn 未赋值！"); isAllValid = false; }
        if (optionBBtn == null) { Debug.LogError("optionBBtn 未赋值！"); isAllValid = false; }
        if (optionCBtn == null) { Debug.LogError("optionCBtn 未赋值！"); isAllValid = false; }
        if (optionATMP == null) { Debug.LogError("optionATMP 未赋值！"); isAllValid = false; }
        if (optionBTMP == null) { Debug.LogError("optionBTMP 未赋值！"); isAllValid = false; }
        if (optionCTMP == null) { Debug.LogError("optionCTMP 未赋值！"); isAllValid = false; }
        return isAllValid;
    }

    // 倒计时更新（仅保留核心逻辑）
    void Update()
    {
        if (!isDecisionMade && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            int roundedTime = Mathf.RoundToInt(remainingTime);
            countdownTMP.text = $"倒计时：{roundedTime}秒";

            // 倒计时结束默认选A
            if (roundedTime <= 0)
            {
                OnOptionAClick();
            }
        }
    }

    // 外部调用：启动暴雨选择题（替代原有StartRainCountdown）
    public void StartRainChoice()
    {
        // 重置状态
        remainingTime = 30f;
        isDecisionMade = false;
        
        // 初始化文本
        countdownTMP.text = "倒计时：30秒";
        rainTipTMP.text = "暴雨将至！修到一半的房子该怎么办？";
        
        // 启用所有选项按钮
        optionABtn.interactable = true;
        optionBBtn.interactable = true;
        optionCBtn.interactable = true;
        
        // 激活面板（如果初始是隐藏的）
        gameObject.SetActive(true);
    }

    // 在RainEventSystem.cs中添加（放在StartRainChoice()方法下方）
    public void StartRainCountdown()
    {
        // 兼容旧调用，内部执行新方法
        StartRainChoice();
    }

    // 选项A点击逻辑（仅保留结果提示）
    private void OnOptionAClick()
    {
        isDecisionMade = true;
        rainTipTMP.text = "方案A：木料未受损，不过工期延长！";
        EndRainChoice(); // 结束选择题
    }

    // 选项B点击逻辑（仅保留结果提示）
    private void OnOptionBClick()
    {
        isDecisionMade = true;
        rainTipTMP.text = "方案B：冒雨加速安装，赶在大雨前完成了！";
        EndRainChoice(); // 结束选择题
    }

    // 选项C点击逻辑（仅保留结果提示）
    private void OnOptionCClick()
    {
        isDecisionMade = true;
        rainTipTMP.text = "方案C：优先转移赵家粮食，民心大涨！";
        EndRainChoice(); // 结束选择题
    }

    // 结束选择题（仅禁用按钮，无外部衔接）
    private void EndRainChoice()
    {
        // 禁用所有选项按钮
        optionABtn.interactable = false;
        optionBBtn.interactable = false;
        optionCBtn.interactable = false;
        
        // 可选：2秒后隐藏面板（根据你的需求调整，不想隐藏可删除这行）
        //Invoke("HideChoicePanel", 2f);
    }

    // 隐藏选择题面板（可选）
    private void HideChoicePanel()
    {
        gameObject.SetActive(false);
    }
}