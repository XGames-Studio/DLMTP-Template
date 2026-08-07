using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DancingLineFanmade.Trigger
{
    public class TimelineTrackSwitcher : MonoBehaviour
    {
        [Title("Main Director")]
        [SerializeField] private PlayableDirector director;

        [Title("Checkpoint Memory Settings")]
        [Tooltip("If enabled, track states are saved at checkpoints and restored on player revival.")]
        [SerializeField] private bool enableCheckpointMemory = true;

        [Title("Track Configuration")]
        [SerializeField] private string defaultTrackName = "Track_Default";
        [SerializeField] private string targetTrackName = "Track_Target";

        private TrackAsset defaultTrack;
        private TrackAsset targetTrack;

        private bool savedDefaultMuted = false;
        private bool savedTargetMuted = true;

        private void Awake()
        {
            if (director == null) director = GetComponent<PlayableDirector>();
            InitTracks();
        }

        private void InitTracks()
        {
            if (director == null || director.playableAsset is not TimelineAsset timeline) return;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == defaultTrackName) defaultTrack = track;
                else if (track.name == targetTrackName) targetTrack = track;
            }

            SetTrackState(defaultMuted: false, targetMuted: true);
        }

        /// <summary>
        /// Switch to target track (Default Muted, Target Active).
        /// </summary>
        public void SwitchToTargetTrack() => SetTrackState(defaultMuted: true, targetMuted: false);

        /// <summary>
        /// Switch to default track (Default Active, Target Muted).
        /// </summary>
        public void SwitchToDefaultTrack() => SetTrackState(defaultMuted: false, targetMuted: true);

        /// <summary>
        /// Saves current track states at checkpoints.
        /// </summary>
        public void SaveState()
        {
            if (!enableCheckpointMemory) return;
            if (defaultTrack != null) savedDefaultMuted = defaultTrack.muted;
            if (targetTrack != null) savedTargetMuted = targetTrack.muted;
        }

        /// <summary>
        /// Restores saved track states upon revival.
        /// </summary>
        public void RestoreState()
        {
            if (!enableCheckpointMemory) return;
            SetTrackState(savedDefaultMuted, savedTargetMuted);
        }

        private void SetTrackState(bool defaultMuted, bool targetMuted)
        {
            if (defaultTrack != null) defaultTrack.muted = defaultMuted;
            if (targetTrack != null) targetTrack.muted = targetMuted;

            RefreshGraphState();
        }

        /// <summary>
        /// Refreshes graph state immediately in Unity 6 without resetting playhead time.
        /// </summary>
        private void RefreshGraphState()
        {
            if (director == null || !director.playableGraph.IsValid()) return;

            double currentTime = director.time;
            bool isPlaying = director.state == PlayState.Playing;

            director.RebuildGraph();
            director.time = currentTime;

            if (isPlaying) director.Play();
        }

        private void OnDestroy() => SetTrackState(defaultMuted: false, targetMuted: true);
    }
}

// Special thanks to Gemini and WindBamboo for assisting with this code!