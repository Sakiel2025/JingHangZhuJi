using UnityEngine;
using UnityEngine.UI;

public class ShowPlayerData : MonoBehaviour
{
    public Text levelText;      // 显示关卡的Text（Inspector拖拽）
    public Text fragmentText;   // 显示碎片的Text（Inspector拖拽）

    void Start()
    {
        // 获取当前登录的用户名
        string username = PlayerPrefs.GetString("lastLogin", "");

        if (!string.IsNullOrEmpty(username))
        {
            // 读取该用户的游戏数据
            int level = PlayerPrefs.GetInt("level_" + username, 0);
            int fragment = PlayerPrefs.GetInt("fragment_" + username, 0);

            // 更新UI显示
            if (levelText != null)
                levelText.text = $"已通关卡：{level} 关";

            if (fragmentText != null)
                fragmentText.text = $"遗书碎片：{fragment} 个";

            Debug.Log($"📊 LSuccess页面显示 - {username} 关卡:{level} 碎片:{fragment}");
        }
        else
        {
            Debug.LogError("未检测到登录用户！");
        }
    }
}