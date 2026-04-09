using UnityEngine;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 匠人之眼系统 - 扫描和分析建筑问题
    /// </summary>
    public class CraftsmanEyeSystem : MonoBehaviour
    {
        [Header("扫描配置")]
        [SerializeField] private Camera scanCamera; // 扫描相机（2D模式需设置为Orthographic）
        [SerializeField] private float scanDuration = 5f; // 扫描持续时间
        [SerializeField] private LayerMask scanLayer; // 可扫描的层

        [Header("视觉效果")]
        [SerializeField] private Material scanMaterial; // 扫描材质（蓝色扫描线）
        [SerializeField] private Material highlightMaterial; // 高亮材质（红色描边）
        [SerializeField] private ParticleSystem scanEffect; // 扫描粒子特效（2D模式使用Renderer Mode: Billboard）
        [SerializeField] private Color normalColor = Color.cyan;
        [SerializeField] private Color problemColor = Color.red;

        [Header("扫描目标")]
        [SerializeField] private Transform[] scanTargets; // 需要扫描的对象

        private bool isScanning = false;
        private float scanProgress = 0f;
        private int problemsFound = 0;

        // 事件
        public System.Action<string> OnProblemFound;
        public System.Action OnScanComplete;

        /// <summary>
        /// 激活匠人之眼
        /// </summary>
        public void Activate()
        {
            if (isScanning) return;

            isScanning = true;
            scanProgress = 0f;
            problemsFound = 0;

            // 点击工匠之眼后，才切换台基为倾斜损坏状态
            FoundationStateManager foundationStateManager = FindObjectOfType<FoundationStateManager>();
            if (foundationStateManager != null)
            {
                foundationStateManager.SetState(FoundationState.Tilted);
            }

            if (scanEffect != null)
                scanEffect.Play();

            UIManager.Instance?.ShowMessage("匠人之眼已激活，正在扫描...");
            
            StartCoroutine(ScanProcess());
        }

        /// <summary>
        /// 扫描流程
        /// </summary>
        private System.Collections.IEnumerator ScanProcess()
        {
            float elapsed = 0f;

            while (elapsed < scanDuration)
            {
                elapsed += Time.deltaTime;
                scanProgress = elapsed / scanDuration;

                // 更新扫描效果
                UpdateScanEffect();

                // 逐步发现问题
                if (scanProgress > 0.3f && problemsFound == 0)
                {
                    FoundProblem("地基积水严重");
                }
                else if (scanProgress > 0.6f && problemsFound == 1)
                {
                    FoundProblem("6根木桩腐朽");
                }
                else if (scanProgress > 0.9f && problemsFound == 2)
                {
                    FoundProblem("地基倾斜2.5°");
                }

                yield return null;
            }

            // 扫描完成
            CompleteScan();
        }

        private void UpdateScanEffect()
        {
            // 更新扫描材质
            if (scanMaterial != null)
            {
                scanMaterial.SetFloat("_ScanProgress", scanProgress);
            }

            // 更新UI进度条
            UIManager.Instance?.UpdateProgress("扫描进度", scanProgress);
        }

        private void FoundProblem(string problem)
        {
            problemsFound++;
            OnProblemFound?.Invoke(problem);
            UIManager.Instance?.ShowMessage($"发现问题 {problemsFound}/3: {problem}");
        }

        private void CompleteScan()
        {
            isScanning = false;

            if (scanEffect != null)
                scanEffect.Stop();

            UIManager.Instance?.ShowMessage("扫描完成！共发现3处问题");
            
            OnScanComplete?.Invoke();
            Level3GameController.Instance?.OnInvestigationComplete();
        }

        /// <summary>
        /// 高亮显示问题区域
        /// </summary>
        public void HighlightProblem(GameObject target)
        {
            if (highlightMaterial != null && target != null)
            {
                SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.material = highlightMaterial;
                }
            }
        }
    }
}
