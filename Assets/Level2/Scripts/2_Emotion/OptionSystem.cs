// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/OptionSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class OptionSystem : MonoBehaviour
{
    [Header("绑定现有对话文本（拖拽赋值！）")]
    public TextMeshProUGUI sonText; // Dialog→Son→text（大儿子台词）
    public TextMeshProUGUI momText; // Dialog→Mom→text（寡妇台词）

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

    void Start()
    {
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

    // 第二步：显示赵家寡妇感谢的剧情
    private IEnumerator ShowMomThanksStory()
    {
        // 隐藏选项面板
        HideOptionPanel();
        yield return new WaitForSeconds(0.5f);

        // 显示寡妇立绘+第一句台词（拿出布包）
        momImage.gameObject.SetActive(true);
        momText.gameObject.SetActive(true);
        momText.text = "先生，我从屋里拿个东西给您。";
        yield return new WaitForSeconds(2.5f);

        // 显示第二句台词（交布包）
        momText.text = "这是您师父当年留下的。他说，如果有人来修房子，就交给他";
        yield return new WaitForSeconds(3f);

        // 隐藏寡妇台词，弹出奖励提示
        momText.gameObject.SetActive(false);
        ShowRewardPanel();
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
        Debug.Log("奖励获取完成，进入修复房子的主线剧情！");
        // 这里加你后续的逻辑：比如启动修复系统
        // FindObjectOfType<RepairSystem>().StartRepairProcess();
    }
}