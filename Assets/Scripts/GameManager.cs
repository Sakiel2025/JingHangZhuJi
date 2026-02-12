using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace JingHangZhuJi
{
    /// <summary>
    /// 游戏管理器 - 全局单例，管理游戏状态、资源、存档等
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("玩家数据")]
        [SerializeField] private int silver = 0; // 银两
        [SerializeField] private int prestige = 0; // 声望
        [SerializeField] private int craftHeart = 0; // 匠心值

        [Header("关卡进度")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int maxUnlockedLevel = 1;
        private Dictionary<int, bool> levelCompleted = new Dictionary<int, bool>();
        private Dictionary<int, string> levelGrades = new Dictionary<int, string>();

        [Header("物品和成就")]
        private HashSet<string> unlockedItems = new HashSet<string>();
        private HashSet<string> unlockedAchievements = new HashSet<string>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadGameData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region 资源管理

        /// <summary>
        /// 增加银两
        /// </summary>
        public void AddSilver(int amount)
        {
            silver += amount;
            Debug.Log($"获得银两 +{amount}，当前: {silver}");
            SaveGameData();
        }

        /// <summary>
        /// 消费银两
        /// </summary>
        public bool SpendSilver(int amount)
        {
            if (silver >= amount)
            {
                silver -= amount;
                Debug.Log($"消费银两 -{amount}，剩余: {silver}");
                SaveGameData();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 增加声望
        /// </summary>
        public void AddPrestige(int amount)
        {
            prestige += amount;
            Debug.Log($"获得声望 +{amount}，当前: {prestige}");
            SaveGameData();
        }

        /// <summary>
        /// 增加匠心值
        /// </summary>
        public void AddCraftHeart(int amount)
        {
            craftHeart += amount;
            Debug.Log($"获得匠心 +{amount}，当前: {craftHeart}");
            SaveGameData();
        }

        public int GetSilver() => silver;
        public int GetPrestige() => prestige;
        public int GetCraftHeart() => craftHeart;

        #endregion

        #region 关卡管理

        /// <summary>
        /// 加载指定关卡
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            currentLevel = levelIndex;
            string sceneName = $"Level{levelIndex}";
            
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning($"场景 {sceneName} 不存在");
            }
        }

        /// <summary>
        /// 解锁关卡
        /// </summary>
        public void UnlockLevel(int levelIndex)
        {
            if (levelIndex > maxUnlockedLevel)
            {
                maxUnlockedLevel = levelIndex;
                Debug.Log($"解锁关卡 {levelIndex}");
                SaveGameData();
            }
        }

        /// <summary>
        /// 完成关卡
        /// </summary>
        public void CompleteLevel(int levelIndex, string grade)
        {
            levelCompleted[levelIndex] = true;
            levelGrades[levelIndex] = grade;
            
            // 自动解锁下一关
            UnlockLevel(levelIndex + 1);
            
            Debug.Log($"完成关卡 {levelIndex}，评级: {grade}");
            SaveGameData();
        }

        public bool IsLevelUnlocked(int levelIndex) => levelIndex <= maxUnlockedLevel;
        public bool IsLevelCompleted(int levelIndex) => levelCompleted.ContainsKey(levelIndex) && levelCompleted[levelIndex];
        public string GetLevelGrade(int levelIndex) => levelGrades.ContainsKey(levelIndex) ? levelGrades[levelIndex] : "未完成";

        #endregion

        #region 物品和成就

        /// <summary>
        /// 解锁物品
        /// </summary>
        public void UnlockItem(string itemName)
        {
            if (unlockedItems.Add(itemName))
            {
                Debug.Log($"解锁物品: {itemName}");
                SaveGameData();
            }
        }

        /// <summary>
        /// 解锁成就
        /// </summary>
        public void UnlockAchievement(string achievementName)
        {
            if (unlockedAchievements.Add(achievementName))
            {
                Debug.Log($"解锁成就: {achievementName}");
                UIManager.Instance?.ShowMessage($"🏆 成就解锁: {achievementName}");
                SaveGameData();
            }
        }

        public bool HasItem(string itemName) => unlockedItems.Contains(itemName);
        public bool HasAchievement(string achievementName) => unlockedAchievements.Contains(achievementName);

        #endregion

        #region 存档系统

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        public void SaveGameData()
        {
            PlayerPrefs.SetInt("Silver", silver);
            PlayerPrefs.SetInt("Prestige", prestige);
            PlayerPrefs.SetInt("CraftHeart", craftHeart);
            PlayerPrefs.SetInt("MaxLevel", maxUnlockedLevel);
            
            // 保存物品和成就（使用JSON或简单字符串）
            PlayerPrefs.SetString("Items", string.Join(",", unlockedItems));
            PlayerPrefs.SetString("Achievements", string.Join(",", unlockedAchievements));
            
            PlayerPrefs.Save();
            Debug.Log("游戏数据已保存");
        }

        /// <summary>
        /// 加载游戏数据
        /// </summary>
        public void LoadGameData()
        {
            silver = PlayerPrefs.GetInt("Silver", 0);
            prestige = PlayerPrefs.GetInt("Prestige", 0);
            craftHeart = PlayerPrefs.GetInt("CraftHeart", 0);
            maxUnlockedLevel = PlayerPrefs.GetInt("MaxLevel", 1);

            // 加载物品
            string itemsStr = PlayerPrefs.GetString("Items", "");
            if (!string.IsNullOrEmpty(itemsStr))
            {
                unlockedItems = new HashSet<string>(itemsStr.Split(','));
            }

            // 加载成就
            string achievementsStr = PlayerPrefs.GetString("Achievements", "");
            if (!string.IsNullOrEmpty(achievementsStr))
            {
                unlockedAchievements = new HashSet<string>(achievementsStr.Split(','));
            }

            Debug.Log("游戏数据已加载");
        }

        /// <summary>
        /// 重置游戏数据
        /// </summary>
        public void ResetGameData()
        {
            PlayerPrefs.DeleteAll();
            silver = 0;
            prestige = 0;
            craftHeart = 0;
            maxUnlockedLevel = 1;
            levelCompleted.Clear();
            levelGrades.Clear();
            unlockedItems.Clear();
            unlockedAchievements.Clear();
            
            Debug.Log("游戏数据已重置");
        }

        #endregion
    }
}
