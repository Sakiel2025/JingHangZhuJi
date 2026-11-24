using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [Header("渐变总时长（秒）")]
    public float fadeDuration = 2f;
    private Image dialogImage;
    private float timer;

    void Start()
    {
        dialogImage = GetComponent<Image>();
        // 初始设为透明
        Color c = dialogImage.color;
        c.a = 0;
        dialogImage.color = c;
        timer = 0;
    }

    void Update()
    {
        if (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            Color c = dialogImage.color;
            c.a = alpha;
            dialogImage.color = c;
        }
    }
}