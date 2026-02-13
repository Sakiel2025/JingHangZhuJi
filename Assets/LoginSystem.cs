using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{
    private InputField usernameInput;
    private InputField passwordInput;

    void Start()
    {
        GameObject usernameObj = GameObject.Find("A");
        GameObject passwordObj = GameObject.Find("B");
        GameObject loginBtn = GameObject.Find("Button2");

        if (usernameObj != null) usernameInput = usernameObj.GetComponent<InputField>();
        if (passwordObj != null) passwordInput = passwordObj.GetComponent<InputField>();

        if (loginBtn != null)
        {
            Button btn = loginBtn.GetComponent<Button>();
            btn.onClick.AddListener(OnLoginButtonClick);
            Debug.Log("按钮绑定成功");
        }
        else
        {
            Debug.LogError("找不到按钮！请确认按钮名字是 Button2");
        }
    }

    public void OnLoginButtonClick()
    {
        if (usernameInput == null || passwordInput == null)
        {
            Debug.LogError("输入框未绑定");
            return;
        }

        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        Debug.Log("尝试登录 - 账号: " + username + " 密码: " + password);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("账号密码不能为空！");
            SceneManager.LoadScene("None");
            return;
        }

        // ----- 检查账号是否存在（兼容新旧格式）-----
        bool isNewFormat = PlayerPrefs.HasKey("pwd_" + username);
        bool isOldFormat = PlayerPrefs.HasKey(username);

        if (!isNewFormat && !isOldFormat)
        {
            Debug.Log("账号不存在！");
            SceneManager.LoadScene("None");
            return;
        }

        // ----- 老用户自动迁移（保留原进度）-----
        if (isOldFormat && !isNewFormat)
        {
            Debug.Log("🔄 检测到老用户，自动迁移数据...");

            // 1. 迁移密码
            string oldPassword = PlayerPrefs.GetString(username);
            PlayerPrefs.SetString("pwd_" + username, oldPassword);

            // 2. 读取老用户的关卡和碎片（如果存在）
            int oldLevel = PlayerPrefs.GetInt("level_" + username, 0);
            int oldFragment = PlayerPrefs.GetInt("fragment_" + username, 0);

            // 3. 存入新格式（完全保留原数值）
            PlayerPrefs.SetInt("level_" + username, oldLevel);
            PlayerPrefs.SetInt("fragment_" + username, oldFragment);

            // 4. 删除老密码数据
            PlayerPrefs.DeleteKey(username);
            PlayerPrefs.Save();

            Debug.Log($"✅ 用户 {username} 迁移完成，关卡：{oldLevel}，碎片：{oldFragment}");
        }

        // ----- 验证密码（使用新格式）-----
        string savedPassword = PlayerPrefs.GetString("pwd_" + username);
        if (savedPassword != password)
        {
            Debug.Log("密码错误！");
            SceneManager.LoadScene("None");
            return;
        }

        // ----- 登录成功，记录当前用户 -----
        PlayerPrefs.SetString("lastLogin", username);
        PlayerPrefs.Save();

        // ----- 读取并显示游戏数据 -----
        int level = PlayerPrefs.GetInt("level_" + username, 0);
        int fragment = PlayerPrefs.GetInt("fragment_" + username, 0);

        Debug.Log("══════════════════════════════");
        Debug.Log($"✅ 登录成功！用户：{username}");
        Debug.Log($"📊 已通关卡：{level} 关");
        Debug.Log($"📦 遗书碎片：{fragment} 个");
        Debug.Log("══════════════════════════════");

        SceneManager.LoadScene("LSuccess");
    }
}