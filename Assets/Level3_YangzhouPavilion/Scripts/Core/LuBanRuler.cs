using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 鲁班尺测量系统 - 测量木桩垂直度
    /// </summary>
    public class LuBanRuler : MonoBehaviour
    {
        [Header("测量配置")]
        [SerializeField] private Transform rulerTransform; // 鲁班尺的Transform
        [SerializeField] private Transform targetPile; // 当前测量的木桩
        [SerializeField] private float measureDuration = 3f; // 测量持续时间（秒）
        [SerializeField] private float verticalThreshold = 1f; // 垂直度合格阈值（度）

        [Header("UI组件")]
        [SerializeField] private GameObject measurementUI; // 测量UI面板
        [SerializeField] private UnityEngine.UI.Slider angleSlider; // 角度调整滑块
        [SerializeField] private UnityEngine.UI.Text angleText; // 角度显示文本
        [SerializeField] private UnityEngine.UI.Text statusText; // 状态文本
        [SerializeField] private UnityEngine.UI.Image angleIndicator; // 角度指示器
        [SerializeField] private Color qualifiedColor = Color.green; // 合格颜色
        [SerializeField] private Color unqualifiedColor = Color.red; // 不合格颜色

        [Header("测量状态")]
        private bool isMeasuring = false;
        private float measureProgress = 0f;
        private float currentAngle = 0f; // 当前角度（与垂直方向的夹角）
        private int currentPileIndex = -1;

        [Header("触摸控制")]
        private Vector2 lastTouchPosition;
        private bool isTouching = false;
        private float rotationSensitivity = 0.5f; // 旋转灵敏度

        [Header("特效")]
        [SerializeField] private ParticleSystem scanEffect; // 扫描特效（2D粒子系统）
        [SerializeField] private GameObject qualifiedEffect; // 合格特效（2D特效预制体）

        // 事件
        public System.Action<int, bool> OnMeasurementComplete; // 木桩索引, 是否合格

        private void Start()
        {
            if (measurementUI != null)
                measurementUI.SetActive(false);
        }

        private void Update()
        {
            if (isMeasuring)
            {
                HandleMeasurementInput();
                UpdateMeasurementProgress();
            }
        }

        /// <summary>
        /// 开始测量指定木桩
        /// </summary>
        public void StartMeasurement(Transform pile, int pileIndex)
        {
            if (isMeasuring)
            {
                UIManager.Instance?.ShowMessage("正在测量中...");
                return;
            }

            targetPile = pile;
            currentPileIndex = pileIndex;
            isMeasuring = true;
            measureProgress = 0f;

            // 移动鲁班尺到木桩位置（2D游戏使用X轴偏移）
            if (rulerTransform != null && targetPile != null)
            {
                // 2D游戏：在木桩右侧0.5单位处放置鲁班尺
                rulerTransform.position = targetPile.position + Vector3.right * 0.5f;
                rulerTransform.rotation = Quaternion.identity; // 初始垂直状态（Z轴=0）
            }

            // 显示测量UI
            if (measurementUI != null)
                measurementUI.SetActive(true);

            // 播放扫描特效
            if (scanEffect != null)
                scanEffect.Play();

            // 初始化角度
            CalculateAngle();
            UpdateUI();

            UIManager.Instance?.ShowMessage($"开始测量第{pileIndex + 1}根木桩，请调整鲁班尺角度");
        }

        /// <summary>
        /// 处理测量输入（触摸/鼠标）
        /// </summary>
        private void HandleMeasurementInput()
        {
            // 支持鼠标和触摸
            if (Input.GetMouseButtonDown(0))
            {
                isTouching = true;
                lastTouchPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isTouching = false;
            }

            if (isTouching && Input.GetMouseButton(0))
            {
                Vector2 currentTouchPosition = Input.mousePosition;
                Vector2 delta = currentTouchPosition - lastTouchPosition;

                // 水平滑动调整角度
                float angleAdjustment = delta.x * rotationSensitivity * Time.deltaTime;
                AdjustRulerAngle(angleAdjustment);

                lastTouchPosition = currentTouchPosition;
            }

            // 也支持滑块调整
            if (angleSlider != null)
            {
                float sliderAngle = angleSlider.value;
                rulerTransform.rotation = Quaternion.Euler(0f, 0f, sliderAngle);
                CalculateAngle();
                UpdateUI();
            }
        }

        /// <summary>
        /// 调整鲁班尺角度
        /// </summary>
        private void AdjustRulerAngle(float delta)
        {
            if (rulerTransform == null) return;

            // 绕Z轴旋转
            rulerTransform.Rotate(0f, 0f, delta);

            // 限制旋转范围 (-45° 到 +45°)
            Vector3 euler = rulerTransform.eulerAngles;
            euler.z = Mathf.Clamp(NormalizeAngle(euler.z), -45f, 45f);
            rulerTransform.eulerAngles = euler;

            // 更新滑块
            if (angleSlider != null)
            {
                angleSlider.value = euler.z;
            }

            CalculateAngle();
            UpdateUI();
        }

        /// <summary>
        /// 计算当前角度（与垂直方向的夹角）- 2D版本
        /// </summary>
        private void CalculateAngle()
        {
            if (rulerTransform == null || targetPile == null) return;

            // 2D游戏中，使用Z轴旋转角度
            // 获取Z轴旋转角度
            float zRotation = rulerTransform.eulerAngles.z;
            
            // 将角度标准化到-180到180范围
            if (zRotation > 180f) zRotation -= 360f;
            
            // 计算与垂直方向（0度）的偏差
            currentAngle = Mathf.Abs(zRotation);
        }

        /// <summary>
        /// 更新测量进度
        /// </summary>
        private void UpdateMeasurementProgress()
        {
            measureProgress += Time.deltaTime / measureDuration;
            measureProgress = Mathf.Clamp01(measureProgress);

            if (measureProgress >= 1f)
            {
                CompleteMeasurement();
            }
        }

        /// <summary>
        /// 完成测量
        /// </summary>
        private void CompleteMeasurement()
        {
            isMeasuring = false;

            // 判断是否合格
            bool isQualified = currentAngle <= verticalThreshold;

            // 停止扫描特效
            if (scanEffect != null)
                scanEffect.Stop();

            // 显示结果
            string resultMessage = isQualified
                ? $"测量完成！垂直度: {currentAngle:F2}° - 合格 ✓"
                : $"测量完成！垂直度: {currentAngle:F2}° - 不合格 ✗\n（标准: ≤{verticalThreshold:F1}°）";

            UIManager.Instance?.ShowMessage(resultMessage);

            // 播放合格特效
            if (isQualified && qualifiedEffect != null)
            {
                GameObject effect = Instantiate(qualifiedEffect, targetPile.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // 通知地基修复管理器
            if (isQualified)
            {
                FoundationRepairManager repairManager = FindObjectOfType<FoundationRepairManager>();
                if (repairManager != null)
                {
                    repairManager.MarkPileMeasured(currentPileIndex);
                }
            }

            // 触发事件
            OnMeasurementComplete?.Invoke(currentPileIndex, isQualified);

            // 延迟隐藏UI
            StartCoroutine(HideMeasurementUI());
        }

        /// <summary>
        /// 更新UI显示
        /// </summary>
        private void UpdateUI()
        {
            if (angleText != null)
            {
                angleText.text = $"垂直度: {currentAngle:F2}°";
            }

            if (statusText != null)
            {
                bool isGood = currentAngle <= verticalThreshold;
                statusText.text = isGood ? "合格" : "不合格";
                statusText.color = isGood ? qualifiedColor : unqualifiedColor;
            }

            if (angleIndicator != null)
            {
                // 根据角度改变颜色
                float t = Mathf.InverseLerp(0f, verticalThreshold * 2f, currentAngle);
                angleIndicator.color = Color.Lerp(qualifiedColor, unqualifiedColor, t);
            }
        }

        /// <summary>
        /// 隐藏测量UI
        /// </summary>
        private IEnumerator HideMeasurementUI()
        {
            yield return new WaitForSeconds(2f);

            if (measurementUI != null)
                measurementUI.SetActive(false);
        }

        /// <summary>
        /// 取消测量
        /// </summary>
        public void CancelMeasurement()
        {
            isMeasuring = false;
            measureProgress = 0f;

            if (scanEffect != null)
                scanEffect.Stop();

            if (measurementUI != null)
                measurementUI.SetActive(false);

            UIManager.Instance?.ShowMessage("已取消测量");
        }

        /// <summary>
        /// 标准化角度到 -180 到 180 范围
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        #region 公共接口

        public bool IsMeasuring() => isMeasuring;
        public float GetCurrentAngle() => currentAngle;
        public float GetMeasureProgress() => measureProgress;

        #endregion

        #region 辅助功能

        /// <summary>
        /// 自动测量模式（用于测试）
        /// </summary>
        public void AutoMeasure(Transform pile, int pileIndex)
        {
            StartMeasurement(pile, pileIndex);
            
            // 自动调整到最佳角度
            StartCoroutine(AutoAdjustAngle());
        }

        private IEnumerator AutoAdjustAngle()
        {
            yield return new WaitForSeconds(0.5f);

            while (isMeasuring && currentAngle > verticalThreshold)
            {
                // 逐步调整到垂直
                float targetRotation = 0f;
                float currentRotation = rulerTransform.eulerAngles.z;
                currentRotation = NormalizeAngle(currentRotation);

                float step = Mathf.Sign(targetRotation - currentRotation) * 5f * Time.deltaTime;
                AdjustRulerAngle(step);

                yield return null;
            }
        }

        #endregion

        #region 可视化调试

        private void OnDrawGizmos()
        {
            if (rulerTransform != null && targetPile != null && isMeasuring)
            {
                // 2D游戏的可视化调试
                // 绘制鲁班尺方向（使用Transform.up在2D中仍然有效）
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(rulerTransform.position, rulerTransform.position + rulerTransform.up * 2f);

                // 绘制垂直参考线
                Gizmos.color = Color.green;
                Gizmos.DrawLine(targetPile.position, targetPile.position + Vector3.up * 2f);

                // 绘制角度指示圆
                if (currentAngle > 0.1f)
                {
                    Gizmos.color = currentAngle <= verticalThreshold ? Color.green : Color.red;
                    Vector3 center = rulerTransform.position;
                    // 2D游戏使用DrawWireSphere仍然可见
                    Gizmos.DrawWireSphere(center, 0.5f);
                }
            }
        }

        #endregion
    }
}
