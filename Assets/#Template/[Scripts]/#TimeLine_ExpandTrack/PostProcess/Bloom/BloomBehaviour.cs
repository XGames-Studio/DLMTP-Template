using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class BloomBehaviour : PlayableBehaviour
{
    [Tooltip("是否启用 Bloom 效果")]
    public bool enableBloom = true;

    [FoldoutGroup("强度与阈值")]
    [Tooltip("发光强度（0~10）：值越大效果越明显"), Range(0f, 10f)]
    public float intensity = 0.5f; // 3.5.1 默认值

    [FoldoutGroup("强度与阈值")]
    [Tooltip("亮度阈值（0~10）：超过此值的区域才会发光"), Range(0f, 10f)]
    public float threshold = 1f; // 3.5.1 默认值

    [FoldoutGroup("过渡与扩散")]
    [Tooltip("软过渡（0~1）：值越大，阈值边缘过渡越柔和"), Range(0f, 1f)] // 用户指定范围
    public float softKnee = 0.5f; // 3.5.1 默认值

    [FoldoutGroup("过渡与扩散")]
    [Tooltip("扩散度（1~10）：值越大，发光扩散范围越广"), Range(1f, 10f)] // 用户指定范围
    public float diffusion = 7f; // 3.5.1 默认值
}