using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 控制音量的UI滑块功能
/// </summary>
public class UI_VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public string parametr;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float multiplier;

    /// <summary>
    /// 滑块值改变时的回调方法,将线性的滑块值转换为分贝值
    /// </summary>
    /// <param name="_value"></param>
    public void SliderValue(float _value) => audioMixer.SetFloat(parametr, Mathf.Log10(_value) * multiplier);

    /// <summary>
    /// 用于从保存的数据中加载音量值
    /// </summary>
    /// <param name="_value"></param>
    public void LoadSlider(float _value)
    {
        if (_value >= 0.001f)
            slider.value = _value;
    }

}
