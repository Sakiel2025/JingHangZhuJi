using UnityEngine;

public class LevelEnd_PropRefresh : MonoBehaviour
{
    // 标记当前是第几关（用于日志区分，1=第一关，2=第二关）
    public int currentLevelNum = 1;

    /// <summary>
    /// 点击“结束关卡”按钮时调用
    /// </summary>
    public void OnLevelEndClick()
    {
        // 1. 从 lastLogin 取出当前登录的用户名
        string username = PlayerPrefs.GetString("lastLogin", "");

        // 防呆：如果没有登录用户，直接报错返回
        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("⚠️ 没有获取到当前登录用户，无法更新关卡和碎片！");
            return;
        }

        // 2. 读取并更新关卡数（对应 KEY: level_用户名）
        int oldLevel = PlayerPrefs.GetInt("level_" + username, 0);
        int newLevel = oldLevel + 1;
        PlayerPrefs.SetInt("level_" + username, newLevel);

        // 3. 读取并更新碎片数（对应 KEY: fragment_用户名）
        int oldFragment = PlayerPrefs.GetInt("fragment_" + username, 0);
        int newFragment = oldFragment + 1;
        PlayerPrefs.SetInt("fragment_" + username, newFragment);

        // 4. 保存到本地
        PlayerPrefs.Save();

        // 5. 打印日志，方便你验证
        Debug.Log($"✅ 第{currentLevelNum}关结束：\n" +
                  $"关卡数：{oldLevel} → {newLevel}\n" +
                  $"碎片数：{oldFragment} → {newFragment}");
    }
}