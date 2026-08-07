using DancingLineFanmade.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class TrackSwitchTrigger : MonoBehaviour
    {
        [Title("Timeline Track Switch Control")]
        [Tooltip("Assign the GameObject that has the TimelineTrackSwitcher component attached.")]
        [SerializeField] private TimelineTrackSwitcher trackSwitcher;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Trigger Timeline track switching
                if (trackSwitcher != null)
                {
                    trackSwitcher.SwitchToTargetTrack();
                }
            }
        }
    }
}

// Special thanks to Gemini for assisting with this code!