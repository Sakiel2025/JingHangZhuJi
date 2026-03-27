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

        // 2. 获取遗书碎片个数
        string fragmentKey = "fragment_" + currentUser;
        int fragmentCount = PlayerPrefs.GetInt(fragmentKey, 0);

        // 打印碎片信息（方便调试）
        Debug.Log("当前用户: " + currentUser + " 遗书碎片个数: " + fragmentCount);

        // 3. 检测碎片个数为0时跳转到Tips界面
        if (fragmentCount == 0)
        {
            Debug.Log("遗书碎片个数为0，跳转到Tips界面");
            SceneManager.LoadScene("Tips");
            return;
        }

        // 4. 检测碎片个数大于等于五时，把碎片清零后跳转到Tips界面
        if (fragmentCount >= 5)
        {
            // 清零碎片
            PlayerPrefs.SetInt(fragmentKey, 0);
            PlayerPrefs.Save();

            Debug.Log("遗书碎片个数大于等于5，已清零，跳转到Tips界面");
            SceneManager.LoadScene("Tips");
            return;
        }

        // 5. 根据用户名拼接level的key
        string levelKey = "level_" + currentUser;

        // 6. 读取通关个数（如果没有数据，默认返回0）
        int levelCount = PlayerPrefs.GetInt(levelKey, 0);

        // 打印信息（方便调试）
        Debug.Log("当前用户: " + currentUser + " 已通关: " + levelCount + " 关");

        // 7. 根据通关个数跳转到不同场景
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
                // 如果通关数大于4，跳转到MainMap
                Debug.Log("通关数大于4，跳转到MainMap");

                // 重置遗书碎片个数为0
                PlayerPrefs.SetInt(fragmentKey, 0);
                PlayerPrefs.Save();

                Debug.Log("已重置用户 " + currentUser + " 的遗书碎片个数为0");

                SceneManager.LoadScene("MainMap");
                break;
        }
    }
}