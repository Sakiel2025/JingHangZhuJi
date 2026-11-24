using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoReturn : MonoBehaviour
{
    public string returnSceneName; // 要返回的场景名字
    public float delay = 3f;        // 延迟秒数

    void Start()
    {
        if (!string.IsNullOrEmpty(returnSceneName))
        {
            Invoke("ReturnToScene", delay);
        }
    }

    void ReturnToScene()
    {
        SceneManager.LoadScene(returnSceneName);
    }
}