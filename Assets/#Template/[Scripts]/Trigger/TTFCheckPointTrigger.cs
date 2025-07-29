using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider))]
    public class TTFCheckPointTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                transform.GetComponentInParent<TTFCheckPoint>().EnterTrigger();
                gameObject.SetActive(false);
            }
        }
    }
}