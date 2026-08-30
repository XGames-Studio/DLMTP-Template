using UnityEngine;
using UnityEditor;

public class AnimationClipKeyframeOffset : EditorWindow
{
    private GameObject rootObject;
    private AnimationClip sourceClip;
    private int intervalFrames = 3; // Time offset between each consecutive child node in frames

    [MenuItem("Tools/Stagger Animation Keyframes")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipKeyframeOffset>("Stagger Keyframes");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Stagger Keyframe Offset Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Hierarchy (A)", rootObject, typeof(GameObject), true);
        sourceClip = (AnimationClip)EditorGUILayout.ObjectField("Target Animation Clip", sourceClip, typeof(AnimationClip), false);
        intervalFrames = EditorGUILayout.IntField("Frame Offset Interval", intervalFrames);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Apply Stagger Offset", GUILayout.Height(30)))
        {
            if (rootObject == null || sourceClip == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both the Root Hierarchy (A) and Target Animation Clip.", "OK");
                return;
            }

            ProcessKeyframeOffset();
        }
    }

    private void ProcessKeyframeOffset()
    {
        // Record undo for safety
        Undo.RegisterCompleteObjectUndo(sourceClip, "Stagger Animation Keyframes");

        // Get clip frame rate (defaults to 60fps if not specified)
        float frameRate = sourceClip.frameRate > 0 ? sourceClip.frameRate : 60f;
        float secondsPerFrame = 1f / frameRate;

        // Retrieve all animation curve bindings in the clip
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(sourceClip);

        int childCount = rootObject.transform.childCount;
        int processedCurves = 0;

        // Iterate through all direct children of the root object
        for (int i = 0; i < childCount; i++)
        {
            Transform child = rootObject.transform.GetChild(i);
            // Calculate time offset in seconds based on frame interval
            float timeOffset = i * intervalFrames * secondsPerFrame;

            foreach (var binding in bindings)
            {
                // Match binding path with child object hierarchy name
                if (binding.path == child.name || binding.path.EndsWith("/" + child.name))
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(sourceClip, binding);
                    if (curve == null || curve.keys.Length == 0) continue;

                    Keyframe[] keys = curve.keys;

                    // Offset every keyframe on this curve
                    for (int k = 0; k < keys.Length; k++)
                    {
                        keys[k].time += timeOffset;
                    }

                    curve.keys = keys;
                    AnimationUtility.SetEditorCurve(sourceClip, binding, curve);
                    processedCurves++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Success", $"Successfully applied a {intervalFrames}-frame stagger offset across {childCount} children! (Frame Rate: {frameRate} FPS)", "OK");
    }
}