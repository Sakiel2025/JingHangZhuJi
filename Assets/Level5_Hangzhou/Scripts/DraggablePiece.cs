using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/// <summary>
/// 可拖拽构件脚本
/// 支持拖拽、自动吸附、位置重置
/// 适用于场景2A-2D的榫卯拼接
/// </summary>
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggablePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("吸附设置")]
    [Tooltip("吸附目标Slot（凹槽位置）")]
    public Transform snapTargetSlot;
    
    [Tooltip("吸附距离（像素）")]
    [Range(50f, 300f)]
    public float snapDistance = 150f;
    
    [Tooltip("是否已经吸附")]
    public bool isSnapped = false;

    [Header("拖拽设置")]
    [Tooltip("拖拽时的透明度")]
    [Range(0f, 1f)]
    public float dragAlpha = 0.7f;

    [Tooltip("拖拽结束后返回原位（如果未吸附）")]
    public bool returnToOrigin = true;

    [Header("视觉反馈")]
    [Tooltip("是否启用吸附高亮")]
    public bool enableSnapHighlight = true;

    [Tooltip("高亮颜色")]
    public Color highlightColor = Color.yellow;

    // 内部状态
    private Vector3 originalPosition;
    private Transform originalParent;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image image;
    private Color originalColor;
    private RectTransform rectTransform;

    // 事件回调
    public Action onSnapSuccess; // 吸附成功回调
    public Action onDragStart;   // 开始拖拽回调
    public Action onDragEnd;     // 结束拖拽回调

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        
        if (image != null)
        {
            originalColor = image.color;
        }

        // 记录初始位置
        originalPosition = rectTransform.localPosition;
        originalParent = transform.parent;
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 如果已经吸附，不允许再拖拽
        if (isSnapped)
        {
            return;
        }

        // 设置透明度
        if (canvasGroup != null)
        {
            canvasGroup.alpha = dragAlpha;
            canvasGroup.blocksRaycasts = false;
        }

        // 触发回调
        onDragStart?.Invoke();
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 如果已经吸附，不允许再拖拽
        if (isSnapped)
        {
            return;
        }

        // 跟随鼠标移动
        if (canvas != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out pos
            );

            rectTransform.localPosition = pos;
        }

        // 检查是否接近吸附目标
        CheckSnapProximity();
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 如果已经吸附，不处理
        if (isSnapped)
        {
            return;
        }

        // 恢复透明度
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        // 恢复原始颜色
        if (image != null && enableSnapHighlight)
        {
            image.color = originalColor;
        }

        // 尝试吸附
        bool snapped = TrySnap();

        // 如果未吸附且需要返回原位
        if (!snapped && returnToOrigin)
        {
            ReturnToOriginalPosition();
        }

        // 触发回调
        onDragEnd?.Invoke();
    }

    /// <summary>
    /// 检查是否接近吸附目标（用于视觉反馈）
    /// </summary>
    void CheckSnapProximity()
    {
        if (snapTargetSlot == null || !enableSnapHighlight)
        {
            return;
        }

        float distance = Vector2.Distance(rectTransform.position, snapTargetSlot.position);

        if (distance < snapDistance)
        {
            // 高亮显示
            if (image != null)
            {
                image.color = highlightColor;
            }
        }
        else
        {
            // 恢复原色
            if (image != null)
            {
                image.color = originalColor;
            }
        }
    }

    /// <summary>
    /// 尝试吸附到目标
    /// </summary>
    bool TrySnap()
    {
        if (snapTargetSlot == null)
        {
            return false;
        }

        float distance = Vector2.Distance(rectTransform.position, snapTargetSlot.position);

        if (distance < snapDistance)
        {
            // 吸附成功
            SnapToTarget();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 吸附到目标位置（Slot位置）
    /// </summary>
    void SnapToTarget()
    {
        // 移动到Slot位置
        rectTransform.position = snapTargetSlot.position;
        
        // 标记为已吸附
        isSnapped = true;

        // 禁用拖拽
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }

        // 恢复原色
        if (image != null)
        {
            image.color = originalColor;
        }

        // 触发回调
        onSnapSuccess?.Invoke();

        Debug.Log($"{gameObject.name} snapped to {snapTargetSlot.name}!");
    }

    /// <summary>
    /// 返回原始位置
    /// </summary>
    void ReturnToOriginalPosition()
    {
        rectTransform.localPosition = originalPosition;
    }

    /// <summary>
    /// 重置位置（用于重新开始）
    /// </summary>
    public void ResetPosition()
    {
        isSnapped = false;
        rectTransform.localPosition = originalPosition;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (image != null)
        {
            image.color = originalColor;
        }
    }

    /// <summary>
    /// 设置吸附目标Slot
    /// </summary>
    public void SetSnapTarget(Transform targetSlot)
    {
        snapTargetSlot = targetSlot;
    }

    /// <summary>
    /// 设置吸附距离
    /// </summary>
    public void SetSnapDistance(float distance)
    {
        snapDistance = distance;
    }

    // Gizmos绘制（Editor中可视化吸附范围）
    void OnDrawGizmosSelected()
    {
        if (snapTargetSlot != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, snapTargetSlot.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(snapTargetSlot.position, snapDistance);
        }
    }
}
