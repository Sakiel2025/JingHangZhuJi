using UnityEngine;
using System.Collections;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 地基加固系统 - 简化版（去除排水，使用一体化木桩）
    /// </summary>
    public class FoundationRepairManager : MonoBehaviour
    {
        [Header("地基组件引用")]
        [SerializeField] private Transform pavilionFoundation; // 六角亭地基
        [SerializeField] private WoodPilesManager woodPilesManager; // 一体化木桩管理器

        [Header("测量数据")]
        [SerializeField] private float currentTiltAngle = 2.5f; // 当前倾斜角度（度）
        [SerializeField] private float stabilityScore = 0f; // 稳定性评分（0-100）
        
        // 常量
        private const float TARGET_TILT_ANGLE = 0f; // 目标倾斜角度（修复后应该完全不倾斜）

        [Header("状态标记")]
        private bool isRepairComplete = false;

        [Header("评级阈值")]
        private const float S_GRADE_TILT = 0.5f;  // S级：倾斜度 ≤ 0.5°（包括完全不倾斜）
        private const float A_GRADE_TILT = 1.0f;  // A级：倾斜度 ≤ 1.0°
        private const float B_GRADE_TILT = 2.0f;  // B级：倾斜度 ≤ 2.0°

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
            // 设置初始倾斜
            ApplyTilt(currentTiltAngle);
            UpdateStability();
            
            Debug.Log("[FoundationRepairManager] 初始化完成");
        }


        #region 简化的修复流程

        /// <summary>
        /// 完成木桩修复（简化版：一键完成）
        /// </summary>
        public void CompletePileRepair()
        {
            if (woodPilesManager != null)
            {
                woodPilesManager.SetQualified();
            }

            // 修复完成后，台基应该完全不倾斜
            currentTiltAngle = 0f;
            ApplyTilt(currentTiltAngle);
            
            UpdateStability();
            CheckRepairComplete();
            
            Debug.Log("[FoundationRepairManager] 木桩修复完成，台基倾斜度已恢复为 0°");
        }

        #endregion

        #region 地基倾斜处理

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

        #region 稳定性评估

        /// <summary>
        /// 更新稳定性评分
        /// </summary>
        private void UpdateStability()
        {
            // 简化评分：只基于倾斜度
            float tiltScore = 0f;
            if (currentTiltAngle <= S_GRADE_TILT)
                tiltScore = 100f;
            else if (currentTiltAngle <= A_GRADE_TILT)
                tiltScore = 85f;
            else if (currentTiltAngle <= B_GRADE_TILT)
                tiltScore = 70f;
            else
                tiltScore = Mathf.Max(50f, 100f - (currentTiltAngle * 20f));

            stabilityScore = Mathf.Clamp(tiltScore, 0f, 100f);

            OnStabilityChanged?.Invoke(stabilityScore);

            // 评级判定
            string grade = GetGrade();
            
            // 特别标注修复后的完美状态
            if (currentTiltAngle == 0f)
            {
                Debug.Log($"[FoundationRepairManager] 🎉 台基修复完成！当前稳定性: {stabilityScore:F1}分 | 倾斜度: {currentTiltAngle:F2}° (完全水平) | 评级: {grade}");
            }
            else
            {
                Debug.Log($"[FoundationRepairManager] 当前稳定性: {stabilityScore:F1}分 | 倾斜度: {currentTiltAngle:F2}° | 评级: {grade}");
            }
        }

        /// <summary>
        /// 获取评级
        /// </summary>
        public string GetGrade()
        {
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
            if (stabilityScore >= 85f && !isRepairComplete)
            {
                isRepairComplete = true;
                string grade = GetGrade();
                
                UIManager.Instance?.ShowMessage($"地基修复完成！评级: {grade}");
                OnGradeAchieved?.Invoke(grade);
                OnRepairComplete?.Invoke();

                Debug.Log("[FoundationRepairManager] 地基修复完成！");
            }
        }

        #endregion

        #region 公共接口

        public float GetCurrentTilt() => currentTiltAngle;
        public float GetStabilityScore() => stabilityScore;
        public bool IsRepairComplete() => isRepairComplete;

        #endregion
    }
}
