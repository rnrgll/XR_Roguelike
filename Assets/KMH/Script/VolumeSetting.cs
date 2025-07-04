using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider VolumeSlider;

    public void SetMusicVolume()
    {
        float volume = VolumeSlider.value;
        mixer.SetFloat("music", volume);
    }
}
