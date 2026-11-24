// 保存路径：Scripts/RepairSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RepairSystem : MonoBehaviour
{
    [Header("TMP组件引用")]
    public TextMeshProUGUI repairTipTMP;
    public Button step1Btn;
    public Button step2Btn;
    public Button step3Btn;
    public Button step4Btn;
    public TextMeshProUGUI step1TMP;
    public TextMeshProUGUI step2TMP;
    public TextMeshProUGUI step3TMP;
    public TextMeshProUGUI step4TMP;

    [Header("暴雨事件引用")]
    public GameObject rainEventPanel;
    public RainEventSystem rainEventSystem;

    [Header("第三天剧情引用")]
    public EmotionSystem emotionSystem; // 新增：第三天情感系统引用

    private int currentStep = 0;

    void Start()
    {
        // ========== 核心修改1：增加空值检查，避免启动报错 ==========
        if (!CheckReferences())
        {
            Debug.LogError("[修复系统] 关键组件未赋值，修复面板无法正常工作！");
            return; // 有组件未赋值时，停止执行后续代码
        }

        // ========== 核心修改2：不在Start里隐藏面板，改为手动初始化 ==========
        // 移除 gameObject.SetActive(false); 避免覆盖后续激活

        // 绑定按钮事件
        step1Btn.onClick.AddListener(OnStep1Click);
        step2Btn.onClick.AddListener(OnStep2Click);
        step3Btn.onClick.AddListener(OnStep3Click);
        step4Btn.onClick.AddListener(OnStep4Click);

        // 初始化文本和按钮状态
        InitPanelState();

        // 初始化时主动隐藏面板（替代Start里的SetActive，更可控）
        gameObject.SetActive(false);
        Debug.Log("[修复系统] 面板初始化完成，初始状态：隐藏");
    }

    // ========== 新增：检查所有引用是否为空 ==========
    private bool CheckReferences()
    {
        bool isAllValid = true;
        if (repairTipTMP == null) { Debug.LogError("repairTipTMP 未赋值！"); isAllValid = false; }
        if (step1Btn == null) { Debug.LogError("step1Btn 未赋值！"); isAllValid = false; }
        if (step2Btn == null) { Debug.LogError("step2Btn 未赋值！"); isAllValid = false; }
        if (step3Btn == null) { Debug.LogError("step3Btn 未赋值！"); isAllValid = false; }
        if (step4Btn == null) { Debug.LogError("step4Btn 未赋值！"); isAllValid = false; }
        if (step1TMP == null) { Debug.LogError("step1TMP 未赋值！"); isAllValid = false; }
        if (step2TMP == null) { Debug.LogError("step2TMP 未赋值！"); isAllValid = false; }
        if (step3TMP == null) { Debug.LogError("step3TMP 未赋值！"); isAllValid = false; }
        if (step4TMP == null) { Debug.LogError("step4TMP 未赋值！"); isAllValid = false; }
        if (rainEventPanel == null) { Debug.LogError("rainEventPanel 未赋值！"); isAllValid = false; }
        if (rainEventSystem == null) { Debug.LogError("rainEventSystem 未赋值！"); isAllValid = false; }
        return isAllValid;
    }

    // ========== 新增：单独初始化面板状态 ==========
    private void InitPanelState()
    {
        // 初始化按钮文本
        step1TMP.text = "木料预处理";
        step2TMP.text = "支撑旧椽（防坍塌）";
        step3TMP.text = "鲁班尺测量尺寸";
        step4TMP.text = "安装新椽";

        // 初始禁用后续按钮
        step2Btn.interactable = false;
        step3Btn.interactable = false;
        step4Btn.interactable = false;

        // 初始化提示文本
        repairTipTMP.text = "请按步骤修复椽子：先完成木料预处理！";
    }

    // 外部调用：启动修复流程
    public void StartRepairProcess()
    {
        Debug.Log("[修复系统] 收到启动指令，当前面板激活状态：" + gameObject.activeSelf);
        
        // 强制激活+置顶+居中，确保显示
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // 放到Canvas最上层，不被遮挡
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 强制居中
        
        Debug.Log("[修复系统] 启动后面板激活状态：" + gameObject.activeSelf);
        currentStep = 0;
    }

    // 步骤1：木料预处理
    private void OnStep1Click()
    {
        currentStep = 1;
        repairTipTMP.text = "步骤1完成：木料预处理（去毛刺、晾干）！";
        step1Btn.interactable = false;
        step2Btn.interactable = true;
        GameManager.Instance?.ChangeValue("craft", 10); // 空值保护
    }

    // 步骤2：支撑旧椽（触发暴雨事件）
    private void OnStep2Click()
    {
        currentStep = 2;
        repairTipTMP.text = "步骤2完成：用工具套装支撑旧椽，防止坍塌！";
        step2Btn.interactable = false;
        step3Btn.interactable = true;

        Invoke("TriggerRainEvent", 1f);
        GameManager.Instance?.ChangeValue("craft", 10); // 空值保护
    }

    // 步骤3：鲁班尺测量
    private void OnStep3Click()
    {
        currentStep = 3;
        repairTipTMP.text = "步骤3完成：鲁班尺测量，尺寸精准无误！";
        step3Btn.interactable = false;
        step4Btn.interactable = true;
        GameManager.Instance?.ChangeValue("craft", 10); // 空值保护
    }

    // 步骤4：安装新椽
    private void OnStep4Click()
    {
        currentStep = 4;
        repairTipTMP.text = "修复完成！新椽安装到位，稳固耐用！";
        step4Btn.interactable = false;
        GameManager.Instance?.ChangeValue("craft", 10); // 空值保护

        Invoke("CloseRepairPanel", 2f);
    }

    // 触发暴雨事件
    private void TriggerRainEvent()
    {
        gameObject.SetActive(false);
        if (rainEventPanel != null) rainEventPanel.SetActive(true);
        if (rainEventSystem != null) rainEventSystem.StartRainCountdown(); // 空值保护
    }

    // 关闭修复面板
    private void CloseRepairPanel()
    {
        gameObject.SetActive(false);
        Debug.Log("[流程推进] 修复完成，准备进入情感选择阶段！");
        
        // 调用第三天情感剧情（新增核心代码）
        if (emotionSystem != null)
        {
            emotionSystem.StartEmotionProcess();
        }
        else
        {
            Debug.LogError("[修复系统] emotionSystem未赋值！请拖拽EmotionPanel的EmotionSystem组件");
        }
    }

    // 外部调用：暴雨事件结束后回到修复流程
    public void ResumeRepairProcess()
    {
        gameObject.SetActive(true);
        if (currentStep == 2)
        {
            repairTipTMP.text = "暴雨事件处理完成，继续修复：请进行鲁班尺测量！";
        }
    }
}