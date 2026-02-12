using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 地基加固系统 - 管理排水、支撑调整、木桩更换
    /// </summary>
    public class FoundationRepairManager : MonoBehaviour
    {
        [Header("地基组件引用")]
        [SerializeField] private Transform pavilionFoundation; // 六角亭地基
        [SerializeField] private Transform[] supportPoints = new Transform[4]; // 4个千斤顶支撑点
        [SerializeField] private Transform[] woodPiles = new Transform[6]; // 6根木桩
        [SerializeField] private ParticleSystem waterDrainEffect; // 排水特效（2D模式）
        [SerializeField] private GameObject drainPipe; // 排水管道（使用SpriteRenderer）

        [Header("测量数据")]
        [SerializeField] private float currentTiltAngle = 2.5f; // 当前倾斜角度（度）
        [SerializeField] private int repairedPilesCount = 0; // 已更换木桩数量
        [SerializeField] private float stabilityScore = 0f; // 稳定性评分（0-100）
        
        // 常量
        private const float TARGET_TILT_ANGLE = 0.3f; // 目标倾斜角度

        [Header("支撑点高度")]
        private float[] supportHeights = new float[4] { 0f, 0f, 0f, 0f }; // 千斤顶高度（米）
        private const float MAX_SUPPORT_HEIGHT = 0.5f; // 最大调整高度
        private const float HEIGHT_STEP = 0.01f; // 每次调整步进

        [Header("状态标记")]
        private bool isWaterDrained = false;
        private bool[] isPileMeasured = new bool[6]; // 木桩是否已测量
        private bool isRepairComplete = false;

        [Header("评级阈值")]
        private const float S_GRADE_TILT = 0.3f;
        private const float A_GRADE_TILT = 0.5f;
        private const float B_GRADE_TILT = 1.0f;

        // 事件
        public System.Action<float> OnStabilityChanged;
        public System.Action<string> OnGradeAchieved;
        public System.Action OnRepairComplete;

        private void Start()
        {
            InitializeFoundation();
        }

        /// <summary>
        /// 初始化地基状态
        /// </summary>
        private void InitializeFoundation()
        {
            // 初始化木桩测量状态
            for (int i = 0; i < isPileMeasured.Length; i++)
            {
                isPileMeasured[i] = false;
            }

            // 设置初始倾斜
            ApplyTilt(currentTiltAngle);
            UpdateStability();
        }

        #region 排水系统

        /// <summary>
        /// 开始排水流程
        /// </summary>
        public void StartDrainWater()
        {
            if (isWaterDrained)
            {
                Debug.Log("水已经排干了");
                return;
            }

            if (drainPipe == null)
            {
                Debug.LogError("未安装排水管道！");
                UIManager.Instance?.ShowMessage("请先安装排水管道");
                return;
            }

            StartCoroutine(DrainWaterProcess());
        }

        private IEnumerator DrainWaterProcess()
        {
            UIManager.Instance?.ShowMessage("开始排水...");
            
            // 播放排水特效
            if (waterDrainEffect != null)
            {
                waterDrainEffect.Play();
            }

            // 模拟10秒排水过程
            float duration = 10f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // 可以在这里控制水位下降动画
                UIManager.Instance?.UpdateProgress("排水进度", progress);
                
                yield return null;
            }

            isWaterDrained = true;
            waterDrainEffect?.Stop();
            UIManager.Instance?.ShowMessage("排水完成！可以开始调整支撑点了");
            
            // 解锁千斤顶功能
            UnlockJacks();
        }

        private void UnlockJacks()
        {
            // 通知UI解锁千斤顶控制
            Debug.Log("千斤顶已解锁");
        }

        #endregion

        #region 支撑调整系统

        /// <summary>
        /// 调整指定支撑点的高度
        /// </summary>
        /// <param name="pointIndex">支撑点索引 (0-3)</param>
        /// <param name="increase">true为升高，false为降低</param>
        public void AdjustSupportHeight(int pointIndex, bool increase)
        {
            if (!isWaterDrained)
            {
                UIManager.Instance?.ShowMessage("请先排水！");
                return;
            }

            if (pointIndex < 0 || pointIndex >= 4)
            {
                Debug.LogError("支撑点索引无效");
                return;
            }

            float delta = increase ? HEIGHT_STEP : -HEIGHT_STEP;
            supportHeights[pointIndex] = Mathf.Clamp(
                supportHeights[pointIndex] + delta, 
                0f, 
                MAX_SUPPORT_HEIGHT
            );

            // 更新支撑点位置
            if (supportPoints[pointIndex] != null)
            {
                Vector3 pos = supportPoints[pointIndex].localPosition;
                pos.y = supportHeights[pointIndex];
                supportPoints[pointIndex].localPosition = pos;
            }

            // 重新计算倾斜度
            CalculateTilt();
            UpdateStability();
        }

        /// <summary>
        /// 批量设置支撑点高度
        /// </summary>
        public void SetSupportHeights(float[] heights)
        {
            if (heights.Length != 4)
            {
                Debug.LogError("高度数组长度必须为4");
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                supportHeights[i] = Mathf.Clamp(heights[i], 0f, MAX_SUPPORT_HEIGHT);
                if (supportPoints[i] != null)
                {
                    Vector3 pos = supportPoints[i].localPosition;
                    pos.y = supportHeights[i];
                    supportPoints[i].localPosition = pos;
                }
            }

            CalculateTilt();
            UpdateStability();
        }

        /// <summary>
        /// 计算地基倾斜度（2D版本 - 基于支撑点高度差）
        /// </summary>
        private void CalculateTilt()
        {
            if (supportPoints.Length < 4) return;

            // 2D游戏：基于支撑点Y坐标（高度）计算倾斜度
            // 假设4个支撑点布局为矩形：左下(0)、右下(1)、右上(2)、左上(3)
            float leftHeight = (supportPoints[0].position.y + supportPoints[3].position.y) / 2f;
            float rightHeight = (supportPoints[1].position.y + supportPoints[2].position.y) / 2f;
            float frontHeight = (supportPoints[0].position.y + supportPoints[1].position.y) / 2f;
            float backHeight = (supportPoints[2].position.y + supportPoints[3].position.y) / 2f;

            // 计算左右倾斜（主要倾斜方向）
            float horizontalDiff = Mathf.Abs(rightHeight - leftHeight);
            // 计算前后倾斜（次要倾斜方向）
            float verticalDiff = Mathf.Abs(backHeight - frontHeight);

            // 取最大倾斜作为当前倾斜角度（简化计算）
            // 实际角度 = arctan(高度差/距离) 转换为度
            // 这里简化为：高度差 * 常数（假设支撑点距离约为2米）
            float maxHeightDiff = Mathf.Max(horizontalDiff, verticalDiff);
            currentTiltAngle = Mathf.Atan(maxHeightDiff / 2f) * Mathf.Rad2Deg;

            // 应用倾斜到地基
            ApplyTilt(currentTiltAngle);
        }

        /// <summary>
        /// 应用倾斜效果到地基（2D版本）
        /// </summary>
        private void ApplyTilt(float tiltAngle)
        {
            if (pavilionFoundation != null)
            {
                // 2D游戏使用Z轴旋转
                float tiltAmount = Mathf.Clamp(tiltAngle, 0f, 5f);
                pavilionFoundation.localRotation = Quaternion.Euler(0f, 0f, tiltAmount);
            }
        }

        #endregion

        #region 木桩更换系统

        /// <summary>
        /// 标记木桩已测量（由鲁班尺系统调用）
        /// </summary>
        public void MarkPileMeasured(int pileIndex)
        {
            if (pileIndex >= 0 && pileIndex < 6)
            {
                isPileMeasured[pileIndex] = true;
                Debug.Log($"木桩 {pileIndex + 1} 已完成测量");
            }
        }

        /// <summary>
        /// 更换木桩
        /// </summary>
        public bool ReplacePile(int pileIndex, GameObject newPilePrefab)
        {
            if (pileIndex < 0 || pileIndex >= 6)
            {
                Debug.LogError("木桩索引无效");
                return false;
            }

            if (!isPileMeasured[pileIndex])
            {
                UIManager.Instance?.ShowMessage($"请先使用鲁班尺测量第{pileIndex + 1}根木桩！");
                return false;
            }

            // 销毁旧木桩
            if (woodPiles[pileIndex] != null)
            {
                // 播放更换特效
                PlayPileReplaceEffect(woodPiles[pileIndex].position);
                Destroy(woodPiles[pileIndex].gameObject);
            }

            // 生成新木桩（2D游戏：rotation保持原样或使用Quaternion.identity）
            if (newPilePrefab != null)
            {
                // 2D游戏中，rotation通常为Quaternion.identity或保持原木桩的Z轴旋转
                GameObject newPile = Instantiate(newPilePrefab, woodPiles[pileIndex].position, 
                                                 Quaternion.identity, transform);
                woodPiles[pileIndex] = newPile.transform;
            }

            repairedPilesCount++;
            UIManager.Instance?.ShowMessage($"已更换第{pileIndex + 1}根木桩 ({repairedPilesCount}/6)");
            
            UpdateStability();
            CheckRepairComplete();

            return true;
        }

        private void PlayPileReplaceEffect(Vector3 position)
        {
            // 播放烟尘特效
            Debug.Log("播放木桩更换特效");
            // TODO: 实例化粒子特效
        }

        #endregion

        #region 稳定性评估

        /// <summary>
        /// 更新稳定性评分
        /// </summary>
        private void UpdateStability()
        {
            // 木桩完成度权重：70%
            float pileScore = (repairedPilesCount / 6f) * 70f;

            // 倾斜度权重：30%
            float tiltScore = 0f;
            if (currentTiltAngle <= S_GRADE_TILT)
                tiltScore = 30f;
            else if (currentTiltAngle <= A_GRADE_TILT)
                tiltScore = 25f;
            else if (currentTiltAngle <= B_GRADE_TILT)
                tiltScore = 20f;
            else
                tiltScore = Mathf.Max(0f, 30f - (currentTiltAngle - B_GRADE_TILT) * 5f);

            stabilityScore = pileScore + tiltScore;
            stabilityScore = Mathf.Clamp(stabilityScore, 0f, 100f);

            OnStabilityChanged?.Invoke(stabilityScore);

            // 评级判定
            string grade = GetGrade();
            Debug.Log($"当前稳定性: {stabilityScore:F1}分 | 倾斜度: {currentTiltAngle:F2}° | 评级: {grade}");
        }

        /// <summary>
        /// 获取评级
        /// </summary>
        public string GetGrade()
        {
            if (repairedPilesCount < 6) return "未完成";
            
            if (currentTiltAngle <= S_GRADE_TILT)
                return "S级 - 完美";
            else if (currentTiltAngle <= A_GRADE_TILT)
                return "A级 - 优秀";
            else if (currentTiltAngle <= B_GRADE_TILT)
                return "B级 - 良好";
            else
                return "C级 - 合格";
        }

        /// <summary>
        /// 检查修复是否完成
        /// </summary>
        private void CheckRepairComplete()
        {
            if (repairedPilesCount >= 6 && stabilityScore >= 95f && !isRepairComplete)
            {
                isRepairComplete = true;
                string grade = GetGrade();
                
                UIManager.Instance?.ShowMessage($"地基修复完成！评级: {grade}");
                OnGradeAchieved?.Invoke(grade);
                OnRepairComplete?.Invoke();

                // 播放完工特效
                PlayCompletionEffect();
            }
        }

        private void PlayCompletionEffect()
        {
            Debug.Log("播放地基修复完成特效");
            // TODO: 播放金色光环特效
        }

        #endregion

        #region 公共接口

        public float GetCurrentTilt() => currentTiltAngle;
        public float GetStabilityScore() => stabilityScore;
        public int GetRepairedPilesCount() => repairedPilesCount;
        public bool IsWaterDrained() => isWaterDrained;
        public bool IsRepairComplete() => isRepairComplete;

        #endregion
    }
}
