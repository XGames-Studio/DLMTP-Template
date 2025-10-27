using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DepthOfFieldBehaviour : PlayableBehaviour
{
    [Tooltip("是否启用景深效果")]
    public bool enableDepthOfField = true;

    [FoldoutGroup("核心参数")]
    [Tooltip("焦距（0~100米）：焦点到相机的距离"), Range(0f, 100f)]
    public float focusDistance = 10f; // 3.5.1 默认值

    [FoldoutGroup("核心参数")]
    [Tooltip("光圈（0.1~32）：数值越小景深越浅"), Range(0.1f, 32f)]
    public float aperture = 5.6f; // 3.5.1 默认值

    [FoldoutGroup("镜头参数")]
    [Tooltip("焦距长度（12~400mm）：影响透视和景深范围"), Range(12f, 400f)]
    public float focalLength = 50f; // 3.5.1 默认值

    // 移除 maxBlurSize：3.5.1 版本中该参数已移除或迁移
}