using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 场景淡入淡出效果
/// 用于场景切换时的视觉过渡
/// </summary>
public class SceneFadeEffect : MonoBehaviour
{
    [Header("淡入淡出设置")]
    public Image fadeImage;             // 用于淡入淡出的黑色遮罩图片
    public float fadeDuration = 1f;     // 淡入淡出持续时间
    public Color fadeColor = Color.black; // 淡入淡出颜色

    private static SceneFadeEffect instance;

    void Awake()
    {
        // 单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化淡入淡出图片
        if (fadeImage == null)
        {
            // 自动创建淡入淡出图片
            CreateFadeImage();
        }
    }

    void Start()
    {
        // 场景开始时淡入
        FadeIn();
    }

    /// <summary>
    /// 创建淡入淡出图片
    /// </summary>
    void CreateFadeImage()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // 最高层级

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建Image
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);

        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = fadeColor;

        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        // 设置为子对象
        canvasObj.transform.SetParent(transform);
    }

    /// <summary>
    /// 淡入（从黑到透明）
    /// </summary>
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration));
    }

    /// <summary>
    /// 淡出（从透明到黑）
    /// </summary>
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration));
    }

    /// <summary>
    /// 淡入淡出协程
    /// </summary>
    IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // 确保最终值正确
        color.a = endAlpha;
        fadeImage.color = color;
    }

    /// <summary>
    /// 静态方法：淡入
    /// </summary>
    public static void DoFadeIn(float duration = 1f)
    {
        if (instance != null)
        {
            instance.fadeDuration = duration;
            instance.FadeIn();
        }
    }

    /// <summary>
    /// 静态方法：淡出
    /// </summary>
    public static void DoFadeOut(float duration = 1f)
    {
        if (instance != null)
        {
            instance.fadeDuration = duration;
            instance.FadeOut();
        }
    }

    /// <summary>
    /// 静态方法：淡出后执行回调
    /// </summary>
    public static void FadeOutThen(System.Action callback, float duration = 1f)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeOutThenCallback(callback, duration));
        }
    }

    /// <summary>
    /// 淡出后执行回调的协程
    /// </summary>
    IEnumerator FadeOutThenCallback(System.Action callback, float duration)
    {
        yield return StartCoroutine(FadeCoroutine(0f, 1f, duration));
        callback?.Invoke();
    }
}
