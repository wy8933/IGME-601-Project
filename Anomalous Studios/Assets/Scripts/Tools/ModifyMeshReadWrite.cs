using UnityEditor;
using UnityEngine;

public class ModifyAssetReadWrite
{
#if UNITY_EDITOR
    [MenuItem("Tools/Set All Meshes Read and Write Enabled")]
    private static void SetAllMeshesReadWrite()
    {
        string[] allMeshGuids = AssetDatabase.FindAssets("t:Mesh");

        foreach (string guid in allMeshGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter asset = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (asset != null)
            {
                if (!asset.isReadable)
                {
                    asset.isReadable = true;
                    asset.SaveAndReimport();
                    Debug.Log($"Set Read/Write enabled for: {assetPath}");
                }
            }
        }
        Debug.Log("Finished setting Read/Write Enabled for all meshes");
    }
#endif
}