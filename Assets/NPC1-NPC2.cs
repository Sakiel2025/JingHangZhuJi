using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToNextScene : MonoBehaviour
{
    void Update()
    {
        // 点击屏幕任意位置就跳转
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("NPC2");
        }
    }
}