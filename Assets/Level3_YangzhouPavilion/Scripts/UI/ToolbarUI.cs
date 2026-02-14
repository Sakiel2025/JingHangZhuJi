using UnityEngine;
using UnityEngine.UI;
using System;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// Level3工具栏UI - 管理匠人之眼、鲁班尺、千斤顶按钮
    /// </summary>
    public class ToolbarUI : MonoBehaviour
    {
        [Header("工具按钮")]
        [SerializeField] private Button craftsmanEyeButton; // 匠人之眼
        [SerializeField] private Button rulerButton; // 鲁班尺
        [SerializeField] private Button jackButton; // 千斤顶

        [Header("按钮图标")]
        [SerializeField] private Image craftsmanEyeIcon;
        [SerializeField] private Image rulerIcon;
        [SerializeField] private Image jackIcon;

        [Header("锁定状态颜色")]
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        [Header("关联的工具对象(可选)")]
        [SerializeField] private GameObject rulerToolObject; // 鲁班尺工具模型
        [SerializeField] private GameObject craftsmanEyeEffect; // 匠人之眼特效
        [SerializeField] private GameObject jackToolObjects; // 千斤顶模型组

        private Action onCraftsmanEyeClicked;
        private Action onRulerClicked;
        private Action onJackClicked;

        private void Start()
        {
            // 初始化按钮监听
            if (craftsmanEyeButton != null)
                craftsmanEyeButton.onClick.AddListener(OnCraftsmanEyeButtonClicked);

            if (rulerButton != null)
                rulerButton.onClick.AddListener(OnRulerButtonClicked);

            if (jackButton != null)
                jackButton.onClick.AddListener(OnJackButtonClicked);

            // 默认状态：匠人之眼解锁，其他锁定
            SetToolUnlocked("craftsmaneye", true);
            SetToolUnlocked("ruler", false);
            SetToolUnlocked("jack", false);
        }

        /// <summary>
        /// 设置匠人之眼按钮回调
        /// </summary>
        public void SetupCraftsmanEyeButton(Action callback)
        {
            onCraftsmanEyeClicked = callback;
        }

        /// <summary>
        /// 设置鲁班尺按钮回调
        /// </summary>
        public void SetupRulerButton(Action callback)
        {
            onRulerClicked = callback;
        }

        /// <summary>
        /// 设置千斤顶按钮回调
        /// </summary>
        public void SetupJackButton(Action callback)
        {
            onJackClicked = callback;
        }

        /// <summary>
        /// 设置工具解锁状态
        /// </summary>
        public void SetToolUnlocked(string toolName, bool unlocked)
        {
            Button targetButton = null;
            Image targetIcon = null;

            switch (toolName.ToLower())
            {
                case "craftsmaneye":
                case "匠人之眼":
                    targetButton = craftsmanEyeButton;
                    targetIcon = craftsmanEyeIcon;
                    break;
                case "ruler":
                case "鲁班尺":
                    targetButton = rulerButton;
                    targetIcon = rulerIcon;
                    break;
                case "jack":
                case "千斤顶":
                    targetButton = jackButton;
                    targetIcon = jackIcon;
                    break;
            }

            if (targetButton != null)
            {
                targetButton.interactable = unlocked;
            }

            if (targetIcon != null)
            {
                targetIcon.color = unlocked ? unlockedColor : lockedColor;
            }
        }

        private void OnCraftsmanEyeButtonClicked()
        {
            if (craftsmanEyeButton.interactable)
            {
                // 显示匠人之眼特效
                if (craftsmanEyeEffect != null)
                    craftsmanEyeEffect.SetActive(true);
                
                onCraftsmanEyeClicked?.Invoke();
                Debug.Log("匠人之眼按钮被点击");
            }
        }

        private void OnRulerButtonClicked()
        {
            if (rulerButton.interactable)
            {
                // 显示鲁班尺工具模型
                if (rulerToolObject != null)
                {
                    rulerToolObject.SetActive(true);
                    Debug.Log("鲁班尺工具已显示");
                }
                
                onRulerClicked?.Invoke();
                Debug.Log("鲁班尺按钮被点击");
            }
        }

        private void OnJackButtonClicked()
        {
            if (jackButton.interactable)
            {
                // 显示千斤顶工具
                if (jackToolObjects != null)
                {
                    jackToolObjects.SetActive(true);
                    Debug.Log("千斤顶工具已显示");
                }
                
                onJackClicked?.Invoke();
                Debug.Log("千斤顶按钮被点击");
            }
        }
        
        /// <summary>
        /// 隐藏所有工具
        /// </summary>
        public void HideAllTools()
        {
            if (rulerToolObject != null)
                rulerToolObject.SetActive(false);
            if (craftsmanEyeEffect != null)
                craftsmanEyeEffect.SetActive(false);
            if (jackToolObjects != null)
                jackToolObjects.SetActive(false);
        }

        private void OnDestroy()
        {
            // 清理监听器
            if (craftsmanEyeButton != null)
                craftsmanEyeButton.onClick.RemoveAllListeners();

            if (rulerButton != null)
                rulerButton.onClick.RemoveAllListeners();

            if (jackButton != null)
                jackButton.onClick.RemoveAllListeners();
        }
    }
}
