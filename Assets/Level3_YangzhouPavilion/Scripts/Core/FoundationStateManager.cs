using UnityEngine;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 台基状态枚举
    /// </summary>
    public enum FoundationState
    {
        Normal,      // 正常状态
        Tilted,      // 倾斜损坏状态
        Opened,      // 开挖施工状态
        Repaired     // 修复完成状态
    }

    /// <summary>
    /// 台基状态管理器
    /// 管理六角亭台基的4种状态切换
    /// 由 Level3GameController 根据剧情进度控制状态变化
    /// </summary>
    public class FoundationStateManager : MonoBehaviour
    {
        [Header("台基状态精灵")]
        [SerializeField] private GameObject normalState;    // foundation_normal
        [SerializeField] private GameObject tiltedState;    // foundation_tilted
        [SerializeField] private GameObject openedState;    // foundation_opened
        [SerializeField] private GameObject repairedState;  // foundation_repaired

        private FoundationState currentState = FoundationState.Normal;

        /// <summary>
        /// 设置台基状态
        /// </summary>
        /// <param name="newState">目标状态</param>
        public void SetState(FoundationState newState)
        {
            // 隐藏所有状态
            if (normalState != null) normalState.SetActive(false);
            if (tiltedState != null) tiltedState.SetActive(false);
            if (openedState != null) openedState.SetActive(false);
            if (repairedState != null) repairedState.SetActive(false);

            // 显示目标状态
            switch (newState)
            {
                case FoundationState.Normal:
                    if (normalState != null) normalState.SetActive(true);
                    break;
                case FoundationState.Tilted:
                    if (tiltedState != null) tiltedState.SetActive(true);
                    break;
                case FoundationState.Opened:
                    if (openedState != null) openedState.SetActive(true);
                    break;
                case FoundationState.Repaired:
                    if (repairedState != null) repairedState.SetActive(true);
                    break;
            }

            currentState = newState;
            Debug.Log($"[FoundationStateManager] 台基状态切换为: {newState}");
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns>当前台基状态</returns>
        public FoundationState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// 初始化状态
        /// 注意：不自动切换状态，由 Level3GameController 根据剧情控制
        /// </summary>
        void Start()
        {
            // 隐藏所有状态，等待控制器调用
            if (normalState != null) normalState.SetActive(false);
            if (tiltedState != null) tiltedState.SetActive(false);
            if (openedState != null) openedState.SetActive(false);
            if (repairedState != null) repairedState.SetActive(false);
        }

        /// <summary>
        /// 测试用快捷键（仅在编辑器模式下启用）
        /// 用于开发调试，正式版本不会响应这些按键
        /// 1-正常 2-倾斜 3-开挖 4-修复
        /// </summary>
        void Update()
        {
            #if UNITY_EDITOR
            // ⚠️ 重要：只在没有输入框聚焦时才响应快捷键（避免与玩家输入冲突）
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null)
            {
                // 如果有UI对象被选中（例如输入框），不响应快捷键
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetState(FoundationState.Normal);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetState(FoundationState.Tilted);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetState(FoundationState.Opened);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SetState(FoundationState.Repaired);
            #endif
        }

        /// <summary>
        /// 根据游戏阶段自动设置对应的台基状态
        /// 由 Level3GameController 调用
        /// </summary>
        public void SetStateByGamePhase(Level3GameController.GamePhase phase)
        {
            switch (phase)
            {
                case Level3GameController.GamePhase.Intro:
                    SetState(FoundationState.Normal);
                    break;
                case Level3GameController.GamePhase.Investigation:
                    SetState(FoundationState.Tilted);  // 匠人之眼发现倾斜
                    break;
                case Level3GameController.GamePhase.DrainWater:
                    SetState(FoundationState.Tilted);  // 排水阶段仍然倾斜
                    break;
                case Level3GameController.GamePhase.AdjustSupport:
                    SetState(FoundationState.Opened);  // 开挖地基，露出木桩
                    break;
                case Level3GameController.GamePhase.ReplacePiles:
                    SetState(FoundationState.Opened);  // 更换木桩时保持开挖状态
                    break;
                case Level3GameController.GamePhase.NightTalk:
                    SetState(FoundationState.Repaired); // 深夜对话时已修复完成
                    break;
                case Level3GameController.GamePhase.Completion:
                    SetState(FoundationState.Repaired); // 结算时展示修复成果
                    break;
            }
        }
    }
}
