// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/DragHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

// 新手专用：简单拖拽脚本（麻绳→木料）
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPos; // 拖拽起始位置
    public GameObject targetObj; // 拖拽目标（木料Image）
    public RepairGameManager gameManager; // 游戏管理器
    public float successDistance = 100f; // 新手友好：放大成功阈值

    void Start()
    {
        startPos = transform.position; // 记录初始位置
    }

    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
    }

    // 拖拽中：跟随鼠标移动
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    // 结束拖拽：判断是否成功
    public void OnEndDrag(PointerEventData eventData)
    {
        float distance = Vector2.Distance(transform.position, targetObj.transform.position);
        
        if (distance < successDistance)
        {
            // 拖拽成功：麻绳贴到木料上，通知管理器
            gameManager.OnStep1Success();
            transform.position = targetObj.transform.position;
        }
        else
        {
            // 拖拽失败：回到初始位置，显示失败提示
            transform.position = startPos;
            gameManager.ShowFailTip();
        }
    }
}