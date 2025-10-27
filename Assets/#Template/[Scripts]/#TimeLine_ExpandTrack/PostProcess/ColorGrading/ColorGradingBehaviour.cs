using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ColorGradingBehaviour : PlayableBehaviour
{
    [Tooltip("是否启用颜色分级效果")]
    public bool enableColorGrading = true;

    [Tooltip("整体亮度（-100=全黑，0=原始，100=极亮）"), Range(-100f, 100f)]
    public float brightness = 0f;

    [Tooltip("对比度（-100=低对比，0=原始，100=高对比）"), Range(-100f, 100f)]
    public float contrast = 0f;

    // 关键修正： saturation 范围改为 [-100, 100]
    [Tooltip("饱和度（-100=黑白，0=原始，100=最高饱和）"), Range(-100f, 100f)]
    public float saturation = 0f; // 0 为原始饱和度（3.5.1 版本基准值）

    [Tooltip("色调偏移（-180~180度）"), Range(-180f, 180f)]
    public float hueShift = 0f;

    [FoldoutGroup("色温与色调")]
    [Tooltip("色温（-100=冷蓝，0=原始，100=暖黄）"), Range(-100f, 100f)]
    public float temperature = 0f;

    [FoldoutGroup("色温与色调")]
    [Tooltip("色调偏移（-100=偏绿，0=原始，100=偏品红）"), Range(-100f, 100f)]
    public float tint = 0f;
}