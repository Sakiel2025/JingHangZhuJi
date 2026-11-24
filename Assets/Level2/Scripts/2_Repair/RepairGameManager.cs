// 保存路径：Assets/Level2/Scripts/2_Repair/RepairGameManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RepairGameManager : MonoBehaviour
{
    [Header("步骤面板（仅两步）")]
    public GameObject step1Panel; // 第一步：木料预处理
    public GameObject step3Panel; // 第二步：测量安装
    public GameObject rainEventPanel; // 新增：暴雨突发事件面板

    [Header("进度条（适配两步）")]
    public Image progressFill; // 进度条填充
    public TextMeshProUGUI progressText; // 进度文本
    private int currentStep = 0; // 当前完成步骤数
    private int totalStep = 2; // 总步骤改为2

    [Header("提示（共用）")]
    public Image successTip; // 成功提示（Image类型）
    public Image failTip; // 失败提示（Image类型）

    [Header("场景切换")]
    public string mainSceneName = "MainScene"; // 主场景名称

    void Start()
    {
        // 初始化：隐藏突发事件面板
        rainEventPanel.SetActive(false);
        // 启动第一步
        StartGame();
    }

    // 启动小游戏（初始化）
    public void StartGame()
    {
        currentStep = 0;
        UpdateProgress();
        step1Panel.SetActive(true);
        step3Panel.SetActive(false);
        successTip.gameObject.SetActive(false);
        failTip.gameObject.SetActive(false);
    }

    // 第一步完成（麻绳拖拽成功）→ 新增：显示暴雨突发事件
    public void OnStep1Success()
    {
        currentStep++;
        UpdateProgress();
        ShowSuccessTip();
        // 1秒后显示暴雨突发事件面板（替换原来的SwitchToStep3）
        Invoke("ShowRainEvent", 1f);
    }

    // 新增：显示暴雨突发事件面板
    private void ShowRainEvent()
    {
        step1Panel.SetActive(false); // 隐藏第一步面板
        rainEventPanel.SetActive(true); // 显示突发事件面板
    }

    // 新增：点击“继续修复”按钮调用 → 切换到第二步
    public void OnContinueRepairClick()
    {
        rainEventPanel.SetActive(false); // 隐藏突发事件面板
        step3Panel.SetActive(true); // 显示第二步面板
    }

    // 第二步完成（角度校准成功）
    public void OnStep2Success()
    {
        currentStep++;
        UpdateProgress();
        ShowSuccessTip();
        // 小游戏完成，2秒后返回主场景
        Invoke("GameComplete", 2f);
    }

    // 更新进度条（适配两步）
    private void UpdateProgress()
    {
        float progress = (float)currentStep / totalStep;
        progressFill.fillAmount = progress;
        progressText.text = $"{currentStep}/{totalStep} 步骤完成";
    }

    // 显示成功提示（1秒后隐藏）
    public void ShowSuccessTip()
    {
        successTip.gameObject.SetActive(true);
        Invoke("HideTip", 1f);
    }

    // 显示失败提示（1秒后隐藏）
    public void ShowFailTip()
    {
        failTip.gameObject.SetActive(true);
        Invoke("HideTip", 1f);
    }

    // 隐藏提示
    private void HideTip()
    {
        successTip.gameObject.SetActive(false);
        failTip.gameObject.SetActive(false);
    }

    // 小游戏完成：直接返回主场景
    private void GameComplete()
    {
        Debug.Log("修复小游戏完成！返回主场景");
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneName);
    }
}