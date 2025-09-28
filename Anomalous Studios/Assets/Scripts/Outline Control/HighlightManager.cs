using UnityEngine;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;

    [Header("Global Parameter")]
    public float highlightDistance = 5f;   // start glowing after player walked in this range
    public Material outlineMat;            // M_Outline
    public LayerMask occlusionMask;        // set this mask in layer for occlusion

    [Header("Gradient Control")]
    public float fadeSpeed = 5f;           // fade in/out speed for glow effects
    public float maxGlow = 3f;             // max glow on fade in/out

    private Transform player;
    private static List<HighlightTarget> targets = new List<HighlightTarget>();

    void Awake()
    {
        Instance = this;
        player = Camera.main.transform;
    }

    void Update()
    {
        foreach (var target in targets)
        {
            if (target == null || target.outlineMatInstance == null) continue;

            float dist = Vector3.Distance(player.position, target.transform.position);
            bool inRange = dist < highlightDistance;
            bool visible = IsVisible(target);

            // target Alpha£¨0 = hide, 1 = highlight£©
            float targetAlpha = (inRange && visible) ? 1f : 0f;

            float currentAlpha = target.outlineMatInstance.GetFloat("_Outline_Alpha");

            // lerp for anim
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

            // update shader 
            target.outlineMatInstance.SetFloat("_Outline_Alpha", newAlpha);
            target.outlineMatInstance.SetFloat("_OutlineGlowIntensity", newAlpha * maxGlow);
        }
    }

    bool IsVisible(HighlightTarget target)
    {
        Vector3 dir = (target.transform.position - player.position).normalized;
        float dist = Vector3.Distance(player.position, target.transform.position);

        if (Physics.Raycast(player.position, dir, out RaycastHit hit, dist, occlusionMask))
        {
            if (hit.collider.gameObject != target.gameObject)
                return false; // if vision blocked, object won't glow
        }
        return true;
    }

    public static void Register(HighlightTarget target)
    {
        if (!targets.Contains(target))
            targets.Add(target);
    }

    public static void Unregister(HighlightTarget target)
    {
        if (targets.Contains(target))
            targets.Remove(target);
    }
}
