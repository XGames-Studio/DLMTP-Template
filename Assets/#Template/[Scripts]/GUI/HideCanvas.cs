using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace GUI.Global
{
    
    public class HideCanvas : MonoBehaviour
    {
        public float duration = 0.5f;
    
        public bool autoHideOnEnable = true;
    
        public bool invokeBeforeAnimation = true;
        public UnityEvent onHideComplete;

        public List<Tween> tweens = new List<Tween>();
    
        bool isHiding = false;
        private void OnEnable()
        {
            if (autoHideOnEnable) BtnHide();
        }

        private UnityAction onComplete;

        public void OnClick()
        {
            Hide();
        }
    
        public void StopTweens()
        {
            foreach (var tween in tweens)
            {
                tween.Kill();
            }
        }
    
        public void Hide(UnityAction onComplete = null)
        {
            if (isHiding) return;
            isHiding = true;
        
            if (invokeBeforeAnimation) InvokeOnComplete();
            if (GetComponent<ShowCanvas>() != null) GetComponent<ShowCanvas>().StopTweens();
        
            tweens.Add(transform.DORotate(new Vector3(0f,0f,-15f), 0.5f).SetEase(Ease.InOutSine));
            tweens.Add(transform.DOLocalMoveY(-400f, 0.5f).SetEase(Ease.InBack));
        
            tweens.Add(GetComponent<CanvasGroup>().DOFade(0f, 0.2f).SetEase(Ease.Linear).SetDelay(0.3f).OnComplete(() =>
            {
                GetComponent<CanvasGroup>().interactable = false;
            }));
            GetComponent<CanvasGroup>().blocksRaycasts = false;

            this.onComplete = onComplete;
            if (!invokeBeforeAnimation) Invoke(nameof(InvokeOnComplete), duration);
        }
    
        [Button("Hide")]
        public void BtnHide()
        {
            transform.localEulerAngles = new Vector3(0f, 0f, 15f);
            transform.localPosition = new Vector3(0f, -400f, 0f);
        
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
        }

        void InvokeOnComplete()
        {
            isHiding = false;
            onComplete?.Invoke();
            onHideComplete?.Invoke();
        }

    }

}