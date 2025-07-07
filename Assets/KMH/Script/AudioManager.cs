using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------------Audio Source-----------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------------------Audio Clip-------------------")]
    public AudioClip BGM;
    public AudioClip mouseClick;
    public AudioClip gameStart;


    private void Start()
    {
        musicSource.clip = BGM;
        musicSource.Play();
    }
}
