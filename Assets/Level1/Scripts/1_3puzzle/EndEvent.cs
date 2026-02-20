using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 新增：必须引入场景管理命名空间

public class EndEvent : MonoBehaviour
{
    public string nextSceneName; // 把变量移到类内部（核心修复）

    public void LoadNextScene()
    {
        // 可选：加空值校验，避免场景名未填写时报错
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("请在Inspector面板填写nextSceneName（下一场景名称）！");
            return;
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
}