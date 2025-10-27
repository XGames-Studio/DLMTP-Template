using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class MotionBlurMixerBehaviour : PlayableBehaviour
{
    private float _originalShutterAngle;
    private int _originalSampleCount; // int����
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

        // ��ʼ������ԭʼֵΪ��׼
        float finalShutterAngle = _originalShutterAngle;
        int finalSampleCount = _originalSampleCount;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // ����߼����ۼ�ƫ���� �� Ȩ�أ�int���������⴦��
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<MotionBlurBehaviour>)playable.GetInput(i)).GetBehaviour();

            // float�����������ۼ�ƫ��
            finalShutterAngle += (input.shutterAngle - _originalShutterAngle) * weight;
            // int��������תfloat�ۼӣ����ȡ��
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

        // ��Χ����
        if (totalWeight > 0)
        {
            finalShutterAngle = Mathf.Clamp(finalShutterAngle, 0f, 360f);
            finalSampleCount = Mathf.Clamp(finalSampleCount, 4, 32); // int��Χ
        }

        // Ӧ�ò���
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