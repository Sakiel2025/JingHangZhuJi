// using UnityEngine;

// public class DialogUI : MonoBehaviour
// {
//     public ClimbControl player;

//     public void CloseDialog()
//     {
//         gameObject.SetActive(false);
//         player.GoOn();
//     }
// }

using UnityEngine;

public class DialogUI : MonoBehaviour
{
    public ClimbControl player; // 拖入挂载了ClimbControl的小人

    // 关闭对话框（兼容新版脚本）
    public void CloseDialog()
    {
        gameObject.SetActive(false);
        // 核心修改：把player.GoOn()换成直接恢复攀爬
        if (player != null)
        {
            player.canClimb = true; // 直接开启攀爬（替代原来的GoOn()）
        }
    }
}