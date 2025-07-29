using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MaterialMerger : EditorWindow
{
    private Material newMaterial;

    [MenuItem("Tools/Merge Materials")]
    public static void ShowWindow()
    {
        GetWindow<MaterialMerger>("Merge Materials");
    }

    void OnGUI()
    {
        GUILayout.Label("Merge Materials Settings", EditorStyles.boldLabel);
        newMaterial = (Material)EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false);

        if (GUILayout.Button("Merge Selected Objects"))
        {
            MergeMaterials();
        }
    }

    void MergeMaterials()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject go in selectedObjects)
        {
            ProcessObject(go);
        }
    }

    void ProcessObject(GameObject go)
    {
        // 处理SkinnedMeshRenderer
        SkinnedMeshRenderer skinnedRenderer = go.GetComponent<SkinnedMeshRenderer>();
        if (skinnedRenderer != null && skinnedRenderer.sharedMaterials.Length > 1)
        {
            MergeSkinnedMeshMaterials(skinnedRenderer);
            return;
        }

        // 处理普通MeshRenderer
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();

        if (meshRenderer != null && meshFilter != null && meshRenderer.sharedMaterials.Length > 1)
        {
            MergeRegularMeshMaterials(meshRenderer, meshFilter);
        }
    }

    void MergeSkinnedMeshMaterials(SkinnedMeshRenderer renderer)
    {
        Mesh originalMesh = renderer.sharedMesh;
        if (originalMesh == null) return;

        // 创建合并后的网格
        Mesh combinedMesh = new Mesh();
        CombineInstance[] combines = new CombineInstance[originalMesh.subMeshCount];

        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            combines[i].mesh = originalMesh;
            combines[i].subMeshIndex = i;
        }

        combinedMesh.CombineMeshes(combines, true);
        combinedMesh.boneWeights = originalMesh.boneWeights;
        combinedMesh.bindposes = originalMesh.bindposes;

        // 保留其他重要属性
        combinedMesh.vertices = originalMesh.vertices;
        combinedMesh.normals = originalMesh.normals;
        combinedMesh.tangents = originalMesh.tangents;
        combinedMesh.uv = originalMesh.uv;

        // 应用新材质
        renderer.sharedMesh = combinedMesh;
        renderer.sharedMaterial = newMaterial != null ? newMaterial : renderer.sharedMaterials[0];

        Debug.Log($"Merged materials on {renderer.gameObject.name}", renderer.gameObject);
    }

    void MergeRegularMeshMaterials(MeshRenderer renderer, MeshFilter filter)
    {
        Mesh originalMesh = filter.sharedMesh;
        if (originalMesh == null) return;

        // 创建合并后的网格
        Mesh combinedMesh = new Mesh();
        CombineInstance[] combines = new CombineInstance[originalMesh.subMeshCount];

        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            combines[i].mesh = originalMesh;
            combines[i].subMeshIndex = i;
        }

        combinedMesh.CombineMeshes(combines);

        // 创建新GameObject来保存合并后的网格
        GameObject newGo = new GameObject(renderer.gameObject.name + "_Merged");
        MeshFilter newFilter = newGo.AddComponent<MeshFilter>();
        MeshRenderer newRenderer = newGo.AddComponent<MeshRenderer>();

        // 应用合并后的网格和材质
        newFilter.sharedMesh = combinedMesh;
        newRenderer.sharedMaterial = newMaterial != null ? newMaterial : renderer.sharedMaterials[0];

        // 复制变换信息
        newGo.transform.SetParent(renderer.transform.parent);
        newGo.transform.localPosition = renderer.transform.localPosition;
        newGo.transform.localRotation = renderer.transform.localRotation;
        newGo.transform.localScale = renderer.transform.localScale;

        // 禁用原物体
        renderer.gameObject.SetActive(false);

        Debug.Log($"Created merged mesh object: {newGo.name}", newGo);
    }
}