using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AutoOutline: MonoBehaviour
{
    [Header("Distance Trigger")]
    public float triggerDistance = 5f;

    [Header("Outline Appearance")]
    public Color outlineColor = Color.yellow;
    public Color highlightColor = Color.green;

    public float thickness = 0.03f;
    public float glowIntensity = 4f;

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
        //player = GameObject.FindGameObjectWithTag("Player")?.transform;

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
        //if (!player)
        //{
        //    Debug.LogWarning("Player not found. Add tag Player.");
        //    return;
        //}

        //float dist = Vector3.Distance(player.position, transform.position);
        //shouldRender = dist <= triggerDistance;

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
