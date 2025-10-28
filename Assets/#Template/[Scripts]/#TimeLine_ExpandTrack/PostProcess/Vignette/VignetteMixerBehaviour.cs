using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class VignetteMixerBehaviour : PlayableBehaviour
{
    // 原始参数：严格匹配源码类型
    private float _originalIntensity;       // float（匹配 intensity.value）
    private Vector2 _originalCenter;        // Vector2（匹配 center.value）
    private float _originalRoundness;       // float（匹配 roundness.value）
    private bool _originalRounded;          // bool（匹配 rounded.value）
    private Color _originalColor;           // Color（匹配 color.value）
    private bool _originalEnabled;          // bool（匹配 enabled.value）

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
            // 初始化：从源码属性读取，类型严格匹配
            _originalIntensity = _vignetteSettings.intensity.value;
            _originalCenter = _vignetteSettings.center.value;
            _originalRoundness = _vignetteSettings.roundness.value; // 修正参数名：roundness
            _originalRounded = _vignetteSettings.rounded.value;     // 修正参数名：rounded
            _originalColor = _vignetteSettings.color.value;
            _originalEnabled = _vignetteSettings.enabled.value;     // enabled 是 BoolParameter，value 为 bool

            // 开启参数覆盖
            _vignetteSettings.intensity.overrideState = true;
            _vignetteSettings.center.overrideState = true;
            _vignetteSettings.roundness.overrideState = true;
            _vignetteSettings.rounded.overrideState = true;
            _vignetteSettings.color.overrideState = true;
            _vignetteSettings.enabled.overrideState = true;

            _isFirstFrame = false;
        }

        // 混合结果变量：类型严格匹配
        float finalIntensity = _originalIntensity;
        Vector2 finalCenter = _originalCenter;
        float finalRoundness = _originalRoundness;
        bool finalRounded = _originalRounded;
        Color finalColor = _originalColor;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 混合逻辑
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<VignetteBehaviour>)playable.GetInput(i)).GetBehaviour();

            // 混合参数（类型均匹配）
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalCenter.x += (input.center.x - _originalCenter.x) * weight;
            finalCenter.y += (input.center.y - _originalCenter.y) * weight;
            finalRoundness += (input.roundness - _originalRoundness) * weight;
            finalRounded = weight > maxWeight ? input.rounded : finalRounded; // bool 取权重最高值
            finalColor += (input.color - _originalColor) * weight;

            totalWeight += weight;

            // 启用状态取权重最高的片段（bool 赋值）
            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableVignette;
            }
        }

        // 范围限制（匹配源码参数范围）
        if (totalWeight > 0)
        {
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 1f);
            finalCenter.x = Mathf.Clamp(finalCenter.x, 0f, 1f);
            finalCenter.y = Mathf.Clamp(finalCenter.y, 0f, 1f);
            finalRoundness = Mathf.Clamp(finalRoundness, 0f, 1f);
            finalColor = ClampColor(finalColor);
        }

        // 应用参数（类型严格匹配，解决 95 行错误）
        _vignetteSettings.intensity.value = finalIntensity;
        _vignetteSettings.center.value = finalCenter;
        _vignetteSettings.roundness.value = finalRoundness;
        _vignetteSettings.rounded.value = finalRounded;
        _vignetteSettings.color.value = finalColor;
        _vignetteSettings.enabled.value = finalEnabled; // bool → bool（正确）
    }

    private Color ClampColor(Color color)
    {
        return new Color(
            Mathf.Clamp01(color.r),
            Mathf.Clamp01(color.g),
            Mathf.Clamp01(color.b),
            Mathf.Clamp01(color.a)
        );
    }

    // 销毁时恢复（解决 118 行错误）
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_vignetteSettings != null)
        {
            _vignetteSettings.intensity.value = _originalIntensity;
            _vignetteSettings.center.value = _originalCenter;
            _vignetteSettings.roundness.value = _originalRoundness;
            _vignetteSettings.rounded.value = _originalRounded;
            _vignetteSettings.color.value = _originalColor;
            _vignetteSettings.enabled.value = _originalEnabled; // bool → bool（正确）

            // 关闭覆盖
            _vignetteSettings.intensity.overrideState = false;
            _vignetteSettings.center.overrideState = false;
            _vignetteSettings.roundness.overrideState = false;
            _vignetteSettings.rounded.overrideState = false;
            _vignetteSettings.color.overrideState = false;
            _vignetteSettings.enabled.overrideState = false;
        }
        _isFirstFrame = true;
    }
}