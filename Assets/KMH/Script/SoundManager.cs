using DesignPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer mixer;
    public AudioSource bgm;
    public static SoundManager instance;
    public AudioClip[] bglist;

    public float bgmValue;
    private AudioSource _source;
    
    private void Awake()
    {
        // if(instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(instance);
        //     SceneManager.sceneLoaded += OnSceneLoaded;
        // }
        
        _instance = this;
        _source = GetComponent<AudioSource>();

        // _source.playOnAwake = false;
        LoadSoundValue();

    }

    private void Start()
    {
        // LoadSoundValue();
        bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0]; 
        BGMVolume(bgmValue);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for(int i=0;i<bglist.Length;i++)
        {
            if(arg0.name == bglist[i].name)
                BGMPlay(bglist[i]);
        }
        
    }

    public void BGMVolume(float sliderValue)        //볼륨을 조절하는 함수
    {
        bgmValue = sliderValue;
        float dB = SliderToMixerVolume(sliderValue);
        PlayerPrefs.SetFloat("BGMSound", sliderValue); // 슬라이더 값을 저장
        mixer.SetFloat("BGM", dB);
    }

    public void BGMPlay(AudioClip clip)
    {
        // GameObject go = new GameObject(bgm + "Sound");
        // AudioSource audioSource = go.AddComponent<AudioSource>();
        // bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        // bgm.clip = clip;
        // bgm.loop = true;
        // bgm.volume = 0.1f;
        // bgm.Play();
        
        if (bgm == null)
        {
            Debug.LogWarning("BGM AudioSource가 없습니다!");
            return;
        }
        bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0]; // 초기 1회만 해도 됨
        bgm.clip = clip;
        bgm.loop = true;
        bgm.volume = 1f; // 볼륨은 믹서로만 조절
        bgm.Play();
    }

    private void LoadSoundValue()
    {
        Debug.Log("저장된 사운드 값을 가져옵니다.");
        if (PlayerPrefs.HasKey("BGMSound"))
        {
            bgmValue = PlayerPrefs.GetFloat("BGMSound");
        }
        else
        {
            bgmValue = 0.5f;
            PlayerPrefs.SetFloat("BGMSound", bgmValue);
        }

        Debug.Log($"슬라이더값 기준 BGM: {bgmValue:F2}");
    }
    
    
    private float SliderToMixerVolume(float sliderValue)
    {
        return Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
    }
    
    
}
