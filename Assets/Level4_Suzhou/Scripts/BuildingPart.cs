using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Level4.Suzhou
{
    /// <summary>
    /// 建筑部件：可点击选择的构件
    /// </summary>
    public class BuildingPart : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("=== 部件信息 ===")]
        [SerializeField] private string partID;        // 部件唯一ID（如 "Base_Stone"）
        [SerializeField] private string partType;      // 部件类型（如 "Base"）
        [SerializeField] private Sprite partSprite;    // 部件图片
        [SerializeField] private string partName;      // 部件名称（如 "石质基座"）
        
        [Header("=== 视觉反馈 ===")]
        [SerializeField] private Image borderImage;    // 边框图片
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.5f, 1f);
        [SerializeField] private Color selectedColor = new Color(0.3f, 1f, 0.3f, 1f);

        private Image partImage;
        private bool isSelected = false;
        private PlacementZone targetZone = null;

        private void Awake()
        {
            partImage = GetComponent<Image>();
            
            if (partImage == null)
            {
                Debug.LogError($"[BuildingPart] {gameObject.name} 缺少Image组件");
            }

            // 自动设置部件图片
            if (partImage != null && partSprite != null)
            {
                partImage.sprite = partSprite;
            }
        }

        private void Start()
        {
            // 查找对应的放置区域
            FindTargetZone();

            // 初始化边框颜色
            UpdateBorderColor(normalColor);

            Debug.Log($"[BuildingPart] 部件初始化: ID={partID}, Type={partType}, Name={partName}");
        }

        /// <summary>
        /// 查找对应类型的放置区域
        /// </summary>
        private void FindTargetZone()
        {
            PlacementZone[] allZones = FindObjectsOfType<PlacementZone>();
            
            foreach (var zone in allZones)
            {
                if (zone.GetZoneType() == partType)
                {
                    targetZone = zone;
                    Debug.Log($"[BuildingPart] {partID} 找到目标区域: {zone.name}");
                    return;
                }
            }

            Debug.LogWarning($"[BuildingPart] {partID} 未找到类型为 {partType} 的放置区域");
        }

        /// <summary>
        /// 点击事件：选择此部件
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            SelectPart();
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
        /// 选择此部件
        /// </summary>
        private void SelectPart()
        {
            Debug.Log($"[BuildingPart] 选择部件: {partID} ({partName})");

            // 取消同类型其他部件的选中状态
            DeselectOtherParts();

            // 标记为选中
            isSelected = true;
            UpdateBorderColor(selectedColor);

            // 放置到目标区域
            if (targetZone != null && partSprite != null)
            {
                targetZone.PlacePart(partID, partSprite);
            }
            else
            {
                Debug.LogError($"[BuildingPart] 无法放置部件 {partID}：目标区域或图片为空");
            }
        }

        /// <summary>
        /// 取消同类型其他部件的选中状态
        /// </summary>
        private void DeselectOtherParts()
        {
            BuildingPart[] allParts = FindObjectsOfType<BuildingPart>();
            
            foreach (var part in allParts)
            {
                if (part != this && part.partType == this.partType)
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

        // 公共访问器（用于调试）
        public string GetPartID() => partID;
        public string GetPartType() => partType;
        public string GetPartName() => partName;
    }
}
