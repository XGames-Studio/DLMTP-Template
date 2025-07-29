using DancingLineFanmade.Level;
using DG.Tweening;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    public enum TriggerType
    {
        Open,
        Final,
        Waiting,
        Stop
    }

    [DisallowMultipleComponent]
    public class Pyramid : MonoBehaviour
    {
        [SerializeField] private float waitingTime = 5f;

        private Transform left;
        private Transform right;
        private float width = 1.8f;
        private float duration = 2f;

        private void Start()
        {
            left = transform.Find("Left");
            right = transform.Find("Right");
        }

        internal void Trigger(TriggerType type)
        {
            if (LevelManager.GameState != GameStatus.Died)
            {
                switch (type)
                {
                    case TriggerType.Open:
                        left.DOLocalMoveZ(width, duration).SetEase(Ease.Linear);
                        right.DOLocalMoveZ(-width, duration).SetEase(Ease.Linear);
                        LevelManager.revivePlayer += ResetDoor;
                        break;
                    case TriggerType.Final:
                        if (CameraFollower.Instance) CameraFollower.Instance.follow = false;
                        LevelManager.GameState = GameStatus.Moving;

                        break;
                    case TriggerType.Waiting: Invoke("Complete", waitingTime); break;
                    case TriggerType.Stop: LevelManager.GameState = GameStatus.Completed; break;
                }
            }
        }

        private void Complete()
        {
            Debug.Log("关卡结束");
            LevelManager.GameOverNormal(true);
        }

        public void StopPlayer()
        {
            if (LevelManager.GameState != GameStatus.Completed)
            {
                Invoke("Complete", 1f);
                LevelManager.GameState = GameStatus.Completed;
            }
        }

        private void ResetDoor()
        {
            LevelManager.revivePlayer -= ResetDoor;
            left.localPosition = Vector3.zero;
            right.localPosition = Vector3.zero;
        }

        private void OnDestroy()
        {
            LevelManager.revivePlayer -= ResetDoor;
        }
    }
}