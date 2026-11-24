using UnityEngine;
using System.Collections;

/// <summary>
/// Level5 音频管理器
/// 管理BGM和音效的播放
/// </summary>
public class Level5AudioManager : MonoBehaviour
{
    [Header("音频源")]
    public AudioSource bgmSource;       // BGM播放器
    public AudioSource sfxSource;       // 音效播放器

    [Header("BGM音频片段")]
    public AudioClip scene1BGM;         // 场景1 BGM
    public AudioClip scene2BGM;         // 场景2 BGM
    public AudioClip scene3BGM;         // 场景3 BGM
    public AudioClip scene4BGM;         // 场景4 BGM

    [Header("音效片段")]
    public AudioClip buttonClickSFX;    // 按钮点击
    public AudioClip paperTurnSFX;      // 翻纸声
    public AudioClip puzzleCorrectSFX;  // 拼图正确
    public AudioClip puzzleWrongSFX;    // 拼图错误
    public AudioClip buildCompleteSFX;  // 建造完成
    public AudioClip hammerSFX;         // 锤子敲击

    [Header("音量设置")]
    [Range(0f, 1f)]
    public float bgmVolume = 0.6f;      // BGM音量
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;      // 音效音量

    private static Level5AudioManager instance;

    void Awake()
    {
        // 单例模式
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化音频源
        InitializeAudioSources();
    }

    /// <summary>
    /// 初始化音频源
    /// </summary>
    void InitializeAudioSources()
    {
        // 如果没有AudioSource，自动创建
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    #region BGM控制

    /// <summary>
    /// 播放BGM
    /// </summary>
    public static void PlayBGM(AudioClip clip, bool loop = true, float volume = -1f)
    {
        if (instance != null && instance.bgmSource != null && clip != null)
        {
            instance.bgmSource.clip = clip;
            instance.bgmSource.loop = loop;
            instance.bgmSource.volume = volume >= 0 ? volume : instance.bgmVolume;
            instance.bgmSource.Play();
        }
    }

    /// <summary>
    /// 停止BGM
    /// </summary>
    public static void StopBGM()
    {
        if (instance != null && instance.bgmSource != null)
        {
            instance.bgmSource.Stop();
        }
    }

    /// <summary>
    /// 暂停BGM
    /// </summary>
    public static void PauseBGM()
    {
        if (instance != null && instance.bgmSource != null)
        {
            instance.bgmSource.Pause();
        }
    }

    /// <summary>
    /// 恢复BGM
    /// </summary>
    public static void ResumeBGM()
    {
        if (instance != null && instance.bgmSource != null)
        {
            instance.bgmSource.UnPause();
        }
    }

    /// <summary>
    /// BGM淡出
    /// </summary>
    public static void FadeOutBGM(float duration = 1f)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeOutBGMCoroutine(duration));
        }
    }

    /// <summary>
    /// BGM淡入
    /// </summary>
    public static void FadeInBGM(float duration = 1f, float targetVolume = -1f)
    {
        if (instance != null)
        {
            float volume = targetVolume >= 0 ? targetVolume : instance.bgmVolume;
            instance.StartCoroutine(instance.FadeInBGMCoroutine(duration, volume));
        }
    }

    IEnumerator FadeOutBGMCoroutine(float duration)
    {
        if (bgmSource == null) yield break;

        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();
    }

    IEnumerator FadeInBGMCoroutine(float duration, float targetVolume)
    {
        if (bgmSource == null) yield break;

        if (!bgmSource.isPlaying)
        {
            bgmSource.volume = 0f;
            bgmSource.Play();
        }

        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    #endregion

    #region 音效控制

    /// <summary>
    /// 播放音效
    /// </summary>
    public static void PlaySFX(AudioClip clip, float volume = -1f)
    {
        if (instance != null && instance.sfxSource != null && clip != null)
        {
            float vol = volume >= 0 ? volume : instance.sfxVolume;
            instance.sfxSource.PlayOneShot(clip, vol);
        }
    }

    /// <summary>
    /// 在指定位置播放音效
    /// </summary>
    public static void PlaySFXAtPoint(AudioClip clip, Vector3 position, float volume = -1f)
    {
        if (instance != null && clip != null)
        {
            float vol = volume >= 0 ? volume : instance.sfxVolume;
            AudioSource.PlayClipAtPoint(clip, position, vol);
        }
    }

    /// <summary>
    /// 播放按钮点击音效
    /// </summary>
    public static void PlayButtonClick()
    {
        if (instance != null && instance.buttonClickSFX != null)
        {
            PlaySFX(instance.buttonClickSFX);
        }
    }

    /// <summary>
    /// 播放翻纸音效
    /// </summary>
    public static void PlayPaperTurn()
    {
        if (instance != null && instance.paperTurnSFX != null)
        {
            PlaySFX(instance.paperTurnSFX);
        }
    }

    /// <summary>
    /// 播放拼图正确音效
    /// </summary>
    public static void PlayPuzzleCorrect()
    {
        if (instance != null && instance.puzzleCorrectSFX != null)
        {
            PlaySFX(instance.puzzleCorrectSFX);
        }
    }

    /// <summary>
    /// 播放拼图错误音效
    /// </summary>
    public static void PlayPuzzleWrong()
    {
        if (instance != null && instance.puzzleWrongSFX != null)
        {
            PlaySFX(instance.puzzleWrongSFX);
        }
    }

    /// <summary>
    /// 播放建造完成音效
    /// </summary>
    public static void PlayBuildComplete()
    {
        if (instance != null && instance.buildCompleteSFX != null)
        {
            PlaySFX(instance.buildCompleteSFX);
        }
    }

    /// <summary>
    /// 播放锤子敲击音效
    /// </summary>
    public static void PlayHammer()
    {
        if (instance != null && instance.hammerSFX != null)
        {
            PlaySFX(instance.hammerSFX);
        }
    }

    #endregion

    #region 场景专用BGM

    /// <summary>
    /// 播放场景1 BGM
    /// </summary>
    public static void PlayScene1BGM()
    {
        if (instance != null && instance.scene1BGM != null)
        {
            PlayBGM(instance.scene1BGM);
        }
    }

    /// <summary>
    /// 播放场景2 BGM
    /// </summary>
    public static void PlayScene2BGM()
    {
        if (instance != null && instance.scene2BGM != null)
        {
            PlayBGM(instance.scene2BGM);
        }
    }

    /// <summary>
    /// 播放场景3 BGM
    /// </summary>
    public static void PlayScene3BGM()
    {
        if (instance != null && instance.scene3BGM != null)
        {
            PlayBGM(instance.scene3BGM);
        }
    }

    /// <summary>
    /// 播放场景4 BGM
    /// </summary>
    public static void PlayScene4BGM()
    {
        if (instance != null && instance.scene4BGM != null)
        {
            PlayBGM(instance.scene4BGM);
        }
    }

    #endregion

    #region 音量控制

    /// <summary>
    /// 设置BGM音量
    /// </summary>
    public static void SetBGMVolume(float volume)
    {
        if (instance != null)
        {
            instance.bgmVolume = Mathf.Clamp01(volume);
            if (instance.bgmSource != null)
            {
                instance.bgmSource.volume = instance.bgmVolume;
            }
        }
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public static void SetSFXVolume(float volume)
    {
        if (instance != null)
        {
            instance.sfxVolume = Mathf.Clamp01(volume);
            if (instance.sfxSource != null)
            {
                instance.sfxSource.volume = instance.sfxVolume;
            }
        }
    }

    /// <summary>
    /// 静音/取消静音
    /// </summary>
    public static void SetMute(bool mute)
    {
        if (instance != null)
        {
            if (instance.bgmSource != null)
            {
                instance.bgmSource.mute = mute;
            }
            if (instance.sfxSource != null)
            {
                instance.sfxSource.mute = mute;
            }
        }
    }

    #endregion
}
