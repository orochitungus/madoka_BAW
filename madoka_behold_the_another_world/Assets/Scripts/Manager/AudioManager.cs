using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public List<AudioClip> BGMList;     // 鳴らすBGMのリスト
    public List<AudioClip> SEList;      // 鳴らすSEのリスト
	public List<AudioClip> VoiceList;	// 鳴らすVOICEのリスト
    public int MaxSE;              // 同時に鳴らせる最大SE数（SEは使わない可能性大）
	public int MaxVoice;

    private AudioSource bgmSource = null;
    private List<AudioSource> seSources = null;
	private List<AudioSource> voiceSources = null;
    private Dictionary<string, AudioClip> bgmDict = null;
    private Dictionary<string, AudioClip> seDict = null;
	private Dictionary<string, AudioClip> voiceDict = null;

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        // （なければ）AudioListener生成
        if (FindObjectsOfType(typeof(AudioListener)).All(o => !((AudioListener)o).enabled))
        {
            this.gameObject.AddComponent<AudioListener>();
        }
        // AudioSourceを生成
        this.bgmSource = this.gameObject.AddComponent<AudioSource>();
        this.seSources = new List<AudioSource>();
		this.voiceSources = new List<AudioSource>();

        // BGMとSEのリストを作成
        this.bgmDict = new Dictionary<string, AudioClip>();
        this.seDict = new Dictionary<string, AudioClip>();
		this.voiceDict = new Dictionary<string, AudioClip>();

        Action<Dictionary<string, AudioClip>, AudioClip> addClipDict = (dict, c) =>
        {
            if (!dict.ContainsKey(c.name))
            {
                dict.Add(c.name, c);
            }
        };
        this.BGMList.ForEach(bgm => addClipDict(this.bgmDict, bgm));
        this.SEList.ForEach(se => addClipDict(this.seDict, se));
		this.VoiceList.ForEach(voice => addClipDict(this.voiceDict, voice));
    }

    // SE再生
    public void PlaySE(string seName)
    {
        if (!this.seDict.ContainsKey(seName)) throw new ArgumentException(seName + " not found", "seName");

        AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.seSources.Count >= this.MaxSE)
            {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            this.seSources.Add(source);
        }

        source.clip = this.seDict[seName];
        source.Play();
    }
    // SE停止
    public void StopSE()
    {
        this.seSources.ForEach(s => s.Stop());
    }
    // BGM再生
    public void PlayBGM(string bgmName)
    {
        if (bgmDict == null)
        {
            return;
        }
        if (!this.bgmDict.ContainsKey(bgmName)) throw new ArgumentException(bgmName + " not found", "bgmName");
        if (this.bgmSource.clip == this.bgmDict[bgmName]) return;
        this.bgmSource.Stop();        
        this.bgmSource.clip = this.bgmDict[bgmName];
        this.bgmSource.Play();
        this.bgmSource.loop = true;
    }
    // BGM停止
    public void StopBGM()
    {
        this.bgmSource.Stop();
        this.bgmSource.clip = null;
    }

	// voice再生
	public void PlayVoice(string voiceName)
	{
		if(voiceDict == null)
		{
			return;
		}
		if (!this.voiceDict.ContainsKey(voiceName)) throw new ArgumentException(voiceName + " not found", "voiceName");
		AudioSource source = this.voiceSources.FirstOrDefault(s => !s.isPlaying);
		if (source == null)
		{
			if (this.voiceSources.Count >= this.MaxVoice)
			{
				Debug.Log("voice AudioSource is full");
				return;
			}

			source = this.gameObject.AddComponent<AudioSource>();
			this.voiceSources.Add(source);
		}

		source.clip = this.voiceDict[voiceName];
		source.Play();
	}

}

