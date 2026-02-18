using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelChecker : MonoBehaviour
{
    // 可以手动绑定Button，也可以自动获取
    private Button checkButton;

    void Start()
    {
        // 获取当前物体上的Button组件
        checkButton = GetComponent<Button>();

        // 为按钮添加点击事件
        checkButton.onClick.AddListener(CheckLevelAndLoadScene);
    }

    void CheckLevelAndLoadScene()
    {
        // 1. 先获取当前登录的用户名
        string currentUser = PlayerPrefs.GetString("lastLogin", "");

        // 检查是否有登录用户
        if (string.IsNullOrEmpty(currentUser))
        {
            Debug.LogError("没有找到登录用户，请先登录！");
            return;
        }

        // 2. 根据用户名拼接level的key
        string levelKey = "level_" + currentUser;

        // 3. 读取通关个数（如果没有数据，默认返回0）
        int levelCount = PlayerPrefs.GetInt(levelKey, 0);

        // 打印信息（方便调试）
        Debug.Log("当前用户: " + currentUser + " 已通关: " + levelCount + " 关");

        // 4. 根据通关个数跳转到不同场景
        switch (levelCount)
        {
            case 0:
                SceneManager.LoadScene("MainMap");
                break;
            case 1:
                SceneManager.LoadScene("MainMap1");
                break;
            case 2:
                SceneManager.LoadScene("MainMap2");
                break;
            case 3:
                SceneManager.LoadScene("MainMap3");
                break;
            case 4:
                SceneManager.LoadScene("MainMap4");
                break;
            default:
                // 如果通关数大于4，可以跳转到最高关卡
                Debug.Log("通关数大于4，跳转到MainMap4");
                SceneManager.LoadScene("MainMap4");
                break;
        }
    }
}
