using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickSun : MonoBehaviour
{
    public GameObject firstDialog;
    public GameObject foundDialog;
    public GameObject nextButton;
    public string nextSceneName; // 新增：下一场景名称（填你要跳的场景名）

    void Start()
    {
        // 游戏开始时：只显示第一句
        firstDialog.SetActive(true);
        foundDialog.SetActive(false);
        nextButton.SetActive(false);

        // 仅新增：给nextButton绑定点击跳场景的事件
        Button btn = nextButton.GetComponent<Button>();
        btn.onClick.AddListener(LoadNextScene);
    }

    void OnMouseDown()
    {
        Debug.Log("点击到榫字！");

        // 点击后才隐藏第一句，显示第二句
        firstDialog.SetActive(false);
        foundDialog.SetActive(true);
        nextButton.SetActive(true);
    }

    // 新增：跳转到下一场景的简单方法
    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}