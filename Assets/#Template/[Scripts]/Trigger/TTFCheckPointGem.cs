using DancingLineFanmade.Level;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider), typeof(MeshRenderer))]
    public class TTFCheckPointGem : MonoBehaviour
    {

        private Player player;
        private GameObject effectPrefab;
        private GameObject effect;
        private bool got = false;
        private int index;

        private MeshRenderer MeshRenderer
        {
            get => GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            MeshRenderer.enabled = true;
            effectPrefab = Resources.Load<GameObject>("Prefabs/GetGem");
            player = Player.Instance;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !got)
            {
                PickUp(true);
            }
        }

        public void PickUp(bool add)
        {
            got = true;
            player.Events?.Invoke(6);
            MeshRenderer.enabled = false;
            index = player.Checkpoints.Count;
            if (QualitySettings.GetQualityLevel() > 0) effect = Instantiate(effectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
            if (add) player.BlockCount++;
            if (add) LevelManager.revivePlayer += ResetData;
            transform.Find("Aura").gameObject.SetActive(false);
        }

        private void ResetData()
        {
            LevelManager.revivePlayer -= ResetData;
            LevelManager.CompareCheckpointIndex(index, () =>
            {
                got = false;
                MeshRenderer.enabled = true;
            });
            if (effect) Destroy(effect);
        }

        private void OnDestroy()
        {
            LevelManager.revivePlayer -= ResetData;
        }
    }
}