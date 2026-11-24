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
        [SerializeField] private AIDialogueManager dialogueManager; // 兼容旧版
        [SerializeField] private Level3_YangzhouPavilion.AIDialogueManager_Enhanced dialogueManagerEnhanced; // 新版AI对话系统
        [SerializeField] private LuBanRuler luBanRuler;
        [SerializeField] private CraftsmanEyeSystem craftsmanEye; // 匠人之眼系统
        [SerializeField] private FoundationStateManager foundationStateManager; // 地基状态管理器

        [Header("场景对象")]
        [SerializeField] private WoodPilesManager woodPilesManager; // 木桩管理器(一体化4状态)
        [SerializeField] private GameObject jackTool; // 千斤顶工具
        [SerializeField] private GameObject rulerTool; // 鲁班尺工具

        [Header("NPC")]
        [SerializeField] private GameObject huangLaoye; // 黄老爷

        [Header("配音")]
        [SerializeField] private AudioClip nightTalkOpeningVoice; // 深夜长谈开场白配音
        
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
            yield return new WaitForSeconds(3f);

            // 直接进入调查阶段，不显示对话框
            phaseCompleted[0] = true;
            TransitionToPhase(GamePhase.Investigation);
        }

        /// <summary>
        /// 调查阶段
        /// </summary>
        private void StartInvestigation()
        {
            // 只提示玩家，不自动激活——等玩家点击工匠之眼按钮后才触发
            UIManager.Instance?.ShowMessage("请点击工具栏中的【匠人之眼】扫描六角亭");
        }

        public void OnInvestigationComplete()
        {
            UIManager.Instance?.ShowMessage("发现问题：地基积水严重，6根木桩腐朽，地基倾斜2.5°");
            phaseCompleted[1] = true;

            // 扫描完成后锁定工匠之眼按钮
            ToolbarUI toolbarUI = FindObjectOfType<ToolbarUI>();
            if (toolbarUI != null)
            {
                toolbarUI.SetToolUnlocked("craftsmaneye", false);
            }
            
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
            
            // 延长等待时间，让玩家看清楚倾斜的台基和腐朽的木桩
            StartCoroutine(TransitionAfterDelay(GamePhase.AdjustSupport, 5f)); // 从3秒改为5秒
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
            UIManager.Instance?.ShowMessage("请点击工具栏中的【千斤顶】使用工具");
            
            // 仅解锁按钮，不直接显示工具——等玩家点击按钮后才显示
            UnlockTool("jack");

            // 预先设置拖动回调（工具显示后立即生效）
            if (jackTool != null)
            {
                jackTool.SetActive(false); // 确保初始隐藏

                SimpleDraggableTool simpleDraggable = jackTool.GetComponent<SimpleDraggableTool>();
                if (simpleDraggable != null)
                {
                    simpleDraggable.OnToolPlaced = () => { OnJackPlaced(); };
                    Debug.Log("[Level3] 千斤顶回调已设置（SimpleDraggableTool）");
                }
                else
                {
                    DraggableTool draggable = jackTool.GetComponent<DraggableTool>();
                    if (draggable != null)
                    {
                        draggable.OnToolPlaced = () => { OnJackPlaced(); };
                        Debug.Log("[Level3] 千斤顶回调已设置（DraggableTool）");
                    }
                    else
                    {
                        Debug.LogWarning("[Level3] 千斤顶缺少拖动组件，将在按钮点击后3秒自动完成");
                    }
                }
            }
        }

        /// <summary>
        /// 千斤顶放置完成
        /// </summary>
        private void OnJackPlaced()
        {
            UIManager.Instance?.ShowMessage("千斤顶已就位！正在掀开台基...");

            // 千斤顶使用完毕，锁定按钮
            ToolbarUI toolbarUI = FindObjectOfType<ToolbarUI>();
            if (toolbarUI != null)
            {
                toolbarUI.SetToolUnlocked("jack", false);
            }

            // 完成任务1：安装千斤顶并掀开台基
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(1);
            }
            
            phaseCompleted[3] = true;
            
            // 切换地基状态
            if (foundationStateManager != null)
            {
                foundationStateManager.SetState(FoundationState.Opened);
            }
            
            // 2秒后进入下一阶段
            StartCoroutine(TransitionAfterDelay(GamePhase.ReplacePiles, 2f));
        }

        /// <summary>
        /// 千斤顶自动完成(没有拖动组件时的备用方案)
        /// </summary>
        private System.Collections.IEnumerator AutoCompleteJack()
        {
            yield return new WaitForSeconds(3f);
            OnJackPlaced();
        }

        /// <summary>
        /// 更换木桩阶段（简化版）
        /// </summary>
        private void StartReplacePiles()
        {
            UIManager.Instance?.ShowMessage("请点击工具栏中的【鲁班尺】使用工具");
            
            // 仅解锁按钮，不直接显示工具——等玩家点击按钮后才显示
            UnlockTool("ruler");

            // 预先设置拖动回调（工具显示后立即生效）
            if (rulerTool != null)
            {
                rulerTool.SetActive(false); // 确保初始隐藏

                SimpleDraggableTool simpleDraggable = rulerTool.GetComponent<SimpleDraggableTool>();
                if (simpleDraggable != null)
                {
                    simpleDraggable.OnToolPlaced = () => { OnRulerPlaced(); };
                    Debug.Log("[Level3] 鲁班尺回调已设置（SimpleDraggableTool）");
                }
                else
                {
                    DraggableTool draggable = rulerTool.GetComponent<DraggableTool>();
                    if (draggable != null)
                    {
                        draggable.OnToolPlaced = () => { OnRulerPlaced(); };
                        Debug.Log("[Level3] 鲁班尺回调已设置（DraggableTool）");
                    }
                    else
                    {
                        Debug.LogWarning("[Level3] 鲁班尺缺少拖动组件，将在按钮点击后自动开始测量");
                    }
                }
            }

            // 切换木桩状态为测量中
            if (woodPilesManager != null)
            {
                woodPilesManager.SetState(WoodPilesState.Measuring);
            }
        }

        /// <summary>
        /// 鲁班尺放置完成,开始测量
        /// </summary>
        private void OnRulerPlaced()
        {
            UIManager.Instance?.ShowMessage("鲁班尺已就位！开始测量...");
            StartCoroutine(MeasurementSequence());
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

            // 鲁班尺使用完毕，锁定按钮
            ToolbarUI toolbarUI = FindObjectOfType<ToolbarUI>();
            if (toolbarUI != null)
            {
                toolbarUI.SetToolUnlocked("ruler", false);
            }

            // 切换木桩状态为合格
            if (woodPilesManager != null)
            {
                woodPilesManager.SetState(WoodPilesState.Qualified);
            }

            // ⭐ 重要：完成木桩修复，自动恢复台基倾斜度为 0°
            FoundationRepairManager repairManager = FindObjectOfType<FoundationRepairManager>();
            if (repairManager != null)
            {
                repairManager.CompletePileRepair();
                Debug.Log("[Level3] 调用 CompletePileRepair()，台基倾斜度将恢复为 0°");
            }
            else
            {
                Debug.LogWarning("[Level3] 未找到 FoundationRepairManager！台基倾斜度不会自动恢复");
            }

            // 完成任务3：更换木桩并测量垂直度
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(2);
            }

            phaseCompleted[4] = true;
            
            yield return new WaitForSeconds(2f);
            
            // 进入深夜长谈阶段
            TransitionToPhase(GamePhase.NightTalk);
        }

        /// <summary>
        /// 深夜长谈阶段
        /// </summary>
        private void StartNightTalk()
        {
            Debug.Log("=== StartNightTalk 被调用 ===");
            Debug.Log($"dialogueManager 是否为空: {dialogueManager == null}");

            // 对话出现前，先完成任务四（索引3）
            global::Level3.UI.TaskListUI taskList = FindObjectOfType<global::Level3.UI.TaskListUI>();
            if (taskList != null)
            {
                taskList.CompleteTask(3);
            }

            UIManager.Instance?.ShowMessage("夜幕降临，黄老爷邀请你深夜长谈...");
            
            // 先显示开场对话，等待玩家点击"继续"后再启动AI对话系统
            UIManager.Instance?.ShowDialogue("黄老爷", 
                "匠人技艺了得！不如今晚在园中畅谈，老夫有些心事想与你探讨。", 
                false,
                () => {
                    // 玩家点击"继续"后，启动AI对话系统
                    // 优先使用新版增强对话管理器
                    if (dialogueManagerEnhanced != null)
                    {
                        Debug.Log("✅ 使用增强版AI对话系统");
                        dialogueManagerEnhanced.StartDialogue();
                    }
                    else if (dialogueManager != null)
                    {
                        Debug.Log("✅ 使用旧版对话系统");
                        dialogueManager.StartDialogue();
                    }
                    else
                    {
                        Debug.LogError("❌ 对话管理器未配置！请在 Inspector 中配置以下任意一个字段：\n" +
                                     "- Dialogue Manager Enhanced（推荐，新版AI系统）\n" +
                                     "- Dialogue Manager（旧版）");
                    }
                },
                nightTalkOpeningVoice);
        }

        public void OnDialoguePhaseComplete()
        {
            phaseCompleted[5] = true;
            
            // 深夜长谈完成后，切换到师父第三封信场景
            Debug.Log("深夜长谈完成，准备切换到 Level3_Letter 场景");
            StartCoroutine(LoadLetterScene());
        }
        
        /// <summary>
        /// 加载师父第三封信场景
        /// </summary>
        private IEnumerator LoadLetterScene()
        {
            UIManager.Instance?.ShowMessage("黄老爷有话要说...");
            yield return new WaitForSeconds(2f);
            
            // 切换场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level3_Letter");
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
            
            // ⭐ 保存进度和更新碎片数
            SaveLevelProgress();

            // 解锁下一关
            GameManager.Instance?.UnlockLevel(4);
        }
        
        /// <summary>
        /// 保存关卡进度和碎片数（仿照第一关实现）
        /// </summary>
        private void SaveLevelProgress()
        {
            // 1. 从 lastLogin 取出当前登录的用户名
            string username = PlayerPrefs.GetString("lastLogin", "");

            // 防呆：如果没有登录用户，直接报错返回
            if (string.IsNullOrEmpty(username))
            {
                Debug.LogError("⚠️ 没有获取到当前登录用户，无法更新关卡和碎片！");
                return;
            }

            // 2. 读取并更新关卡数（对应 KEY: level_用户名）
            int oldLevel = PlayerPrefs.GetInt("level_" + username, 0);
            int newLevel = oldLevel + 1;
            PlayerPrefs.SetInt("level_" + username, newLevel);

            // 3. 读取并更新碎片数（对应 KEY: fragment_用户名）
            int oldFragment = PlayerPrefs.GetInt("fragment_" + username, 0);
            int newFragment = oldFragment + 1;
            PlayerPrefs.SetInt("fragment_" + username, newFragment);

            // 4. 保存到本地
            PlayerPrefs.Save();

            // 5. 打印日志，方便验证
            Debug.Log($"✅ 第3关结束：\n" +
                      $"关卡数：{oldLevel} → {newLevel}\n" +
                      $"碎片数：{oldFragment} → {newFragment}");
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

            // Investigation 阶段保持 Normal，等玩家点击工匠之眼后再切换为 Tilted
            if (foundationStateManager != null && newPhase != GamePhase.Investigation)
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
            
            // 鲁班尺按钮：显示工具后，若没有拖动组件则自动开始测量
            toolbarUI.SetupRulerButton(() => {
                if (rulerTool != null)
                {
                    rulerTool.SetActive(true);
                    bool hasDraggable = rulerTool.GetComponent<SimpleDraggableTool>() != null
                                     || rulerTool.GetComponent<DraggableTool>() != null;
                    if (!hasDraggable)
                        StartCoroutine(AutoStartMeasurement());
                }
                UIManager.Instance?.ShowMessage("拖动鲁班尺到木桩位置进行测量");
            });
            
            // 千斤顶按钮：显示工具后，若没有拖动组件则自动完成
            toolbarUI.SetupJackButton(() => {
                if (jackTool != null)
                {
                    jackTool.SetActive(true);
                    bool hasDraggable = jackTool.GetComponent<SimpleDraggableTool>() != null
                                     || jackTool.GetComponent<DraggableTool>() != null;
                    if (!hasDraggable)
                        StartCoroutine(AutoCompleteJack());
                }
                UIManager.Instance?.ShowMessage("拖动千斤顶到台基下方");
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
