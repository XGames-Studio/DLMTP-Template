using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class EnvironmentBehaviour : PlayableBehaviour
{
    [ColorUsage(false,true)]public Color Ambient_SkyColor = new Color(1, 1, 1);
    [ColorUsage(false,true)]public Color EquatorColor = new Color(1, 1, 1);
    [ColorUsage(false,true)]public Color GroundColor = new Color(1, 1, 1);
}