using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace LDH.LDH_Script.Audio
{
    public class AudioMixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;


        private float epsilon = 0.0001f;

        private void Start()
        {
            _audioMixer = SoundManager.Instance.mixer;
            bgmSlider.value = SoundManager.Instance.bgmValue;
            bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        }

        private void OnDestroy()
        {
            bgmSlider.onValueChanged.RemoveListener(SetBgmVolume);
        }


        public void SetBgmVolume(float value)
        {  
            //Debug.Log("슬라이드 값 변경에 따른 이벤트 호출");
            if (Mathf.Abs(value - SoundManager.Instance.bgmValue) > epsilon)
            {
                SoundManager.Instance.BGMVolume(value);
            }
        }
        
    }
}