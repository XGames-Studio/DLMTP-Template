using DancingLineFanmade.Level;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class Speed : MonoBehaviour
    {
        [SerializeField] private bool setFakePlayer = false;
        [SerializeField, ShowIf("setFakePlayer")] private FakePlayer player;
        [SerializeField, MinValue(0)] private float speed = 12;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !setFakePlayer)
            {
                Player.Instance.Speed = speed;
            }
            if ((other.CompareTag("FakePlayer") || other.CompareTag("Obstacle")) && setFakePlayer) player.speed = speed;
        }
    }
}