using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class ClickToDesktop : MonoBehaviour
{
    [Header("最后一页设置")]
    public string lastSceneName = "End2";  // 最后一页的场景名

    // Windows API 声明
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    private const int SW_MINIMIZE = 6;
#endif

    void Update()
    {
        // 只检测当前场景是否是End2
        if (SceneManager.GetActiveScene().name != lastSceneName)
            return;

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("在End2场景点击，返回桌面");
            ReturnToDesktop();
        }
    }

    void ReturnToDesktop()
    {
#if UNITY_STANDALONE_WIN
        // 获取当前窗口句柄并最小化
        System.IntPtr windowHandle = GetActiveWindow();
        ShowWindow(windowHandle, SW_MINIMIZE);
#else
        // 非Windows平台直接退出
        Application.Quit();
#endif
    }
}