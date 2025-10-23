using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AutoAssignURPBakedTextures : EditorWindow
{
    [MenuItem("Tools/Auto Assign URP Baked Textures (Smart Match)")]
    public static void AssignTextures()
    {
        string textureFolder = "Assets/Graphics/Textures/Baked_Textures";
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { textureFolder });
        string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Graphics/BakedMaterials" });

        foreach (string matGuid in matGuids)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(matGuid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null) continue;

            string matName = Path.GetFileNameWithoutExtension(matPath);
            Texture2D baseTex = null;
            Texture2D normalTex = null;
            Texture2D roughTex = null;

            // 在贴图列表中模糊匹配
            foreach (string texGuid in textureGuids)
            {
                string texPath = AssetDatabase.GUIDToAssetPath(texGuid);
                string texName = Path.GetFileNameWithoutExtension(texPath).ToLower();

                if (texName.Contains(matName.ToLower()))
                {
                    if (texName.Contains("basecolor")) baseTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                    if (texName.Contains("normal")) normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                    if (texName.Contains("rough")) roughTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                }
            }

            if (baseTex != null)
            {
                mat.SetTexture("_BaseMap", baseTex);
                mat.SetColor("_BaseColor", Color.white);
            }

            if (normalTex != null)
            {
                mat.SetTexture("_BumpMap", normalTex);
                mat.EnableKeyword("_NORMALMAP");
            }

            if (roughTex != null)
            {
                mat.SetTexture("_MetallicGlossMap", roughTex);
                mat.SetFloat("_Smoothness", 0.5f);
            }

            EditorUtility.SetDirty(mat);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("✅ Smart URP Baked Textures assigned successfully!");
    }
}
