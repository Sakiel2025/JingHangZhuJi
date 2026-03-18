// 保存路径：Assets/Level1/Scripts/2_Taierzhuang/OptionSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; 

public class OptionSystem : MonoBehaviour
{
    [Header("核心显示")]
    public TextMeshProUGUI dialogText;
    public Image speakerImage;

    [Header("角色名字（新增）")]
    public TextMeshProUGUI nameText;       // 名字框
    public string sonName = "大柱";
    public string momName = "赵家寡妇";

    [Header("立绘")]
    public Sprite sonSprite;
    public Sprite momSprite;

    [Header("选项")]
    public Button optionABtn;
    public Button optionBBtn;
    public Button optionCBtn;
    public GameObject optionPanel;

    [Header("奖励")]
    public GameObject rewardPanel;
    public Button closeRewardBtn;

    [Header("场景")]
    public string nextSceneName = "RepairHouseScene";

    [Header("对话内容")]
    public string sonFirstLine = "先生，求您收我为徒！";
    public string momFirstLine = "大柱，别胡闹！";
    public string momThanksLine1 = "多谢先生仗义相助！";
    public string momThanksLine2 = "这份恩情，赵家永世不忘！";

    void Start()
    {
        optionPanel.SetActive(false);
        rewardPanel.SetActive(false);

        optionABtn.onClick.AddListener(OnOptionAClick);
        optionBBtn.onClick.AddListener(OnOptionBClick);
        optionCBtn.onClick.AddListener(OnOptionCClick);
        closeRewardBtn.onClick.AddListener(CloseRewardPanel);

        StartCoroutine(ShowStoryBeforeOption());
    }

    private IEnumerator ShowStoryBeforeOption()
    {
        // 儿子说话
        speakerImage.sprite = sonSprite;
        speakerImage.gameObject.SetActive(true);
        nameText.text = sonName;
        nameText.gameObject.SetActive(true);
        dialogText.text = sonFirstLine;
        yield return new WaitForSeconds(1f);

        // 农妇说话
        speakerImage.sprite = momSprite;
        speakerImage.gameObject.SetActive(true);
        nameText.text = momName;
        nameText.gameObject.SetActive(true);
        dialogText.text = momFirstLine;
        yield return new WaitForSeconds(1f);

        optionPanel.SetActive(true);
    }

    private void OnOptionAClick() => StartCoroutine(ShowMomThanksStory());
    private void OnOptionBClick() => StartCoroutine(ShowMomThanksStory());
    private void OnOptionCClick() => StartCoroutine(ShowMomThanksStory());

    private IEnumerator ShowMomThanksStory()
    {
        optionPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        // 农妇感谢1
        speakerImage.sprite = momSprite;
        nameText.text = momName;
        dialogText.text = momThanksLine1;
        yield return new WaitForSeconds(1f);

        // 农妇感谢2
        dialogText.text = momThanksLine2;
        yield return new WaitForSeconds(2f);

        ShowRewardPanel();
    }

    private void ShowRewardPanel()
    {
        rewardPanel.SetActive(true);
        Invoke("CloseRewardPanel", 5f);
    }

    private void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
        FinalStoryStep();
    }

    private void FinalStoryStep()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}