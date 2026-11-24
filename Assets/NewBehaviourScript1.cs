using UnityEngine;
using UnityEngine.UI;

public class QuitGameOnButton : MonoBehaviour
{
    void Start()
    {
        // 获取 Button 组件并绑定点击事件
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("未找到 Button 组件，请将此脚本挂载到 Button 对象上");
        }
    }

    void QuitGame()
    {
        Debug.Log("退出程序，返回桌面");

#if UNITY_EDITOR
        // 编辑器模式下停止运行
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 正式打包后退出应用
            Application.Quit();
#endif
    }
}