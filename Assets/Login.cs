using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button1Login : MonoBehaviour
{
    void Start()
    {
        // 自动找到叫Button1的按钮，绑定点击事件
        GameObject btnObj = GameObject.Find("Button1");
        if (btnObj != null)
        {
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(跳转到登录页);
        }
    }

    void 跳转到登录页()
    {
        SceneManager.LoadScene("Login");
    }
}