using UnityEngine;  
using DG.Tweening;
public class ACChanger : MonoBehaviour  
{  
	public Color newAmbientColor = Color.gray;  
	public float duration = 1f;
	
	void OnTriggerEnter(Collider other)  
	{  
		if (other.CompareTag("Player"))  
		{  
			DOTween.To(() => RenderSettings.ambientLight, x => RenderSettings.ambientLight = x, newAmbientColor, duration);
		}  
	}  
}