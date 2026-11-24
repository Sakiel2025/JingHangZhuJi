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
    public TextMeshProUGUI nameText;
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

    // ====================== 【配音 自动添加】 ======================
    [Header("🔊 配音音频（直接拖进来）")]
    public AudioClip voice_sonFirst;    // 大柱第一句
    public AudioClip voice_momFirst;    // 妈妈第一句
    public AudioClip voice_momThanks1;  // 妈妈感谢1
    public AudioClip voice_momThanks2;  // 妈妈感谢2

    private AudioSource audioPlayer;
    // ===============================================================

    void Start()
    {
        // 自动生成声音播放器（不用你管）
        audioPlayer = gameObject.AddComponent<AudioSource>();
        audioPlayer.volume = 1f;

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
        PlayVoice(voice_sonFirst); // 自动播配音
        yield return new WaitForSeconds(2f);

        // 农妇说话
        speakerImage.sprite = momSprite;
        speakerImage.gameObject.SetActive(true);
        nameText.text = momName;
        nameText.gameObject.SetActive(true);
        dialogText.text = momFirstLine;
        PlayVoice(voice_momFirst); // 自动播配音
        yield return new WaitForSeconds(2f);

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
        PlayVoice(voice_momThanks1); // 自动播配音
        yield return new WaitForSeconds(4f);

        // 农妇感谢2
        dialogText.text = momThanksLine2;
        PlayVoice(voice_momThanks2); // 自动播配音
        yield return new WaitForSeconds(3f);

        ShowRewardPanel();
    }

    // ====================== 【播放声音】 ======================
    void PlayVoice(AudioClip clip)
    {
        if (clip == null) return;
        audioPlayer.Stop();    // 切换句子自动停上一句
        audioPlayer.PlayOneShot(clip); // 播放当前配音
    }
    // ===========================================================

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