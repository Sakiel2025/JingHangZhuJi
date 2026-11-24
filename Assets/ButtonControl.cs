using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    public Button btnLetter;   // 信按钮
    public Button btnBag;     // 背包按钮

    void Start()
    {
        btnLetter.onClick.AddListener(ToLetter);
        btnBag.onClick.AddListener(ToBag);
    }

    void ToLetter()
    {
        Debug.Log("跳转到 信");
        SceneManager.LoadScene("Letter");
    }

    void ToBag()
    {
        Debug.Log("跳转到 背包");
        SceneManager.LoadScene("Bag");
    }
}