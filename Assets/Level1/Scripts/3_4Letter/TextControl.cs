using System.Collections;
using UnityEngine;
using TMPro;

// 类名和文件名保持一致：TextController.cs
public class TextController : MonoBehaviour
{
    // 你要显示的完整文字（已经加好换行符，格式和你要的一样）
    public string showWords = 
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

    // 显示速度（0.18秒一个字，不快不慢，读着舒服）
    public float typeSpeed = 0.18f;
    // 自动找TMP文字组件，不用拖
    private TMP_Text tmpText;

    private void Start()
    {
        // 自动获取当前物体上的TMP文字组件
        tmpText = GetComponent<TMP_Text>();
        // 初始化清空文字
        tmpText.text = "";
        // 开始逐字显示
        StartCoroutine(TypeText());
    }

    // 逐字显示核心逻辑（保留换行格式）
    IEnumerator TypeText()
    {
        int charIndex = 0;
        // 逐字截取，保留所有换行符
        while (charIndex < showWords.Length)
        {
            // 截取前charIndex+1个字符，包括换行符
            tmpText.text = showWords.Substring(0, ++charIndex);
            // 等待指定时间，再显示下一个字
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    // 手动重新显示（右键脚本就能点，方便奶奶调试）
    [ContextMenu("重新开始显示文字")]
    public void RestartTypeText()
    {
        tmpText.text = "";
        StopAllCoroutines();
        StartCoroutine(TypeText());
    }
}