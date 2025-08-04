using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.Windows;

using static UnityEngine.UI.ContentSizeFitter;

public class EnvironmentMixerBehaviour : PlayableBehaviour
{
    #region Define Default Value [Default_]
    // Define Default Value [Default_]
    Color Default_SkyColor;
    Color Default_EquatorColor;
    Color Default_GroundColor;
    #endregion

    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!m_FirstFrameHappened)
        {
            #region FirstFrame Value Operation
            //FirstFrame Value Operation
            Default_SkyColor = RenderSettings.ambientSkyColor;
            Default_EquatorColor = RenderSettings.ambientEquatorColor;
            Default_GroundColor = RenderSettings.ambientGroundColor;
            #endregion

            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        #region Define Blend Value 
        //Define Blend Value [blended_]
        Color blended_SkyColor = new Color();
        Color blended_EquatorColor = new Color();
        Color blended_GroundColor = new Color();
        #endregion

        float totalWeight = 0f;
        float greatestWeight = 0f;
        int currentInputs = 0;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<EnvironmentBehaviour> inputPlayable = (ScriptPlayable<EnvironmentBehaviour>)playable.GetInput(i);
            EnvironmentBehaviour input = inputPlayable.GetBehaviour();

            #region Blend Initialzation
            // Blend Initialzation
            blended_SkyColor += input.Ambient_SkyColor * inputWeight;
            blended_EquatorColor += input.EquatorColor * inputWeight;
            blended_GroundColor += input.GroundColor * inputWeight;
            #endregion

            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
            }

            if (!Mathf.Approximately(inputWeight, 0f))
                currentInputs++;
        }

        #region Blend Value Assignment
        //Blend Value Assignment
        if (RenderSettings.ambientMode == AmbientMode.Flat)
        {
            RenderSettings.ambientSkyColor = blended_SkyColor + Default_SkyColor * (1f - totalWeight);
        }
        else if (RenderSettings.ambientMode == AmbientMode.Trilight)
        {
            RenderSettings.ambientSkyColor = blended_SkyColor + Default_SkyColor * (1f - totalWeight);
            RenderSettings.ambientEquatorColor = blended_EquatorColor + Default_SkyColor * (1f - totalWeight);
            RenderSettings.ambientGroundColor = blended_GroundColor + Default_SkyColor * (1f - totalWeight);
        }
        #endregion
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        m_FirstFrameHappened = false;

        #region Default Value
        //End Value Default
        RenderSettings.ambientSkyColor = Default_SkyColor;
        RenderSettings.ambientEquatorColor = Default_EquatorColor;
        RenderSettings.ambientGroundColor = Default_GroundColor;
        #endregion
    }
}
