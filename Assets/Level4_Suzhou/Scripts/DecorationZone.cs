using UnityEngine;
using UnityEngine.UI;
using System;

namespace Level4.Suzhou
{
    /// <summary>
    /// 装饰区域：管理多个位置的装饰物放置
    /// 玩家选择一种风格，自动在3个位置放置配套装饰
    /// </summary>
    public class DecorationZone : MonoBehaviour
    {
        [Header("=== 装饰显示位置 ===")]
        [SerializeField] private Image leftHangingImage;    // 左侧悬挂物显示
        [SerializeField] private Image rightHangingImage;   // 右侧悬挂物显示
        [SerializeField] private Image groundImage;         // 地面装饰显示
        
        [Header("=== 占位符（初始显示）===")]
        [SerializeField] private GameObject leftPlaceholder;
        [SerializeField] private GameObject rightPlaceholder;
        [SerializeField] private GameObject groundPlaceholder;
        
        [Header("=== 参考位置（用于计算偏移）===")]
        [SerializeField] private RectTransform roofReference;  // 屋顶区域的RectTransform
        
        // 事件：装饰风格被放置时触发
        public event Action<string> OnStylePlaced;
        
        private string currentStyleID = null;
        
        private void Start()
        {
            // 初始化：隐藏所有装饰图片
            HideAllDecorations();
            
            // 显示所有占位符
            ShowAllPlaceholders();
            
            Debug.Log("[DecorationZone] 装饰区域初始化完成");
        }
        
        /// <summary>
        /// 放置装饰风格套装
        /// </summary>
        public void PlaceDecorationSet(DecorationSet decoSet)
        {
            if (decoSet == null)
            {
                Debug.LogError("[DecorationZone] 装饰套装为空");
                return;
            }
            
            currentStyleID = decoSet.styleID;
            
            // 放置左侧悬挂物
            if (leftHangingImage != null && decoSet.leftHangingSprite != null)
            {
                leftHangingImage.sprite = decoSet.leftHangingSprite;
                leftHangingImage.rectTransform.sizeDelta = decoSet.hangingSize;
                
                // 设置位置（相对屋顶）
                if (roofReference != null)
                {
                    leftHangingImage.rectTransform.anchoredPosition = 
                        roofReference.anchoredPosition + decoSet.leftOffset;
                }
                
                leftHangingImage.gameObject.SetActive(true);
            }
            
            // 放置右侧悬挂物
            if (rightHangingImage != null && decoSet.rightHangingSprite != null)
            {
                rightHangingImage.sprite = decoSet.rightHangingSprite;
                rightHangingImage.rectTransform.sizeDelta = decoSet.hangingSize;
                
                // 设置位置（相对屋顶）
                if (roofReference != null)
                {
                    rightHangingImage.rectTransform.anchoredPosition = 
                        roofReference.anchoredPosition + decoSet.rightOffset;
                }
                
                rightHangingImage.gameObject.SetActive(true);
            }
            
            // 放置地面装饰
            if (groundImage != null && decoSet.groundSprite != null)
            {
                groundImage.sprite = decoSet.groundSprite;
                groundImage.rectTransform.sizeDelta = decoSet.groundSize;
                
                // 设置位置（相对屋顶）
                if (roofReference != null)
                {
                    groundImage.rectTransform.anchoredPosition = 
                        roofReference.anchoredPosition + decoSet.groundOffset;
                }
                
                groundImage.gameObject.SetActive(true);
            }
            
            // 隐藏占位符
            HideAllPlaceholders();
            
            Debug.Log($"[DecorationZone] 已放置装饰风格: {decoSet.styleName} (ID: {decoSet.styleID})");
            
            // 触发事件
            OnStylePlaced?.Invoke(decoSet.styleID);
        }
        
        /// <summary>
        /// 隐藏所有装饰图片
        /// </summary>
        private void HideAllDecorations()
        {
            if (leftHangingImage != null) leftHangingImage.gameObject.SetActive(false);
            if (rightHangingImage != null) rightHangingImage.gameObject.SetActive(false);
            if (groundImage != null) groundImage.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 显示所有占位符
        /// </summary>
        private void ShowAllPlaceholders()
        {
            if (leftPlaceholder != null) leftPlaceholder.SetActive(true);
            if (rightPlaceholder != null) rightPlaceholder.SetActive(true);
            if (groundPlaceholder != null) groundPlaceholder.SetActive(true);
        }
        
        /// <summary>
        /// 隐藏所有占位符
        /// </summary>
        private void HideAllPlaceholders()
        {
            if (leftPlaceholder != null) leftPlaceholder.SetActive(false);
            if (rightPlaceholder != null) rightPlaceholder.SetActive(false);
            if (groundPlaceholder != null) groundPlaceholder.SetActive(false);
        }
        
        /// <summary>
        /// 获取当前风格ID
        /// </summary>
        public string GetCurrentStyleID()
        {
            return currentStyleID;
        }
        
        /// <summary>
        /// 清空所有装饰（用于重新选择）
        /// </summary>
        public void ClearDecorations()
        {
            currentStyleID = null;
            HideAllDecorations();
            ShowAllPlaceholders();
            
            Debug.Log("[DecorationZone] 已清空所有装饰");
        }
    }
}
