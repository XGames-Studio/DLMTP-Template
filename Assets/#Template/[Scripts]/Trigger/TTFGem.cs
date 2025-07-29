using DancingLineFanmade.Level;
using UnityEngine;

namespace DancingLineFanmade.Trigger
{
    [DisallowMultipleComponent, RequireComponent(typeof(Collider), typeof(MeshRenderer))]
    public class TTFGem : MonoBehaviour
    {
        [SerializeField] private bool fake = false;

        private Player player;
        private GameObject effectPrefab;
        private GameObject effect;
        private bool got = false;
        private int index;

        private MeshRenderer MeshRenderer
        {
            get => GetComponent<MeshRenderer>();
        }

        private GameObject Aura
        {
            get => transform.Find("Aura").gameObject;
        }

        private void Start()
        {
            MeshRenderer.enabled = true;
            effectPrefab = Resources.Load<GameObject>("Prefabs/GetGem");
            player = Player.Instance;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * 60f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !got)
            {
                got = true;
                player.Events?.Invoke(6);
                MeshRenderer.enabled = false;
                Aura.SetActive(false);
                index = player.Checkpoints.Count;
                if (QualitySettings.GetQualityLevel() > 0) effect = Instantiate(effectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));

                player.BlockCount++;

                LevelManager.revivePlayer += ResetData;
            }
        }

        private void ResetData()
        {
            LevelManager.revivePlayer -= ResetData;
            LevelManager.CompareCheckpointIndex(index, () =>
            {
                got = false;
                MeshRenderer.enabled = true;
                Aura.SetActive(true);
            });
            if (effect) Destroy(effect);
        }

        private void OnDestroy()
        {
            LevelManager.revivePlayer -= ResetData;
        }
    }
}