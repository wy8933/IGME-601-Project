using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class HighlightTarget : MonoBehaviour
{
    [HideInInspector] public Renderer rend;
    [HideInInspector] public Material[] originalMats;
    [HideInInspector] public Material outlineMatInstance;

    [HideInInspector] public bool IsHighlighted = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMats = rend.materials;

        if (!TryInit())
        {
            StartCoroutine(DelayedInit());
        }
    }

    bool TryInit()
    {
        if (HighlightManager.Instance == null)
        {
            Debug.LogWarning("HighlightManager not found yet, will retry...");
            return false;
        }

        if (HighlightManager.Instance.outlineMat == null)
        {
            Debug.LogError("Outline Mat not assigned in HighlightManager!");
            return false;
        }

        // Clone mat here
        outlineMatInstance = new Material(HighlightManager.Instance.outlineMat);

        Material[] mats = new Material[2];
        mats[0] = originalMats[0];
        mats[1] = outlineMatInstance;
        rend.materials = mats;

        // initialize mat 
        outlineMatInstance.SetFloat("_Outline_Alpha", 0f);
        outlineMatInstance.SetFloat("_OutlineGlowIntensity", 0f);

        HighlightManager.Register(this);

        return true;
    }

    IEnumerator DelayedInit()
    {
        yield return null;
        TryInit();  // init again if first time failed
    }

    void OnDestroy()
    {
        HighlightManager.Unregister(this);

        if (outlineMatInstance != null)
        {
            Destroy(outlineMatInstance);
        }
    }
}
