using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class StopMusicOnExit : MonoBehaviour
{
    public string lastSceneName = "End2";  // 最后一页场景名

    private AudioSource audioSource;

    // Windows API 声明
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    private const int SW_MINIMIZE = 6;
#endif

    void Start()
    {
        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();

        // 确保音乐在场景切换时不销毁
        DontDestroyOnLoad(gameObject);

        // 监听场景加载
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 如果是最后一页
        if (scene.name == lastSceneName)
        {
            Debug.Log("进入最后一页，准备点击退出");
        }
    }

    void Update()
    {
        // 检查当前场景是否是最后一页
        if (SceneManager.GetActiveScene().name == lastSceneName)
        {
            // 检测鼠标点击
            if (Input.GetMouseButtonDown(0))
            {
                // 停止音乐
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    Debug.Log("音乐停止");
                }

                // 返回桌面
#if UNITY_STANDALONE_WIN
                ShowWindow(GetActiveWindow(), SW_MINIMIZE);
#endif
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}