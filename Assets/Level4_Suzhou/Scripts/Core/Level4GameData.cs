using UnityEngine;

/// <summary>
/// 第四关游戏数据（跨场景持久化）
/// 使用单例模式，在场景切换时不会被销毁
/// </summary>
public class Level4GameData : MonoBehaviour
{
    public static Level4GameData Instance;
    
    [Header("场景进度")]
    public bool scene1_Intro = false;              // 前置剧情完成
    public bool scene2_LotusPool = false;          // 荷花池完成
    public bool scene3_MountainTower = false;      // 见山楼完成
    public bool scene4_Corridor = false;           // 漏窗长廊完成
    public bool scene5_RainPavilion = false;       // 听雨轩完成
    public bool scene6_CentralPavilion = false;    // 中央亭子完成
    public bool scene7_Building = false;           // 建造完成
    
    [Header("线索收集")]
    public bool clue_Jie = false;    // "借"字
    public bool clue_Shan = false;   // "山"字
    public bool clue_Shui = false;   // "水"字
    public bool clue_Yi = false;     // "意"字
    
    [Header("问答结果")]
    public int correctAnswers = 0;   // 答对的题目数（满分3）
    
    [Header("建造结果")]
    public int buildingScore = 0;    // 建造得分（0-100）
    public string buildingGrade = "";  // 建造等级（S/A/B/C/D）
    
    void Awake()
    {
        // 单例模式：确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 场景切换时不销毁
            Debug.Log("Level4GameData 初始化成功");
        }
        else
        {
            Destroy(gameObject);  // 如果已存在实例，销毁当前对象
        }
    }
    
    /// <summary>
    /// 检查所有线索是否收集完毕
    /// </summary>
    public bool AllCluesCollected()
    {
        return clue_Jie && clue_Shan && clue_Shui && clue_Yi;
    }
    
    /// <summary>
    /// 获取已收集的线索数量
    /// </summary>
    public int GetCollectedClueCount()
    {
        int count = 0;
        if (clue_Jie) count++;
        if (clue_Shan) count++;
        if (clue_Shui) count++;
        if (clue_Yi) count++;
        return count;
    }
    
    /// <summary>
    /// 重置所有数据（用于重新开始关卡）
    /// </summary>
    public void ResetData()
    {
        scene1_Intro = false;
        scene2_LotusPool = false;
        scene3_MountainTower = false;
        scene4_Corridor = false;
        scene5_RainPavilion = false;
        scene6_CentralPavilion = false;
        scene7_Building = false;
        
        clue_Jie = false;
        clue_Shan = false;
        clue_Shui = false;
        clue_Yi = false;
        
        correctAnswers = 0;
        buildingScore = 0;
        buildingGrade = "";
        
        Debug.Log("Level4 数据已重置");
    }
    
    /// <summary>
    /// 打印当前进度（调试用）
    /// </summary>
    public void PrintProgress()
    {
        Debug.Log($"=== Level4 进度 ===");
        Debug.Log($"场景进度: {GetCompletedSceneCount()}/7");
        Debug.Log($"线索收集: {GetCollectedClueCount()}/4");
        Debug.Log($"问答结果: {correctAnswers}/3");
        Debug.Log($"建造评分: {buildingScore}分 ({buildingGrade}级)");
    }
    
    /// <summary>
    /// 获取已完成的场景数量
    /// </summary>
    int GetCompletedSceneCount()
    {
        int count = 0;
        if (scene1_Intro) count++;
        if (scene2_LotusPool) count++;
        if (scene3_MountainTower) count++;
        if (scene4_Corridor) count++;
        if (scene5_RainPavilion) count++;
        if (scene6_CentralPavilion) count++;
        if (scene7_Building) count++;
        return count;
    }
}
