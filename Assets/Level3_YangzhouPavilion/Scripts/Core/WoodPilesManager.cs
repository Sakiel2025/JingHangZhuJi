using UnityEngine;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 木桩整体状态管理器（子物体架构，与 FoundationStateManager 一致）
    /// 负责管理6根木桩整体的4种状态切换
    /// </summary>
    public enum WoodPilesState
    {
        Normal,      // 正常新桩（6根）
        Rotten,      // 腐朽旧桩（6根）
        Measuring,   // 测量中（6根）
        Qualified    // 验收合格（6根）
    }

    public class WoodPilesManager : MonoBehaviour
    {
        [Header("四种状态的子物体")]
        [SerializeField] private GameObject normalState;    // WoodPiles_Normal
        [SerializeField] private GameObject rottenState;    // WoodPiles_Rotten
        [SerializeField] private GameObject measuringState; // WoodPiles_Measuring
        [SerializeField] private GameObject qualifiedState; // WoodPiles_Qualified

        private WoodPilesState currentState = WoodPilesState.Rotten;

        /// <summary>
        /// 初始化 - 隐藏所有状态，等待控制器调用
        /// </summary>
        void Start()
        {
            // 隐藏所有状态，等待控制器调用
            if (normalState != null) normalState.SetActive(false);
            if (rottenState != null) rottenState.SetActive(false);
            if (measuringState != null) measuringState.SetActive(false);
            if (qualifiedState != null) qualifiedState.SetActive(false);

            Debug.Log("[WoodPilesManager] 初始化完成，所有状态已隐藏");
        }

        /// <summary>
        /// 切换木桩状态（整体切换）
        /// </summary>
        public void SetState(WoodPilesState newState)
        {
            // 隐藏所有状态
            if (normalState != null) normalState.SetActive(false);
            if (rottenState != null) rottenState.SetActive(false);
            if (measuringState != null) measuringState.SetActive(false);
            if (qualifiedState != null) qualifiedState.SetActive(false);

            // 显示目标状态
            switch (newState)
            {
                case WoodPilesState.Normal:
                    if (normalState != null) 
                        normalState.SetActive(true);
                    else
                        Debug.LogWarning("[WoodPilesManager] normalState 未设置！");
                    break;

                case WoodPilesState.Rotten:
                    if (rottenState != null) 
                        rottenState.SetActive(true);
                    else
                        Debug.LogWarning("[WoodPilesManager] rottenState 未设置！");
                    break;

                case WoodPilesState.Measuring:
                    if (measuringState != null) 
                        measuringState.SetActive(true);
                    else
                        Debug.LogWarning("[WoodPilesManager] measuringState 未设置！");
                    break;

                case WoodPilesState.Qualified:
                    if (qualifiedState != null) 
                        qualifiedState.SetActive(true);
                    else
                        Debug.LogWarning("[WoodPilesManager] qualifiedState 未设置！");
                    break;
            }

            currentState = newState;
            Debug.Log($"[WoodPilesManager] 木桩状态切换为: {newState}");
        }

        /// <summary>
        /// 显示木桩（AdjustSupport 阶段调用）
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            SetState(WoodPilesState.Rotten); // 显示时默认为腐朽状态
            Debug.Log("[WoodPilesManager] 木桩系统已显示（腐朽状态）");
        }

        /// <summary>
        /// 隐藏木桩（NightTalk 阶段调用）
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            Debug.Log("[WoodPilesManager] 木桩系统已隐藏");
        }

        /// <summary>
        /// 玩家更换木桩（剧情调用）
        /// </summary>
        public void ReplaceWithNew()
        {
            SetState(WoodPilesState.Normal);
            Debug.Log("[WoodPilesManager] 木桩已更换为新桩");
        }

        /// <summary>
        /// 开始测量（剧情调用）
        /// </summary>
        public void StartMeasuring()
        {
            SetState(WoodPilesState.Measuring);
            Debug.Log("[WoodPilesManager] 开始测量木桩垂直度");
        }

        /// <summary>
        /// 测量合格（剧情调用）
        /// </summary>
        public void SetQualified()
        {
            SetState(WoodPilesState.Qualified);
            Debug.Log("[WoodPilesManager] 木桩测量合格！");
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        public WoodPilesState GetCurrentState() => currentState;

        /// <summary>
        /// 测试用快捷键（仅编辑器模式）
        /// Q-腐朽 W-正常 E-测量 R-合格
        /// </summary>
        void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Q)) 
            {
                SetState(WoodPilesState.Rotten);
                Debug.Log("[测试] 按下 Q - 切换到腐朽状态");
            }
            if (Input.GetKeyDown(KeyCode.W)) 
            {
                SetState(WoodPilesState.Normal);
                Debug.Log("[测试] 按下 W - 切换到正常状态");
            }
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                SetState(WoodPilesState.Measuring);
                Debug.Log("[测试] 按下 E - 切换到测量状态");
            }
            if (Input.GetKeyDown(KeyCode.R)) 
            {
                SetState(WoodPilesState.Qualified);
                Debug.Log("[测试] 按下 R - 切换到合格状态");
            }
            #endif
        }

        /// <summary>
        /// 在编辑器中验证配置
        /// </summary>
        void OnValidate()
        {
            #if UNITY_EDITOR
            // 检查所有必需的子物体是否已设置
            if (normalState == null)
                Debug.LogWarning("[WoodPilesManager] 请拖入 WoodPiles_Normal 子物体！", this);
            if (rottenState == null)
                Debug.LogWarning("[WoodPilesManager] 请拖入 WoodPiles_Rotten 子物体！", this);
            if (measuringState == null)
                Debug.LogWarning("[WoodPilesManager] 请拖入 WoodPiles_Measuring 子物体！", this);
            if (qualifiedState == null)
                Debug.LogWarning("[WoodPilesManager] 请拖入 WoodPiles_Qualified 子物体！", this);
            #endif
        }
    }
}
