using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Slider volumeSlider;
    [SerializeField] GameObject offIcon, lowMedIcon, highMedIcon, onIcon;
    void Start()
    {
        if (!PlayerPrefs.HasKey("audioVolume"))
        {
            PlayerPrefs.SetFloat("audioVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        if (volumeSlider.value == 0)
        {
            offIcon.SetActive(true);
            lowMedIcon.SetActive(false);
            highMedIcon.SetActive(false);
            onIcon.SetActive(false);
        }
        else if (volumeSlider.value > 0 && volumeSlider.value <= 0.3)
        {
            offIcon.SetActive(false);
            lowMedIcon.SetActive(true);
            highMedIcon.SetActive(false);
            onIcon.SetActive(false);
        }
        else if (volumeSlider.value > 0.3 && volumeSlider.value <= 0.7)
        {
            offIcon.SetActive(false);
            lowMedIcon.SetActive(false);
            highMedIcon.SetActive(true);
            onIcon.SetActive(false);
        }
        else if (volumeSlider.value > 0.7)
        {
            offIcon.SetActive(false);
            lowMedIcon.SetActive(false);
            highMedIcon.SetActive(false);
            onIcon.SetActive(true);
        }
        Save();
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("audioVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("audioVolume", volumeSlider.value);
    }
}
