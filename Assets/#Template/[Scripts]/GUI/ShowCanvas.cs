using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace GUI.Global
{
    
    public class ShowCanvas : MonoBehaviour
    {
        public float duration = 0.5f;
    
        private UnityAction onComplete;
    
        public bool invokeBeforeAnimation = true;
        public UnityEvent onShowComplete;
    
        bool isShowing = false;
    
        public List<Tween> tweens = new List<Tween>();

        public void OnClick()
        {
            Show();
        }
    
        public void StopTweens()
        {
            foreach (var tween in tweens)
            {
                tween.Kill();
            }
        }
    
        public void Show(UnityAction onComplete = null)
        {
            if (isShowing) return;
            isShowing = true;
        
            if (invokeBeforeAnimation) InvokeOnComplete();
            if (GetComponent<HideCanvas>() != null) GetComponent<HideCanvas>().StopTweens();
        
            GetComponent<CanvasGroup>().interactable = true;
            tweens.Add(transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutCirc));
            tweens.Add(transform.DOLocalMoveY(0f, 0.5f).SetEase(Ease.OutCirc));
            tweens.Add(GetComponent<CanvasGroup>().DOFade(1f, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }));
        
            this.onComplete = onComplete;
        
            if (!invokeBeforeAnimation) Invoke(nameof(InvokeOnComplete), duration);
        }
    
        [Button("Show")]
        public void BtnShow()
        {
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        
            transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            transform.localPosition = new Vector3(0f, 0f, 0f);
        
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
        }
    
        void InvokeOnComplete()
        {
            isShowing = false;
            onComplete?.Invoke();
            onShowComplete?.Invoke();
        }
    }
}