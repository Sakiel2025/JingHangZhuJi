using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToDesktop : MonoBehaviour
{
    [Header("场景设置")]
    public string targetSceneName = "00_Cover";  // 目标场景名
    public string lastSceneName = "End2";       // 最后一页的场景名

    void Update()
    {
        // 只检测当前场景是否是End2
        if (SceneManager.GetActiveScene().name != lastSceneName)
            return;

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("在End2场景点击，跳转到 " + targetSceneName);
            JumpToCoverScene();
        }
    }

    void JumpToCoverScene()
    {
        // 加载目标场景
        SceneManager.LoadScene(targetSceneName);
    }
}