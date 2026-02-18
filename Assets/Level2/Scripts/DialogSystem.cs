// 保存路径：Scripts/DialogSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("TMP组件引用（第一天）")]
    public TextMeshProUGUI dialogTMP;
    public Button option1Btn;
    public Button option2Btn;
    public TextMeshProUGUI option1TMP;
    public TextMeshProUGUI option2TMP;

    [Header("流程串联（跨天）")]
    public RepairSystem repairSystem;

    private string initialDialog = "商贩：客官，榆木椽子缺货，要么加价2倍等3天，要么用枣木替代（便宜但耐用性稍差）？";

    void Start()
    {
        if (!CheckReferences())
        {
            Debug.LogError("[对话系统] 关键组件未赋值！请检查TMP/按钮/RepairSystem引用");
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(false);

        option1Btn.onClick.AddListener(OnOption1Click);
        option2Btn.onClick.AddListener(OnOption2Click);

        option1TMP.text = "枣木替代（便宜，匠心值+40，银子-2.5两）";
        option2TMP.text = "坚持榆木（加价，匠心值+80，银子-10两）";
    }

    private bool CheckReferences()
    {
        bool isAllValid = true;
        if (dialogTMP == null) { Debug.LogError("dialogTMP 未赋值！"); isAllValid = false; }
        if (option1Btn == null) { Debug.LogError("option1Btn 未赋值！"); isAllValid = false; }
        if (option2Btn == null) { Debug.LogError("option2Btn 未赋值！"); isAllValid = false; }
        if (option1TMP == null) { Debug.LogError("option1TMP 未赋值！"); isAllValid = false; }
        if (option2TMP == null) { Debug.LogError("option2TMP 未赋值！"); isAllValid = false; }
        if (repairSystem == null) { Debug.LogWarning("repairSystem 未赋值！修复面板不会弹出"); }
        return isAllValid;
    }

    public void StartMerchantDialog()
    {
        gameObject.SetActive(true);
        dialogTMP.text = initialDialog;
        Debug.Log("[对话系统] 启动商贩对话");
    }

    // 选项1：枣木替代（核心流程）
    private void OnOption1Click()
    {
        dialogTMP.text = "商贩：枣木就枣木！2.5两银子，匠心值+40，就是耐用性差些～";
        // ========== 修复：银子转整数（2.5两 → 25钱） ==========
        GameManager.Instance?.ChangeValue("silver", -25); // -2.5两 = -25钱
        GameManager.Instance?.ChangeValue("craft", 40); // 匠心值是int，直接传
        GameManager.Instance?.UnlockAchievement("节约达人");
        
        option1Btn.interactable = false;
        option2Btn.interactable = false;
        Invoke("CloseDialogPanel", 2f);
    }

    // 选项2：坚持榆木（备选流程）
    private void OnOption2Click()
    {
        dialogTMP.text = "商贩：够爽快！10两银子，榆木椽子3天后到，匠心值+80～";
        // ========== 修复：银子转整数（10两 → 100钱） ==========
        GameManager.Instance?.ChangeValue("silver", -100); // -10两 = -100钱
        GameManager.Instance?.ChangeValue("craft", 80); // 匠心值是int，直接传
        GameManager.Instance?.UnlockAchievement("精益求精");
        
        option1Btn.interactable = false;
        option2Btn.interactable = false;
        Invoke("CloseDialogPanel", 2f);
    }

    private void CloseDialogPanel()
    {
        gameObject.SetActive(false);
        Debug.Log("[流程推进] 商贩对话结束，准备进入修复操作阶段！");

        if (repairSystem != null)
        {
            repairSystem.StartRepairProcess();
        }
        else
        {
            Debug.LogError("[对话系统] repairSystem为空！请拖拽RepairPanel到DialogSystem的Repair System槽位");
        }
    }
}