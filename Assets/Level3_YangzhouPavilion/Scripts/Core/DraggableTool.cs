using UnityEngine;
using UnityEngine.EventSystems;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 可拖动工具基类
    /// 支持鼠标/触摸拖动,到达目标位置后触发完成事件
    /// </summary>
    public class DraggableTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("拖动配置")]
        [SerializeField] private Transform targetPosition;  // 目标位置
        [SerializeField] private float snapDistance = 0.5f; // 吸附距离
        [SerializeField] private bool useWorldPosition = true; // 使用世界坐标(2D场景推荐)
        
        [Header("视觉反馈")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color draggingColor = Color.yellow;
        [SerializeField] private Color correctColor = Color.green;
        
        [Header("目标位置提示")]
        [SerializeField] private GameObject targetHint; // 目标位置提示(圆圈/高亮)
        
        private Vector3 offset;
        private bool isDragging = false;
        private bool isPlaced = false;
        private Vector3 startPosition;
        private Camera mainCamera;
        
        // 完成事件
        public System.Action OnToolPlaced;

        void Start()
        {
            mainCamera = Camera.main;
            startPosition = transform.position;
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
                
            if (spriteRenderer != null)
                spriteRenderer.color = normalColor;
                
            // 显示目标位置提示
            if (targetHint != null && targetPosition != null)
            {
                targetHint.transform.position = targetPosition.position;
                targetHint.SetActive(true);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isPlaced) return; // 已放置,不能再拖动
            
            isDragging = true;
            
            // 计算鼠标点击位置和物体中心的偏移
            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);
            offset = transform.position - mouseWorldPos;
            
            // 改变颜色提示正在拖动
            if (spriteRenderer != null)
                spriteRenderer.color = draggingColor;
                
            Debug.Log($"[{gameObject.name}] 开始拖动");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || isPlaced) return;
            
            // 更新物体位置跟随鼠标/触摸
            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);
            transform.position = mouseWorldPos + offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging || isPlaced) return;
            
            isDragging = false;
            
            // 检查是否在目标位置附近
            if (targetPosition != null)
            {
                float distance = Vector3.Distance(transform.position, targetPosition.position);
                
                if (distance < snapDistance)
                {
                    // 吸附到目标位置
                    transform.position = targetPosition.position;
                    isPlaced = true;
                    
                    // 改变颜色表示正确放置
                    if (spriteRenderer != null)
                        spriteRenderer.color = correctColor;
                        
                    // 隐藏目标提示
                    if (targetHint != null)
                        targetHint.SetActive(false);
                    
                    Debug.Log($"[{gameObject.name}] 已正确放置!");
                    
                    // 触发完成事件
                    OnToolPlaced?.Invoke();
                }
                else
                {
                    // 距离太远,恢复普通颜色
                    if (spriteRenderer != null)
                        spriteRenderer.color = normalColor;
                        
                    Debug.Log($"[{gameObject.name}] 距离目标还有 {distance:F2} 米");
                }
            }
        }

        /// <summary>
        /// 获取鼠标/触摸在世界坐标系中的位置
        /// </summary>
        private Vector3 GetMouseWorldPosition(Vector2 screenPosition)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
                
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
            
            if (useWorldPosition)
            {
                // 2D游戏,保持Z轴不变
                worldPos.z = transform.position.z;
            }
            
            return worldPos;
        }

        /// <summary>
        /// 重置工具位置
        /// </summary>
        public void ResetPosition()
        {
            transform.position = startPosition;
            isPlaced = false;
            isDragging = false;
            
            if (spriteRenderer != null)
                spriteRenderer.color = normalColor;
                
            if (targetHint != null)
                targetHint.SetActive(true);
        }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void SetTargetPosition(Transform target)
        {
            targetPosition = target;
            if (targetHint != null && target != null)
            {
                targetHint.transform.position = target.position;
            }
        }

        /// <summary>
        /// 检查是否已正确放置
        /// </summary>
        public bool IsPlaced()
        {
            return isPlaced;
        }

        void OnDrawGizmosSelected()
        {
            // 在编辑器中显示目标位置和吸附范围
            if (targetPosition != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(targetPosition.position, snapDistance);
                Gizmos.DrawLine(transform.position, targetPosition.position);
            }
        }
    }
}
