using UnityEngine;
using UnityEngine.SceneManagement; // 场景管理必需

public class SceneSwitcher : MonoBehaviour
{
    // 可在Inspector面板填写的“下一场景名称”
    public string nextSceneName; 

    // 按钮要绑定的核心方法（必须是public、无参）
    public void LoadNextScene()
    {
        // 切换到指定场景
        SceneManager.LoadScene(nextSceneName);
    }
}