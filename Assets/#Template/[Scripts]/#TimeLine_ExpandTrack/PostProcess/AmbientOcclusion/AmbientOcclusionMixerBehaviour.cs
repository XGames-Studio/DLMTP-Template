using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class AmbientOcclusionMixerBehaviour : PlayableBehaviour
{
    // 仅保留 3.5.1 版本存在的参数
    private float _originalIntensity; // 场景原始强度（关键：作为混合基准）
    private float _originalRadius;    // 场景原始半径
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

        // 第一帧：强制获取场景当前参数（而非默认值）
        if (_isFirstFrame)
        {
            _originalIntensity = _aoSettings.intensity.value; // 关键：从场景读取实际值
            _originalRadius = _aoSettings.radius.value;
            _originalEnabled = _aoSettings.enabled.value;

            // 开启参数覆盖（确保修改生效）
            _aoSettings.intensity.overrideState = true;
            _aoSettings.radius.overrideState = true;

            _isFirstFrame = false;
        }

        // 初始化混合值为场景原始参数（无片段时保持原始状态）
        float finalIntensity = _originalIntensity;
        float finalRadius = _originalRadius;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f; // 总权重：用于归一化
        float maxWeight = 0f;   // 最大权重：用于判断启用状态

        // 混合所有片段参数（核心修复：累加加权值，避免被单个片段覆盖）
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue; // 跳过权重为0的片段

            var input = ((ScriptPlayable<AmbientOcclusionBehaviour>)playable.GetInput(i)).GetBehaviour();

            // 累加：每个片段的参数 × 权重（基于原始值的偏移）
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalRadius += (input.radius - _originalRadius) * weight;

            // 记录总权重（用于后续归一化）
            totalWeight += weight;

            // 启用状态取权重最高的片段
            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableAmbientOcclusion;
            }
        }

        // 归一化：当有多个片段时，确保参数过渡平滑（核心修复）
        if (totalWeight > 0)
        {
            // 限制参数范围（避免超出合理值）
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 4f);
            finalRadius = Mathf.Clamp(finalRadius, 0.1f, 10f);
        }

        // 应用最终参数
        _aoSettings.intensity.value = finalIntensity;
        _aoSettings.radius.value = finalRadius;
        _aoSettings.enabled.value = finalEnabled;
    }

    // 销毁时恢复原始设置
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