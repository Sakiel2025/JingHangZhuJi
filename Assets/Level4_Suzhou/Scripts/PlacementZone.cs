using UnityEngine;
using UnityEngine.UI;
using System;

namespace Level4.Suzhou
{
    /// <summary>
    /// 放置区域：接收并显示选中的部件
    /// </summary>
    public class PlacementZone : MonoBehaviour
    {
        [Header("=== 区域设置 ===")]
        [SerializeField] private string zoneType; // "Base", "Pillar", "Roof", "Decoration"
        [SerializeField] private Image displayImage; // 显示当前部件的Image组件
        [SerializeField] private GameObject placeholderObject; // 占位提示（如虚线框）

        [Header("=== 视觉效果 ===")]
        [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color highlightColor = new Color(0.3f, 1f, 0.3f, 0.5f);

        // 事件：部件被放置时触发
        public event Action<string> OnPartPlaced;

        private string currentPartID = null;

        private void Start()
        {
            // 初始化显示
            if (displayImage != null)
            {
                displayImage.gameObject.SetActive(false);
            }

            if (placeholderObject != null)
            {
                placeholderObject.SetActive(true);
            }

            Debug.Log($"[PlacementZone] {zoneType} 区域初始化完成");
        }

        /// <summary>
        /// 放置部件到此区域
        /// </summary>
        public void PlacePart(string partID, Sprite partSprite)
        {
            if (string.IsNullOrEmpty(partID) || partSprite == null)
            {
                Debug.LogError("[PlacementZone] 无效的部件ID或图片");
                return;
            }

            currentPartID = partID;

            // 显示部件图片
            if (displayImage != null)
            {
                displayImage.sprite = partSprite;
                displayImage.gameObject.SetActive(true);
            }

            // 隐藏占位符
            if (placeholderObject != null)
            {
                placeholderObject.SetActive(false);
            }

            Debug.Log($"[PlacementZone] {zoneType} 已放置部件: {partID}");

            // 触发事件
            OnPartPlaced?.Invoke(partID);
        }

        /// <summary>
        /// 高亮区域（当选择对应类型的部件时）
        /// </summary>
        public void Highlight(bool isHighlight)
        {
            if (displayImage != null)
            {
                Color targetColor = isHighlight ? highlightColor : normalColor;
                
                // 如果已有部件，保持完全不透明
                if (currentPartID != null)
                {
                    targetColor.a = 1f;
                }

                displayImage.color = targetColor;
            }
        }

        /// <summary>
        /// 获取当前放置的部件ID
        /// </summary>
        public string GetCurrentPartID()
        {
            return currentPartID;
        }

        /// <summary>
        /// 清空当前部件（用于重新选择）
        /// </summary>
        public void ClearPart()
        {
            currentPartID = null;

            if (displayImage != null)
            {
                displayImage.gameObject.SetActive(false);
            }

            if (placeholderObject != null)
            {
                placeholderObject.SetActive(true);
            }

            Debug.Log($"[PlacementZone] {zoneType} 已清空");
        }

        /// <summary>
        /// 获取区域类型
        /// </summary>
        public string GetZoneType()
        {
            return zoneType;
        }
    }
}
