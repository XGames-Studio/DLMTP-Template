using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class DepthOfFieldMixerBehaviour : PlayableBehaviour
{
    private float _originalFocusDistance;
    private float _originalAperture;
    private float _originalFocalLength;
    private bool _originalEnabled;

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private DepthOfField _dofSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _dofSettings))
            return;

        if (_isFirstFrame)
        {
            _originalFocusDistance = _dofSettings.focusDistance.value;
            _originalAperture = _dofSettings.aperture.value;
            _originalFocalLength = _dofSettings.focalLength.value;
            _originalEnabled = _dofSettings.enabled.value;

            _dofSettings.focusDistance.overrideState = true;
            _dofSettings.aperture.overrideState = true;
            _dofSettings.focalLength.overrideState = true;

            _isFirstFrame = false;
        }

        // 初始化：以原始值为基准
        float finalFocusDistance = _originalFocusDistance;
        float finalAperture = _originalAperture;
        float finalFocalLength = _originalFocalLength;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 混合逻辑：累加偏移量 × 权重
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<DepthOfFieldBehaviour>)playable.GetInput(i)).GetBehaviour();

            finalFocusDistance += (input.focusDistance - _originalFocusDistance) * weight;
            finalAperture += (input.aperture - _originalAperture) * weight;
            finalFocalLength += (input.focalLength - _originalFocalLength) * weight;

            totalWeight += weight;

            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableDepthOfField;
            }
        }

        // 范围限制
        if (totalWeight > 0)
        {
            finalFocusDistance = Mathf.Clamp(finalFocusDistance, 0f, 100f);
            finalAperture = Mathf.Clamp(finalAperture, 0.1f, 32f);
            finalFocalLength = Mathf.Clamp(finalFocalLength, 12f, 400f);
        }

        // 应用参数
        _dofSettings.focusDistance.value = finalFocusDistance;
        _dofSettings.aperture.value = finalAperture;
        _dofSettings.focalLength.value = finalFocalLength;
        _dofSettings.enabled.value = finalEnabled;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (_dofSettings != null)
        {
            _dofSettings.focusDistance.value = _originalFocusDistance;
            _dofSettings.aperture.value = _originalAperture;
            _dofSettings.focalLength.value = _originalFocalLength;
            _dofSettings.enabled.value = _originalEnabled;

            _dofSettings.focusDistance.overrideState = false;
            _dofSettings.aperture.overrideState = false;
            _dofSettings.focalLength.overrideState = false;
        }
        _isFirstFrame = true;
    }
}