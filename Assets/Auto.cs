using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoSceneChanger : MonoBehaviour
{
    [Tooltip("延迟跳转的时间（秒）")]
    public float delayTime = 2f;

    void Start()
    {
        // 启动协程，等待指定时间后跳转
        StartCoroutine(WaitAndChangeScene());
    }

    IEnumerator WaitAndChangeScene()
    {
        // 等待 delayTime 秒
        yield return new WaitForSeconds(delayTime);

        // 跳转到名为 "Map" 的场景
        SceneManager.LoadScene("Map");
    }
}