// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/OptionSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; 

public class OptionSystem : MonoBehaviour
{
    [Header("绑定现有对话文本（拖拽赋值！）")]
    public TextMeshProUGUI sonText; // Dialog→Son→text（大儿子台词）
    public TextMeshProUGUI momText; // Dialog→Mom→text（寡妇台词）

    [Header("绑定寡妇感谢台词文本【新增！】")]
    public TextMeshProUGUI momThanksText1; // Dialog→MomThanksText1（第一句感谢）
    public TextMeshProUGUI momThanksText2; // Dialog→MomThanksText2（第二句感谢）

    [Header("绑定选项按钮（拖拽赋值！）")]
    public Button optionABtn; // OptionPanel→OptionABtn
    public Button optionBBtn; // OptionPanel→OptionBBtn
    public Button optionCBtn; // OptionPanel→OptionCBtn
    public GameObject optionPanel; // OptionPanel（整个选项容器）

    [Header("绑定立绘（可选）")]
    public Image sonImage; // 大儿子立绘
    public Image momImage; // 赵家寡妇立绘

    [Header("绑定奖励弹窗（新增！）")]
    public GameObject rewardPanel; // Canvas→RewardPanel
    public Button closeRewardBtn; // RewardPanel→CloseRewardBtn

    [Header("场景配置【新增！】")]
    public string nextSceneName = "RepairHouseScene"; // 下一个场景名称（直接在Inspector改）

    void Start()
    {
        // ========== 新增：空值检查（新手保护） ==========
        CheckReferences();

        // 初始状态：隐藏选项面板、奖励弹窗、寡妇感谢台词
        optionPanel.SetActive(false);
        rewardPanel.SetActive(false);
        HideMomThanksTexts(); // 隐藏感谢台词

        // 绑定按钮事件
        optionABtn.onClick.AddListener(OnOptionAClick);
        optionBBtn.onClick.AddListener(OnOptionBClick);
        optionCBtn.onClick.AddListener(OnOptionCClick);
        closeRewardBtn.onClick.AddListener(CloseRewardPanel);

        // 启动前置剧情
        StartCoroutine(ShowStoryBeforeOption());
    }

    // ========== 新增：空值检查（避免新手赋值遗漏报错） ==========
    private void CheckReferences()
    {
        if (sonText == null) Debug.LogError("sonText 未赋值！");
        if (momText == null) Debug.LogError("momText 未赋值！");
        if (momThanksText1 == null) Debug.LogError("momThanksText1 未赋值！");
        if (momThanksText2 == null) Debug.LogError("momThanksText2 未赋值！");
        if (optionABtn == null) Debug.LogError("optionABtn 未赋值！");
        if (optionBBtn == null) Debug.LogError("optionBBtn 未赋值！");
        if (optionCBtn == null) Debug.LogError("optionCBtn 未赋值！");
        if (optionPanel == null) Debug.LogError("optionPanel 未赋值！");
        if (rewardPanel == null) Debug.LogError("rewardPanel 未赋值！");
        if (closeRewardBtn == null) Debug.LogError("closeRewardBtn 未赋值！");
    }

    // 第一步：显示大儿子→寡妇的前置台词
    private IEnumerator ShowStoryBeforeOption()
    {
        // 大儿子台词
        sonText.text = "先生，求您收我为徒！";
        sonImage.gameObject.SetActive(true);
        momImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        // 寡妇台词
        momText.text = "大柱，别胡闹！";
        sonImage.gameObject.SetActive(true);
        momImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        // 显示选项面板
        sonText.gameObject.SetActive(true);
        momText.gameObject.SetActive(true);
        optionPanel.SetActive(true);
    }

    // 选项A/B/C通用逻辑：隐藏选项→显示寡妇感谢台词
    private void OnOptionAClick() => StartCoroutine(ShowMomThanksStory());
    private void OnOptionBClick() => StartCoroutine(ShowMomThanksStory());
    private void OnOptionCClick() => StartCoroutine(ShowMomThanksStory());

    // ========== 核心修改：显示寡妇感谢剧情（不再硬编码台词） ==========
    private IEnumerator ShowMomThanksStory()
    {
        // 隐藏选项面板
        HideOptionPanel();
        yield return new WaitForSeconds(0.5f);

        // 隐藏原有寡妇台词，显示立绘
        momText.gameObject.SetActive(false);
        momImage.gameObject.SetActive(true);

        // 显示第一句感谢台词（直接显示场景中预设的文本）
        momThanksText1.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        // 隐藏第一句，显示第二句感谢台词
        momThanksText1.gameObject.SetActive(false);
        momThanksText2.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        // 隐藏第二句感谢台词，弹出奖励提示
        momThanksText2.gameObject.SetActive(false);
        ShowRewardPanel();
    }

    // ========== 新增：隐藏寡妇感谢台词（通用方法） ==========
    private void HideMomThanksTexts()
    {
        if (momThanksText1 != null) momThanksText1.gameObject.SetActive(false);
        if (momThanksText2 != null) momThanksText2.gameObject.SetActive(false);
    }

    // 显示奖励弹窗
    private void ShowRewardPanel()
    {
        rewardPanel.SetActive(true);
        Debug.Log("获得物品：一封信、绣赵字的手帕、枣树种子");
        // 可选：自动关闭弹窗（5秒后）
        Invoke("CloseRewardPanel", 5f);
    }

    // 关闭奖励弹窗（手动/自动）
    private void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
        // 弹窗关闭后推进最终剧情（比如修复流程）
        FinalStoryStep();
    }

    // 隐藏选项面板（通用方法）
    private void HideOptionPanel()
    {
        optionPanel.SetActive(false);
        optionABtn.interactable = false;
        optionBBtn.interactable = false;
        optionCBtn.interactable = false;
    }

    // 最终剧情步骤（可自定义）
     private void FinalStoryStep()
    {
        Debug.Log("奖励获取完成，准备跳转到：" + nextSceneName);
        
        // 新手保护：先检查场景名是否为空
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("场景跳转失败！原因：nextSceneName未填写（请在Inspector面板设置）");
            return;
        }

        try
        {
            // 核心：加载指定场景
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"场景跳转失败！\n错误信息：{e.Message}\n排查步骤：\n1. 检查nextSceneName是否和Build Settings中的场景名一致（大小写敏感）\n2. 检查场景是否添加到Build Settings");
        }
    }
}