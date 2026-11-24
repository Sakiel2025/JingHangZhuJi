using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickJumpScene : MonoBehaviour
{
    [Header("在这里填下一个场景的名字（自己随便改）")]
    public string jumpSceneName;

    void Update()
    {
        // 鼠标左键点击 / 手机触屏点击
        if (Input.GetMouseButtonDown(0))
        {
            if (!string.IsNullOrEmpty(jumpSceneName))
            {
                SceneManager.LoadScene(jumpSceneName);
            }
        }
    }
}