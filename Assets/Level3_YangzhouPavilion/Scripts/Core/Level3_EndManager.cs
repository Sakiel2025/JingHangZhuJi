using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 第三关结尾场景管理器
/// 点击继续按钮跳转到 MainMap3
/// </summary>
public class Level3_EndManager : MonoBehaviour
{
    [Header("按钮引用")]
    public Button continueButton;

    [Header("场景切换")]
    public string nextSceneName = "MainMap3";

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        else
        {
            Debug.LogError("❌ continueButton 未赋值！");
        }
    }

    void OnContinueClicked()
    {
        SaveProgressData();
        Debug.Log($"Level3_End 结束，跳转到 {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// 保存第三关完成后的关卡进度和碎片数
    /// </summary>
    void SaveProgressData()
    {
        string username = PlayerPrefs.GetString("lastLogin", "");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("⚠️ 没有获取到当前登录用户，无法更新关卡和碎片！");
            return;
        }

        int oldLevel = PlayerPrefs.GetInt("level_" + username, 0);
        int newLevel = oldLevel + 1;
        PlayerPrefs.SetInt("level_" + username, newLevel);

        int oldFragment = PlayerPrefs.GetInt("fragment_" + username, 0);
        int newFragment = oldFragment + 1;
        PlayerPrefs.SetInt("fragment_" + username, newFragment);

        PlayerPrefs.Save();

        Debug.Log($"✅ 第3关结束（End场景）：\n" +
                  $"关卡数：{oldLevel} → {newLevel}\n" +
                  $"碎片数：{oldFragment} → {newFragment}");
    }
}
