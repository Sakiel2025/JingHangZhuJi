using UnityEngine;
using TMPro;

public class DialogueVoice : MonoBehaviour
{
    [Header("对话文本组件")]
    public TextMeshProUGUI dialogueText;

    [Header("音频播放器")]
    public AudioSource voicePlayer;

    [Header("对话文案+配音一一对应")]
    public DialoguePair[] dialogueList;

    // 存储当前播放音频
    private AudioClip currentVoice;

    void Awake()
    {
        // 自动挂载音源
        if (voicePlayer == null)
            voicePlayer = gameObject.AddComponent<AudioSource>();
        voicePlayer.loop = false;
        voicePlayer.volume = 1f;
    }

    // 调用这个方法：传入台词文本，自动播放对应配音
    public void ShowDialogue(string talkContent)
    {
        // 先停掉上一句语音
        voicePlayer.Stop();
        dialogueText.text = talkContent;

        // 遍历匹配文案，找到对应音频
        foreach (var item in dialogueList)
        {
            if (item.talkText == talkContent)
            {
                currentVoice = item.voiceClip;
                PlayCurrentVoice();
                break;
            }
        }
    }

    // 播放当前台词配音
    void PlayCurrentVoice()
    {
        if (currentVoice == null) return;
        voicePlayer.clip = currentVoice;
        voicePlayer.Play();
    }

    // 清空对话+停止配音
    public void ClearDialogue()
    {
        dialogueText.text = "";
        voicePlayer.Stop();
    }
}

// 台词与配音绑定结构体
[System.Serializable]
public class DialoguePair
{
    [TextArea] public string talkText;  // 角色台词
    public AudioClip voiceClip;        // 对应录音配音
}