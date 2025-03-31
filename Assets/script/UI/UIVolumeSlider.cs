using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string parametr;
                             
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier;
    private void Awake()
    {
        SliderValue(slider.value);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        SliderValue(slider.value);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void SliderValue(float value)
    {
       audioMixer.SetFloat(parametr, Mathf.Log10(value) * multiplier);
       // Debug.Log(Mathf.Log10(value) * multiplier);
    }
    public void LoadSlider(float value)
    {
        if(value>=0.001f)
        {
           // Debug.Log(value);
            //slider.value = Mathf.Pow(10, value / multiplier);
            slider.value =value;
            SliderValue(value);
        }
    }
}
