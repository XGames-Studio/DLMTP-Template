using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementTrigger : MonoBehaviour
{
    public string achievementKey;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // AchievementManager.instance.AddAchievement(achievementKey);
        }
    }
}
