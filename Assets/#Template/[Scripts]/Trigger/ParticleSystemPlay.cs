using DancingLineFanmade.Animated;
using DancingLineFanmade.Level;
using UnityEngine;

public class ParticleSystemPlay : MonoBehaviour
{
    public ParticleSystem particlesystem;

    void Start()
    {
        particlesystem.Pause();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            particlesystem.Play();
            LevelManager.revivePlayer += ResetData;
        }
    }
    private void ResetData()
    {
        Debug.Log("复活恢复：" + gameObject.name);
        LevelManager.revivePlayer -= ResetData;
        if (particlesystem.isPlaying)
        {
            particlesystem.Stop();
        }
    }
}
