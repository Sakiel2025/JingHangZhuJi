using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Level3.UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private float animationDuration = 0.5f;

        private Coroutine currentAnimation;

        public void SetProgress(float value, string description = "")
        {
            // 停止之前的动画
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            // 开始新的平滑过渡动画
            currentAnimation = StartCoroutine(AnimateProgress(value, description));
        }

        private IEnumerator AnimateProgress(float targetValue, string description)
        {
            float startValue = progressSlider.value;
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                
                // 使用缓动函数（OutQuad）
                t = 1f - (1f - t) * (1f - t);
                
                progressSlider.value = Mathf.Lerp(startValue, targetValue, t);
                
                // 更新文字
                if (progressText != null)
                {
                    progressText.text = $"{description} {(progressSlider.value * 100):F0}%";
                }
                
                yield return null;
            }

            // 确保最终值准确
            progressSlider.value = targetValue;
            if (progressText != null)
            {
                progressText.text = $"{description} {(targetValue * 100):F0}%";
            }
        }

        public void Reset()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
            
            progressSlider.value = 0f;
            if (progressText != null) progressText.text = "0%";
        }

        void Awake()
        {
            if (progressSlider == null)
            {
                progressSlider = GetComponent<Slider>();
            }
        }
    }
}
