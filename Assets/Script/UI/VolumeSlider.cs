using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private void Start()
    {
        AudioManager.instance.ChangeMasterVolume(slider.value);
        slider.onValueChanged.AddListener(val => AudioManager.instance.ChangeMasterVolume(val));
    }
}
