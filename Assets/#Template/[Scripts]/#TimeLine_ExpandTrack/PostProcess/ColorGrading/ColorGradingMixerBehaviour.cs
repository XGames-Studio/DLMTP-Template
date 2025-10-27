using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class ColorGradingMixerBehaviour : PlayableBehaviour
{
    private float _originalBrightness;
    private float _originalContrast;
    private float _originalSaturation;
    private float _originalHueShift;
    private float _originalTemperature;
    private float _originalTint;
    private bool _originalEnabled;

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private ColorGrading _colorGradingSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _colorGradingSettings))
            return;

        if (_isFirstFrame)
        {
            // 读取场景原始值（关键：而非脚本默认值）
            _originalBrightness = _colorGradingSettings.brightness.value;
            _originalContrast = _colorGradingSettings.contrast.value;
            _originalSaturation = _colorGradingSettings.saturation.value;
            _originalHueShift = _colorGradingSettings.hueShift.value;
            _originalTemperature = _colorGradingSettings.temperature.value;
            _originalTint = _colorGradingSettings.tint.value;
            _originalEnabled = _colorGradingSettings.enabled.value;

            // 开启覆盖
            _colorGradingSettings.brightness.overrideState = true;
            _colorGradingSettings.contrast.overrideState = true;
            _colorGradingSettings.saturation.overrideState = true;
            _colorGradingSettings.hueShift.overrideState = true;
            _colorGradingSettings.temperature.overrideState = true;
            _colorGradingSettings.tint.overrideState = true;

            _isFirstFrame = false;
        }

        // 初始化：以原始值为基准
        float finalBrightness = _originalBrightness;
        float finalContrast = _originalContrast;
        float finalSaturation = _originalSaturation;
        float finalHueShift = _originalHueShift;
        float finalTemperature = _originalTemperature;
        float finalTint = _originalTint;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 混合逻辑：累加偏移量 × 权重
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<ColorGradingBehaviour>)playable.GetInput(i)).GetBehaviour();

            // 计算偏移量（片段值 - 原始值），再乘以权重累加
            finalBrightness += (input.brightness - _originalBrightness) * weight;
            finalContrast += (input.contrast - _originalContrast) * weight;
            finalSaturation += (input.saturation - _originalSaturation) * weight;
            finalHueShift += (input.hueShift - _originalHueShift) * weight;
            finalTemperature += (input.temperature - _originalTemperature) * weight;
            finalTint += (input.tint - _originalTint) * weight;

            totalWeight += weight;

            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableColorGrading;
            }
        }

        // 范围限制（避免异常值）
        if (totalWeight > 0)
        {
            finalBrightness = Mathf.Clamp(finalBrightness, -100f, 100f);
            finalContrast = Mathf.Clamp(finalContrast, -100f, 100f);
            finalSaturation = Mathf.Clamp(finalSaturation, -100f, 100f);
            finalHueShift = Mathf.Clamp(finalHueShift, -180f, 180f);
            finalTemperature = Mathf.Clamp(finalTemperature, -100f, 100f);
            finalTint = Mathf.Clamp(finalTint, -100f, 100f);
        }

        // 应用参数
        _colorGradingSettings.brightness.value = finalBrightness;
        _colorGradingSettings.contrast.value = finalContrast;
        _colorGradingSettings.saturation.value = finalSaturation;
        _colorGradingSettings.hueShift.value = finalHueShift;
        _colorGradingSettings.temperature.value = finalTemperature;
        _colorGradingSettings.tint.value = finalTint;
        _colorGradingSettings.enabled.value = finalEnabled;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (_colorGradingSettings != null)
        {
            _colorGradingSettings.brightness.value = _originalBrightness;
            _colorGradingSettings.contrast.value = _originalContrast;
            _colorGradingSettings.saturation.value = _originalSaturation;
            _colorGradingSettings.hueShift.value = _originalHueShift;
            _colorGradingSettings.temperature.value = _originalTemperature;
            _colorGradingSettings.tint.value = _originalTint;
            _colorGradingSettings.enabled.value = _originalEnabled;

            _colorGradingSettings.brightness.overrideState = false;
            _colorGradingSettings.contrast.overrideState = false;
            _colorGradingSettings.saturation.overrideState = false;
            _colorGradingSettings.hueShift.overrideState = false;
            _colorGradingSettings.temperature.overrideState = false;
            _colorGradingSettings.tint.overrideState = false;
        }
        _isFirstFrame = true;
    }
}