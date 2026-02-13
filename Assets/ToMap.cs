using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMap : MonoBehaviour
{
    void Start()
    {
        // 3秒后自动跳转到游戏主场景
        Invoke("GoToMap", 3f);
    }

    void GoToMap()
    {
        SceneManager.LoadScene("Map");  // 你的主场景名
    }
}