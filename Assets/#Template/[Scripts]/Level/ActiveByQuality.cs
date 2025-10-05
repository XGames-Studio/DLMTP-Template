using Sirenix.OdinInspector;
using UnityEngine;

namespace DancingLineFanmade.Level
{
    [DisallowMultipleComponent]
    public class ActiveByQuality : MonoBehaviour
    {
        public bool showInLow, showInMedium, showInHigh;

        internal void OnEnable()
        {
            int quality = QualitySettings.GetQualityLevel();
            switch (quality)
            {
                case 0:
                    gameObject.SetActive(showInLow);
                    break;
                case 1:
                    gameObject.SetActive(showInMedium);
                    break;
                case >=2:
                    gameObject.SetActive(showInHigh);
                    break;
            }
        }
    }
}