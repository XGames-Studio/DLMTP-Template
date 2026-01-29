using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class VignetteMixerBehaviour : PlayableBehaviour
{
    // 原始参数备份
    private float _originalIntensity;
    private Vector2 _originalCenter;
    private float _originalRoundness;
    private bool _originalRounded;
    private Color _originalColor;
    private bool _originalEnabled;

    // 关键：备份原始的 overrideState 状态（而非直接设为 false）
    private bool _originalIntensityOverrideState;
    private bool _originalCenterOverrideState;
    private bool _originalRoundnessOverrideState;
    private bool _originalRoundedOverrideState;
    private bool _originalColorOverrideState;
    private bool _originalEnabledOverrideState;

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private Vignette _vignetteSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _vignetteSettings))
            return;

        if (_isFirstFrame)
        {
            // 1. 备份原始参数值
            _originalIntensity = _vignetteSettings.intensity.value;
            _originalCenter = _vignetteSettings.center.value;
            _originalRoundness = _vignetteSettings.roundness.value;
            _originalRounded = _vignetteSettings.rounded.value;
            _originalColor = _vignetteSettings.color.value;
            _originalEnabled = _vignetteSettings.enabled.value;

            // 2. 备份原始 overrideState 状态（核心修正：用 overrideState 而非 override）
            _originalIntensityOverrideState = _vignetteSettings.intensity.overrideState;
            _originalCenterOverrideState = _vignetteSettings.center.overrideState;
            _originalRoundnessOverrideState = _vignetteSettings.roundness.overrideState;
            _originalRoundedOverrideState = _vignetteSettings.rounded.overrideState;
            _originalColorOverrideState = _vignetteSettings.color.overrideState;
            _originalEnabledOverrideState = _vignetteSettings.enabled.overrideState;

            // 3. 强制开启 overrideState 以修改参数（v3.5.1 正确写法）
            _vignetteSettings.intensity.overrideState = true;
            _vignetteSettings.center.overrideState = true;
            _vignetteSettings.roundness.overrideState = true;
            _vignetteSettings.rounded.overrideState = true;
            _vignetteSettings.color.overrideState = true;
            _vignetteSettings.enabled.overrideState = true;

            _isFirstFrame = false;
        }

        // 初始化最终参数为原始值
        float finalIntensity = _originalIntensity;
        Vector2 finalCenter = _originalCenter;
        float finalRoundness = _originalRoundness;
        bool finalRounded = _originalRounded;
        Color finalColor = _originalColor;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 混合多个 Clip 的参数
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<VignetteBehaviour>)playable.GetInput(i)).GetBehaviour();

            // 线性混合数值型参数
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalCenter.x += (input.center.x - _originalCenter.x) * weight;
            finalCenter.y += (input.center.y - _originalCenter.y) * weight;
            finalRoundness += (input.roundness - _originalRoundness) * weight;

            // Bool 型参数取权重最大的 Clip 值
            finalRounded = weight > maxWeight ? input.rounded : finalRounded;
            finalColor += (input.color - _originalColor) * weight;

            totalWeight += weight;

            // 启用状态取权重最大的 Clip 值
            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableVignette;
            }
        }

        // 参数范围限制
        if (totalWeight > 0)
        {
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 1f);
            finalCenter.x = Mathf.Clamp(finalCenter.x, 0f, 1f);
            finalCenter.y = Mathf.Clamp(finalCenter.y, 0f, 1f);
            finalRoundness = Mathf.Clamp(finalRoundness, 0f, 1f);
            finalColor = ClampColor(finalColor);
        }

        // 应用最终参数到 PostProcessVolume
        _vignetteSettings.intensity.value = finalIntensity;
        _vignetteSettings.center.value = finalCenter;
        _vignetteSettings.roundness.value = finalRoundness;
        _vignetteSettings.rounded.value = finalRounded;
        _vignetteSettings.color.value = finalColor;
        _vignetteSettings.enabled.value = finalEnabled;
    }

    // 颜色值范围限制（0~1）
    private Color ClampColor(Color color)
    {
        return new Color(
            Mathf.Clamp01(color.r),
            Mathf.Clamp01(color.g),
            Mathf.Clamp01(color.b),
            Mathf.Clamp01(color.a)
        );
    }

    // 销毁时恢复原始参数和 overrideState 状态
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_vignetteSettings != null)
        {
            // 恢复原始参数值
            _vignetteSettings.intensity.value = _originalIntensity;
            _vignetteSettings.center.value = _originalCenter;
            _vignetteSettings.roundness.value = _originalRoundness;
            _vignetteSettings.rounded.value = _originalRounded;
            _vignetteSettings.color.value = _originalColor;
            _vignetteSettings.enabled.value = _originalEnabled;

            // 核心修正：恢复原始 overrideState 状态（而非直接设为 false）
            _vignetteSettings.intensity.overrideState = _originalIntensityOverrideState;
            _vignetteSettings.center.overrideState = _originalCenterOverrideState;
            _vignetteSettings.roundness.overrideState = _originalRoundnessOverrideState;
            _vignetteSettings.rounded.overrideState = _originalRoundedOverrideState;
            _vignetteSettings.color.overrideState = _originalColorOverrideState;
            _vignetteSettings.enabled.overrideState = _originalEnabledOverrideState;
        }
        _isFirstFrame = true;
    }
}