using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class VignetteMixerBehaviour : PlayableBehaviour
{
    // ========================= 原始Vignette参数（用于销毁时恢复） =========================
    private float _originalIntensity;
    private Vector2 _originalCenter;
    private float _originalSmoothness;
    private float _originalRoundness;
    private Color _originalColor;
    private bool _originalRounded;
    private bool _originalEnabled;

    // ========================= 状态变量 =========================
    private bool _isFirstFrame = true; // 标记是否为第一帧（仅初始化一次原始参数）
    private PostProcessVolume _boundVolume; // 轨道绑定的PostProcessVolume
    private Vignette _vignetteSettings; // PostProcessVolume中的Vignette组件

    /// <summary>
    /// 每帧执行：混合多片段Vignette参数，并应用到后期体积
    /// </summary>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // 1. 获取轨道绑定的PostProcessVolume（playerData即绑定对象）
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || _boundVolume.profile == null)
            return;

        // 2. 从后期体积中获取Vignette组件（无则返回，避免空引用）
        if (!_boundVolume.profile.TryGetSettings(out _vignetteSettings))
            return;

        // 3. 第一帧：保存原始Vignette参数（避免修改后无法恢复）
        if (_isFirstFrame)
        {
            SaveOriginalVignetteSettings();
            _isFirstFrame = false;
        }

        // 4. 初始化混合参数（用原始值兜底，无片段时显示初始暗角）
        float blendedIntensity = _originalIntensity;
        Vector2 blendedCenter = _originalCenter;
        float blendedSmoothness = _originalSmoothness;
        float blendedRoundness = _originalRoundness;
        Color blendedColor = _originalColor;
        bool blendedRounded = _originalRounded;
        bool blendedEnabled = _originalEnabled;

        float totalWeight = 0f; // 所有片段的总权重（用于归一化混合值）
        float maxWeight = 0f; // 最大权重（用于bool类型参数选择：取权重最高的片段值）

        // 5. 遍历所有Vignette片段，按权重混合参数
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (Mathf.Approximately(inputWeight, 0f))
                continue; // 权重为0的片段跳过（无影响）

            // 获取当前片段的Vignette参数
            ScriptPlayable<VignetteBehaviour> inputPlayable =
                (ScriptPlayable<VignetteBehaviour>)playable.GetInput(i);
            VignetteBehaviour inputVignette = inputPlayable.GetBehaviour();

            // -------------------------- 数值型参数：加权累加（支持多片段混合） --------------------------
            blendedIntensity += inputVignette.intensity * inputWeight;
            blendedSmoothness += inputVignette.smoothness * inputWeight;
            blendedRoundness += inputVignette.roundness * inputWeight;
            // Vector2（中心位置）：x/y分量分别加权累加
            blendedCenter.x += inputVignette.center.x * inputWeight;
            blendedCenter.y += inputVignette.center.y * inputWeight;
            // 颜色：RGB分量分别加权累加
            blendedColor += inputVignette.vignetteColor * inputWeight;

            // -------------------------- bool型参数：取权重最大的片段值（无法直接加权） --------------------------
            if (inputWeight > maxWeight)
            {
                maxWeight = inputWeight;
                blendedRounded = inputVignette.rounded;
                blendedEnabled = inputVignette.enableVignette;
            }

            // 累加总权重（用于后续归一化）
            totalWeight += inputWeight;
        }

        // 6. 归一化混合参数（避免总权重>1导致参数异常，如强度超过1）
        if (totalWeight > 0f)
        {
            blendedIntensity /= totalWeight;
            blendedSmoothness /= totalWeight;
            blendedRoundness /= totalWeight;
            blendedCenter.x /= totalWeight;
            blendedCenter.y /= totalWeight;
            blendedColor /= totalWeight;
        }

        // 7. 应用混合后的参数到Vignette组件（Post Process V2参数需访问.value属性）
        ApplyBlendedVignetteSettings(
            blendedIntensity, blendedCenter, blendedSmoothness,
            blendedRoundness, blendedColor, blendedRounded, blendedEnabled
        );
    }

    /// <summary>
    /// 混合器销毁时：恢复原始Vignette参数（退出播放/ Timeline停止时生效）
    /// </summary>
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_vignetteSettings != null)
        {
            _vignetteSettings.intensity.value = _originalIntensity;
            _vignetteSettings.center.value = _originalCenter;
            _vignetteSettings.smoothness.value = _originalSmoothness;
            _vignetteSettings.roundness.value = _originalRoundness;
            _vignetteSettings.color.value = _originalColor;
            _vignetteSettings.rounded.value = _originalRounded;
            _vignetteSettings.enabled.value = _originalEnabled;
        }

        // 重置状态（避免下次使用时参数残留）
        _isFirstFrame = true;
        _boundVolume = null;
        _vignetteSettings = null;
    }

    /// <summary>
    /// 保存原始Vignette参数（第一帧调用）
    /// </summary>
    private void SaveOriginalVignetteSettings()
    {
        _originalIntensity = _vignetteSettings.intensity.value;
        _originalCenter = _vignetteSettings.center.value;
        _originalSmoothness = _vignetteSettings.smoothness.value;
        _originalRoundness = _vignetteSettings.roundness.value;
        _originalColor = _vignetteSettings.color.value;
        _originalRounded = _vignetteSettings.rounded.value;
        _originalEnabled = _vignetteSettings.enabled.value;
    }

    /// <summary>
    /// 将混合后的参数应用到Vignette组件
    /// </summary>
    private void ApplyBlendedVignetteSettings(
        float intensity, Vector2 center, float smoothness,
        float roundness, Color color, bool rounded, bool enabled
    )
    {
        _vignetteSettings.intensity.value = intensity;
        _vignetteSettings.center.value = center;
        _vignetteSettings.smoothness.value = smoothness;
        _vignetteSettings.roundness.value = roundness;
        _vignetteSettings.color.value = color;
        _vignetteSettings.rounded.value = rounded;
        _vignetteSettings.enabled.value = enabled;
    }
}