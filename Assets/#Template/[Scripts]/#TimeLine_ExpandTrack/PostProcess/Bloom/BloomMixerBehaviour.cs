using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class BloomMixerBehaviour : PlayableBehaviour
{
    // 原始参数存储（匹配 3.5.1 版本）
    private float _originalIntensity;
    private float _originalThreshold;
    private float _originalSoftKnee; // [0,1] 范围
    private float _originalDiffusion; // [1,10] 范围
    private bool _originalEnabled;

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private Bloom _bloomSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _bloomSettings))
            return;

        // 第一帧：读取场景原始值（非脚本默认值）
        if (_isFirstFrame)
        {
            _originalIntensity = _bloomSettings.intensity.value;
            _originalThreshold = _bloomSettings.threshold.value;
            _originalSoftKnee = _bloomSettings.softKnee.value;
            _originalDiffusion = _bloomSettings.diffusion.value;
            _originalEnabled = _bloomSettings.enabled.value;

            // 开启参数覆盖（确保修改生效）
            _bloomSettings.intensity.overrideState = true;
            _bloomSettings.threshold.overrideState = true;
            _bloomSettings.softKnee.overrideState = true;
            _bloomSettings.diffusion.overrideState = true;

            _isFirstFrame = false;
        }

        // 初始化：以场景原始值为基准
        float finalIntensity = _originalIntensity;
        float finalThreshold = _originalThreshold;
        float finalSoftKnee = _originalSoftKnee;
        float finalDiffusion = _originalDiffusion;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 混合逻辑：偏移量 × 权重 累加（修复重叠问题）
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<BloomBehaviour>)playable.GetInput(i)).GetBehaviour();

            // 计算每个参数相对于原始值的偏移，再乘以权重累加
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalThreshold += (input.threshold - _originalThreshold) * weight;
            finalSoftKnee += (input.softKnee - _originalSoftKnee) * weight; // 适配 [0,1]
            finalDiffusion += (input.diffusion - _originalDiffusion) * weight; // 适配 [1,10]

            totalWeight += weight;

            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableBloom;
            }
        }

        // 范围限制（严格遵循用户指定范围）
        if (totalWeight > 0)
        {
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 10f);
            finalThreshold = Mathf.Clamp(finalThreshold, 0f, 10f);
            finalSoftKnee = Mathf.Clamp(finalSoftKnee, 0f, 1f); // 强制 [0,1]
            finalDiffusion = Mathf.Clamp(finalDiffusion, 1f, 10f); // 强制 [1,10]
        }

        // 应用最终参数
        _bloomSettings.intensity.value = finalIntensity;
        _bloomSettings.threshold.value = finalThreshold;
        _bloomSettings.softKnee.value = finalSoftKnee;
        _bloomSettings.diffusion.value = finalDiffusion;
        _bloomSettings.enabled.value = finalEnabled;
    }

    // 销毁时恢复原始设置
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_bloomSettings != null)
        {
            _bloomSettings.intensity.value = _originalIntensity;
            _bloomSettings.threshold.value = _originalThreshold;
            _bloomSettings.softKnee.value = _originalSoftKnee;
            _bloomSettings.diffusion.value = _originalDiffusion;
            _bloomSettings.enabled.value = _originalEnabled;

            _bloomSettings.intensity.overrideState = false;
            _bloomSettings.threshold.overrideState = false;
            _bloomSettings.softKnee.overrideState = false;
            _bloomSettings.diffusion.overrideState = false;
        }
        _isFirstFrame = true;
    }
}