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
            
            // 诊断：检查配置
            Debug.Log($"[DecorationZone] ===== 配置诊断 =====");
            Debug.Log($"[DecorationZone] leftHangingImage: {(leftHangingImage != null ? leftHangingImage.name : "NULL")}");
            Debug.Log($"[DecorationZone] rightHangingImage: {(rightHangingImage != null ? rightHangingImage.name : "NULL")}");
            Debug.Log($"[DecorationZone] groundImage: {(groundImage != null ? groundImage.name : "NULL")}");
            
            // 检查是否是同一个对象
            if (leftHangingImage != null && rightHangingImage != null)
            {
                if (leftHangingImage == rightHangingImage)
                {
                    Debug.LogError("[DecorationZone] ⚠️ 严重错误：leftHangingImage 和 rightHangingImage 是同一个对象！");
                }
                else
                {
                    Debug.Log($"[DecorationZone] ✓ 左右悬挂物是不同对象");
                    Debug.Log($"[DecorationZone]   左侧位置: {leftHangingImage.rectTransform.anchoredPosition}");
                    Debug.Log($"[DecorationZone]   右侧位置: {rightHangingImage.rectTransform.anchoredPosition}");
                }
            }
            
            if (leftPlaceholder != null)
                Debug.Log($"[DecorationZone] leftPlaceholder: {leftPlaceholder.name}, 位置: {leftPlaceholder.GetComponent<RectTransform>()?.anchoredPosition}");
            if (rightPlaceholder != null)
                Debug.Log($"[DecorationZone] rightPlaceholder: {rightPlaceholder.name}, 位置: {rightPlaceholder.GetComponent<RectTransform>()?.anchoredPosition}");
            if (groundPlaceholder != null)
                Debug.Log($"[DecorationZone] groundPlaceholder: {groundPlaceholder.name}, 位置: {groundPlaceholder.GetComponent<RectTransform>()?.anchoredPosition}");
            
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
            
            // 检查 roofReference
            if (roofReference == null)
            {
                Debug.LogError("[DecorationZone] Roof Reference 未配置！装饰物位置可能不正确！");
            }
            else
            {
                Debug.Log($"[DecorationZone] Roof Reference 位置: {roofReference.anchoredPosition}");
            }
            
            // 放置左侧悬挂物
            Debug.Log($"[DecorationZone] ----- 放置左侧悬挂物 -----");
            if (leftHangingImage != null && decoSet.leftHangingSprite != null)
            {
                Debug.Log($"[DecorationZone] 左侧Image对象: {leftHangingImage.name}, 初始位置: {leftHangingImage.rectTransform.anchoredPosition}");
                
                leftHangingImage.sprite = decoSet.leftHangingSprite;
                leftHangingImage.rectTransform.sizeDelta = decoSet.hangingSize;
                
                // 使用占位符的位置（如果有的话）
                if (leftPlaceholder != null)
                {
                    RectTransform placeholderRect = leftPlaceholder.GetComponent<RectTransform>();
                    if (placeholderRect != null)
                    {
                        Vector2 targetPos = placeholderRect.anchoredPosition;
                        leftHangingImage.rectTransform.anchoredPosition = targetPos;
                        Debug.Log($"[DecorationZone] ✓ 左侧悬挂物使用占位符 {leftPlaceholder.name} 的位置: {targetPos}");
                    }
                }
                else if (roofReference != null)
                {
                    // 如果没有占位符，使用相对屋顶的偏移
                    leftHangingImage.rectTransform.anchoredPosition = 
                        roofReference.anchoredPosition + decoSet.leftOffset;
                    Debug.Log($"[DecorationZone] ✓ 左侧悬挂物使用屋顶偏移位置: {leftHangingImage.rectTransform.anchoredPosition}");
                }
                
                leftHangingImage.gameObject.SetActive(true);
                Debug.Log($"[DecorationZone] ✓ 左侧悬挂物已激活，精灵: {leftHangingImage.sprite.name}, 最终位置: {leftHangingImage.rectTransform.anchoredPosition}");
            }
            else
            {
                if (leftHangingImage == null)
                    Debug.LogError("[DecorationZone] leftHangingImage 为空！");
                if (decoSet.leftHangingSprite == null)
                    Debug.LogError("[DecorationZone] leftHangingSprite 未配置！");
            }
            
            // 放置右侧悬挂物
            Debug.Log($"[DecorationZone] ----- 放置右侧悬挂物 -----");
            if (rightHangingImage != null && decoSet.rightHangingSprite != null)
            {
                Debug.Log($"[DecorationZone] 右侧Image对象: {rightHangingImage.name}, 初始位置: {rightHangingImage.rectTransform.anchoredPosition}");
                
                rightHangingImage.sprite = decoSet.rightHangingSprite;
                rightHangingImage.rectTransform.sizeDelta = decoSet.hangingSize;
                
                // 使用占位符的位置（如果有的话）
                if (rightPlaceholder != null)
                {
                    RectTransform placeholderRect = rightPlaceholder.GetComponent<RectTransform>();
                    if (placeholderRect != null)
                    {
                        Vector2 targetPos = placeholderRect.anchoredPosition;
                        rightHangingImage.rectTransform.anchoredPosition = targetPos;
                        Debug.Log($"[DecorationZone] ✓ 右侧悬挂物使用占位符 {rightPlaceholder.name} 的位置: {targetPos}");
                    }
                }
                else if (roofReference != null)
                {
                    // 如果没有占位符，使用相对屋顶的偏移
                    rightHangingImage.rectTransform.anchoredPosition = 
                        roofReference.anchoredPosition + decoSet.rightOffset;
                    Debug.Log($"[DecorationZone] ✓ 右侧悬挂物使用屋顶偏移位置: {rightHangingImage.rectTransform.anchoredPosition}");
                }
                
                rightHangingImage.gameObject.SetActive(true);
                Debug.Log($"[DecorationZone] ✓ 右侧悬挂物已激活，精灵: {rightHangingImage.sprite.name}, 最终位置: {rightHangingImage.rectTransform.anchoredPosition}");
            }
            else
            {
                if (rightHangingImage == null)
                    Debug.LogError("[DecorationZone] rightHangingImage 为空！");
                if (decoSet.rightHangingSprite == null)
                    Debug.LogError("[DecorationZone] rightHangingSprite 未配置！");
            }
            
            // 放置地面装饰
            Debug.Log($"[DecorationZone] ----- 放置地面装饰 -----");
            if (groundImage != null && decoSet.groundSprite != null)
            {
                Debug.Log($"[DecorationZone] 地面Image对象: {groundImage.name}, 初始位置: {groundImage.rectTransform.anchoredPosition}");
                
                groundImage.sprite = decoSet.groundSprite;
                groundImage.rectTransform.sizeDelta = decoSet.groundSize;
                
                // 使用占位符的位置（如果有的话）
                if (groundPlaceholder != null)
                {
                    RectTransform placeholderRect = groundPlaceholder.GetComponent<RectTransform>();
                    if (placeholderRect != null)
                    {
                        Vector2 targetPos = placeholderRect.anchoredPosition;
                        groundImage.rectTransform.anchoredPosition = targetPos;
                        Debug.Log($"[DecorationZone] ✓ 地面装饰使用占位符 {groundPlaceholder.name} 的位置: {targetPos}");
                    }
                }
                else if (roofReference != null)
                {
                    // 如果没有占位符，使用相对屋顶的偏移
                    groundImage.rectTransform.anchoredPosition = 
                        roofReference.anchoredPosition + decoSet.groundOffset;
                    Debug.Log($"[DecorationZone] ✓ 地面装饰使用屋顶偏移位置: {groundImage.rectTransform.anchoredPosition}");
                }
                
                groundImage.gameObject.SetActive(true);
                Debug.Log($"[DecorationZone] ✓ 地面装饰已激活，精灵: {groundImage.sprite.name}, 最终位置: {groundImage.rectTransform.anchoredPosition}");
            }
            else
            {
                if (groundImage == null)
                    Debug.LogError("[DecorationZone] groundImage 为空！");
                if (decoSet.groundSprite == null)
                    Debug.LogError("[DecorationZone] groundSprite 未配置！");
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
