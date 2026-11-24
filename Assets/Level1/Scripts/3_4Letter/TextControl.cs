using System.Collections;
using UnityEngine;
using TMPro;

// 通用逐字显示文字控制器（无需改代码，全在Inspector配置）
[RequireComponent(typeof(TMP_Text))] // 自动挂载TMP组件，避免遗漏
public class TextController : MonoBehaviour
{
    [Header("===== 文字配置（直接在这改，支持换行） =====")]
    [TextArea(10, 20)] // 关键：多行文本框，支持直接换行，10行显示，最多20行
    public string targetText = 
@"榫儿：
你能找到这里，说明你还记得我教的""观察""。
故宫的斗拱，承载的不是屋顶，是国家的尊严。
当年西洋人说，木结构落后，要拆了改钢筋混凝土。
我问他们：你们的钢筋，能活三百年吗？
他们哑口无言。

记住：
技艺，从来不只是技艺。
它是文化，是信仰，是我们民族的脊梁。

下一站，去台儿庄。
那里有你该学的第二课：匠人的心。

师父 于光绪十八年";

    [Header("===== 显示速度配置 =====")]
    [Tooltip("逐字显示间隔（秒/字），默认0.18，越小越快")]
    public float typeSpeed = 0.18f;

    [Header("===== 高级配置（可选） =====")]
    [Tooltip("是否自动开始显示（取消勾选则手动调用StartTypeText()）")]
    public bool autoStart = true;
    [Tooltip("是否显示完成后自动停止（一般不用改）")]
    public bool stopAfterComplete = true;

    // 核心组件（自动获取，无需拖拽）
    private TMP_Text tmpText;
    // 标记：是否正在显示文字（避免重复调用）
    private bool isTyping = false;

    private void Awake()
    {
        // 自动获取当前物体上的TMP组件（新手保护：避免漏挂载）
        tmpText = GetComponent<TMP_Text>();
        if (tmpText == null)
        {
            Debug.LogError("[TextController] 当前物体没有TMP_Text组件！已自动添加");
            tmpText = gameObject.AddComponent<TMP_Text>();
        }
        // 初始化清空文字
        tmpText.text = string.Empty;
    }

    private void Start()
    {
        // 自动开始显示（可在Inspector关闭）
        if (autoStart)
        {
            StartTypeText();
        }
    }

    // ========== 核心：逐字显示逻辑（保留所有格式，包括换行） ==========
    IEnumerator TypeTextCoroutine()
    {
        isTyping = true;
        tmpText.text = string.Empty; // 清空原有文字
        int charIndex = 0;

        // 逐字截取，保留换行、空格、标点等所有格式
        while (charIndex < targetText.Length)
        {
            // 截取前charIndex+1个字符，完全保留原始格式
            tmpText.text = targetText.Substring(0, ++charIndex);
            // 等待指定间隔，显示下一个字
            yield return new WaitForSeconds(typeSpeed);

            // 防止中途修改文字导致异常
            if (charIndex > targetText.Length)
            {
                break;
            }
        }

        isTyping = false;
        Debug.Log("[TextController] 文字显示完成！");
    }

    // ========== 外部/手动调用：开始显示文字（通用方法） ==========
    [ContextMenu("开始/重新显示文字")] // 右键脚本可直接点击调用
    public void StartTypeText()
    {
        // 停止正在运行的协程，避免重复显示
        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
        }
        // 重新开始逐字显示
        StartCoroutine(TypeTextCoroutine());
    }

    // ========== 外部/手动调用：立即显示全部文字（跳过逐字） ==========
    [ContextMenu("立即显示全部文字")] // 右键脚本可直接点击调用
    public void ShowAllTextImmediately()
    {
        StopAllCoroutines();
        isTyping = false;
        tmpText.text = targetText; // 直接显示完整文字，保留所有格式
        Debug.Log("[TextController] 已立即显示全部文字！");
    }

    // ========== 外部/手动调用：清空文字 ==========
    [ContextMenu("清空文字")] // 右键脚本可直接点击调用
    public void ClearText()
    {
        StopAllCoroutines();
        isTyping = false;
        tmpText.text = string.Empty;
        Debug.Log("[TextController] 文字已清空！");
    }
}