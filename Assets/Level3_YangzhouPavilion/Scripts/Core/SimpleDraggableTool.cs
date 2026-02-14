using UnityEngine;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 简化版拖动工具 - 使用鼠标事件而不是 EventSystem
    /// 更可靠，适用于世界空间的 Sprite 对象
    /// </summary>
    public class SimpleDraggableTool : MonoBehaviour
    {
        [Header("拖动配置")]
        [SerializeField] private Transform targetPosition;  // 目标位置
        [SerializeField] private float snapDistance = 0.5f; // 吸附距离
        
        [Header("视觉反馈")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color draggingColor = Color.yellow;
        [SerializeField] private Color correctColor = Color.green;
        
        [Header("目标位置提示")]
        [SerializeField] private GameObject targetHint; // 目标位置提示
        
        private bool isDragging = false;
        private bool isPlaced = false;
        private Vector3 offset;
        private Camera mainCamera;
        
        // 完成事件
        public System.Action OnToolPlaced;

        void Start()
        {
            mainCamera = Camera.main;
            
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
            
            Debug.Log($"[{gameObject.name}] SimpleDraggableTool 初始化完成");
        }

        void OnMouseDown()
        {
            if (isPlaced)
            {
                Debug.Log($"[{gameObject.name}] 已放置，不能再拖动");
                return;
            }
            
            isDragging = true;
            
            // 计算鼠标点击位置和物体中心的偏移
            Vector3 mouseWorldPos = GetMouseWorldPosition(Input.mousePosition);
            offset = transform.position - mouseWorldPos;
            
            // 改变颜色提示正在拖动
            if (spriteRenderer != null)
                spriteRenderer.color = draggingColor;
                
            Debug.Log($"[{gameObject.name}] 开始拖动！鼠标按下");
        }

        void OnMouseDrag()
        {
            if (!isDragging || isPlaced) return;
            
            // 更新物体位置跟随鼠标
            Vector3 mouseWorldPos = GetMouseWorldPosition(Input.mousePosition);
            transform.position = mouseWorldPos + offset;
            
            // 显示距离目标的距离（用于调试）
            if (targetPosition != null)
            {
                float distance = Vector3.Distance(transform.position, targetPosition.position);
                if (distance < snapDistance * 2) // 接近时才显示
                {
                    Debug.Log($"[{gameObject.name}] 距离目标: {distance:F2} 米");
                }
            }
        }

        void OnMouseUp()
        {
            if (!isDragging || isPlaced) return;
            
            isDragging = false;
            
            Debug.Log($"[{gameObject.name}] 鼠标松开");
            
            // 检查是否在目标位置附近
            if (targetPosition != null)
            {
                float distance = Vector3.Distance(transform.position, targetPosition.position);
                
                Debug.Log($"[{gameObject.name}] 最终距离目标: {distance:F2} 米 (吸附距离: {snapDistance})");
                
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
                    
                    Debug.Log($"[{gameObject.name}] ✅ 已正确放置！触发完成事件");
                    
                    // 触发完成事件
                    OnToolPlaced?.Invoke();
                }
                else
                {
                    // 距离太远，恢复普通颜色
                    if (spriteRenderer != null)
                        spriteRenderer.color = normalColor;
                        
                    Debug.Log($"[{gameObject.name}] ❌ 距离太远，未能放置");
                }
            }
        }

        /// <summary>
        /// 获取鼠标在世界坐标系中的位置
        /// </summary>
        private Vector3 GetMouseWorldPosition(Vector2 screenPosition)
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
                
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
            worldPos.z = transform.position.z; // 保持Z轴不变
            
            return worldPos;
        }

        /// <summary>
        /// 重置工具位置
        /// </summary>
        public void ResetPosition()
        {
            isPlaced = false;
            isDragging = false;
            
            if (spriteRenderer != null)
                spriteRenderer.color = normalColor;
                
            if (targetHint != null)
                targetHint.SetActive(true);
                
            Debug.Log($"[{gameObject.name}] 位置已重置");
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

        // 测试用：在编辑器中显示信息
        void OnGUI()
        {
            if (Application.isPlaying && (isDragging || isPlaced))
            {
                Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);
                screenPos.y = Screen.height - screenPos.y; // Unity GUI 坐标系是左上角为原点
                
                string status = isPlaced ? "✅ 已放置" : (isDragging ? "🟡 拖动中" : "");
                GUI.Label(new Rect(screenPos.x - 50, screenPos.y - 40, 100, 20), status);
            }
        }
    }
}
