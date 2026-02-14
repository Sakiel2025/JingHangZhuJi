using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneJump : MonoBehaviour
{
    [Header("场景设置")]
    [SerializeField] private string sceneName;  // 要跳转的场景名称

    [Header("按钮设置")]
    [SerializeField] private Button targetButton;  // 可手动拖拽按钮，不拖就自动找

    void Start()
    {
        // 如果没有手动指定按钮，就尝试获取当前对象上的按钮
        if (targetButton == null)
        {
            targetButton = GetComponent<Button>();
        }

        // 如果找到了按钮，绑定点击事件
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(JumpToScene);
        }
        else
        {
            Debug.LogWarning("未找到按钮组件！");
        }
    }

    void JumpToScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("场景名称未设置！");
        }
    }
}
