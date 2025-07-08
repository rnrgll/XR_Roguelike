using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource bgm;
    public static SoundManager instance;
    public AudioClip[] bglist;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for(int i=0;i<bglist.Length;i++)
        {
            if(arg0.name == bglist[i].name)
                BGMPlay(bglist[i]);
        }
        
    }

    public void BGMVolume(float val)        //볼륨을 조절하는 함수
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(val) * 20);
    }

    public void BGMPlay(AudioClip clip)
    {
        GameObject go = new GameObject(bgm + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        bgm.clip = clip;
        bgm.loop = true;
        bgm.volume = 0.1f;
        bgm.Play();
    }
}
