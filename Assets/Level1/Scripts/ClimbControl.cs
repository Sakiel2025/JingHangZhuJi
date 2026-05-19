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

    [Header("【UI坐标设置】")]
    public float middleY = 0f;
    public float topY = 450f;
    public float bottomY = -450f;

    [Header("【攀爬翻转设置】")]
    public float flipDistance = 30f; // 爬多少距离翻转一次
    private float moveRecord;        // 记录移动距离
    private bool isFlip = false;     // 翻转状态

    [Header("【拖拽赋值区】")]
    public RectTransform playerUI;
    public GameObject dialogPanel;
    public TextMeshProUGUI tipText;

    private bool isDialogShowed = false;
    private bool isHoldingButton = false;
    private Vector2 originSize;      // 记录原始尺寸

    // 长按攀爬按钮事件
    public void OnClimbButtonDown()
    {
        isHoldingButton = true;
    }
    public void OnClimbButtonUp()
    {
        isHoldingButton = false;
    }

    void Start()
    {
        if (playerUI == null)
        {
            playerUI = GetComponent<RectTransform>();
        }
        // 开局牢牢记住原始大小，全程不改动
        originSize = playerUI.sizeDelta;

        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (tipText != null) tipText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 强制锁定原始尺寸，防止变小
        playerUI.sizeDelta = originSize;

        // 键盘上键 + 长按按钮 双攀爬
        if (canClimb && (Input.GetKey(KeyCode.UpArrow) || isHoldingButton))
        {
            float moveY = climbSpeed * Time.deltaTime;
            playerUI.anchoredPosition += new Vector2(0, moveY);

            // 累加移动距离，达到阈值就翻转
            moveRecord += moveY;
            if (moveRecord >= flipDistance)
            {
                FlipPlayer();
                moveRecord = 0; // 重置计数
            }
        }

        // 停止攀爬恢复正向
        if(!Input.GetKey(KeyCode.UpArrow) && !isHoldingButton)
        {
            isFlip = false;
            playerUI.localRotation = Quaternion.Euler(0,0,0);
        }

        // 剧情触发
        if (playerUI.anchoredPosition.y >= middleY && !isDialogShowed)
        {
            ShowDialog();
        }

        // 到达顶部切换场景
        if (playerUI.anchoredPosition.y >= topY)
        {
            Debug.Log("✅ 小人爬到顶部，可切场景！");
            UnityEngine.SceneManagement.SceneManager.LoadScene("1_2_Arch"); 
        }
    }

    // 纯左右镜面翻转，不改变任何大小
    void FlipPlayer()
    {
        isFlip = !isFlip;
        if (isFlip)
        {
            // Y轴旋转180度实现左右镜像，尺寸完全不变
            playerUI.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            playerUI.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // 三个选择按钮
    public void Btn_Explain()
    {
        if (tipText != null)
        {
            tipText.text = "工匠好感+10";
            tipText.color = new Color(0, 1, 0);
            tipText.gameObject.SetActive(true);
        }
        CloseDialog();
        Invoke("HideTip", 2f);
    }

    public void Btn_Silent()
    {
        CloseDialog();
    }

    public void Btn_Lie()
    {
        if (tipText != null)
        {
            tipText.text = "工匠警觉，增加难度";
            tipText.color = new Color(1, 0, 0);
            tipText.gameObject.SetActive(true);
        }
        playerUI.anchoredPosition = new Vector2(playerUI.anchoredPosition.x, bottomY);
        CloseDialog();
        Invoke("HideTip", 2f);
    }

    void ShowDialog()
    {
        isDialogShowed = true;
        canClimb = false;
        if (dialogPanel != null) dialogPanel.SetActive(true);
    }

    void CloseDialog()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
        canClimb = true;
    }

    void HideTip()
    {
        if (tipText != null) tipText.gameObject.SetActive(false);
    }
}