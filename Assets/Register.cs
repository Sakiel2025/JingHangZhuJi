using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button2Register : MonoBehaviour
{
    void Start()
    {
        // 自动找到叫Button2的按钮，绑定点击事件
        GameObject btnObj = GameObject.Find("Button2");
        if (btnObj != null)
        {
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(跳转到注册页);
        }
    }

    void 跳转到注册页()
    {
        SceneManager.LoadScene("Register");
    }
}