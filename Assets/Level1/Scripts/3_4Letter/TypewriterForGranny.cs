using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 脚本名必须是：TypewriterForGranny.cs（和类名完全一致）
public class TypewriterForGranny : MonoBehaviour
{
    // 要显示的文字（直接在面板改就行）
    public string showWords = "奶奶好～这是逐字显示的文字，慢慢看不累眼❤️";
    // 显示速度（数字越大越慢，0.2就是每个字等0.2秒）
    public float speed = 0.2f;
    // 自动开始显示（勾上就不用手动点）
    public bool autoPlay = true;

    // 内部用的文字组件（自动找，不用拖）
    private Text textComponent;

    // 游戏启动时初始化
    void Start()
    {
        // 自动找到当前物体上的文字组件
        textComponent = GetComponent<Text>();
        // 先清空文字，避免一开始就显示全部
        textComponent.text = "";

        // 如果勾了自动开始，就直接显示
        if (autoPlay)
        {
            StartCoroutine(ShowWordByWord());
        }
    }

    // 逐字显示的核心逻辑（协程，不会卡）
    IEnumerator ShowWordByWord()
    {
        // 一个字一个字加进去
        for (int i = 0; i <= showWords.Length; i++)
        {
            // 显示前i个字
            textComponent.text = showWords.Substring(0, i);
            // 等一等，让奶奶看清
            yield return new WaitForSeconds(speed);
        }
    }

    // 手动开始显示（比如点按钮用，右键脚本也能点）
    [ContextMenu("手动开始显示文字")]
    public void StartShow()
    {
        // 先清空再重新显示
        textComponent.text = "";
        StartCoroutine(ShowWordByWord());
    }
}