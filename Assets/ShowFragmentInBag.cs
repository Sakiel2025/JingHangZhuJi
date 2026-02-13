using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowFragmentInBag : MonoBehaviour
{
    // 两种Text类型都支持
    private Text legacyText;
    private TextMeshProUGUI tmpText;

    void Start()
    {
        // ----- 1. 获取当前登录的用户名 -----
        string username = PlayerPrefs.GetString("lastLogin", "");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("❌ 没有登录用户！");
            return;
        }

        // ----- 2. 读取碎片个数（你的数据结构）-----
        int fragmentCount = PlayerPrefs.GetInt("fragment_" + username, 0);

        // ----- 3. 获取Text组件 -----
        legacyText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();

        // ----- 4. 显示碎片个数 -----
       
        // 如果你想显示"碎片: 12"，用这行：
        string displayText = "遗书碎片: " + fragmentCount;

        if (legacyText != null)
            legacyText.text = displayText;
        else if (tmpText != null)
            tmpText.text = displayText;
        else
            Debug.LogError("❌ 这个物体上没有Text组件！");

        Debug.Log($"📦 当前碎片数量: {fragmentCount}");
    }

    // ----- 5. 刷新方法（如果背包需要实时更新）-----
    public void RefreshFragmentCount()
    {
        Start();  // 重新执行一遍
    }
}