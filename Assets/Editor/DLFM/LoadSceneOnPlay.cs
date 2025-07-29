using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class LoadFromFirstScene
{
    private const string LoadFromFirstScenePrefKey = "LoadFromFirstSceneOnPlay";
    private const string CurrentScenePrefKey = "CurrentScene";

    static LoadFromFirstScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 如果进入运行模式
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (EditorPrefs.GetBool(LoadFromFirstScenePrefKey, false))
            {
                SaveAndRecordCurrentScene();
                OpenFirstScene();
            }
        }
        // 如果退出运行模式
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // 检查 AlwaysLoadFromFirstScene 开关状态，开启时恢复上一个场景
            if (EditorPrefs.GetBool(LoadFromFirstScenePrefKey, false))
            {
                RestorePreviousScene();
            }
        }
    }

    public static void Separator() { }

    [MenuItem("Tools/DLFM/Scene/Load/Load First Scene", false, 2)]
    public static void LoadFirstScene()
    {
        SaveAndRecordCurrentScene();
        OpenFirstScene();
    }

    [MenuItem("Tools/DLFM/Scene/Load/Back To Previous Scene", false, 2)]
    public static void BackToPreviousScene()
    {
        RestorePreviousScene();
    }

    // 创建一个窗口，显示所有的场景
    [MenuItem("Tools/DLFM/Scene/Load/Load Scene", false, 2)]
    public static void LoadScene()
    {
        SceneSelectorWindow window = EditorWindow.GetWindow<SceneSelectorWindow>();
        window.titleContent = new GUIContent("Load Scene");
        window.Show();
    }

    private static void SaveAndRecordCurrentScene()
    {
        // 保存当前场景
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        // 记录当前场景
        EditorPrefs.SetString(CurrentScenePrefKey, SceneManager.GetActiveScene().path);
    }

    private static void OpenFirstScene()
    {
        // 获取BuildSettings中的第一个场景
        string firstScenePath = SceneUtility.GetScenePathByBuildIndex(0);
        if (!string.IsNullOrEmpty(firstScenePath))
        {
            EditorSceneManager.OpenScene(firstScenePath);
        }
        else
        {
            Debug.LogError("未找到Build Settings中的第一个场景。");
        }
    }

    private static void RestorePreviousScene()
    {
        // 恢复之前的场景
        string previousScenePath = EditorPrefs.GetString(CurrentScenePrefKey);
        if (!string.IsNullOrEmpty(previousScenePath))
        {
            EditorSceneManager.OpenScene(previousScenePath);
            EditorPrefs.DeleteKey(CurrentScenePrefKey);
        }
    }

    [MenuItem("Tools/DLFM/Scene/Preferences/Play From First Scene", false, 1)]
    public static void PlayFromFirstScene()
    {
        SaveAndRecordCurrentScene();
        OpenFirstScene();
        // 运行游戏
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Tools/DLFM/Scene/Preferences/Always Load From First Scene", false, 1)]
    [MenuItem("Tools/DLFM/Scene/Preferences/Always Load From First Scene %g", false, 1)]
    public static void ToggleLoadFromFirstSceneOnPlay()
    {
        bool currentValue = EditorPrefs.GetBool(LoadFromFirstScenePrefKey, false);
        EditorPrefs.SetBool(LoadFromFirstScenePrefKey, !currentValue);
    }

    [MenuItem("Tools/DLFM/Scene/Preferences/Always Load From First Scene", true)]
    public static bool ToggleLoadFromFirstSceneOnPlayValidate()
    {
        bool currentValue = EditorPrefs.GetBool(LoadFromFirstScenePrefKey, false);
        Menu.SetChecked("Tools/DLFM/Scene/Preferences/Always Load From First Scene", currentValue);
        return true;
    }

    public class SceneSelectorWindow : EditorWindow
    {
        private void OnGUI()
        {
            GUILayout.Label("Select a scene to load", EditorStyles.boldLabel);

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
                if (GUILayout.Button(sceneName))
                {
                    SaveAndRecordCurrentScene();
                    EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
                }
            }

            if (EditorPrefs.HasKey(CurrentScenePrefKey))
            {
                if (GUILayout.Button("Back to previous scene"))
                {
                    RestorePreviousScene();
                }
            }
        }
    }
}