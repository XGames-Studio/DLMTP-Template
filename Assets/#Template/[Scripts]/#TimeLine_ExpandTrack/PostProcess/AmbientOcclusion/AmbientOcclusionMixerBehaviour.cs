using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class AmbientOcclusionMixerBehaviour : PlayableBehaviour
{
    // ������ 3.5.1 �汾���ڵĲ���
    private float _originalIntensity; // ����ԭʼǿ�ȣ��ؼ�����Ϊ��ϻ�׼��
    private float _originalRadius;    // ����ԭʼ�뾶
    private bool _originalEnabled;

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private AmbientOcclusion _aoSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _aoSettings))
            return;

        // ��һ֡��ǿ�ƻ�ȡ������ǰ����������Ĭ��ֵ��
        if (_isFirstFrame)
        {
            _originalIntensity = _aoSettings.intensity.value; // �ؼ����ӳ�����ȡʵ��ֵ
            _originalRadius = _aoSettings.radius.value;
            _originalEnabled = _aoSettings.enabled.value;

            // �����������ǣ�ȷ���޸���Ч��
            _aoSettings.intensity.overrideState = true;
            _aoSettings.radius.overrideState = true;

            _isFirstFrame = false;
        }

        // ��ʼ�����ֵΪ����ԭʼ��������Ƭ��ʱ����ԭʼ״̬��
        float finalIntensity = _originalIntensity;
        float finalRadius = _originalRadius;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f; // ��Ȩ�أ����ڹ�һ��
        float maxWeight = 0f;   // ���Ȩ�أ������ж�����״̬

        // �������Ƭ�β����������޸����ۼӼ�Ȩֵ�����ⱻ����Ƭ�θ��ǣ�
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue; // ����Ȩ��Ϊ0��Ƭ��

            var input = ((ScriptPlayable<AmbientOcclusionBehaviour>)playable.GetInput(i)).GetBehaviour();

            // �ۼӣ�ÿ��Ƭ�εĲ��� �� Ȩ�أ�����ԭʼֵ��ƫ�ƣ�
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalRadius += (input.radius - _originalRadius) * weight;

            // ��¼��Ȩ�أ����ں�����һ����
            totalWeight += weight;

            // ����״̬ȡȨ����ߵ�Ƭ��
            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableAmbientOcclusion;
            }
        }

        // ��һ�������ж��Ƭ��ʱ��ȷ����������ƽ���������޸���
        if (totalWeight > 0)
        {
            // ���Ʋ�����Χ�����ⳬ������ֵ��
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 4f);
            finalRadius = Mathf.Clamp(finalRadius, 0.1f, 10f);
        }

        // Ӧ�����ղ���
        _aoSettings.intensity.value = finalIntensity;
        _aoSettings.radius.value = finalRadius;
        _aoSettings.enabled.value = finalEnabled;
    }

    // ����ʱ�ָ�ԭʼ����
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_aoSettings != null)
        {
            _aoSettings.intensity.value = _originalIntensity;
            _aoSettings.radius.value = _originalRadius;
            _aoSettings.enabled.value = _originalEnabled;

            _aoSettings.intensity.overrideState = false;
            _aoSettings.radius.overrideState = false;
        }
        _isFirstFrame = true;
    }
}