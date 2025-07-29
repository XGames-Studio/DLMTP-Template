using System;
using DancingLineFanmade.Level;
using DG.Tweening;
using UnityEngine;

public class FadeOutMusic : MonoBehaviour
{
    public float duration = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && LevelManager.GameState == GameStatus.Playing)
        {
            DOTween.To(() => Player.Instance.SoundTrack.volume, x => Player.Instance.SoundTrack.volume = x, 0f, duration)
                .OnComplete(() => Player.Instance.SoundTrack.Stop());
        }
    }
}
