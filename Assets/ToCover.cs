using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToCoverScene : MonoBehaviour
{
    // 要跳转的目标场景名称
    [SerializeField] private string targetSceneName = "00_Cover";

    void Update()
    {
        // 检测鼠标左键点击（适用于PC、Mac等）
        if (Input.GetMouseButtonDown(0))
        {
            // 跳转到指定场景
            SceneManager.LoadScene(targetSceneName);
        }

        // 可选：支持触摸屏点击（移动端）
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}