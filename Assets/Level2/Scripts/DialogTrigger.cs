// 保存路径：Scripts/DialogTrigger.cs
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public DialogSystem dialogSystem; // 拖拽DialogPanel物体（含DialogSystem脚本）

    void Update()
    {
        // 按空格键启动对话（仅测试用，后续可改为“进入场景自动触发”或“碰撞触发”）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 检查对话系统是否存在，避免报错
            if (dialogSystem != null)
            {
                dialogSystem.StartMerchantDialog();
                Debug.Log("[触发对话] 按空格键启动商贩讨价还价！");
            }
            else
            {
                Debug.LogError("[错误] 未赋值DialogSystem组件！");
            }
        }
    }
}