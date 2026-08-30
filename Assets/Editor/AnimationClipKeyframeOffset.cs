using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Editor tool for applying stagger keyframe offsets across child hierarchies in Unity Timeline.
/// Supports multi-attribute shifting, smart prefix/word boundary matching, and live UI preview.
/// </summary>
public class AnimationClipKeyframeOffset : EditorWindow
{
    public enum SortOrder
    {
        HierarchyOrder,
        AlphabeticalOrder
    }

    public enum FilterMode
    {
        AllChildren,            // Process every direct child
        SmartPrefix,            // Matches prefix with word boundary (e.g. 'a' matches 'a_001' but ignores 'ab_001')
        ExactStartsWith,        // Literal String.StartsWith
        ContainsSubstring       // Literal String.Contains
    }

    private PlayableDirector playableDirector;
    private string targetTrackName = "Animation Track";
    private GameObject rootObject;
    private int intervalFrames = 3;
    private SortOrder sortOrder = SortOrder.HierarchyOrder;
    private FilterMode filterMode = FilterMode.SmartPrefix;
    private string nameFilter = "a";

    private Vector2 scrollPosition;

    [MenuItem("Tools/Stagger Animation Keyframes")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipKeyframeOffset>("Stagger Keyframes Pro");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Timeline Multi-Attribute Stagger Offset", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Target Configuration Section
        playableDirector = (PlayableDirector)EditorGUILayout.ObjectField("Playable Director", playableDirector, typeof(PlayableDirector), true);
        targetTrackName = EditorGUILayout.TextField("Target Track Name", targetTrackName);
        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Hierarchy", rootObject, typeof(GameObject), true);
        intervalFrames = EditorGUILayout.IntField("Frame Offset Interval", intervalFrames);
        sortOrder = (SortOrder)EditorGUILayout.EnumPopup("Child Sorting Rule", sortOrder);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Filter & Matching Settings", EditorStyles.boldLabel);
        filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", filterMode);

        if (filterMode != FilterMode.AllChildren)
        {
            nameFilter = EditorGUILayout.TextField("Name Filter / Prefix", nameFilter);
        }

        EditorGUILayout.Space(10);

        // Render Live Interactive Preview Area
        DrawPreviewSection();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Apply Universal Stagger Offset", GUILayout.Height(35)))
        {
            if (playableDirector == null || rootObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both the Playable Director and Root Hierarchy.", "OK");
                return;
            }

            ProcessTimelineTrackKeyframeOffset();
        }
    }

    /// <summary>
    /// Renders a live preview list of matching children and their calculated frame offsets.
    /// </summary>
    private void DrawPreviewSection()
    {
        EditorGUILayout.LabelField("Live Target Preview", EditorStyles.boldLabel);

        if (rootObject == null)
        {
            EditorGUILayout.HelpBox("Assign a Root Hierarchy to view live child offset preview.", MessageType.Info);
            return;
        }

        List<Transform> matchedChildren = GetFilteredAndSortedChildren(out int totalChildCount);

        EditorGUILayout.HelpBox($"Matched {matchedChildren.Count} / {totalChildCount} children for stagger processing.", MessageType.None);

        // Fixed skin parameter to built-in GUIStyle string "box"
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, "box", GUILayout.Height(140));

        if (matchedChildren.Count == 0)
        {
            GUILayout.Label("No children match the specified filter.", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            for (int i = 0; i < matchedChildren.Count; i++)
            {
                int calculatedFrameOffset = i * intervalFrames;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"[{i + 1}] {matchedChildren[i].name}", GUILayout.Width(200));
                GUILayout.Label($"-> Offset: +{calculatedFrameOffset} frames", EditorStyles.miniBoldLabel);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Core process to extract animation clips from the target timeline track and offset keyframe curves.
    /// </summary>
    private void ProcessTimelineTrackKeyframeOffset()
    {
        if (!(playableDirector.playableAsset is TimelineAsset timelineAsset))
        {
            EditorUtility.DisplayDialog("Error", "Selected PlayableDirector does not have a valid TimelineAsset assigned.", "OK");
            return;
        }

        // Find target animation track by name
        AnimationTrack targetTrack = timelineAsset.GetOutputTracks()
            .OfType<AnimationTrack>()
            .FirstOrDefault(t => t.name == targetTrackName);

        if (targetTrack == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not find Animation Track '{targetTrackName}' in Timeline.", "OK");
            return;
        }

        // Collect all AnimationClips from track (including Infinite Clip for recorded mode)
        HashSet<AnimationClip> targetClips = new HashSet<AnimationClip>();
        if (targetTrack.infiniteClip != null) targetClips.Add(targetTrack.infiniteClip);

        foreach (var clip in targetTrack.GetClips())
        {
            if (clip.asset is AnimationPlayableAsset animPlayable && animPlayable.clip != null)
            {
                targetClips.Add(animPlayable.clip);
            }
        }

        if (targetClips.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", $"No valid Animation Clip found on track '{targetTrackName}'.", "OK");
            return;
        }

        List<Transform> validChildren = GetFilteredAndSortedChildren(out _);
        if (validChildren.Count == 0)
        {
            EditorUtility.DisplayDialog("Warning", $"No child objects matched the filter condition '{nameFilter}'.", "OK");
            return;
        }

        // Apply shift to all target clips
        foreach (var animClip in targetClips)
        {
            Undo.RegisterCompleteObjectUndo(animClip, "Universal Stagger Keyframes");
            ShiftClipCurves(animClip, validChildren);
        }

        AssetDatabase.SaveAssets();
        TimelineEditor.Refresh(RefreshReason.ContentsModified);

        EditorUtility.DisplayDialog("Success", $"Successfully applied stagger offset on {validChildren.Count} matched children across {targetClips.Count} clip(s)!", "OK");
    }

    /// <summary>
    /// Shifts curve keyframes for each child transform based on its index order.
    /// </summary>
    private void ShiftClipCurves(AnimationClip animClip, List<Transform> children)
    {
        float frameRate = animClip.frameRate > 0 ? animClip.frameRate : 60f;
        float secondsPerFrame = 1f / frameRate;
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animClip);

        for (int index = 0; index < children.Count; index++)
        {
            Transform child = children[index];
            float timeOffset = index * intervalFrames * secondsPerFrame;

            foreach (var binding in bindings)
            {
                // Match binding path with child object hierarchy name
                if (binding.path == child.name || binding.path.EndsWith("/" + child.name))
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(animClip, binding);
                    if (curve == null || curve.keys.Length == 0) continue;

                    Keyframe[] keys = curve.keys;
                    for (int k = 0; k < keys.Length; k++)
                    {
                        keys[k].time += timeOffset;
                    }

                    curve.keys = keys;
                    AnimationUtility.SetEditorCurve(animClip, binding, curve);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves direct child transforms filtered by matching rules and sorted by specified order.
    /// </summary>
    private List<Transform> GetFilteredAndSortedChildren(out int totalChildCount)
    {
        List<Transform> children = new List<Transform>();
        totalChildCount = rootObject != null ? rootObject.transform.childCount : 0;

        if (rootObject == null) return children;

        for (int i = 0; i < totalChildCount; i++)
        {
            children.Add(rootObject.transform.GetChild(i));
        }

        // Apply sorting
        if (sortOrder == SortOrder.AlphabeticalOrder)
        {
            children = children.OrderBy(c => c.name).ToList();
        }

        // Apply filter condition
        return children.Where(c => IsChildValid(c.name)).ToList();
    }

    /// <summary>
    /// Evaluates if a child's name fulfills the selected FilterMode rule.
    /// </summary>
    private bool IsChildValid(string childName)
    {
        if (filterMode == FilterMode.AllChildren || string.IsNullOrEmpty(nameFilter)) return true;

        switch (filterMode)
        {
            case FilterMode.SmartPrefix:
                // Checks prefix and ensures the boundary character is non-alphabetic (e.g. '_', '-', ' ' or digits)
                if (!childName.StartsWith(nameFilter)) return false;
                if (childName.Length == nameFilter.Length) return true;
                char boundaryChar = childName[nameFilter.Length];
                return !char.IsLetter(boundaryChar);

            case FilterMode.ExactStartsWith:
                return childName.StartsWith(nameFilter);

            case FilterMode.ContainsSubstring:
                return childName.Contains(nameFilter);

            default:
                return true;
        }
    }
}

//Thanks to Gemini for coding this script!