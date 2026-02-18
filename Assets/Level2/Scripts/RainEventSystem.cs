// 保存路径：Scripts/RainEventSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RainEventSystem : MonoBehaviour
{
    [Header("TMP组件引用")]
    public TextMeshProUGUI countdownTMP;
    public TextMeshProUGUI rainTipTMP;
    public Button optionABtn;
    public Button optionBBtn;
    public Button optionCBtn;
    public TextMeshProUGUI optionATMP;
    public TextMeshProUGUI optionBTMP;
    public TextMeshProUGUI optionCTMP;

    [Header("修复系统引用")]
    public RepairSystem repairSystem;

    private float remainingTime = 30f;
    private bool isDecisionMade = false;

    void Start()
    {
        // 新增：空值检查
        if (!CheckReferences())
        {
            Debug.LogError("[暴雨系统] 关键组件未赋值！");
            return;
        }

        gameObject.SetActive(false);

        optionABtn.onClick.AddListener(OnOptionAClick);
        optionBBtn.onClick.AddListener(OnOptionBClick);
        optionCBtn.onClick.AddListener(OnOptionCClick);

        optionATMP.text = "方案A：油布遮挡工地";
        optionBTMP.text = "方案B：加速完成安装";
        optionCTMP.text = "方案C：先转移粮食";
    }

    // 新增：检查引用
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
        if (repairSystem == null) { Debug.LogError("repairSystem 未赋值！"); isAllValid = false; }
        return isAllValid;
    }

    void Update()
    {
        if (!isDecisionMade && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            int roundedTime = Mathf.RoundToInt(remainingTime);
            countdownTMP.text = $"倒计时：{roundedTime}秒";

            if (roundedTime <= 0)
            {
                OnOptionAClick();
            }
        }
    }

    public void StartRainCountdown()
    {
        remainingTime = 30f;
        isDecisionMade = false;
        countdownTMP.text = "倒计时：30秒";
        rainTipTMP.text = "暴雨将至！修到一半的房子该怎么办？";
        
        optionABtn.interactable = true;
        optionBBtn.interactable = true;
        optionCBtn.interactable = true;
    }

    private void OnOptionAClick()
    {
        isDecisionMade = true;
        rainTipTMP.text = "方案A：油布遮挡工地，木料/半成品未受损！工期延长，匠心值+10";
        GameManager.Instance?.ChangeValue("craft", 10); // 空值保护
        EndRainEvent();
    }

    private void OnOptionBClick()
    {
        isDecisionMade = true;
        rainTipTMP.text = "方案B：冒雨加速安装，赶在大雨前完成！解锁成就“疾风快手”，匠心值+20";
        GameManager.Instance?.ChangeValue("craft", 20); // 空值保护
        GameManager.Instance?.UnlockAchievement("疾风快手"); // 空值保护
        
        if (repairSystem != null) // 空值保护
        {
            repairSystem.step3Btn.interactable = false;
            repairSystem.step4Btn.interactable = true;
        }
        EndRainEvent();
    }

    private void OnOptionCClick()
    {
        isDecisionMade = true;
        rainTipTMP.text = "方案C：优先转移赵家粮食，民心大涨！民心值+30，匠心值+5";
        GameManager.Instance?.ChangeValue("heart", 30); // 空值保护
        GameManager.Instance?.ChangeValue("craft", 5); // 空值保护
        EndRainEvent();
    }

    private void EndRainEvent()
    {
        optionABtn.interactable = false;
        optionBBtn.interactable = false;
        optionCBtn.interactable = false;
        Invoke("BackToRepair", 2f);
    }

    private void BackToRepair()
    {
        gameObject.SetActive(false);
        if (repairSystem != null) repairSystem.ResumeRepairProcess(); // 空值保护
    }
}