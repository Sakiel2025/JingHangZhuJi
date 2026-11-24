using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Level4.Suzhou
{
    /// <summary>
    /// 装饰风格选项：可点击选择的装饰套装
    /// </summary>
    public class DecorationPart : MonoBehaviour, IPointerClickHandler
    {
        [Header("=== 装饰风格套装 ===")]
        [SerializeField] private DecorationSet decorationSet;  // 装饰套装配置
        
        [Header("=== 视觉反馈（可选）===")]
        [SerializeField] private Color selectedColor = new Color(0.5f, 1f, 0.5f, 1f);  // 选中时的颜色
        
        private bool isSelected = false;
        private DecorationZone targetZone = null;
        private Button buttonComponent;
        private ColorBlock originalColors;
        
        private void Awake()
        {
            // 获取Button组件（如果有）
            buttonComponent = GetComponent<Button>();
            if (buttonComponent != null)
            {
                originalColors = buttonComponent.colors;
            }
        }
        
        private void Start()
        {
            // 查找装饰区域
            targetZone = FindObjectOfType<DecorationZone>();
            
            if (targetZone == null)
            {
                Debug.LogError("[DecorationPart] 未找到DecorationZone");
            }
            else
            {
                Debug.Log($"[DecorationPart] 找到DecorationZone: {targetZone.name}");
            }
            
            if (decorationSet != null)
            {
                Debug.Log($"[DecorationPart] 装饰风格初始化: {decorationSet.styleName} (ID: {decorationSet.styleID})");
            }
            else
            {
                Debug.LogError($"[DecorationPart] {gameObject.name} 的 decorationSet 未配置！");
            }
            
            Debug.Log($"[DecorationPart] {gameObject.name} 已启动，父对象: {transform.parent?.name}");
        }
        
        /// <summary>
        /// 点击事件：选择此装饰风格
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            SelectStyle();
        }
        
        /// <summary>
        /// 选择此装饰风格
        /// </summary>
        private void SelectStyle()
        {
            if (decorationSet == null)
            {
                Debug.LogError("[DecorationPart] 装饰套装未配置");
                return;
            }
            
            Debug.Log($"[DecorationPart] 选择装饰风格: {decorationSet.styleName}");
            
            // 取消其他风格的选中状态
            DeselectOtherStyles();
            
            // 标记为选中
            isSelected = true;
            UpdateVisualFeedback();
            
            // 放置装饰套装到区域
            if (targetZone != null)
            {
                targetZone.PlaceDecorationSet(decorationSet);
            }
            else
            {
                Debug.LogError("[DecorationPart] 无法放置：目标区域为空");
            }
        }
        
        /// <summary>
        /// 取消其他装饰风格的选中状态
        /// </summary>
        private void DeselectOtherStyles()
        {
            DecorationPart[] allParts = FindObjectsOfType<DecorationPart>();
            
            foreach (var part in allParts)
            {
                if (part != this)
                {
                    part.Deselect();
                }
            }
        }
        
        /// <summary>
        /// 取消选中
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            UpdateVisualFeedback();
        }
        
        /// <summary>
        /// 更新视觉反馈
        /// </summary>
        private void UpdateVisualFeedback()
        {
            if (buttonComponent != null)
            {
                ColorBlock colors = buttonComponent.colors;
                if (isSelected)
                {
                    // 选中时使用自定义颜色
                    colors.normalColor = selectedColor;
                }
                else
                {
                    // 未选中时恢复原始颜色
                    colors.normalColor = originalColors.normalColor;
                }
                buttonComponent.colors = colors;
            }
        }
        
        // 公共访问器
        public string GetStyleID() => decorationSet?.styleID;
        public string GetStyleName() => decorationSet?.styleName;
    }
}
