using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class MotionBlurMixerBehaviour : PlayableBehaviour
{
    private float _originalShutterAngle;
    private int _originalSampleCount; // int类型
    private bool _originalEnabled;

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private MotionBlur _motionBlurSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _motionBlurSettings))
            return;

        if (_isFirstFrame)
        {
            _originalShutterAngle = _motionBlurSettings.shutterAngle.value;
            _originalSampleCount = _motionBlurSettings.sampleCount.value;
            _originalEnabled = _motionBlurSettings.enabled.value;

            _motionBlurSettings.shutterAngle.overrideState = true;
            _motionBlurSettings.sampleCount.overrideState = true;

            _isFirstFrame = false;
        }

        // 初始化：以原始值为基准
        float finalShutterAngle = _originalShutterAngle;
        int finalSampleCount = _originalSampleCount;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 混合逻辑：累加偏移量 × 权重（int类型需特殊处理）
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<MotionBlurBehaviour>)playable.GetInput(i)).GetBehaviour();

            // float参数：正常累加偏移
            finalShutterAngle += (input.shutterAngle - _originalShutterAngle) * weight;
            // int参数：先转float累加，最后取整
            finalSampleCount = Mathf.RoundToInt(
                finalSampleCount + (input.sampleCount - _originalSampleCount) * weight
            );

            totalWeight += weight;

            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableMotionBlur;
            }
        }

        // 范围限制
        if (totalWeight > 0)
        {
            finalShutterAngle = Mathf.Clamp(finalShutterAngle, 0f, 360f);
            finalSampleCount = Mathf.Clamp(finalSampleCount, 4, 32); // int范围
        }

        // 应用参数
        _motionBlurSettings.shutterAngle.value = finalShutterAngle;
        _motionBlurSettings.sampleCount.value = finalSampleCount;
        _motionBlurSettings.enabled.value = finalEnabled;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (_motionBlurSettings != null)
        {
            _motionBlurSettings.shutterAngle.value = _originalShutterAngle;
            _motionBlurSettings.sampleCount.value = _originalSampleCount;
            _motionBlurSettings.enabled.value = _originalEnabled;

            _motionBlurSettings.shutterAngle.overrideState = false;
            _motionBlurSettings.sampleCount.overrideState = false;
        }
        _isFirstFrame = true;
    }
}