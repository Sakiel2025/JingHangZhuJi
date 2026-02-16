using UnityEngine;
using UnityEngine.EventSystems;

namespace Level4.Suzhou
{
    /// <summary>
    /// 可拖拽的字脚本
    /// 实现UI元素的拖拽功能
    /// 用于场景6解谜场景
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class DraggableCharacter : MonoBehaviour, 
        IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("字的信息")]
    [Tooltip("字的名称：借/山/水/意")]
    public string characterName;
    
    [Header("拖拽设置")]
    [Tooltip("拖拽失败时是否返回初始位置")]
    public bool returnToStart = true;
    
    [Tooltip("拖拽时的透明度（0-1）")]
    [Range(0f, 1f)]
    public float dragAlpha = 0.6f;
    
    [Header("状态（运行时）")]
    [Tooltip("是否已放置到槽位")]
    public bool isPlaced = false;
    
    [Tooltip("当前所在的槽位")]
    public DropSlot currentSlot;
    
    // 私有变量
    private Vector2 startPosition;      // 初始位置
    private CanvasGroup canvasGroup;    // 用于控制透明度和射线检测
    private RectTransform rectTransform;
    private Canvas canvas;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        
        // 记录初始位置
        startPosition = rectTransform.anchoredPosition;
        
        // 确保 CanvasGroup 设置正确
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
    
    /// <summary>
    /// 开始拖拽时调用
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"开始拖拽 '{characterName}'");
        
        // 如果已放置在槽位中，先从槽位移除
        if (isPlaced && currentSlot != null)
        {
            currentSlot.RemoveCharacter();
            isPlaced = false;
            currentSlot = null;
        }
        
        // 拖拽时半透明
        if (canvasGroup != null)
        {
            canvasGroup.alpha = dragAlpha;
            // 不阻挡射线，可以检测到下方的槽位
            canvasGroup.blocksRaycasts = false;
        }
    }
    
    /// <summary>
    /// 拖拽中持续调用
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 跟随鼠标/触摸点移动
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
    
    /// <summary>
    /// 结束拖拽时调用
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"结束拖拽 '{characterName}'，是否成功放置: {isPlaced}");
        
        // 恢复透明度和射线检测
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        // 如果没有成功放置到槽位，返回初始位置
        if (!isPlaced && returnToStart)
        {
            ReturnToStartPosition();
        }
    }
    
    /// <summary>
    /// 返回到初始位置
    /// </summary>
    public void ReturnToStartPosition()
    {
        rectTransform.anchoredPosition = startPosition;
        Debug.Log($"'{characterName}' 返回初始位置");
    }
    
    /// <summary>
    /// 重置状态（用于重新开始解谜）
    /// </summary>
    public void ResetState()
    {
        if (currentSlot != null)
        {
            currentSlot.RemoveCharacter();
        }
        
        isPlaced = false;
        currentSlot = null;
        ReturnToStartPosition();
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
    }
}
