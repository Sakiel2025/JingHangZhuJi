using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToLogin : MonoBehaviour
{
    void Start()
    {
        // 找到名字为 Button 的按钮
        Button btn = GameObject.Find("Button").GetComponent<Button>();

        // 点击时跳转到 LoginScene
        btn.onClick.AddListener(GoLoginScene);
    }

    void GoLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }
}