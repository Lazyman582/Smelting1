using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeCtrl : MonoBehaviour
{
    [Header("BGM播放器")]
    public AudioSource bgmAudio;
    [Header("UI")]
    public Slider bgmSlider;
    public Toggle muteToggle;

    private float lastVolume;

    void Start()
    {
        // 初始音量
        bgmSlider.minValue = 0;
        bgmSlider.maxValue = 1;
        bgmSlider.value = bgmAudio.volume;
        lastVolume = bgmAudio.volume;

        // 绑定事件
        bgmSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
    }

    // 滑块改音量
    void SetVolume(float value)
    {
        bgmAudio.volume = value;
        lastVolume = value;

        if (value > 0)
        {
            muteToggle.isOn = false;
        }
    }

    // 静音开关
    void ToggleMute(bool isOn)
    {
        if (isOn)
        {
            bgmAudio.volume = 0;
        }
        else
        {
            bgmAudio.volume = lastVolume;
        }
    }
}