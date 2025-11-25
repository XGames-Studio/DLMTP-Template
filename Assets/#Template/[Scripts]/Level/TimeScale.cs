using System;
using DLMTP_GAME;
using UnityEngine;

namespace DancingLineFanmade.Level
{
    [DisallowMultipleComponent]
    public class TimeScale : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private KeyCode key = KeyCode.T;
        [SerializeField, Range(0f, 3f)] private float enabledValue = 1.25f;
        [SerializeField, Range(0f, 3f)] private float disabledValue = 1f;

        private new bool enabled = false;

        private void Update()
        {
            if (LevelManager.GameState == GameStatus.Playing)
            {
                KeyBoardManager.instance.AddKeyFunction(key, "切换时间倍速", () =>
                {
                    if (!enabled)
                    {
                        AudioManager.Pitch = enabledValue;
                        Time.timeScale = enabledValue;
                        enabled = true;
                    }
                    else
                    {
                        AudioManager.Pitch = disabledValue;
                        Time.timeScale = disabledValue;
                        enabled = false;
                    }
                });
            }
        }
#endif
    }
}