using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class EnvironmentMixerBehaviour : PlayableBehaviour
{
    #region 全局环境光默认值（仅保留Color模式需要的参数）
    private Color Default_AmbientColor; // 原始环境光颜色（Source=Color模式对应的值）
    #endregion

    private bool m_FirstFrameHappened; // 标记是否为第一帧（仅初始化一次默认值）

    /// <summary>
    /// 每帧执行：混合多片段颜色参数，并应用到全局环境光
    /// </summary>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // 第一帧：保存原始环境光颜色（避免修改后无法恢复）
        if (!m_FirstFrameHappened)
        {
            Default_AmbientColor = RenderSettings.ambientSkyColor; // Color模式下环境光存储于ambientSkyColor
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount(); // 当前轨道上的片段数量
        Color blended_AmbientColor = Color.clear; // 混合后的环境光颜色
        float totalWeight = 0f; // 所有片段的总权重（用于归一化混合值）

        // 遍历所有片段，按权重混合环境光颜色
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i); // 当前片段的权重（0~1）
            if (Mathf.Approximately(inputWeight, 0f))
                continue; // 权重为0的片段跳过（无影响）

            // 获取当前片段的环境光参数
            ScriptPlayable<EnvironmentBehaviour> inputPlayable =
                (ScriptPlayable<EnvironmentBehaviour>)playable.GetInput(i);
            EnvironmentBehaviour input = inputPlayable.GetBehaviour();

            // 按权重累加颜色（权重越高，对混合结果影响越大）
            blended_AmbientColor += input.Ambient_Color * inputWeight;
            totalWeight += inputWeight;
        }

        // 应用混合后的颜色到全局环境光（处理“无片段”和“多片段”两种情况）
        if (totalWeight > 0f)
        {
            // 多片段：归一化混合颜色（避免总权重>1导致颜色过亮），并与原始颜色过渡
            blended_AmbientColor /= totalWeight;
            RenderSettings.ambientSkyColor = blended_AmbientColor + Default_AmbientColor * (1f - totalWeight);
        }
        else
        {
            // 无片段：恢复原始环境光颜色
            RenderSettings.ambientSkyColor = Default_AmbientColor;
        }

        // 强制设置AmbientMode为Flat（确保与Source=Color模式匹配，避免模式不兼容）
        if (RenderSettings.ambientMode != AmbientMode.Flat)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
        }
    }

    /// <summary>
    /// 混合器销毁时：恢复原始环境光颜色（退出播放/ Timeline停止时生效）
    /// </summary>
    public override void OnPlayableDestroy(Playable playable)
    {
        m_FirstFrameHappened = false;
        RenderSettings.ambientSkyColor = Default_AmbientColor; // 恢复初始颜色
        RenderSettings.ambientMode = AmbientMode.Flat; // 保持与Source=Color模式一致
    }
}