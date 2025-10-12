using UnityEngine;

/// <summary>
/// Manages the dirt mask (RenderTexture) for cleanable surfaces.
/// Attach this to any surface you want to clean (requires MeshCollider).
/// </summary>
[RequireComponent(typeof(Renderer))]
public class SurfaceCleaner : MonoBehaviour
{
    [Header("Mask Settings")]
    [Tooltip("Resolution of the mask texture. Higher = more detail but slower.")]
    public int maskResolution = 1024;
    public RenderTextureFormat maskFormat = RenderTextureFormat.R8;

    [Header("Brush Settings")]
    [Tooltip("Brush radius in UV space.")]
    [Range(0.001f, 0.2f)] public float brushRadiusUV = 0.05f;
    [Tooltip("Brush softness (0 = hard edge, 1 = very soft).")]
    [Range(0f, 1f)] public float brushSoftness = 0.5f;
    [Tooltip("How much to add per stroke (0–1).")]
    [Range(0f, 1f)] public float brushStrength = 0.6f;

    [Header("Runtime References")]
    [Tooltip("Hidden/CleanBrush shader (found automatically at runtime).")]
    public Shader brushShader;

    private Material brushMat;
    private RenderTexture maskRT;
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private static readonly int maskID = Shader.PropertyToID("_MaskTex");

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // 1️⃣ Create mask RenderTexture (black = dirty, white = clean)
        maskRT = new RenderTexture(maskResolution, maskResolution, 0, maskFormat)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        maskRT.Create();
        ClearMask(0f);

        // 2️⃣ Create brush material
        if (brushShader == null)
            brushShader = Shader.Find("Hidden/CleanBrush");
        brushMat = new Material(brushShader);

        // 3️⃣ Bind _MaskTex to the material via property block
        mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);
        mpb.SetTexture(maskID, maskRT);
        rend.SetPropertyBlock(mpb);
    }

    /// <summary>
    /// Cleans a circular area on the mask at the given UV coordinate.
    /// </summary>
    public void CleanAtUV(Vector2 uv, float strengthMultiplier = 1f)
    {
        if (brushMat == null || maskRT == null) return;

        brushMat.SetVector("_BrushUV", uv);
        brushMat.SetFloat("_BrushRadius", brushRadiusUV);
        brushMat.SetFloat("_BrushSoft", brushSoftness);
        brushMat.SetFloat("_BrushStrength", brushStrength * strengthMultiplier);
        brushMat.SetTexture("_MainTex", maskRT);

        // Perform a GPU blit to accumulate the brush stroke
        RenderTexture tmp = RenderTexture.GetTemporary(maskRT.descriptor);
        Graphics.Blit(maskRT, tmp, brushMat);
        Graphics.Blit(tmp, maskRT);
        RenderTexture.ReleaseTemporary(tmp);
    }

    /// <summary>
    /// Clears the entire mask to a constant value.
    /// 0 = fully dirty, 1 = fully clean.
    /// </summary>
    public void ClearMask(float value)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = maskRT;
        GL.Clear(false, true, new Color(value, value, value, 1));
        RenderTexture.active = active;
    }

    /// <summary>
    /// Returns how much of the surface is clean (0 = all dirty, 1 = all clean)
    /// </summary>
    public float GetCleanProgress()
    {
        if (maskRT == null) return 0f;

        // Read pixel data from GPU
        RenderTexture.active = maskRT;
        Texture2D tex = new Texture2D(maskRT.width, maskRT.height, TextureFormat.R8, false);
        tex.ReadPixels(new Rect(0, 0, maskRT.width, maskRT.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Color32[] pixels = tex.GetPixels32();
        float total = 0f;
        for (int i = 0; i < pixels.Length; i++)
            total += pixels[i].r / 255f;

        Destroy(tex);
        return total / pixels.Length;
    }

    void OnDestroy()
    {
        if (maskRT != null)
        {
            maskRT.Release();
            Destroy(maskRT);
        }
        if (brushMat != null)
        {
            Destroy(brushMat);
        }
    }
}
