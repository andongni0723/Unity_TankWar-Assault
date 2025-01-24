using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSlider : MonoBehaviour
{
    [Header("Component")]
    public Slider slider;
    public TMP_Text trackText;
    
    //[Header("Settings")]
    //[Header("Debug")]
    
    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float newValue)
    {
        trackText.text = newValue.ToString("0.00");
    }
}
