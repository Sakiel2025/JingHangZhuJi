using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoverSceneLoader : MonoBehaviour
{
    [SerializeField] private Button targetButton; // 在Inspector中拖拽绑定按钮

    void Start()
    {
        if (targetButton != null)
        {
            targetButton.onClick.RemoveAllListeners();
            targetButton.onClick.AddListener(GoToLoginScene);
            Debug.Log("✅ 按钮已绑定！按钮名字：" + targetButton.gameObject.name);
        }
        else
        {
            Debug.LogError("❌ 请先在Inspector中绑定按钮！");
        }
    }

    public void GoToLoginScene()
    {
        Debug.Log("🚀 跳转到登录场景");
        SceneManager.LoadScene("LoginScene");
    }
}