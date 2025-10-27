using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class BloomMixerBehaviour : PlayableBehaviour
{
    // ԭʼBloom������ͬ���޸�Ϊ�ٷ����������Ƴ�highQualityFiltering��
    private float _originalIntensity;
    private float _originalThreshold;
    private float _originalSoftKnee;
    private float _originalDiffusion; // �ؼ��޸���_originalRadius �� _originalDiffusion
    private Color _originalColor;
    private Texture _originalDirtTexture;
    private float _originalDirtIntensity;
    private bool _originalEnableBloom;

    // ״̬����
    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private Bloom _bloomSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _bloomSettings))
            return;

        // ��һ֡����ԭʼ����
        if (_isFirstFrame)
        {
            SaveOriginalBloomSettings();
            _isFirstFrame = false;
        }

        // ��ʼ����ϲ�������ԭʼֵ���ף�
        float blendedIntensity = _originalIntensity;
        float blendedThreshold = _originalThreshold;
        float blendedSoftKnee = _originalSoftKnee;
        float blendedDiffusion = _originalDiffusion; // �ؼ��޸���blendedRadius �� blendedDiffusion
        Color blendedColor = _originalColor;
        Texture blendedDirtTex = _originalDirtTexture;
        float blendedDirtIntensity = _originalDirtIntensity;
        bool blendedEnable = _originalEnableBloom;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // ��������Ƭ�λ�ϲ���
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (Mathf.Approximately(inputWeight, 0f))
                continue;

            ScriptPlayable<BloomBehaviour> inputPlayable =
                (ScriptPlayable<BloomBehaviour>)playable.GetInput(i);
            BloomBehaviour inputBloom = inputPlayable.GetBehaviour();

            totalWeight += inputWeight;

            // ��ֵ�Ͳ�������Ȩ���
            blendedIntensity += inputBloom.intensity * inputWeight;
            blendedThreshold += inputBloom.threshold * inputWeight;
            blendedSoftKnee += inputBloom.softKnee * inputWeight;
            blendedDiffusion += inputBloom.diffusion * inputWeight; // �ؼ��޸���inputBloom.radius �� inputBloom.diffusion
            blendedColor += inputBloom.bloomColor * inputWeight;
            blendedDirtIntensity += inputBloom.dirtIntensity * inputWeight;

            // ����/����ȡȨ�����Ĳ���
            if (inputWeight > maxWeight)
            {
                maxWeight = inputWeight;
                blendedDirtTex = inputBloom.dirtTexture;
                blendedEnable = inputBloom.enableBloom;
            }
        }

        // ��һ����ϲ���������Ȩ���ܺ��쳣��
        if (totalWeight > 0f)
        {
            blendedIntensity /= totalWeight;
            blendedThreshold /= totalWeight;
            blendedSoftKnee /= totalWeight;
            blendedDiffusion /= totalWeight; // �ؼ��޸���blendedRadius �� blendedDiffusion
            blendedColor /= totalWeight;
            blendedDirtIntensity /= totalWeight;
        }

        // Ӧ�û�ϲ�����Bloom
        ApplyBlendedSettings(blendedIntensity, blendedThreshold, blendedSoftKnee,
            blendedDiffusion, blendedColor, blendedDirtTex, blendedDirtIntensity, blendedEnable);
    }

    // ����ʱ�ָ�ԭʼ����
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_bloomSettings != null)
        {
            _bloomSettings.intensity.value = _originalIntensity;
            _bloomSettings.threshold.value = _originalThreshold;
            _bloomSettings.softKnee.value = _originalSoftKnee;
            _bloomSettings.diffusion.value = _originalDiffusion; // �ؼ��޸���radius �� diffusion
            _bloomSettings.color.value = _originalColor;
            _bloomSettings.dirtTexture.value = _originalDirtTexture;
            _bloomSettings.dirtIntensity.value = _originalDirtIntensity;
            _bloomSettings.enabled.value = _originalEnableBloom;
        }

        _isFirstFrame = true;
        _boundVolume = null;
        _bloomSettings = null;
    }

    // ����ԭʼ������ͬ���޸Ĳ�������
    private void SaveOriginalBloomSettings()
    {
        _originalIntensity = _bloomSettings.intensity.value;
        _originalThreshold = _bloomSettings.threshold.value;
        _originalSoftKnee = _bloomSettings.softKnee.value;
        _originalDiffusion = _bloomSettings.diffusion.value; // �ؼ��޸���radius �� diffusion
        _originalColor = _bloomSettings.color.value;
        _originalDirtTexture = _bloomSettings.dirtTexture.value;
        _originalDirtIntensity = _bloomSettings.dirtIntensity.value;
        _originalEnableBloom = _bloomSettings.enabled.value;
    }

    // Ӧ�û�ϲ�����ͬ���޸Ĳ�������
    private void ApplyBlendedSettings(float intensity, float threshold, float softKnee,
        float diffusion, Color color, Texture dirtTex, float dirtIntensity, bool enable)
    {
        _bloomSettings.intensity.value = intensity;
        _bloomSettings.threshold.value = threshold;
        _bloomSettings.softKnee.value = softKnee;
        _bloomSettings.diffusion.value = diffusion; // �ؼ��޸���radius �� diffusion
        _bloomSettings.color.value = color;
        _bloomSettings.dirtTexture.value = dirtTex;
        _bloomSettings.dirtIntensity.value = dirtIntensity;
        _bloomSettings.enabled.value = enable;
    }
}