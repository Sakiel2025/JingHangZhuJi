// using UnityEngine;

// public class ClimbControl : MonoBehaviour
// {
//     public float speed = 2f;
//     public bool canClimb = true;

//     void Update()
//     {
//         if (canClimb)
//         {
//             float v = Input.GetAxis("Vertical");
//             transform.Translate(0, v * speed * Time.deltaTime, 0);
//         }
//     }

//     public void Stop()
//     {
//         canClimb = false;
//     }

//     public void GoOn()
//     {
//         canClimb = true;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 仅适配2D UI Image小人，所有逻辑针对UI优化
public class ClimbControl : MonoBehaviour
{
    [Header("【攀爬速度】（UI专用，建议100）")]
    public float climbSpeed = 100f; 
    [Header("【是否允许攀爬】（初始勾选）")]
    public bool canClimb = true;  

    [Header("【UI坐标设置】（直接填下面的值）")]
    public float middleY = 0f;    // UI垂直中点（固定0）
    public float topY = 450f;     // UI顶部（固定450）
    public float bottomY = -450f; // UI底部（固定-450）

    [Header("【拖拽赋值区】（必拖！）")]
    public RectTransform playerUI; // 小人的RectTransform（拖小人自身）
    public GameObject dialogPanel; // 对话+选择框总面板
    public TextMeshProUGUI tipText; // 提示文本

    private bool isDialogShowed = false; // 剧情是否触发（避免重复）

    // ===================== 新增：按钮长按控制 =====================
    private bool isHoldingButton = false;

    // 按钮【按下时】调用
    public void OnClimbButtonDown()
    {
        isHoldingButton = true;
    }

    // 按钮【抬起时】调用
    public void OnClimbButtonUp()
    {
        isHoldingButton = false;
    }
    // ==============================================================

    void Start()
    {
        // 自动补全小人的RectTransform（防止漏拖）
        if (playerUI == null)
        {
            playerUI = GetComponent<RectTransform>();
        }

        // 初始隐藏UI
        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);
    }

    void Update()
    {
        // ===================== 这里修改了 =====================
        // 键盘 上键  OR  按钮长按 → 都能爬
        if (canClimb && (Input.GetKey(KeyCode.UpArrow) || isHoldingButton))
        {
            playerUI.anchoredPosition += new Vector2(0, climbSpeed * Time.deltaTime);
        }
        // ======================================================

        // 爬到中点触发剧情（只触发一次）
        if (playerUI.anchoredPosition.y >= middleY && !isDialogShowed)
        {
            ShowDialog();
        }

        // 爬到顶部提示（后续可加场景切换）
        if (playerUI.anchoredPosition.y >= topY)
        {
            Debug.Log("✅ 小人爬到顶部，可切场景！");
            UnityEngine.SceneManagement.SceneManager.LoadScene("1_2_Arch"); 
        }
    }

    // ========== 以下3个方法是按钮要绑定的核心！ ==========
    #region 按钮绑定方法（公开、无参，下拉框必显示）
    /// <summary>
    /// 说明来意按钮绑定这个方法
    /// </summary>
    public void Btn_Explain()
    {
        // 显示好感提示
        if (tipText != null)
        {
            tipText.text = "工匠好感+10";
            tipText.color = new Color(0, 1, 0); // 绿色
            tipText.gameObject.SetActive(true);
        }
        // 关闭对话框，恢复攀爬
        CloseDialog();
        // 2秒后隐藏提示
        Invoke("HideTip", 2f);
    }

    /// <summary>
    /// 沉默攀爬按钮绑定这个方法
    /// </summary>
    public void Btn_Silent()
    {
        // 直接关闭对话框，恢复攀爬
        CloseDialog();
    }

    /// <summary>
    /// 撒谎按钮绑定这个方法
    /// </summary>
    public void Btn_Lie()
    {
        // 显示难度提示
        if (tipText != null)
        {
            tipText.text = "工匠警觉，增加难度";
            tipText.color = new Color(1, 0, 0); // 红色
            tipText.gameObject.SetActive(true);
        }
        // 小人掉回UI底部
        playerUI.anchoredPosition = new Vector2(playerUI.anchoredPosition.x, bottomY);
        // 关闭对话框，恢复攀爬
        CloseDialog();
        // 2秒后隐藏提示
        Invoke("HideTip", 2f);
    }
    #endregion

    // 显示对话框：暂停攀爬
    void ShowDialog()
    {
        isDialogShowed = true;
        canClimb = false;
        if (dialogPanel != null) dialogPanel.SetActive(true);
    }

    // 关闭对话框：恢复攀爬
    void CloseDialog()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        canClimb = true;
    }

    // 隐藏提示文本
    void HideTip()
    {
        if (tipText != null) tipText.gameObject.SetActive(false);
    }
}