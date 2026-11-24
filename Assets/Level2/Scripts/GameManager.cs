// 保存路径：Scripts/GameManager.cs
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 单例模式：全局唯一访问入口
    public static GameManager Instance;

    // 核心游戏数据（初始值按需求设置）
    public float currentMoney = 2f; // 初始2两银子
    public int heartValue = 0;      // 民心值
    public int craftValue = 0;      // 匠心值

    // 道具列表（存储收集到的物品，如信件、手帕）
    public List<string> collectedItems = new List<string>();
    // 成就列表（存储解锁的成就）
    public List<string> unlockedAchievements = new List<string>();

    void Awake()
    {
        // 保证场景中只有一个GameManager，切换场景不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保存数据
        }
        else
        {
            Destroy(gameObject); // 重复创建则销毁
        }
    }

    // 【外部调用】添加道具（如信件、种子）
    public void AddItem(string itemName)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            Debug.Log("[道具收集] 获得：" + itemName); // 控制台打印，方便测试
        }
    }

    // 【外部调用】解锁成就
    public void UnlockAchievement(string achievementName)
    {
        if (!unlockedAchievements.Contains(achievementName))
        {
            unlockedAchievements.Add(achievementName);
            Debug.Log("[成就解锁] 恭喜：" + achievementName); // 控制台打印，方便测试
        }
    }

    // 【外部调用】修改数值（民心/匠心）
    public void ChangeValue(string valueType, int changeAmount)
    {
        switch (valueType)
        {
            case "heart":
                heartValue += changeAmount;
                break;
            case "craft":
                craftValue += changeAmount;
                break;
        }
        Debug.Log("[数值更新] 民心值：" + heartValue + " | 匠心值：" + craftValue);
    }

    // 【外部调用】修改银子（扣钱/加钱）
    public void ChangeMoney(float changeAmount)
    {
        currentMoney += changeAmount;
        Debug.Log("[银子更新] 当前剩余：" + currentMoney + "两");
    }
}