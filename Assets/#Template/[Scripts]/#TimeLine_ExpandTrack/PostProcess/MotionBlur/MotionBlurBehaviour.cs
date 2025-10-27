using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class MotionBlurBehaviour : PlayableBehaviour
{
    // 修正变量名：将 enableMotionMotionBlur 改为 enableMotionBlur
    [Tooltip("是否启用运动模糊效果")]
    public bool enableMotionBlur = true; // 此处原拼写为 enableMotionMotionBlur，导致引用错误

    [FoldoutGroup("模糊强度")]
    [Tooltip("快门角度（0~360度）：值越大模糊越强"), Range(0f, 360f)]
    public float shutterAngle = 270f;

    [FoldoutGroup("模糊质量")]
    [Tooltip("样本数量（4~32）：值越高模糊越平滑（性能消耗增加）"), Range(4, 32)]
    public int sampleCount = 8;
}