// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/EmotionSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 第三天情感选择系统（全TMP文本适配，修复int类型转换）
public class EmotionSystem : MonoBehaviour
{
    [Header("TMP文本组件（第三天核心）")]
    public TextMeshProUGUI emotionTipTMP; // 剧情提示TMP
    public TextMeshProUGUI option1TMP;    // 选项1文本TMP
    public TextMeshProUGUI option2TMP;    // 选项2文本TMP
    public TextMeshProUGUI option3TMP;    // 选项3文本TMP

    [Header("交互按钮")]
    public Button option1Btn; // 关怀村民按钮
    public Button option2Btn; // 专注工艺按钮
    public Button option3Btn; // 平衡兼顾按钮

    [Header("流程串联")]
    public GameObject endPanel; // 结局面板（可选，最后显示）

    // 初始剧情文本
    private string initialEmotionDialog = 
        "第三天：修复完成后，赵家感谢你的帮忙，村民们也围了过来...\n你更看重哪一点？";

    void Start()
    {
        // 空值检查（避免TMP/按钮未赋值报错）
        if (!CheckReferences())
        {
            Debug.LogError("[第三天] 关键TMP/按钮未赋值！");
            gameObject.SetActive(false);
            return;
        }

        // 初始隐藏情感面板
        gameObject.SetActive(false);

        // 绑定按钮点击事件
        option1Btn.onClick.AddListener(OnOption1Click);
        option2Btn.onClick.AddListener(OnOption2Click);
        option3Btn.onClick.AddListener(OnOption3Click);

        // 初始化TMP选项文本（脚本控制，避免手动改）
        option1TMP.text = "关怀村民：和大家聊聊家常，安抚受灾情绪";
        option2TMP.text = "专注工艺：检查新椽安装细节，确保百年耐用";
        option3TMP.text = "平衡兼顾：先问候村民，再复查工艺细节";
    }

    // 检查TMP/按钮引用（新手必加）
    private bool CheckReferences()
    {
        bool isAllValid = true;
        if (emotionTipTMP == null) { Debug.LogError("emotionTipTMP 未赋值！"); isAllValid = false; }
        if (option1TMP == null) { Debug.LogError("option1TMP 未赋值！"); isAllValid = false; }
        if (option2TMP == null) { Debug.LogError("option2TMP 未赋值！"); isAllValid = false; }
        if (option3TMP == null) { Debug.LogError("option3TMP 未赋值！"); isAllValid = false; }
        if (option1Btn == null) { Debug.LogError("option1Btn 未赋值！"); isAllValid = false; }
        if (option2Btn == null) { Debug.LogError("option2Btn 未赋值！"); isAllValid = false; }
        if (option3Btn == null) { Debug.LogError("option3Btn 未赋值！"); isAllValid = false; }
        return isAllValid;
    }

    // 【外部调用】启动第三天情感剧情（从RepairSystem调用）
    public void StartEmotionProcess()
    {
        gameObject.SetActive(true);
        emotionTipTMP.text = initialEmotionDialog; // TMP显示初始剧情
        // 确保按钮可点击
        option1Btn.interactable = true;
        option2Btn.interactable = true;
        option3Btn.interactable = true;
        Debug.Log("[第三天] 启动情感选择剧情");
    }

    // 选项1：关怀村民（民心+50，匠心+10）
    private void OnOption1Click()
    {
        emotionTipTMP.text = "❤️ 你和村民聊起家常，安抚了受灾后的焦虑情绪，民心大涨！\n民心+50，匠心+10";
        // ========== 修复：传int类型数值（去掉float，直接写整数） ==========
        UpdateValuesAndAchievement("heart", 50, "craft", 10, "民心所向");
        DisableAllButtons();
    }

    // 选项2：专注工艺（匠心+50，民心+10）
    private void OnOption2Click()
    {
        emotionTipTMP.text = "🔧 你仔细复查每一根新椽的安装，工艺堪称完美！\n匠心+50，民心+10";
        // ========== 修复：传int类型数值 ==========
        UpdateValuesAndAchievement("craft", 50, "heart", 10, "匠心独运");
        DisableAllButtons();
    }

    // 选项3：平衡兼顾（民心+30，匠心+30，解锁稀有成就）
    private void OnOption3Click()
    {
        emotionTipTMP.text = "🌟 你既安抚了村民，又确保了工艺质量，两全其美！\n民心+30，匠心+30";
        // ========== 修复：传int类型数值 ==========
        UpdateValuesAndAchievement("heart", 30, "craft", 30, "两全其美");
        DisableAllButtons();
    }

    // 通用：更新数值+解锁成就+2秒后推进结局
    // ========== 核心修复：参数类型改为int ==========
    private void UpdateValuesAndAchievement(string key1, int val1, string key2, int val2, string achievementName)
    {
        // 更新数值（传int类型，匹配GameManager的ChangeValue方法）
        GameManager.Instance?.ChangeValue(key1, val1);
        GameManager.Instance?.ChangeValue(key2, val2);
        // 解锁成就
        GameManager.Instance?.UnlockAchievement(achievementName);
        // 2秒后关闭情感面板，启动结局
        Invoke("CloseEmotionPanel", 2f);
    }

    // 禁用所有按钮（避免重复点击）
    private void DisableAllButtons()
    {
        option1Btn.interactable = false;
        option2Btn.interactable = false;
        option3Btn.interactable = false;
    }

    // 关闭情感面板，推进结局
    private void CloseEmotionPanel()
    {
        gameObject.SetActive(false);
        Debug.Log("[第三天] 情感选择完成，进入结局阶段");
        // 若有结局面板，显示结局
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
    }
}