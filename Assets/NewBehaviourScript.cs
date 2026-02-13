using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoverSceneLoader : MonoBehaviour
{
    void Start()
    {
        // 找到场景中唯一的按钮
        Button theOnlyButton = FindObjectOfType<Button>();

        if (theOnlyButton != null)
        {
            // 给这个唯一的按钮添加跳转功能
            theOnlyButton.onClick.AddListener(GoToLoginScene);
            Debug.Log("? 按钮已绑定！按钮名字：" + theOnlyButton.gameObject.name);
        }
        else
        {
            Debug.LogError("? 场景中没有找到任何按钮！");
        }
    }

    public void GoToLoginScene()
    {
        Debug.Log("?? 跳转到登录场景");
        SceneManager.LoadScene("LoginScene");
    }
}