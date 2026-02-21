using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonScaleEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("缩放设置")]
    public float normalScale = 1f;        // 正常大小
    public float hoverScale = 1.2f;       // 鼠标悬停大小
    public float clickScale = 0.9f;       // 点击时大小
    public float animationSpeed = 10f;     // 动画速度

    private RectTransform rectTransform;
    private Vector3 targetScale;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetScale = Vector3.one * normalScale;
    }

    void Update()
    {
        // 平滑过渡到目标大小
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );
    }

    // 鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = Vector3.one * hoverScale;
    }

    // 鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector3.one * normalScale;
    }

    // 鼠标按下
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = Vector3.one * clickScale;
    }

    // 鼠标抬起
    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = Vector3.one * hoverScale;
    }
}