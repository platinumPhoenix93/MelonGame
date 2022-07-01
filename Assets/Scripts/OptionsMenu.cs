using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{

    private TextMeshProUGUI volumePercent;
    private Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {

        volumePercent = GameObject.Find("VolumePercent").GetComponent<TextMeshProUGUI>();
        volumeSlider = FindObjectOfType<Slider>();
        volumeSlider.value = AudioListener.volume;
    }

    void Update()
    {

        //Update volume and volume percent readout based on slider value
        int volume = Mathf.RoundToInt(volumeSlider.value * 100);
        volumePercent.text = volume + "%";
        AudioListener.volume = volumeSlider.value;
    }
}
