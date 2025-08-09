using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏音频管理器，负责控制游戏中的音效（SFX）和背景音乐（BGM）的播放、停止等功能
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinimumDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;


    public bool playBgm;
    private int bgmIndex;

    private bool canPlaySFX;

    /// <summary>
    /// 单例
    /// </summary>
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        // 延迟1秒允许播放音效（避免场景加载时的音效冲突）
        Invoke("AllowSFX", 1f);
    }

    /// <summary>
    /// 每帧检查BGM状态
    /// </summary>
    private void Update()
    {
        if (!playBgm)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }


    /// <summary>
    /// 音效（SFX）播放控制
    /// </summary>
    /// <param name="_sfxIndex"></param>
    /// <param name="_source"></param>
    public void PlaySFX(int _sfxIndex, Transform _source)
    {
        //if (sfx[_sfxIndex].isPlaying)
        //    return;

        // 检查是否允许播放音效
        if (canPlaySFX == false)
            return;

        // 若指定了音源位置，且距离玩家超过限定距离，则不播放
        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;

        // 检查索引有效性，随机调整音调，播放音效
        if (_sfxIndex < sfx.Length)
        {

            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    /// <summary>
    /// 直接停止指定音效
    /// </summary>
    /// <param name="_index"></param>
    public void StopSFX(int _index) => sfx[_index].Stop();

    /// <summary>
    /// 渐进式降低音量后停止（淡出效果）
    /// </summary>
    /// <param name="_index"></param>
    public void StopSFXWithTime(int _index) => StartCoroutine(DecreaseVolume(sfx[_index]));

    /// <summary>
    /// 协程，渐进式降低音量后停止
    /// </summary>
    /// <param name="_audio"></param>
    /// <returns></returns>
    private IEnumerator DecreaseVolume(AudioSource _audio)
    {
        // 保存原始音量
        float defaultVolume = _audio.volume;

        // 渐进式降低音量
        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.6f);

            // 音量过低时停止并恢复原始音量
            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defaultVolume;
                break;
            }
        }

    }

    /// <summary>
    /// 随机播放一首BGM
    /// </summary>
    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    /// <summary>
    /// 播放指定索引的BGM
    /// </summary>
    /// <param name="_bgmIndex"></param>
    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    /// <summary>
    /// 停止所有BGM
    /// </summary>
    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    /// <summary>
    /// 允许播放音效
    /// </summary>
    private void AllowSFX() => canPlaySFX = true;
}
