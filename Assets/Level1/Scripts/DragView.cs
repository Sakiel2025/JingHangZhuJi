//以下注释掉的代码仅供左右拖动背景，留作参考 2026.2.15
// using UnityEngine;

// public class DragView : MonoBehaviour
// {
//     float dragSpeed = 0.5f;

//     void Update()
//     {
//         if (Input.GetMouseButton(0))
//         {
//             float h = Input.GetAxis("Mouse X");
//             transform.Translate(-h * dragSpeed, 0, 0);
//         }
//     }
// }

using UnityEngine;

public class DragView : MonoBehaviour
{
    [Header("拖动设置")]
    public float dragSpeed = 0.5f;          // 拖动速度
    public float minScaleToDrag = 1.2f;     // 允许拖动的最小缩放值（放大到1.2倍才可以拖）
    public Camera mainCamera;               // 主相机（拖拽赋值）

    private Vector3 originalScale;          // 背景初始缩放
    private float bgWidth;                  // 背景原始宽度
    private float maxDragX;                 // 最大可拖动X值（右边界）
    private float minDragX;                 // 最小可拖动X值（左边界）

    void Start()
    {
        // 初始化：记录背景初始缩放和宽度
        originalScale = transform.localScale;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 获取背景精灵的原始宽度（适配不同尺寸的背景图）
            bgWidth = spriteRenderer.bounds.size.x / originalScale.x;
        }
        else
        {
            // 如果没有SpriteRenderer（比如是UI），手动设置背景宽度（需根据你的背景调整）
            bgWidth = 10f; // 替换成你的背景实际宽度
            Debug.LogWarning("未检测到SpriteRenderer，请手动设置bgWidth的值！");
        }

        // 自动获取主相机（如果没赋值）
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // 只有满足两个条件才允许拖动：1.鼠标左键按下 2.缩放比例＞最小缩放值
        if (Input.GetMouseButton(0) && transform.localScale.x > minScaleToDrag)
        {
            // 计算可拖动边界（每次缩放变化都重新算，适配动态缩放）
            CalculateDragBounds();

            // 原有拖动逻辑
            float h = Input.GetAxis("Mouse X");
            transform.Translate(-h * dragSpeed, 0, 0);

            // 限制拖动范围：强制X坐标在[minDragX, maxDragX]之间
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minDragX, maxDragX);
            transform.position = clampedPos;
        }
    }

    // 计算背景可拖动的左右边界（核心！防止露出相机外的区域）
    void CalculateDragBounds()
    {
        // 1. 获取相机可视区域的左右边界（正交相机）
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraLeft = mainCamera.transform.position.x - cameraHalfWidth;
        float cameraRight = mainCamera.transform.position.x + cameraHalfWidth;

        // 2. 计算背景当前缩放后的实际宽度
        float currentBgWidth = bgWidth * transform.localScale.x;
        float bgHalfWidth = currentBgWidth / 2;

        // 3. 算出背景可拖动的最大/最小X值（保证背景始终覆盖相机可视区域）
        // 左边界：背景左边缘 ≤ 相机左边缘
        minDragX = cameraLeft + bgHalfWidth;
        // 右边界：背景右边缘 ≥ 相机右边缘
        maxDragX = cameraRight - bgHalfWidth;
    }

    // 可选：给外部调用的缩放方法（比如绑定缩放按钮）
    public void ScaleBackground(float scaleFactor)
    {
        transform.localScale = originalScale * scaleFactor;
    }
}