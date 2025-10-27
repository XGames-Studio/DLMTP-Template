using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class BloomMixerBehaviour : PlayableBehaviour
{
    // ԭʼ�����洢��ƥ�� 3.5.1 �汾��
    private float _originalIntensity;
    private float _originalThreshold;
    private float _originalSoftKnee; // [0,1] ��Χ
    private float _originalDiffusion; // [1,10] ��Χ
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

        // ��һ֡����ȡ����ԭʼֵ���ǽű�Ĭ��ֵ��
        if (_isFirstFrame)
        {
            _originalIntensity = _bloomSettings.intensity.value;
            _originalThreshold = _bloomSettings.threshold.value;
            _originalSoftKnee = _bloomSettings.softKnee.value;
            _originalDiffusion = _bloomSettings.diffusion.value;
            _originalEnabled = _bloomSettings.enabled.value;

            // �����������ǣ�ȷ���޸���Ч��
            _bloomSettings.intensity.overrideState = true;
            _bloomSettings.threshold.overrideState = true;
            _bloomSettings.softKnee.overrideState = true;
            _bloomSettings.diffusion.overrideState = true;

            _isFirstFrame = false;
        }

        // ��ʼ�����Գ���ԭʼֵΪ��׼
        float finalIntensity = _originalIntensity;
        float finalThreshold = _originalThreshold;
        float finalSoftKnee = _originalSoftKnee;
        float finalDiffusion = _originalDiffusion;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // ����߼���ƫ���� �� Ȩ�� �ۼӣ��޸��ص����⣩
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<BloomBehaviour>)playable.GetInput(i)).GetBehaviour();

            // ����ÿ�����������ԭʼֵ��ƫ�ƣ��ٳ���Ȩ���ۼ�
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalThreshold += (input.threshold - _originalThreshold) * weight;
            finalSoftKnee += (input.softKnee - _originalSoftKnee) * weight; // ���� [0,1]
            finalDiffusion += (input.diffusion - _originalDiffusion) * weight; // ���� [1,10]

            totalWeight += weight;

            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableBloom;
            }
        }

        // ��Χ���ƣ��ϸ���ѭ�û�ָ����Χ��
        if (totalWeight > 0)
        {
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 10f);
            finalThreshold = Mathf.Clamp(finalThreshold, 0f, 10f);
            finalSoftKnee = Mathf.Clamp(finalSoftKnee, 0f, 1f); // ǿ�� [0,1]
            finalDiffusion = Mathf.Clamp(finalDiffusion, 1f, 10f); // ǿ�� [1,10]
        }

        // Ӧ�����ղ���
        _bloomSettings.intensity.value = finalIntensity;
        _bloomSettings.threshold.value = finalThreshold;
        _bloomSettings.softKnee.value = finalSoftKnee;
        _bloomSettings.diffusion.value = finalDiffusion;
        _bloomSettings.enabled.value = finalEnabled;
    }

    // ����ʱ�ָ�ԭʼ����
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