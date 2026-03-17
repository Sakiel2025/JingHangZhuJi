// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/OptionSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; 

public class OptionSystem : MonoBehaviour
{
    [Header("核心显示（只用这两个！）")]
    public TextMeshProUGUI dialogText; // 通用对话文本框（所有角色的话都显示在这里）
    public Image speakerImage; // 通用立绘显示框（显示当前说话人的立绘）

    [Header("立绘资源（拖拽赋值）")]
    public Sprite sonSprite; // 大儿子立绘图片
    public Sprite momSprite; // 赵家寡妇立绘图片

    [Header("绑定选项按钮")]
    public Button optionABtn; // OptionPanel→OptionABtn
    public Button optionBBtn; // OptionPanel→OptionBBtn
    public Button optionCBtn; // OptionPanel→OptionCBtn
    public GameObject optionPanel; // OptionPanel（整个选项容器）

    [Header("绑定奖励弹窗")]
    public GameObject rewardPanel; // Canvas→RewardPanel
    public Button closeRewardBtn; // RewardPanel→CloseRewardBtn

    [Header("场景配置")]
    public string nextSceneName = "RepairHouseScene"; // 下一个场景名称

    // 新增：寡妇感谢台词（直接在Inspector改，不用多文本框）
    [Header("对话内容（直接在这里改文字）")]
    public string sonFirstLine = "先生，求您收我为徒！";
    public string momFirstLine = "大柱，别胡闹！";
    public string momThanksLine1 = "多谢先生仗义相助！";
    public string momThanksLine2 = "这份恩情，赵家永世不忘！";

    void Start()
    {
        // 空值检查（新手保护）
        CheckReferences();

        // 初始状态：隐藏选项面板、奖励弹窗
        optionPanel.SetActive(false);
        rewardPanel.SetActive(false);

        // 绑定按钮事件
        optionABtn.onClick.AddListener(OnOptionAClick);
        optionBBtn.onClick.AddListener(OnOptionBClick);
        optionCBtn.onClick.AddListener(OnOptionCClick);
        closeRewardBtn.onClick.AddListener(CloseRewardPanel);

        // 启动前置剧情
        StartCoroutine(ShowStoryBeforeOption());
    }

    // 空值检查（避免新手赋值遗漏报错）
    private void CheckReferences()
    {
        if (dialogText == null) Debug.LogError("dialogText 未赋值！");
        if (speakerImage == null) Debug.LogError("speakerImage 未赋值！");
        if (sonSprite == null) Debug.LogError("sonSprite 未赋值！");
        if (momSprite == null) Debug.LogError("momSprite 未赋值！");
        if (optionABtn == null) Debug.LogError("optionABtn 未赋值！");
        if (optionBBtn == null) Debug.LogError("optionBBtn 未赋值！");
        if (optionCBtn == null) Debug.LogError("optionCBtn 未赋值！");
        if (optionPanel == null) Debug.LogError("optionPanel 未赋值！");
        if (rewardPanel == null) Debug.LogError("rewardPanel 未赋值！");
        if (closeRewardBtn == null) Debug.LogError("closeRewardBtn 未赋值！");
    }

    // 第一步：显示大儿子→寡妇的前置台词（核心：切换立绘+文字）
    private IEnumerator ShowStoryBeforeOption()
    {
        // 大儿子说话：显示他的立绘+他的话
        ShowSpeaker(sonSprite, sonFirstLine);
        yield return new WaitForSeconds(1f);

        // 寡妇说话：切换成她的立绘+她的话（自动隐藏大儿子的）
        ShowSpeaker(momSprite, momFirstLine);
        yield return new WaitForSeconds(1f);

        // 显示选项面板
        optionPanel.SetActive(true);
    }

    // 通用方法：切换说话人（核心！）
    private void ShowSpeaker(Sprite sprite, string text)
    {
        speakerImage.sprite = sprite; // 换立绘
        speakerImage.gameObject.SetActive(true); // 显示立绘框
        dialogText.text = text; // 换对话文字
        dialogText.gameObject.SetActive(true); // 显示文字框
    }

    // 选项A/B/C通用逻辑
    private void OnOptionAClick() => StartCoroutine(ShowMomThanksStory());
    private void OnOptionBClick() => StartCoroutine(ShowMomThanksStory());
    private void OnOptionCClick() => StartCoroutine(ShowMomThanksStory());

    // 显示寡妇感谢剧情（只用一个文本框切换）
    private IEnumerator ShowMomThanksStory()
    {
        // 隐藏选项面板
        HideOptionPanel();
        yield return new WaitForSeconds(0.5f);

        // 寡妇第一句感谢
        ShowSpeaker(momSprite, momThanksLine1);
        yield return new WaitForSeconds(1f);

        // 寡妇第二句感谢
        ShowSpeaker(momSprite, momThanksLine2);
        yield return new WaitForSeconds(2f);

        // 弹出奖励提示
        ShowRewardPanel();
    }

    // 显示奖励弹窗
    private void ShowRewardPanel()
    {
        rewardPanel.SetActive(true);
        Debug.Log("获得物品：一封信、绣赵字的手帕、枣树种子");
        // 可选：5秒后自动关闭弹窗
        Invoke("CloseRewardPanel", 5f);
    }

    // 关闭奖励弹窗（手动/自动）
    private void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
        // 弹窗关闭后推进最终剧情
        FinalStoryStep();
    }

    // 隐藏选项面板
    private void HideOptionPanel()
    {
        optionPanel.SetActive(false);
        optionABtn.interactable = false;
        optionBBtn.interactable = false;
        optionCBtn.interactable = false;
    }

    // 最终剧情步骤（场景跳转）
    private void FinalStoryStep()
    {
        Debug.Log("奖励获取完成，准备跳转到：" + nextSceneName);
        
        // 新手保护：检查场景名
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("场景跳转失败！原因：nextSceneName未填写");
            return;
        }

        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"场景跳转失败！\n错误信息：{e.Message}\n排查：1. 场景名大小写 2. 是否添加到Build Settings");
        }
    }
}