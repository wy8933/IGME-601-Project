using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AutoOutline: MonoBehaviour
{
    [Header("Outline Appearance")]
    private Color outlineColor = Color.gray;
    private Color highlightColor = Color.yellow;
    private float glowIntensity = 1f;

    public float thickness = 0.03f;

    //private Transform player;
    private MeshFilter[] meshFilters;
    private Material outlineMat;

    private bool shouldRender = false;
    private bool isHighlighted = false;

    /// <summary>
    /// Whether the object should be highlighted, based on proximity
    /// </summary>
    public bool ShouldRender { get => shouldRender; set => shouldRender = value; }

    /// <summary>
    /// Determines if the highlight to the item when true
    /// </summary>
    public bool IsHighlighted { get => isHighlighted; set => isHighlighted = value; }


    void Start()
    {
        Shader s = Shader.Find("Custom/OutlineExtrudeGlow");
        if (s == null)
        {
            Debug.LogError("Shader Custom/OutlineExtrudeGlow not found!");
            return;
        }

        outlineMat = new Material(s);

        meshFilters = GetComponentsInChildren<MeshFilter>();

        // Moved the auto outline variables into start, no need to reset them every frame
        outlineMat.SetColor("_OutlineColor", outlineColor);
        outlineMat.SetFloat("_Thickness", thickness);
        outlineMat.SetFloat("_GlowIntensity", glowIntensity);

        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    void Update()
    {
        // Keep updating the variables for testing purposes, able ot change while playing the game
        outlineMat.SetColor("_OutlineColor", outlineColor);
        outlineMat.SetFloat("_Thickness", thickness);
        outlineMat.SetFloat("_GlowIntensity", glowIntensity);
    }

    private void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
    {
        if (!outlineMat) return;
        if (!shouldRender) return;

        outlineMat.SetColor("_OutlineColor", isHighlighted ? highlightColor : outlineColor);

        foreach (var mf in meshFilters)
        {
            if (!mf) continue;

            Graphics.DrawMesh(
                mf.sharedMesh,
                mf.transform.localToWorldMatrix,
                outlineMat,
                0,
                cam,
                0,
                null,
                true,
                true
            );
        }
    }

    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
}
