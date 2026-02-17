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
        [SerializeField] private Image completedBuildingImage;        // 完成后的整体建筑图
        [SerializeField] private Button continueButton;               // 继续按钮

        [Header("=== 建造区域 ===")]
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
                confirmButton.onClick.AddListener(OnConfirmButtonClicked);

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

            // 启用确认按钮
            if (confirmButton != null)
                confirmButton.interactable = true;
        }

        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        private void OnConfirmButtonClicked()
        {
            Debug.Log($"[BuildingScene] 确认当前阶段: {currentStage}");

            // 禁用确认按钮
            if (confirmButton != null)
                confirmButton.interactable = false;

            // 进入下一阶段
            switch (currentStage)
            {
                case BuildingStage.Base:
                    StartStage(BuildingStage.Pillar);
                    break;
                case BuildingStage.Pillar:
                    StartStage(BuildingStage.Roof);
                    break;
                case BuildingStage.Roof:
                    StartStage(BuildingStage.Decoration);
                    break;
                case BuildingStage.Decoration:
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

            if (resultPanel != null)
            {
                resultPanel.SetActive(true);

                // 显示完成后的建筑图（这里需要根据选择的部件组合生成完整图）
                // TODO: 实现建筑图的组合逻辑
                if (completedBuildingImage != null)
                {
                    // 暂时可以设置一个预设的完成图片
                    // completedBuildingImage.sprite = loadCompletedBuildingSprite();
                    Debug.Log("[BuildingScene] 显示完成的建筑图");
                }

                // 保存完成状态
                PlayerPrefs.SetInt("Level4_Completed", 1);
                PlayerPrefs.SetInt("Level4_Score", totalScore);
                PlayerPrefs.Save();
                Debug.Log($"[BuildingScene] 第四关完成，得分：{totalScore}/100");
            }
        }

        /// <summary>
        /// 继续按钮点击事件
        /// </summary>
        private void OnContinueButtonClicked()
        {
            Debug.Log("[BuildingScene] 进入第五关（杭州）");
            
            // 淡出背景音乐
            if (bgmSource != null)
                StartCoroutine(FadeOutMusic());
            
            // 加载下一关（根据实际情况修改场景名）
            SceneManager.LoadScene("Level5_Intro"); // 或者 "MainMap" 返回地图
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
