using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }
    
    public void SetMusicVolume()
    {
        float sliderVolume = musicSlider.value;
        mainMixer.SetFloat("music", Mathf.Log10(sliderVolume)*20);
        PlayerPrefs.SetFloat("musicVolume", sliderVolume);
    }
    
    public void SetSFXVolume()
    {
        float sliderVolume = SFXSlider.value;
        mainMixer.SetFloat("SFX", Mathf.Log10(sliderVolume)*20);
        PlayerPrefs.SetFloat("SFXVolume", sliderVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        SetMusicVolume();
        SetSFXVolume();
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
