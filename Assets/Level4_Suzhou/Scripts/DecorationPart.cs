using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Level4.Suzhou
{
    /// <summary>
    /// 装饰风格选项：可点击选择的装饰套装
    /// </summary>
    public class DecorationPart : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("=== 装饰风格套装 ===")]
        [SerializeField] private DecorationSet decorationSet;  // 装饰套装配置
        
        [Header("=== 预览图 ===")]
        [SerializeField] private Sprite previewSprite;         // 风格预览图（显示在选项上）
        
        [Header("=== 视觉反馈 ===")]
        [SerializeField] private Image borderImage;            // 边框图片
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.5f, 1f);
        [SerializeField] private Color selectedColor = new Color(0.3f, 1f, 0.3f, 1f);
        
        private Image previewImage;
        private bool isSelected = false;
        private DecorationZone targetZone = null;
        
        private void Awake()
        {
            previewImage = GetComponent<Image>();
            
            if (previewImage == null)
            {
                Debug.LogError($"[DecorationPart] {gameObject.name} 缺少Image组件");
            }
            
            // 设置预览图
            if (previewImage != null && previewSprite != null)
            {
                previewImage.sprite = previewSprite;
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
            
            // 初始化边框颜色
            UpdateBorderColor(normalColor);
            
            if (decorationSet != null)
            {
                Debug.Log($"[DecorationPart] 装饰风格初始化: {decorationSet.styleName}");
            }
        }
        
        /// <summary>
        /// 点击事件：选择此装饰风格
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            SelectStyle();
        }
        
        /// <summary>
        /// 鼠标悬停事件
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isSelected)
            {
                UpdateBorderColor(hoverColor);
            }
        }
        
        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isSelected)
            {
                UpdateBorderColor(normalColor);
            }
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
            UpdateBorderColor(selectedColor);
            
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
            UpdateBorderColor(normalColor);
        }
        
        /// <summary>
        /// 更新边框颜色
        /// </summary>
        private void UpdateBorderColor(Color color)
        {
            if (borderImage != null)
            {
                borderImage.color = color;
            }
        }
        
        // 公共访问器
        public string GetStyleID() => decorationSet?.styleID;
        public string GetStyleName() => decorationSet?.styleName;
    }
}
