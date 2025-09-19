using UnityEngine;

public class DisableInPlayMode : MonoBehaviour
{
    [Tooltip("是否在播放模式下禁用物体")]
    public bool disableInPlayMode = true;
    
    void Awake()
    {
        if (Application.isPlaying && disableInPlayMode)
        {
            gameObject.SetActive(false);
        }
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        // 确保在编辑器模式下可见
        if (!Application.isPlaying && !gameObject.activeSelf && disableInPlayMode)
        {
            gameObject.SetActive(true);
        }
    }
    #endif
}