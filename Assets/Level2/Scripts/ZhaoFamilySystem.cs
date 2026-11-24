using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 赵家剧情+选择系统（新手友好版）
public class ZhaoFamilySystem : MonoBehaviour
{
    [Header("核心组件引用")]
    public TextMeshProUGUI storyTextTMP; // 剧情文本
    public Button bargainBtn; // 讨价还价按钮
    public Button jujubeWoodBtn; // 枣木代替按钮
    public GameObject dialogPanel; // 讨价还价对话面板
    public TextMeshProUGUI dialogTextTMP; // 对话文本
    public Button nextDialogBtn; // 下一句对话按钮（可选）
    public GameObject jujubeWoodPanel; // 枣木知识面板
    public TextMeshProUGUI jujubeWoodTextTMP; // 枣木知识文本
    public Button repairBtn; // 进行修复按钮

    [Header("场景配置")]
    public string repairSceneName = "RepairScene"; // 修复场景名称（改你自己的）

    // 讨价还价对话内容（数组存储，按顺序显示）
    private string[] bargainDialogs = {
        "你：老板，我要3根椽子，6.6两太贵了，赵家只有2两，能不能便宜点？",
        "商贩：2两？太少了！椽子都是上好的木料，成本都不够！",
        "你：赵家是师父帮助过的人家，寡妇带着孩子不容易，通融一下吧！",
        "商贩：看你是老师傅的徒弟，我亏点！2两就2两，算帮个忙！"
    };
    private int currentDialogIndex = 0; // 当前对话索引

    void Start()
    {
        // 新手保护：空值检查，避免报错
        CheckReferences();

        // 绑定按钮点击事件
        bargainBtn.onClick.AddListener(OnBargainClick);
        jujubeWoodBtn.onClick.AddListener(OnJujubeWoodClick);
        nextDialogBtn.onClick.AddListener(ShowNextDialog);
        repairBtn.onClick.AddListener(OnRepairClick);

        // 初始状态：隐藏对话、知识面板、修复按钮
        dialogPanel.SetActive(false);
        jujubeWoodPanel.SetActive(false);
        repairBtn.gameObject.SetActive(false);
    }

    // 新手必看：检查所有组件是否赋值（没赋值会在Console提示）
    private void CheckReferences()
    {
        if (storyTextTMP == null) Debug.LogError("storyTextTMP 未赋值！");
        if (bargainBtn == null) Debug.LogError("bargainBtn 未赋值！");
        if (jujubeWoodBtn == null) Debug.LogError("jujubeWoodBtn 未赋值！");
        if (dialogPanel == null) Debug.LogError("dialogPanel 未赋值！");
        if (dialogTextTMP == null) Debug.LogError("dialogTextTMP 未赋值！");
        if (nextDialogBtn == null) Debug.LogError("nextDialogBtn 未赋值！");
        if (jujubeWoodPanel == null) Debug.LogError("jujubeWoodPanel 未赋值！");
        if (jujubeWoodTextTMP == null) Debug.LogError("jujubeWoodTextTMP 未赋值！");
        if (repairBtn == null) Debug.LogError("repairBtn 未赋值！");
    }

    // 选择一：点击“讨价还价”按钮
    private void OnBargainClick()
    {
        // 隐藏选择按钮、枣木面板，显示对话面板
        bargainBtn.gameObject.SetActive(false);
        jujubeWoodBtn.gameObject.SetActive(false);
        jujubeWoodPanel.SetActive(false);
        dialogPanel.SetActive(true);

        // 显示第一句对话
        currentDialogIndex = 0;
        dialogTextTMP.text = bargainDialogs[currentDialogIndex];

        // 若对话只有1句，直接显示修复按钮（可选）
        // if (bargainDialogs.Length == 1) repairBtn.gameObject.SetActive(true);
    }

    // 显示下一句对话（可选，若对话有多句）
    private void ShowNextDialog()
    {
        currentDialogIndex++;
        // 如果对话显示完了，隐藏“下一句”按钮，显示“进行修复”按钮
        if (currentDialogIndex >= bargainDialogs.Length)
        {
            nextDialogBtn.gameObject.SetActive(false);
            repairBtn.gameObject.SetActive(true);
        }
        else
        {
            dialogTextTMP.text = bargainDialogs[currentDialogIndex];
        }
    }

    // 选择二：点击“枣木代替”按钮
    private void OnJujubeWoodClick()
    {
        // 隐藏选择按钮、对话面板，显示枣木知识面板
        bargainBtn.gameObject.SetActive(false);
        jujubeWoodBtn.gameObject.SetActive(false);
        dialogPanel.SetActive(false);
        jujubeWoodPanel.SetActive(true);

        // 直接显示“进行修复”按钮
        repairBtn.gameObject.SetActive(true);
    }

    // 点击“进行修复”按钮：跳转到修复场景
    private void OnRepairClick()
    {
        Debug.Log("跳转到修复场景：" + repairSceneName);
        // 加载修复场景（确保场景已添加到Build Settings）
        SceneManager.LoadScene(repairSceneName);
    }
}