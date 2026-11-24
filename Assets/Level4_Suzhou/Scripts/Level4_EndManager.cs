using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Level4.Suzhou
{
    /// <summary>
    /// 第四关结尾场景管理器
    /// 保存关卡进度和碎片数，点击继续跳转到 MainMap4
    /// </summary>
    public class Level4_EndManager : MonoBehaviour
    {
        [Header("按钮引用")]
        public Button continueButton;

        [Header("场景切换")]
        public string nextSceneName = "MainMap4";

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
            Debug.Log($"Level4_End 结束，跳转到 {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }

        /// <summary>
        /// 保存第四关完成后的关卡进度和碎片数
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

            Debug.Log($"✅ 第4关结束（End场景）：\n" +
                      $"关卡数：{oldLevel} → {newLevel}\n" +
                      $"碎片数：{oldFragment} → {newFragment}");
        }
    }
}
