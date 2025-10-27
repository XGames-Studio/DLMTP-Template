using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class EnvironmentBehaviour : PlayableBehaviour
{
    /// <summary>
    /// 环境光颜色（对应 Environment Lighting → Source=Color 模式的颜色值）
    /// [ColorUsage(false, true)]：禁用 HDR 拾取器，但支持 HDR 数值（符合环境光设置）
    /// </summary>
    [Tooltip("环境光颜色（Source=Color模式下的全局环境光色）")]
    [ColorUsage(false, true)]
    public Color Ambient_Color = new Color(0.2f, 0.4f, 0.8f); // 默认浅蓝色（模拟白天天空）
}