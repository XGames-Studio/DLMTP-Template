using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace DancingLineFanmade.Level
{
    [DisallowMultipleComponent]
    public class CameraFollower : MonoBehaviour
    {
        private Transform selfTransform;

        public static CameraFollower Instance { get; private set; }
        public Camera thisCamera { get; set; }

        public Transform target;
        public bool follow = true;
        public bool smooth = true;

        internal Transform rotator;
        internal Transform scale;

        private Tween offsetTween { get; set; }
        private Tween rotationTween { get; set; }
        private Tween scaleTween { get; set; }
        private Tween shakeTween { get; set; }
        private Tween fovTween { get; set; }
        private float shakePower { get; set; }

        private readonly Vector3 followSpeed = new(1.2f, 3f, 6f);
        private readonly Quaternion rotation = Quaternion.Euler(0, -45, 0);

        private Vector3 Translation
        {
            get
            {
                var targetPosition = rotation * target.position;
                var selfPosition = rotation * selfTransform.position;
                return targetPosition - selfPosition;
            }
        }

        private Transform origin;

        private void Awake()
        {
            Instance = this;
            selfTransform = transform;
        }

        private void Start()
        {
            rotator = selfTransform.Find("Rotator");
            scale = rotator.Find("Scale");
            thisCamera = scale.Find("Camera").GetComponent<Camera>();
            origin = new GameObject("CameraMovementOrigin")
            {
                transform =
                {
                    position = Vector3.zero,
                    rotation = Quaternion.Euler(0, 45, 0),
                    localScale = Vector3.one
                }
            }.transform;
        }

        private void Update()
        {
            var result = new Vector3(Translation.x * Time.smoothDeltaTime * followSpeed.x,
                    Translation.y * Time.smoothDeltaTime * followSpeed.y,
                    Translation.z * Time.smoothDeltaTime * followSpeed.z);
            if (LevelManager.GameState == GameStatus.Playing && follow)
            {
                if (smooth)
                    selfTransform.Translate(result, origin);
                else selfTransform.Translate(result);
            }
        }

        public void Trigger(Vector3 n_offset, Vector3 n_rotation, Vector3 n_scale, float n_fov, float duration,
            Ease ease, RotateMode mode, UnityEvent callback, bool use, AnimationCurve curve)
        {
            SetOffset(n_offset, duration, ease, use, curve);
            SetRotation(n_rotation, duration, mode, ease, use, curve);
            SetScale(n_scale, duration, ease, use, curve);
            SetFov(n_fov, duration, ease, use, curve);
            rotationTween.OnComplete(callback.Invoke);
        }

        public void KillAll()
        {
            offsetTween?.Kill();
            rotationTween?.Kill();
            scaleTween?.Kill();
            shakeTween?.Kill();
            fovTween?.Kill();
        }

        private void SetOffset(Vector3 n_offset, float duration, Ease ease, bool use, AnimationCurve curve)
        {
            if (offsetTween != null)
            {
                offsetTween.Kill();
                offsetTween = null;
            }

            offsetTween = !use
                ? rotator.DOLocalMove(n_offset, duration).SetEase(ease)
                : rotator.DOLocalMove(n_offset, duration).SetEase(curve);
        }

        private void SetRotation(Vector3 n_rotation, float duration, RotateMode mode, Ease ease, bool use,
            AnimationCurve curve)
        {
            if (rotationTween != null)
            {
                rotationTween.Kill();
                rotationTween = null;
            }

            rotationTween = !use
                ? rotator.DOLocalRotate(n_rotation, duration, mode).SetEase(ease)
                : rotator.DOLocalRotate(n_rotation, duration, mode).SetEase(curve);
        }

        private void SetScale(Vector3 n_scale, float duration, Ease ease, bool use, AnimationCurve curve)
        {
            if (scaleTween != null)
            {
                scaleTween.Kill();
                scaleTween = null;
            }

            scaleTween = !use
                ? scale.DOScale(n_scale, duration).SetEase(ease)
                : scale.DOScale(n_scale, duration).SetEase(curve);
        }

        private void SetFov(float n_fov, float duration, Ease ease, bool use, AnimationCurve curve)
        {
            if (fovTween != null)
            {
                fovTween.Kill();
                fovTween = null;
            }

            fovTween = !use
                ? thisCamera.DOFieldOfView(n_fov, duration).SetEase(ease)
                : thisCamera.DOFieldOfView(n_fov, duration).SetEase(curve);
        }

        public void DoShake(float power = 1f, float duration = 3f)
        {
            if (shakeTween != null)
            {
                shakeTween.Kill();
                shakeTween = null;
            }

            shakeTween = DOTween.To(() => shakePower, x => shakePower = x, power, duration * 0.5f).SetEase(Ease.Linear);
            shakeTween.SetLoops(2, LoopType.Yoyo);
            shakeTween.OnUpdate(ShakeUpdate);
            shakeTween.OnComplete(ShakeFinished);
        }

        private void ShakeUpdate()
        {
            scale.transform.localPosition = new Vector3(UnityEngine.Random.value * shakePower,
                UnityEngine.Random.value * shakePower, UnityEngine.Random.value * shakePower);
        }

        private void ShakeFinished()
        {
            scale.transform.localPosition = Vector3.zero;
        }
    }

    [Serializable]
    public class CameraSettings
    {
        public Vector3 offset;
        public Vector3 rotation;
        public Vector3 scale;
        public float fov;
        public bool follow;

        internal CameraSettings GetCamera()
        {
            var settings = new CameraSettings();
            var follower = CameraFollower.Instance;
            settings.offset = follower.rotator.localPosition;
            settings.rotation = follower.rotator.localEulerAngles;
            settings.scale = follower.scale.localScale;
            settings.fov = follower.thisCamera.fieldOfView;
            settings.follow = follower.follow;
            return settings;
        }

        internal void SetCamera()
        {
            var follower = CameraFollower.Instance;
            follower.rotator.localPosition = offset;
            follower.rotator.localEulerAngles = rotation;
            follower.scale.localScale = scale;
            follower.scale.localPosition = Vector3.zero;
            follower.thisCamera.fieldOfView = fov;
            follower.follow = follow;
        }
    }
}