using UnityEngine;
using UnityEngine.EventSystems;

namespace Level4.Suzhou
{
    /// <summary>
    /// 凹槽脚本 - 接收拖拽的字
    /// 用于场景6解谜场景
    /// </summary>
    public class DropSlot : MonoBehaviour, IDropHandler
{
    [Header("槽位设置")]
    [Tooltip("槽位索引（0-3）")]
    public int slotIndex;
    
    [Tooltip("正确的字：借/山/水/意")]
    public string correctCharacter;
    
    [Header("状态")]
    [Tooltip("是否已被占用")]
    public bool isOccupied = false;
    
    [Tooltip("当前放置的字对象")]
    public GameObject currentCharacter;
    
    /// <summary>
    /// 当有物体拖拽到此槽位时触发
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // 如果槽位已有字，不能再放
        if (isOccupied)
        {
            Debug.Log($"槽位 {slotIndex} 已被占用");
            return;
        }
        
        // 获取拖拽的对象
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;
        
        // 获取拖拽对象的脚本
        DraggableCharacter draggable = droppedObj.GetComponent<DraggableCharacter>();
        if (draggable != null)
        {
            // 将字移动到槽位中心
            RectTransform droppedRect = droppedObj.GetComponent<RectTransform>();
            RectTransform slotRect = GetComponent<RectTransform>();
            droppedRect.anchoredPosition = slotRect.anchoredPosition;
            
            // 标记槽位被占用
            isOccupied = true;
            currentCharacter = droppedObj;
            draggable.isPlaced = true;
            draggable.currentSlot = this;
            
            Debug.Log($"字 '{draggable.characterName}' 放入槽位 {slotIndex}");
            
            // 通知管理器检查是否完成解谜
            PuzzleManager manager = FindObjectOfType<PuzzleManager>();
            if (manager != null)
            {
                manager.CheckPuzzleCompletion();
            }
        }
    }
    
    /// <summary>
    /// 移除字（当字被拖走时调用）
    /// </summary>
    public void RemoveCharacter()
    {
        isOccupied = false;
        currentCharacter = null;
        Debug.Log($"槽位 {slotIndex} 已清空");
    }
    
    /// <summary>
    /// 检查当前槽位的字是否正确
    /// </summary>
    public bool IsCorrect()
    {
        if (!isOccupied || currentCharacter == null)
            return false;
        
        DraggableCharacter draggable = currentCharacter.GetComponent<DraggableCharacter>();
        if (draggable == null)
            return false;
        
        return draggable.characterName == correctCharacter;
    }
    }
}
