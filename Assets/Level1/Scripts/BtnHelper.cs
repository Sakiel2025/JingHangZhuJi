using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnHelper : MonoBehaviour
{
    // 跳到工具箱（楔子→工具箱）
    public void ToToolbox()
    {
        SceneManager.LoadScene("1_Toolbox");
    }

    // 跳到地图（工具箱→地图）
    public void ToMap()
    {
        SceneManager.LoadScene("2_Map");
    }

    // 跳到北京第一关
    public void ToBeijing()
    {
        SceneManager.LoadScene("3_Beijing");
    }
}