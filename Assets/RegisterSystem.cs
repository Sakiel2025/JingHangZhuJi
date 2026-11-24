using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterSystem : MonoBehaviour
{
    private InputField usernameInput;
    private InputField passwordInput;

    void Start()
    {
        GameObject usernameObj = GameObject.Find("1");
        GameObject passwordObj = GameObject.Find("2");
        GameObject registerBtn = GameObject.Find("Button1");

        if (usernameObj != null) usernameInput = usernameObj.GetComponent<InputField>();
        if (passwordObj != null) passwordInput = passwordObj.GetComponent<InputField>();

        if (registerBtn != null)
        {
            Button btn = registerBtn.GetComponent<Button>();
            btn.onClick.AddListener(OnRegisterButtonClick);
        }
    }

    public void OnRegisterButtonClick()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        // ----- 1. 空值验证 -----
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("账号和密码不能为空！");
            SceneManager.LoadScene("Wrong");
            return;
        }

        // ----- 2. 密码长度验证（至少6位）-----
        if (password.Length < 6)
        {
            Debug.Log("密码至少6位！跳转到Wrong场景");
            SceneManager.LoadScene("Wrong");
            return;
        }

        // ----- 3. 检查账号是否已存在（兼容新旧格式）-----
        if (PlayerPrefs.HasKey("pwd_" + username) || PlayerPrefs.HasKey(username))
        {
            Debug.Log("账号已存在，请直接登录！");
            SceneManager.LoadScene("Move");
            return;
        }

        // ----- 4. 保存新用户数据 -----
        // 4.1 保存密码（加前缀防冲突）
        PlayerPrefs.SetString("pwd_" + username, password);

        // 4.2 初始化关卡数（0关）
        PlayerPrefs.SetInt("level_" + username, 0);

        // 4.3 初始化遗书碎片数（0个）
        PlayerPrefs.SetInt("fragment_" + username, 0);

        // 立即保存
        PlayerPrefs.Save();

        Debug.Log($"✅ 注册成功！用户：{username}，初始关卡：0，初始碎片：0");

        // ----- 5. 跳转到注册成功页 -----
        SceneManager.LoadScene("Rsuccess");
    }
}