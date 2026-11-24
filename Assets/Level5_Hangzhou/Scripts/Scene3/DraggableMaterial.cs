using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/// <summary>
/// 可拖拽的建造材料
/// 用于场景3的材料拖拽到目标位置
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DraggableMaterial : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拖拽配置")]
    [Tooltip("目标凹槽（Slot）")]
    public RectTransform targetSlot;
    
    [Tooltip("吸附距离（像素）")]
    public float snapDistance = 100f;

    [Header("事件")]
    public Action OnMaterialSnapped;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 originalPosition;
    private bool isSnapped = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
        // 记录初始位置
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSnapped) return;

        // 拖拽时半透明
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isSnapped) return;

        // 跟随鼠标移动
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isSnapped) return;

        // 恢复不透明
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // 检查是否靠近目标凹槽
        if (targetSlot != null)
        {
            float distance = Vector2.Distance(
                rectTransform.anchoredPosition,
                targetSlot.anchoredPosition
            );

            if (distance < snapDistance)
            {
                // 吸附成功
                SnapToTarget();
            }
            else
            {
                // 返回原位
                ReturnToOriginal();
            }
        }
    }

    void SnapToTarget()
    {
        // 移动到目标凹槽位置
        rectTransform.anchoredPosition = targetSlot.anchoredPosition;

        // 标记为已吸附
        isSnapped = true;

        // 禁用拖拽
        canvasGroup.blocksRaycasts = false;

        // 触发事件
        OnMaterialSnapped?.Invoke();
    }

    void ReturnToOriginal()
    {
        // 返回初始位置
        rectTransform.anchoredPosition = originalPosition;
    }

    /// <summary>
    /// 重置材料状态（用于新阶段）
    /// </summary>
    public void ResetMaterial()
    {
        rectTransform.anchoredPosition = originalPosition;
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        isSnapped = false;
    }
}
