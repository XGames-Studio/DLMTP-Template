using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class VignetteMixerBehaviour : PlayableBehaviour
{
    // ========================= ԭʼVignette��������������ʱ�ָ��� =========================
    private float _originalIntensity;
    private Vector2 _originalCenter;
    private float _originalSmoothness;
    private float _originalRoundness;
    private Color _originalColor;
    private bool _originalRounded;
    private bool _originalEnabled;

    // ========================= ״̬���� =========================
    private bool _isFirstFrame = true; // ����Ƿ�Ϊ��һ֡������ʼ��һ��ԭʼ������
    private PostProcessVolume _boundVolume; // ����󶨵�PostProcessVolume
    private Vignette _vignetteSettings; // PostProcessVolume�е�Vignette���

    /// <summary>
    /// ÿִ֡�У���϶�Ƭ��Vignette��������Ӧ�õ��������
    /// </summary>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // 1. ��ȡ����󶨵�PostProcessVolume��playerData���󶨶���
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || _boundVolume.profile == null)
            return;

        // 2. �Ӻ�������л�ȡVignette��������򷵻أ���������ã�
        if (!_boundVolume.profile.TryGetSettings(out _vignetteSettings))
            return;

        // 3. ��һ֡������ԭʼVignette�����������޸ĺ��޷��ָ���
        if (_isFirstFrame)
        {
            SaveOriginalVignetteSettings();
            _isFirstFrame = false;
        }

        // 4. ��ʼ����ϲ�������ԭʼֵ���ף���Ƭ��ʱ��ʾ��ʼ���ǣ�
        float blendedIntensity = _originalIntensity;
        Vector2 blendedCenter = _originalCenter;
        float blendedSmoothness = _originalSmoothness;
        float blendedRoundness = _originalRoundness;
        Color blendedColor = _originalColor;
        bool blendedRounded = _originalRounded;
        bool blendedEnabled = _originalEnabled;

        float totalWeight = 0f; // ����Ƭ�ε���Ȩ�أ����ڹ�һ�����ֵ��
        float maxWeight = 0f; // ���Ȩ�أ�����bool���Ͳ���ѡ��ȡȨ����ߵ�Ƭ��ֵ��

        // 5. ��������VignetteƬ�Σ���Ȩ�ػ�ϲ���
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (Mathf.Approximately(inputWeight, 0f))
                continue; // Ȩ��Ϊ0��Ƭ����������Ӱ�죩

            // ��ȡ��ǰƬ�ε�Vignette����
            ScriptPlayable<VignetteBehaviour> inputPlayable =
                (ScriptPlayable<VignetteBehaviour>)playable.GetInput(i);
            VignetteBehaviour inputVignette = inputPlayable.GetBehaviour();

            // -------------------------- ��ֵ�Ͳ�������Ȩ�ۼӣ�֧�ֶ�Ƭ�λ�ϣ� --------------------------
            blendedIntensity += inputVignette.intensity * inputWeight;
            blendedSmoothness += inputVignette.smoothness * inputWeight;
            blendedRoundness += inputVignette.roundness * inputWeight;
            // Vector2������λ�ã���x/y�����ֱ��Ȩ�ۼ�
            blendedCenter.x += inputVignette.center.x * inputWeight;
            blendedCenter.y += inputVignette.center.y * inputWeight;
            // ��ɫ��RGB�����ֱ��Ȩ�ۼ�
            blendedColor += inputVignette.vignetteColor * inputWeight;

            // -------------------------- bool�Ͳ�����ȡȨ������Ƭ��ֵ���޷�ֱ�Ӽ�Ȩ�� --------------------------
            if (inputWeight > maxWeight)
            {
                maxWeight = inputWeight;
                blendedRounded = inputVignette.rounded;
                blendedEnabled = inputVignette.enableVignette;
            }

            // �ۼ���Ȩ�أ����ں�����һ����
            totalWeight += inputWeight;
        }

        // 6. ��һ����ϲ�����������Ȩ��>1���²����쳣����ǿ�ȳ���1��
        if (totalWeight > 0f)
        {
            blendedIntensity /= totalWeight;
            blendedSmoothness /= totalWeight;
            blendedRoundness /= totalWeight;
            blendedCenter.x /= totalWeight;
            blendedCenter.y /= totalWeight;
            blendedColor /= totalWeight;
        }

        // 7. Ӧ�û�Ϻ�Ĳ�����Vignette�����Post Process V2���������.value���ԣ�
        ApplyBlendedVignetteSettings(
            blendedIntensity, blendedCenter, blendedSmoothness,
            blendedRoundness, blendedColor, blendedRounded, blendedEnabled
        );
    }

    /// <summary>
    /// ���������ʱ���ָ�ԭʼVignette�������˳�����/ Timelineֹͣʱ��Ч��
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

        // ����״̬�������´�ʹ��ʱ����������
        _isFirstFrame = true;
        _boundVolume = null;
        _vignetteSettings = null;
    }

    /// <summary>
    /// ����ԭʼVignette��������һ֡���ã�
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
    /// ����Ϻ�Ĳ���Ӧ�õ�Vignette���
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