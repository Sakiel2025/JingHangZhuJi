using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Level4.Suzhou
{
    /// <summary>
    /// 场景8：2D DIY建造系统
    /// 玩家分4个部位选择构件组装亭子
    /// </summary>
    public class BuildingSceneController : MonoBehaviour
    {
        [Header("=== UI引用 ===")]
        [SerializeField] private TextMeshProUGUI titleText;           // 标题文本
        [SerializeField] private TextMeshProUGUI guideText;           // 引导文本
        [SerializeField] private Button confirmButton;                // 确认按钮
        [SerializeField] private GameObject resultPanel;              // 结果面板
        [SerializeField] private Button continueButton;               // 继续按钮

        [Header("=== 建造区域 ===")]
        [SerializeField] private RectTransform buildingArea;          // 建造区域（用于动态展示）
        [SerializeField] private PlacementZone baseZone;              // 基座区域
        [SerializeField] private PlacementZone pillarZone;            // 柱子区域
        [SerializeField] private PlacementZone roofZone;              // 屋顶区域
        [SerializeField] private DecorationZone decorationZone;       // 装饰区域（支持多位置）

        [Header("=== 部件容器 ===")]
        [SerializeField] private GameObject basePartsContainer;       // 基座选项容器
        [SerializeField] private GameObject pillarPartsContainer;     // 柱子选项容器
        [SerializeField] private GameObject roofPartsContainer;       // 屋顶选项容器
        [SerializeField] private GameObject decorationPartsContainer; // 装饰选项容器

        [Header("=== 音效 ===")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioClip selectSound;               // 选择音效
        [SerializeField] private AudioClip placeSound;                // 放置音效
        [SerializeField] private AudioClip completeSound;             // 完成音效

        // 当前建造阶段
        private enum BuildingStage
        {
            Base,       // 选择基座
            Pillar,     // 选择柱子
            Roof,       // 选择屋顶
            Decoration, // 选择装饰
            Complete    // 完成
        }

        private BuildingStage currentStage = BuildingStage.Base;
        private int totalScore = 0;

        // 最佳搭配方案（用于评分）
        private Dictionary<string, int> bestCombination = new Dictionary<string, int>()
        {
            { "Base_Stone", 25 },      // 石质基座
            { "Pillar_Wood", 25 },     // 木质圆柱
            { "Roof_Flying", 30 },     // 飞檐屋顶
            { "Style_Palace", 20 }     // 宫廷风装饰套装
        };

        private void Start()
        {
            InitializeScene();
            SetupEventListeners();
            StartStage(BuildingStage.Base);
        }

        private void InitializeScene()
        {
            // 初始化UI
            if (titleText != null)
                titleText.text = "建造小亭";

            if (resultPanel != null)
                resultPanel.SetActive(false);

            if (confirmButton != null)
                confirmButton.interactable = false;

            // 隐藏所有部件容器
            HideAllContainers();

            // 播放背景音乐
            if (bgmSource != null)
            {
                bgmSource.loop = true;
                bgmSource.Play();
            }

            Debug.Log("[BuildingScene] 场景初始化完成");
        }

        private void SetupEventListeners()
        {
            // 确认按钮事件
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmButtonClicked);
                Debug.Log("[BuildingScene] 确认按钮事件已绑定");
            }
            else
            {
                Debug.LogError("[BuildingScene] 确认按钮为空，无法绑定事件！");
            }

            // 继续按钮事件
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueButtonClicked);

            // 为所有放置区域添加事件监听
            if (baseZone != null) baseZone.OnPartPlaced += OnPartPlaced;
            if (pillarZone != null) pillarZone.OnPartPlaced += OnPartPlaced;
            if (roofZone != null) roofZone.OnPartPlaced += OnPartPlaced;
            if (decorationZone != null) decorationZone.OnStylePlaced += OnPartPlaced;
        }

        private void StartStage(BuildingStage stage)
        {
            currentStage = stage;
            HideAllContainers();

            Debug.Log($"[BuildingScene] ===== 进入新阶段: {stage} =====");

            switch (stage)
            {
                case BuildingStage.Base:
                    ShowStage("第一步：选择基座", basePartsContainer);
                    break;
                case BuildingStage.Pillar:
                    ShowStage("第二步：选择柱子", pillarPartsContainer);
                    break;
                case BuildingStage.Roof:
                    ShowStage("第三步：选择屋顶", roofPartsContainer);
                    break;
                case BuildingStage.Decoration:
                    Debug.Log("[BuildingScene] 准备显示装饰阶段...");
                    if (decorationPartsContainer != null)
                    {
                        Debug.Log($"[BuildingScene] decorationPartsContainer 存在: {decorationPartsContainer.name}");
                    }
                    else
                    {
                        Debug.LogError("[BuildingScene] decorationPartsContainer 为 NULL！请在Inspector中配置！");
                    }
                    ShowStage("第四步：添加装饰", decorationPartsContainer);
                    break;
                case BuildingStage.Complete:
                    CompleteBuilding();
                    break;
            }
        }

        private void ShowStage(string guideMsg, GameObject container)
        {
            if (guideText != null)
                guideText.text = guideMsg;

            if (container != null)
            {
                container.SetActive(true);
                Debug.Log($"[BuildingScene] 显示容器: {container.name}");
                
                // 调试：检查容器内的子对象
                int childCount = container.transform.childCount;
                Debug.Log($"[BuildingScene] 容器 {container.name} 有 {childCount} 个子对象");
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = container.transform.GetChild(i);
                    Debug.Log($"[BuildingScene]   子对象 {i}: {child.name}, Active={child.gameObject.activeSelf}");
                }
            }
            else
            {
                Debug.LogError($"[BuildingScene] 容器为空！无法显示：{guideMsg}");
            }

            if (confirmButton != null)
                confirmButton.interactable = false;
        }

        private void HideAllContainers()
        {
            if (basePartsContainer != null) basePartsContainer.SetActive(false);
            if (pillarPartsContainer != null) pillarPartsContainer.SetActive(false);
            if (roofPartsContainer != null) roofPartsContainer.SetActive(false);
            if (decorationPartsContainer != null) decorationPartsContainer.SetActive(false);
        }

        /// <summary>
        /// 当部件被放置到区域时调用
        /// </summary>
        private void OnPartPlaced(string partID)
        {
            Debug.Log($"[BuildingScene] 部件已放置: {partID}");

            // 播放放置音效
            if (placeSound != null)
                AudioSource.PlayClipAtPoint(placeSound, Camera.main.transform.position, 0.5f);

            // 启用确认按钮（玩家需要手动点击才能进入下一阶段）
            if (confirmButton != null)
            {
                confirmButton.interactable = true;
                Debug.Log("[BuildingScene] 确认按钮已启用，等待玩家点击");
            }
            else
            {
                Debug.LogError("[BuildingScene] 确认按钮为空！");
            }
        }

        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        private void OnConfirmButtonClicked()
        {
            Debug.Log($"[BuildingScene] ===== 确认按钮被点击 ===== 当前阶段: {currentStage}");

            // 禁用确认按钮
            if (confirmButton != null)
                confirmButton.interactable = false;

            // 进入下一阶段
            switch (currentStage)
            {
                case BuildingStage.Base:
                    Debug.Log("[BuildingScene] 从基座阶段进入柱子阶段");
                    StartStage(BuildingStage.Pillar);
                    break;
                case BuildingStage.Pillar:
                    Debug.Log("[BuildingScene] 从柱子阶段进入屋顶阶段");
                    StartStage(BuildingStage.Roof);
                    break;
                case BuildingStage.Roof:
                    Debug.Log("[BuildingScene] 从屋顶阶段进入装饰阶段");
                    StartStage(BuildingStage.Decoration);
                    break;
                case BuildingStage.Decoration:
                    Debug.Log("[BuildingScene] 从装饰阶段进入完成阶段");
                    StartStage(BuildingStage.Complete);
                    break;
            }
        }

        /// <summary>
        /// 完成建造，计算分数
        /// </summary>
        private void CompleteBuilding()
        {
            Debug.Log("[BuildingScene] 建造完成，计算分数");

            // 隐藏引导文本
            if (guideText != null)
                guideText.gameObject.SetActive(false);
            
            // 隐藏所有部件容器
            HideAllContainers();

            // 计算分数
            CalculateScore();

            // 播放完成音效
            if (completeSound != null)
                AudioSource.PlayClipAtPoint(completeSound, Camera.main.transform.position, 0.7f);

            // 显示结果面板
            StartCoroutine(ShowResultWithDelay());
        }

        private void CalculateScore()
        {
            totalScore = 0;

            // 检查每个部位的选择
            if (baseZone != null && baseZone.GetCurrentPartID() != null)
            {
                string baseID = baseZone.GetCurrentPartID();
                if (bestCombination.ContainsKey(baseID))
                    totalScore += bestCombination[baseID];
                Debug.Log($"[Score] 基座: {baseID} (+{(bestCombination.ContainsKey(baseID) ? bestCombination[baseID] : 0)}分)");
            }

            if (pillarZone != null && pillarZone.GetCurrentPartID() != null)
            {
                string pillarID = pillarZone.GetCurrentPartID();
                if (bestCombination.ContainsKey(pillarID))
                    totalScore += bestCombination[pillarID];
                Debug.Log($"[Score] 柱子: {pillarID} (+{(bestCombination.ContainsKey(pillarID) ? bestCombination[pillarID] : 0)}分)");
            }

            if (roofZone != null && roofZone.GetCurrentPartID() != null)
            {
                string roofID = roofZone.GetCurrentPartID();
                if (bestCombination.ContainsKey(roofID))
                    totalScore += bestCombination[roofID];
                Debug.Log($"[Score] 屋顶: {roofID} (+{(bestCombination.ContainsKey(roofID) ? bestCombination[roofID] : 0)}分)");
            }

            if (decorationZone != null && decorationZone.GetCurrentStyleID() != null)
            {
                string styleID = decorationZone.GetCurrentStyleID();
                if (bestCombination.ContainsKey(styleID))
                    totalScore += bestCombination[styleID];
                Debug.Log($"[Score] 装饰风格: {styleID} (+{(bestCombination.ContainsKey(styleID) ? bestCombination[styleID] : 0)}分)");
            }

            Debug.Log($"[Score] 总分: {totalScore}/100");
        }

        private IEnumerator ShowResultWithDelay()
        {
            yield return new WaitForSeconds(1.0f);

            Debug.Log("[BuildingScene] 开始展示成果");

            // 隐藏确认按钮
            if (confirmButton != null)
                confirmButton.gameObject.SetActive(false);

            // 放大并居中显示建造区域
            if (buildingArea != null)
            {
                yield return StartCoroutine(ZoomInBuildingArea());
            }
            else
            {
                Debug.LogError("[BuildingScene] ✗ buildingArea 未配置！请在 Inspector 中设置！");
            }

            yield return new WaitForSeconds(0.5f);

            // 显示结果面板
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);

                // 保存完成状态
                PlayerPrefs.SetInt("Level4_Completed", 1);
                PlayerPrefs.SetInt("Level4_Score", totalScore);
                PlayerPrefs.Save();
                Debug.Log($"[BuildingScene] 第四关完成，得分：{totalScore}/100");
            }
        }

        /// <summary>
        /// 放大建造区域以展示成果
        /// </summary>
        private IEnumerator ZoomInBuildingArea()
        {
            if (buildingArea == null)
            {
                Debug.LogError("[BuildingScene] buildingArea 为空！");
                yield break;
            }

            Vector3 originalScale = buildingArea.localScale;
            Vector2 originalPos = buildingArea.anchoredPosition;

            Vector3 targetScale = originalScale * 0.5f; // 缩小到一半
            Vector2 targetPos = Vector2.zero; // 移动到屏幕中央

            float duration = 0.8f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 使用缓动函数
                float easeT = Mathf.SmoothStep(0f, 1f, t);

                buildingArea.localScale = Vector3.Lerp(originalScale, targetScale, easeT);
                buildingArea.anchoredPosition = Vector2.Lerp(originalPos, targetPos, easeT);

                yield return null;
            }

            buildingArea.localScale = targetScale;
            buildingArea.anchoredPosition = targetPos;

            Debug.Log("[BuildingScene] ✓ 建造区域已放大展示");
        }

        /// <summary>
        /// 继续按钮点击事件
        /// </summary>
        private void OnContinueButtonClicked()
        {
            Debug.Log("[BuildingScene] 第四关完成，进入MainMap4");
            
            // ⭐ 保存第4关进度和碎片数
            SaveLevelProgress();
            
            // 淡出背景音乐
            if (bgmSource != null)
                StartCoroutine(FadeOutMusic());
            
            // 加载MainMap4（解锁第5关）
            SceneManager.LoadScene("MainMap4");
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
            Debug.Log($"✅ 第4关结束：\n" +
                      $"关卡数：{oldLevel} → {newLevel}\n" +
                      $"碎片数：{oldFragment} → {newFragment}");
        }
        


        private IEnumerator FadeOutMusic()
        {
            if (bgmSource == null) yield break;

            float startVolume = bgmSource.volume;
            float fadeTime = 1.0f;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
                yield return null;
            }

            bgmSource.Stop();
        }

        private void OnDestroy()
        {
            // 清理事件监听
            if (confirmButton != null)
                confirmButton.onClick.RemoveAllListeners();
            if (continueButton != null)
                continueButton.onClick.RemoveAllListeners();

            if (baseZone != null) baseZone.OnPartPlaced -= OnPartPlaced;
            if (pillarZone != null) pillarZone.OnPartPlaced -= OnPartPlaced;
            if (roofZone != null) roofZone.OnPartPlaced -= OnPartPlaced;
            if (decorationZone != null) decorationZone.OnStylePlaced -= OnPartPlaced;
        }
    }
}
