using UnityEngine;
using TMPro;

public class OneSentenceAudio : MonoBehaviour
{
    public TextMeshProUGUI myText;
    public AudioClip sound;
    private AudioSource aud;
    private bool isPlayed;

    void Awake()
    {
        aud = GetComponent<AudioSource>();
        if (aud == null) aud = gameObject.AddComponent<AudioSource>();
        aud.volume = 1;
        aud.loop = false;
    }

    void Update()
    {
        // 文字显示且没播放过，就出声
        if (!string.IsNullOrWhiteSpace(myText.text) && !isPlayed)
        {
            aud.clip = sound;
            aud.Play();
            isPlayed = true;
        }
        // 文字清空重置状态
        if (string.IsNullOrWhiteSpace(myText.text))
        {
            aud.Stop();
            isPlayed = false;
        }
    }
}