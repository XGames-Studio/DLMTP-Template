using System;
using DancingLineFanmade.Level;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace DancingLineFanmade.UI
{
    [DisallowMultipleComponent]
    public class SetQuality : MonoBehaviour
    {
        [SerializeField] private Text qualityText, antiAliasText;
        [SerializeField] private Toggle postProcessToggle, shadowToggle;

        private int qualityLevel = 0;
        private int antiAliasLevel = 0;

        private void Start()
        {
            qualityLevel = QualitySettings.GetQualityLevel();
            antiAliasLevel = QualitySettings.antiAliasing;
            
            SetText();
            foreach (ActiveByQuality a in FindObjectsOfType<ActiveByQuality>(true)) a.OnEnable();
            
            postProcessToggle.onValueChanged.AddListener(delegate { SetPostProcess(postProcessToggle.isOn); });
            shadowToggle.onValueChanged.AddListener(delegate { SetShadow(shadowToggle.isOn); });
        }

        public void SetQualityLevel(bool add)
        {
            if (add) qualityLevel = qualityLevel++ >= 2 ? qualityLevel = 0 : qualityLevel++;
            else qualityLevel = qualityLevel-- <= 0 ? qualityLevel = 2 : qualityLevel--;
            QualitySettings.SetQualityLevel(qualityLevel);
            SetText();
            foreach (ActiveByQuality a in FindObjectsOfType<ActiveByQuality>(true)) a.OnEnable();
        }

        public void SetAntiAlias(bool add)
        {
            // Off, 2x, 4x, 8x
            if (add) antiAliasLevel = antiAliasLevel++ >= 3 ? antiAliasLevel = 0 : antiAliasLevel++;
            else antiAliasLevel = antiAliasLevel-- <= 0 ? antiAliasLevel = 3 : antiAliasLevel--;
            QualitySettings.antiAliasing = antiAliasLevel;
            antiAliasText.text = antiAliasLevel > 0 ? $"x{Math.Pow(2, antiAliasLevel)}" : "Off";
        }
        
        public void SetPostProcess(bool enable)
        {
            postProcessToggle.isOn = enable;
            
            SetText();
        }
        
        public void SetShadow(bool enable)
        {
            shadowToggle.isOn = enable;
            
            SetText();
        }

        private void SetText()
        {
            LevelManager.SetFPSLimit(int.MaxValue);
            switch (qualityLevel)
            {
                case 0:
                    qualityText.text = "低";
                    QualitySettings.shadows = ShadowQuality.Disable;
                    break;
                case 1:
                    qualityText.text = "中";
                    QualitySettings.shadows = ShadowQuality.Disable;
                    break;
                case 2:
                    qualityText.text = "高";
                    QualitySettings.shadows = ShadowQuality.All;
                    break;
            }

            switch (antiAliasLevel)
            {
                case 1:
                    antiAliasText.text = "x2";
                    break;
                case 2:
                    antiAliasText.text = "x4";
                    break;
                case 3:
                    antiAliasText.text = "x8";
                    break;
                default:
                    antiAliasText.text = "Off";
                    break;
            }
            
            QualitySettings.shadows = shadowToggle.isOn ? ShadowQuality.All : ShadowQuality.Disable;

            PostProcessVolume[] postProcessVolumes = FindObjectsOfType<PostProcessVolume>(true);
            foreach (PostProcessVolume p in postProcessVolumes)
            {
                p.enabled = postProcessToggle.isOn;
            }
        }
    }
}