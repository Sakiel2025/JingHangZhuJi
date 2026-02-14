using UnityEngine;
using System.Collections;

namespace JingHangZhuJi.Level3
{
    /// <summary>
    /// 第三关游戏流程控制器
    /// 负责协调各个系统，管理关卡流程
    /// </summary>
    public class Level3GameController : MonoBehaviour
    {
        public static Level3GameController Instance { get; private set; }

        /// <summary>
        /// 游戏阶段枚举
        /// </summary>
        public enum GamePhase
        {
            Intro,              // 开场剧情
            Investigation,      // 调查阶段（匠人之眼）
            DrainWater,        // 排水阶段
            AdjustSupport,     // 调整支撑
            ReplacePiles,      // 更换木桩
            NightTalk,         // 深夜长谈
            Completion         // 完成
        }

        [Header("游戏阶段")]
        [SerializeField] private GamePhase currentPhase = GamePhase.Intro;

        [Header("系统引用")]
        [SerializeField] private FoundationRepairManager repairManager;
        [SerializeField] private AIDialogueManager dialogueManager;
        [SerializeField] private LuBanRuler luBanRuler;
        [SerializeField] private CraftsmanEyeSystem craftsmanEye; // 匠人之眼系统
        [SerializeField] private FoundationStateManager foundationStateManager; // 地基状态管理器

        [Header("场景对象")]
        [SerializeField] private WoodPilesManager woodPilesManager; // 木桩管理器(一体化4状态)
        [SerializeField] private GameObject jackTool; // 千斤顶工具
        [SerializeField] private GameObject rulerTool; // 鲁班尺工具

        [Header("NPC")]
        [SerializeField] private GameObject huangLaoye; // 黄老爷

        [Header("时间控制")]
        [SerializeField] private Light mainLight; // 主光源（2D使用Directional Light 2D或全局照明）
        [SerializeField] private float nightLightIntensity = 0.3f;
        
        // 常量
        private const float DAY_LIGHT_INTENSITY = 1f;

        [Header("关卡目标")]
        private bool[] phaseCompleted = new bool[7];
        private float levelStartTime;
        private int playerScore = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeLevel();
            SetupToolbarButtons(); // 绑定工具栏按钮
            StartLevel();
        }

        /// <summary>
        /// 初始化关卡
        /// </summary>
        private void InitializeLevel()
        {
            levelStartTime = Time.time;

            // 获取系统引用
            if (repairManager == null)
                repairManager = FindObjectOfType<FoundationRepairManager>();
            if (dialogueManager == null)
                dialogueManager = FindObjectOfType<AIDialogueManager>();
            if (luBanRuler == null)
                luBanRuler = FindObjectOfType<LuBanRuler>();
            if (foundationStateManager == null)
                foundationStateManager = FindObjectOfType<FoundationStateManager>();

            // 注册事件
            RegisterEvents();

            // 初始化工具（开始时都隐藏）
            if (rulerTool != null) rulerTool.SetActive(false);
            if (jackTool != null) jackTool.SetActive(false);

            // 初始化木桩状态为正常
            if (woodPilesManager != null)
            {
                woodPilesManager.SetState(WoodPilesState.Normal);
            }

            Debug.Log("第三关初始化完成");
        }

        /// <summary>
        /// 注册事件监听
        /// </summary>
        private void RegisterEvents()
        {
            if (repairManager != null)
            {
                repairManager.OnStabilityChanged += OnStabilityChanged;
                repairManager.OnGradeAchieved += OnGradeAchieved;
                repairManager.OnRepairComplete += OnRepairComplete;
            }

            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueComplete += OnDialogueComplete;
            }
        }

        /// <summary>
        /// 开始关卡
        /// </summary>
        private void StartLevel()
        {
            currentPhase = GamePhase.Intro;
            StartCoroutine(IntroSequence());
        }

        #region 关卡流程

        /// <summary>
        /// 开场剧情
        /// </summary>
        private IEnumerator IntroSequence()
        {
            UIManager.Instance?.ShowMessage("扬州·盐商园中六角亭");
            yield return new WaitForSeconds(2f);

            // 显示对话
            UIManager.Instance?.ShowDialogue("黄老爷", 
                "这座六角亭是我耗费十年心血建造的...如今地基下沉，已经倾斜严重。还望匠人施以援手！", 
                false);
            yield return new WaitForSeconds(4f);

            // 进入调查阶段
            phaseCompleted[0] = true;
            TransitionToPhase(GamePhase.Investigation);
        }

        /// <summary>
        /// 调查阶段
        /// </summary>
        private void StartInvestigation()
        {
            UIManager.Instance?.ShowMessage("使用匠人之眼扫描六角亭");
            
            // 激活匠人之眼
            if (craftsmanEye != null)
            {
                craftsmanEye.Activate();
            }
            else
            {
                // 如果没有匠人之眼系统，直接跳过
                OnInvestigationComplete();
            }
        }

        public void OnInvestigationComplete()
        {
            UIManager.Instance?.ShowMessage("发现问题：地基积水严重，6根木桩腐朽，地基倾斜2.5°");
            phaseCompleted[1] = true;
            
            // 完成任务0：使用匠人之眼扫描亭子
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(0);
            }
            
            // 切换木桩状态为腐朽
            if (woodPilesManager != null)
            {
                woodPilesManager.SetState(WoodPilesState.Rotten);
            }
            
            StartCoroutine(TransitionAfterDelay(GamePhase.AdjustSupport, 3f)); // 跳过排水阶段
        }

        /// <summary>
        /// 排水阶段（已简化，直接跳过）
        /// </summary>
        private void StartDrainWater()
        {
            // 排水阶段已被简化移除
            Debug.Log("排水阶段已简化，直接进入支撑调整");
            TransitionToPhase(GamePhase.AdjustSupport);
        }

        /// <summary>
        /// 调整支撑阶段（简化版）
        /// </summary>
        private void StartAdjustSupport()
        {
            UIManager.Instance?.ShowMessage("使用千斤顶掀开台基");
            
            // 解锁千斤顶工具
            UnlockTool("jack");
            
            // 显示千斤顶工具
            if (jackTool != null)
            {
                jackTool.SetActive(true);
            }

            // 完成任务2：安装千斤顶并掀开台基
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(1);
            }

            // 简化流程：千斤顶自动使用（跳过按钮点击）
            UIManager.Instance?.ShowMessage("正在使用千斤顶...");
            phaseCompleted[3] = true;
            
            // 切换地基状态
            if (foundationStateManager != null)
            {
                foundationStateManager.SetState(FoundationState.Opened);
            }
            
            // 2秒后自动进入下一阶段
            StartCoroutine(TransitionAfterDelay(GamePhase.ReplacePiles, 2f));
        }

        /// <summary>
        /// 更换木桩阶段（简化版）
        /// </summary>
        private void StartReplacePiles()
        {
            UIManager.Instance?.ShowMessage("使用鲁班尺测量并更换木桩");
            
            // 解锁鲁班尺工具
            UnlockTool("ruler");
            
            // 显示鲁班尺工具
            if (rulerTool != null)
            {
                rulerTool.SetActive(true);
            }

            // 切换木桩状态为测量中
            if (woodPilesManager != null)
            {
                woodPilesManager.SetState(WoodPilesState.Measuring);
            }

            // 简化流程：自动开始测量（2秒后）
            StartCoroutine(AutoStartMeasurement());
        }

        /// <summary>
        /// 自动开始测量（延迟2秒）
        /// </summary>
        private IEnumerator AutoStartMeasurement()
        {
            yield return new WaitForSeconds(2f);
            StartCoroutine(MeasurementSequence());
        }

        private IEnumerator MeasurementSequence()
        {
            UIManager.Instance?.ShowMessage("正在测量木桩垂直度...");
            yield return new WaitForSeconds(2f);

            UIManager.Instance?.ShowMessage("测量完成！木桩已更换并调整合格");
            
            // 切换木桩状态为合格
            if (woodPilesManager != null)
            {
                woodPilesManager.SetState(WoodPilesState.Qualified);
            }

            // 完成任务3：更换木桩并测量垂直度
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(2);
            }

            phaseCompleted[4] = true;
            
            yield return new WaitForSeconds(2f);
            TransitionToPhase(GamePhase.Completion); // 简化版直接完成，跳过深夜长谈
        }

        /// <summary>
        /// 深夜长谈阶段
        /// </summary>
        private void StartNightTalk()
        {
            // 切换到夜晚
            StartCoroutine(TransitionToNight());
        }

        private IEnumerator TransitionToNight()
        {
            UIManager.Instance?.ShowMessage("夜幕降临...");
            
            // 渐变光照
            float duration = 3f;
            float elapsed = 0f;
            float startIntensity = mainLight.intensity;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                mainLight.intensity = Mathf.Lerp(startIntensity, nightLightIntensity, elapsed / duration);
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            // 开始对话
            UIManager.Instance?.ShowDialogue("黄老爷", 
                "匠人技艺了得！不如今晚在园中畅谈，老夫有些心事想与你探讨。", 
                false);
            
            yield return new WaitForSeconds(3f);

            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue();
            }
        }

        public void OnDialoguePhaseComplete()
        {
            phaseCompleted[5] = true;
            TransitionToPhase(GamePhase.Completion);
        }

        /// <summary>
        /// 完成阶段
        /// </summary>
        private void CompleteLevel()
        {
            phaseCompleted[6] = true;

            // 完成任务3：重铺石质台基
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(3);
            }

            // 计算总分
            CalculateFinalScore();

            // 显示完成UI
            UIManager.Instance?.ShowLevelComplete(
                levelName: "扬州·盐商园中六角亭",
                grade: repairManager.GetGrade(),
                score: playerScore,
                time: Time.time - levelStartTime
            );

            // 解锁下一关
            GameManager.Instance?.UnlockLevel(4);
        }

        #endregion

        #region 事件回调

        private void OnStabilityChanged(float stability)
        {
            // 更新UI显示稳定性
            UIManager.Instance?.UpdateStabilityBar(stability);
        }

        private void OnGradeAchieved(string grade)
        {
            Debug.Log($"获得评级: {grade}");
            
            // 根据评级给予奖励
            int bonus = 0;
            if (grade.Contains("S")) bonus = 1000;
            else if (grade.Contains("A")) bonus = 500;
            else if (grade.Contains("B")) bonus = 300;
            else bonus = 100;

            GameManager.Instance?.AddSilver(bonus);
            playerScore += bonus;
        }

        private void OnRepairComplete()
        {
            Debug.Log("地基修复完成！");
        }

        private void OnDialogueComplete()
        {
            Debug.Log("对话完成");
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 切换到新阶段
        /// </summary>
        private void TransitionToPhase(GamePhase newPhase)
        {
            currentPhase = newPhase;
            Debug.Log($"进入阶段: {newPhase}");

            // 根据游戏阶段自动切换地基状态
            if (foundationStateManager != null)
            {
                foundationStateManager.SetStateByGamePhase(newPhase);
            }

            switch (newPhase)
            {
                case GamePhase.Investigation:
                    StartInvestigation();
                    break;
                case GamePhase.DrainWater:
                    StartDrainWater();
                    break;
                case GamePhase.AdjustSupport:
                    StartAdjustSupport();
                    break;
                case GamePhase.ReplacePiles:
                    StartReplacePiles();
                    break;
                case GamePhase.NightTalk:
                    StartNightTalk();
                    break;
                case GamePhase.Completion:
                    CompleteLevel();
                    break;
            }
        }

        private IEnumerator TransitionAfterDelay(GamePhase newPhase, float delay)
        {
            yield return new WaitForSeconds(delay);
            TransitionToPhase(newPhase);
        }

        /// <summary>
        /// 计算最终得分
        /// </summary>
        private void CalculateFinalScore()
        {
            // 基础分
            playerScore += 500;

            // 时间加成
            float timeTaken = Time.time - levelStartTime;
            if (timeTaken < 600f) // 10分钟内完成
                playerScore += 300;
            else if (timeTaken < 900f) // 15分钟内完成
                playerScore += 150;

            // 对话质量加成
            if (dialogueManager != null)
            {
                var stats = dialogueManager.GetResponseStats();
                playerScore += stats[AIDialogueManager.ResponseLevel.Profound] * 100;
                playerScore += stats[AIDialogueManager.ResponseLevel.Deep] * 50;
            }

            Debug.Log($"最终得分: {playerScore}");
        }

        #endregion

        #region 工具栏设置

        /// <summary>
        /// 设置工具栏按钮事件
        /// </summary>
        private void SetupToolbarButtons()
        {
            ToolbarUI toolbarUI = FindObjectOfType<ToolbarUI>();
            if (toolbarUI == null)
            {
                Debug.LogWarning("未找到 ToolbarUI，工具栏功能将不可用");
                return;
            }

            // 匠人之眼按钮
            toolbarUI.SetupCraftsmanEyeButton(() => {
                if (craftsmanEye != null)
                {
                    craftsmanEye.Activate(); // 正确的方法名
                }
                else
                {
                    Debug.LogWarning("匠人之眼系统未配置");
                }
            });
            
            // 鲁班尺按钮 - 鲁班尺需要先选择木桩才能测量,所以这里只是提示
            toolbarUI.SetupRulerButton(() => {
                if (luBanRuler != null)
                {
                    UIManager.Instance?.ShowMessage("请点击木桩开始测量");
                    // 实际测量在 AddPileInteraction 中通过点击木桩触发
                }
                else
                {
                    Debug.LogWarning("鲁班尺系统未配置");
                }
            });
            
            // 千斤顶按钮
            toolbarUI.SetupJackButton(() => {
                ShowSupportControlPanel();
            });

            Debug.Log("工具栏按钮绑定完成");
        }

        /// <summary>
        /// 显示千斤顶控制面板（简化版）
        /// </summary>
        private void ShowSupportControlPanel()
        {
            UIManager.Instance?.ShowMessage("点击千斤顶按钮使用工具");
        }

        /// <summary>
        /// 解锁工具（在游戏流程中调用）
        /// </summary>
        public void UnlockTool(string toolName)
        {
            ToolbarUI toolbarUI = FindObjectOfType<ToolbarUI>();
            if (toolbarUI != null)
            {
                toolbarUI.SetToolUnlocked(toolName, true);
                UIManager.Instance?.ShowMessage($"解锁工具：{toolName}");
            }
        }

        #endregion

        #region 公共接口

        public GamePhase GetCurrentPhase() => currentPhase;
        public bool IsPhaseComplete(GamePhase phase) => phaseCompleted[(int)phase];
        public int GetPlayerScore() => playerScore;

        /// <summary>
        /// 跳过当前阶段（调试用）
        /// </summary>
        public void SkipCurrentPhase()
        {
            GamePhase nextPhase = (GamePhase)(((int)currentPhase + 1) % 7);
            TransitionToPhase(nextPhase);
        }

        #endregion
    }
}
