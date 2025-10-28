using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class VignetteMixerBehaviour : PlayableBehaviour
{
    // ԭʼ�������ϸ�ƥ��Դ������
    private float _originalIntensity;       // float��ƥ�� intensity.value��
    private Vector2 _originalCenter;        // Vector2��ƥ�� center.value��
    private float _originalRoundness;       // float��ƥ�� roundness.value��
    private bool _originalRounded;          // bool��ƥ�� rounded.value��
    private Color _originalColor;           // Color��ƥ�� color.value��
    private bool _originalEnabled;          // bool��ƥ�� enabled.value��

    private bool _isFirstFrame = true;
    private PostProcessVolume _boundVolume;
    private Vignette _vignetteSettings;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        _boundVolume = playerData as PostProcessVolume;
        if (_boundVolume == null || !_boundVolume.enabled || _boundVolume.profile == null)
            return;

        if (!_boundVolume.profile.TryGetSettings(out _vignetteSettings))
            return;

        if (_isFirstFrame)
        {
            // ��ʼ������Դ�����Զ�ȡ�������ϸ�ƥ��
            _originalIntensity = _vignetteSettings.intensity.value;
            _originalCenter = _vignetteSettings.center.value;
            _originalRoundness = _vignetteSettings.roundness.value; // ������������roundness
            _originalRounded = _vignetteSettings.rounded.value;     // ������������rounded
            _originalColor = _vignetteSettings.color.value;
            _originalEnabled = _vignetteSettings.enabled.value;     // enabled �� BoolParameter��value Ϊ bool

            // ������������
            _vignetteSettings.intensity.overrideState = true;
            _vignetteSettings.center.overrideState = true;
            _vignetteSettings.roundness.overrideState = true;
            _vignetteSettings.rounded.overrideState = true;
            _vignetteSettings.color.overrideState = true;
            _vignetteSettings.enabled.overrideState = true;

            _isFirstFrame = false;
        }

        // ��Ͻ�������������ϸ�ƥ��
        float finalIntensity = _originalIntensity;
        Vector2 finalCenter = _originalCenter;
        float finalRoundness = _originalRoundness;
        bool finalRounded = _originalRounded;
        Color finalColor = _originalColor;
        bool finalEnabled = _originalEnabled;

        float totalWeight = 0f;
        float maxWeight = 0f;

        // ����߼�
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float weight = playable.GetInputWeight(i);
            if (weight <= 0) continue;

            var input = ((ScriptPlayable<VignetteBehaviour>)playable.GetInput(i)).GetBehaviour();

            // ��ϲ��������;�ƥ�䣩
            finalIntensity += (input.intensity - _originalIntensity) * weight;
            finalCenter.x += (input.center.x - _originalCenter.x) * weight;
            finalCenter.y += (input.center.y - _originalCenter.y) * weight;
            finalRoundness += (input.roundness - _originalRoundness) * weight;
            finalRounded = weight > maxWeight ? input.rounded : finalRounded; // bool ȡȨ�����ֵ
            finalColor += (input.color - _originalColor) * weight;

            totalWeight += weight;

            // ����״̬ȡȨ����ߵ�Ƭ�Σ�bool ��ֵ��
            if (weight > maxWeight)
            {
                maxWeight = weight;
                finalEnabled = input.enableVignette;
            }
        }

        // ��Χ���ƣ�ƥ��Դ�������Χ��
        if (totalWeight > 0)
        {
            finalIntensity = Mathf.Clamp(finalIntensity, 0f, 1f);
            finalCenter.x = Mathf.Clamp(finalCenter.x, 0f, 1f);
            finalCenter.y = Mathf.Clamp(finalCenter.y, 0f, 1f);
            finalRoundness = Mathf.Clamp(finalRoundness, 0f, 1f);
            finalColor = ClampColor(finalColor);
        }

        // Ӧ�ò����������ϸ�ƥ�䣬��� 95 �д���
        _vignetteSettings.intensity.value = finalIntensity;
        _vignetteSettings.center.value = finalCenter;
        _vignetteSettings.roundness.value = finalRoundness;
        _vignetteSettings.rounded.value = finalRounded;
        _vignetteSettings.color.value = finalColor;
        _vignetteSettings.enabled.value = finalEnabled; // bool �� bool����ȷ��
    }

    private Color ClampColor(Color color)
    {
        return new Color(
            Mathf.Clamp01(color.r),
            Mathf.Clamp01(color.g),
            Mathf.Clamp01(color.b),
            Mathf.Clamp01(color.a)
        );
    }

    // ����ʱ�ָ������ 118 �д���
    public override void OnPlayableDestroy(Playable playable)
    {
        if (_vignetteSettings != null)
        {
            _vignetteSettings.intensity.value = _originalIntensity;
            _vignetteSettings.center.value = _originalCenter;
            _vignetteSettings.roundness.value = _originalRoundness;
            _vignetteSettings.rounded.value = _originalRounded;
            _vignetteSettings.color.value = _originalColor;
            _vignetteSettings.enabled.value = _originalEnabled; // bool �� bool����ȷ��

            // �رո���
            _vignetteSettings.intensity.overrideState = false;
            _vignetteSettings.center.overrideState = false;
            _vignetteSettings.roundness.overrideState = false;
            _vignetteSettings.rounded.overrideState = false;
            _vignetteSettings.color.overrideState = false;
            _vignetteSettings.enabled.overrideState = false;
        }
        _isFirstFrame = true;
    }
}