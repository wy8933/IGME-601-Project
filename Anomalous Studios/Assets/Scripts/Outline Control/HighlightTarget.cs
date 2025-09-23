using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class HighlightTarget : MonoBehaviour
{
    [HideInInspector] public Renderer rend;
    [HideInInspector] public Material[] originalMats;
    [HideInInspector] public Material outlineMatInstance;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMats = rend.materials;

        // initialize, if failed try after 1 frame
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

        // clone outline mat
        outlineMatInstance = new Material(HighlightManager.Instance.outlineMat);

        Material[] mats = new Material[2];
        mats[0] = originalMats[0];
        mats[1] = outlineMatInstance;
        rend.materials = mats;

        // initialize alpha
        outlineMatInstance.SetFloat("_OutlineAlpha", 0f);

        HighlightManager.Register(this);

        Debug.Log("Outline material added to " + gameObject.name);
        return true;
    }

    IEnumerator DelayedInit()
    {
        yield return null; // wait for 1 frame
        TryInit();
    }

    void OnDestroy()
    {
        HighlightManager.Unregister(this);
    }
}
