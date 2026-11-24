using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    private Button resetButton;

    void Start()
    {
        // 获取当前物体上的Button组件
        resetButton = GetComponent<Button>();

        // 为按钮添加点击事件
        resetButton.onClick.AddListener(CheckAndResetData);
    }

    void CheckAndResetData()
    {
        // 1. 先获取当前登录的用户名
        string currentUser = PlayerPrefs.GetString("lastLogin", "");

        // 检查是否有登录用户
        if (string.IsNullOrEmpty(currentUser))
        {
            Debug.LogError("没有找到登录用户，请先登录！");
            return;
        }

        // 2. 根据用户名拼接key
        string levelKey = "level_" + currentUser;
        string fragmentKey = "fragment_" + currentUser;

        // 3. 读取当前数据
        int levelCount = PlayerPrefs.GetInt(levelKey, 0);
        int fragmentCount = PlayerPrefs.GetInt(fragmentKey, 0);

        // 打印当前状态
        Debug.Log($"当前用户: {currentUser}, 通关数: {levelCount}, 碎片数: {fragmentCount}");

        // 4. 检查是否满足清零条件（碎片=5且通关数=5）
        if (fragmentCount >= 0 && levelCount >= 0)
        {
            // 清零数据
            PlayerPrefs.SetInt(levelKey, 0);
            PlayerPrefs.SetInt(fragmentKey, 0);
            PlayerPrefs.Save();

            Debug.Log("条件满足，已清零通关数和碎片数！");

            // 可选：跳转到初始场景
            // SceneManager.LoadScene("MainMap");
        }
        else
        {
            // 不满足条件的提示
            Debug.Log($"条件不满足，当前碎片数: {fragmentCount}/5，通关数: {levelCount}/5，无法清零");
        }
    }
}