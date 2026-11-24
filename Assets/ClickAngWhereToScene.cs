using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickAnyWhereToScene : MonoBehaviour
{
    [Header("点击屏幕任意处跳转")]
    public string 从这个场景跳去 = "Letter";

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(从这个场景跳去);
        }
    }
}