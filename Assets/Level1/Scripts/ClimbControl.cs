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

// 极简版攀爬+剧情脚本，所有逻辑拆分清晰
public class ClimbControl : MonoBehaviour
{
    [Header("攀爬基础设置")]
    public float climbSpeed = 5f; // 攀爬速度（调大一点，手感更好）
    public bool canClimb = true;  // 是否允许攀爬（初始勾选）

    [Header("剧情触发坐标（必须根据你的场景调整！）")]
    public float middleY = 0f;    // 屏幕一半的Y坐标（比如小人初始Y=-4，顶部Y=4，填0）
    public float topY = 4f;       // 顶部Y坐标（爬到这里切场景）
    public float bottomY = -4f;   // 底部Y坐标（撒谎后掉回这里）

    [Header("UI引用（拖拽赋值）")]
    public GameObject dialogWindow; // 对话+选择框的总面板（初始隐藏）
    public TextMeshProUGUI tipText; // 提示文本（初始隐藏）

    private bool hasTriggeredDialog = false; // 剧情是否已触发（避免重复弹出）
    private Vector3 playerStartPos;          // 小人初始位置

    void Start()
    {
        // 初始化：隐藏UI、记录小人初始位置
        if (dialogWindow != null) dialogWindow.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);
        playerStartPos = transform.position;
    }

    void Update()
    {
        // 核心攀爬逻辑：只响应上箭头，且允许攀爬时才移动
        if (canClimb && Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, climbSpeed * Time.deltaTime, 0);
        }

        // 触发剧情：爬到中间Y坐标，且剧情未触发
        if (transform.position.y >= middleY && !hasTriggeredDialog)
        {
            ShowDialog();
        }

        // 爬到顶部：打印日志（先不切场景，避免报错）
        if (transform.position.y >= topY)
        {
            Debug.Log("小人爬到顶部！可手动切场景");
            // 场景切换代码（后续解锁）：
            // UnityEngine.SceneManagement.SceneManager.LoadScene("下一场景名称");
        }
    }

    // ========== 以下是按钮要绑定的方法（核心！） ==========
    // 方法1：显示对话面板（自动触发，无需绑定）
    void ShowDialog()
    {
        hasTriggeredDialog = true;
        canClimb = false; // 暂停攀爬
        if (dialogWindow != null) dialogWindow.SetActive(true);
    }

    // 方法2：按钮绑定 → 说明来意
    public void OnExplainButtonClick()
    {
        // 显示好感提示
        if (tipText != null)
        {
            tipText.text = "工匠好感+10";
            tipText.color = Color.green;
            tipText.gameObject.SetActive(true);
        }
        // 隐藏对话框，恢复攀爬
        CloseDialog();
        // 2秒后隐藏提示
        Invoke("HideTipText", 2f);
    }

    // 方法3：按钮绑定 → 沉默攀爬
    public void OnSilentButtonClick()
    {
        // 直接隐藏对话框，恢复攀爬
        CloseDialog();
    }

    // 方法4：按钮绑定 → 撒谎
    public void OnLieButtonClick()
    {
        // 显示难度提示
        if (tipText != null)
        {
            tipText.text = "工匠警觉，增加难度";
            tipText.color = Color.red;
            tipText.gameObject.SetActive(true);
        }
        // 小人掉回底部
        transform.position = new Vector3(transform.position.x, bottomY, transform.position.z);
        // 隐藏对话框，恢复攀爬
        CloseDialog();
        // 2秒后隐藏提示
        Invoke("HideTipText", 2f);
    }

    // 通用方法：隐藏对话框+恢复攀爬
    void CloseDialog()
    {
        if (dialogWindow != null) dialogWindow.SetActive(false);
        canClimb = true;
    }

    // 隐藏提示文本
    void HideTipText()
    {
        if (tipText != null) tipText.gameObject.SetActive(false);
    }
}