using UnityEngine;

public class ClickSun : MonoBehaviour
{
    public GameObject firstDialog;
    public GameObject foundDialog;

    void Start()
    {
        // 游戏开始时：只显示第一句
        firstDialog.SetActive(true);
        foundDialog.SetActive(false);
    }

    void OnMouseDown()
    {
        Debug.Log("点击到榫字！");

        // 点击后才隐藏第一句，显示第二句
        firstDialog.SetActive(false);
        foundDialog.SetActive(true);
    }
}