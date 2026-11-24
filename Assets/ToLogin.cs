using UnityEngine;
using UnityEngine.SceneManagement;

public class ToLogin : MonoBehaviour
{
    void Start()
    {
        // 3秒后自动跳转到登录页
        Invoke("GoToLogin", 3f);
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("Login");
    }
}

