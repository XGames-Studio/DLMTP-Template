using System;
using DancingLineFanmade.Level;
using UnityEngine;
using UnityEngine.UI;

public class SetLatency : MonoBehaviour
{
    public Text latencyText;
    public Text volumeText;

    private void Start()
    {
        Player.Instance.musicDelay = PlayerPrefs.GetFloat("MusicDelay", 0f);
        Player.Instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        SetText();
    }

    public void AddLatency(int step)
    {
        Player.Instance.musicDelay += step * 0.001f;
        
        SetText();
    }
    
    public void SubtractLatency(int step)
    {
        Player.Instance.musicDelay -= step * 0.001f;
        
        SetText();
    }
    
    public void AddVolume(int step)
    {
        Player.Instance.musicVolume += step * 0.1f;
        Player.Instance.musicVolume = Math.Clamp(Player.Instance.musicVolume, 0f, 1f);
        
        SetText();
    }
    
    public void SubtractVolume(int step)
    {
        Player.Instance.musicVolume -= step * 0.1f;
        Player.Instance.musicVolume = Math.Clamp(Player.Instance.musicVolume, 0f, 1f);
        
        SetText();
    }

    void SetText()
    {
        latencyText.text = $"{Math.Round(Player.Instance.musicDelay * 1000)} ms";
        volumeText.text = $"{Math.Round(Player.Instance.musicVolume * 100)}%";
        
        PlayerPrefs.SetFloat("MusicDelay", Player.Instance.musicDelay);
        PlayerPrefs.SetFloat("MusicVolume", Player.Instance.musicVolume);
    }
}
