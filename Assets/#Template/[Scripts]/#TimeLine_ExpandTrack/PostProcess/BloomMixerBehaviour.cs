using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class BloomMixerBehaviour : PlayableBehaviour
{
    // 原始Bloom参数（同步修改为官方参数名，移除highQualityFiltering）
    private float _originalIntensity;
    private float _originalThreshold;
    private float _originalSoftKnee;
    private float _originalDiffusion; // 关键修复：_originalRadius → _originalDiffusion
    private Color _originalColor;
    private Texture _originalDirtTexture;
    private float _originalDirtIntensity;
    private bool _originalEnableBloom;

    // 状态变量
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

        // 第一帧保存原始参数
        if (_isFirstFrame)
        {
            SaveOriginalBloomSettings();
            _isFirstFrame = false;
        }

        // 初始化混合参数（用原始值兜底）
        float blendedIntensity = _originalIntensity;
        float blendedThreshold = _originalThreshold;
        float blendedSoftKnee = _originalSoftKnee;
        float blendedDiffusion = _originalDiffusion; // 关键修复：blendedRadius → blendedDiffusion
        Color blendedColor = _originalColor;
        Texture blendedDirtTex = _originalDirtTexture;
        float blendedDirtIntensity = _originalDirtIntensity;
        bool blendedEnable = _originalEnableBloom;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // 遍历所有片段混合参数
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

            // 数值型参数：加权混合
            blendedIntensity += inputBloom.intensity * inputWeight;
            blendedThreshold += inputBloom.threshold * inputWeight;
            blendedSoftKnee += inputBloom.softKnee * inputWeight;
            blendedDiffusion += inputBloom.diffusion * inputWeight; // 关键修复：inputBloom.radius → inputBloom.diffusion
            blendedColor += inputBloom.bloomColor * inputWeight;
            blendedDirtIntensity += inputBloom.dirtIntensity * inputWeight;

            // 布尔/纹理：取权重最大的参数
            if (inputWeight > maxWeight)
            {
                maxWeight = inputWeight;
                blendedDirtTex = inputBloom.dirtTexture;
                blendedEnable = inputBloom.enableBloom;
            }
        }

        // 归一化混合参数（避免权重总和异常）
        if (totalWeight > 0f)
        {
            blendedIntensity /= totalWeight;
            blendedThreshold /= totalWeight;
            blendedSoftKnee /= totalWeight;
            blendedDiffusion /= totalWeight; // 关键修复：blendedRadius → blendedDiffusion
            blendedColor /= totalWeight;
            blendedDirtIntensity /= totalWeight;
        }

        // 应用混合参数到Bloom
        ApplyBlendedSettings(blendedIntensity, blendedThreshold, blendedSoftKnee,
            blendedDiffusion, blendedColor, blendedDirtTex, blendedDirtIntensity, blendedEnable);
    }

    // 销毁时恢复原始参数
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_bloomSettings != null)
        {
            _bloomSettings.intensity.value = _originalIntensity;
            _bloomSettings.threshold.value = _originalThreshold;
            _bloomSettings.softKnee.value = _originalSoftKnee;
            _bloomSettings.diffusion.value = _originalDiffusion; // 关键修复：radius → diffusion
            _bloomSettings.color.value = _originalColor;
            _bloomSettings.dirtTexture.value = _originalDirtTexture;
            _bloomSettings.dirtIntensity.value = _originalDirtIntensity;
            _bloomSettings.enabled.value = _originalEnableBloom;
        }

        _isFirstFrame = true;
        _boundVolume = null;
        _bloomSettings = null;
    }

    // 保存原始参数（同步修改参数名）
    private void SaveOriginalBloomSettings()
    {
        _originalIntensity = _bloomSettings.intensity.value;
        _originalThreshold = _bloomSettings.threshold.value;
        _originalSoftKnee = _bloomSettings.softKnee.value;
        _originalDiffusion = _bloomSettings.diffusion.value; // 关键修复：radius → diffusion
        _originalColor = _bloomSettings.color.value;
        _originalDirtTexture = _bloomSettings.dirtTexture.value;
        _originalDirtIntensity = _bloomSettings.dirtIntensity.value;
        _originalEnableBloom = _bloomSettings.enabled.value;
    }

    // 应用混合参数（同步修改参数名）
    private void ApplyBlendedSettings(float intensity, float threshold, float softKnee,
        float diffusion, Color color, Texture dirtTex, float dirtIntensity, bool enable)
    {
        _bloomSettings.intensity.value = intensity;
        _bloomSettings.threshold.value = threshold;
        _bloomSettings.softKnee.value = softKnee;
        _bloomSettings.diffusion.value = diffusion; // 关键修复：radius → diffusion
        _bloomSettings.color.value = color;
        _bloomSettings.dirtTexture.value = dirtTex;
        _bloomSettings.dirtIntensity.value = dirtIntensity;
        _bloomSettings.enabled.value = enable;
    }
}