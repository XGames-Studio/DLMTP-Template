using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FindReferences
{
    [MenuItem("Assets/Find References", false, 10)]
    static private void Find()
    {
        Dictionary<string, string> guidDics = new Dictionary<string, string>();
        foreach (Object o in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (!string.IsNullOrEmpty(path))
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                if (!guidDics.ContainsKey(guid))
                {
                    guidDics[guid] = o.name;
                }
            }
        }

        if (guidDics.Count > 0)
        {
            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (i % 20 == 0)
                {
                    bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)i / (float)files.Length);
                    if (isCancel)
                    {
                        break;
                    }
                }
                foreach (KeyValuePair<string, string> guidItem in guidDics)
                {
                    if (Regex.IsMatch(File.ReadAllText(file), guidItem.Key))
                    {
                        string relativePath = GetRelativeAssetsPath(file);
                        Debug.Log(string.Format("name: {0} file: {1}", guidItem.Value, relativePath), AssetDatabase.LoadAssetAtPath<Object>(relativePath));
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("匹配结束");
        }
    }

    // 新增方法：将绝对路径转换为相对于 Assets 文件夹的相对路径
    static private string GetRelativeAssetsPath(string absolutePath)
    {
        string assetsPath = Application.dataPath;
        return "Assets" + Path.GetRelativePath(assetsPath, absolutePath);
    }
}