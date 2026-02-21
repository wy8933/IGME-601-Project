using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    [Header("Assign outline material (Custom/SimpleOutlineURP)")]
    public Material outlineMaterial;

    [Header("Default OFF")]
    public bool highlightOnStart = false;

    [Header("Apply to all child renderers")]
    public bool includeChildren = true;

    Renderer[] _renderers;
    Material[][] _originalMats;

    void Awake()
    {
        _renderers = includeChildren
            ? GetComponentsInChildren<Renderer>(true)
            : new[] { GetComponent<Renderer>() };

        _originalMats = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMats[i] = _renderers[i] ? _renderers[i].sharedMaterials : null;
        }

        SetHighlight(highlightOnStart);
    }

    public void SetHighlight(bool on)
    {
        if (outlineMaterial == null) return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            var r = _renderers[i];
            if (!r) continue;

            var original = _originalMats[i];
            if (original == null) continue;

            if (on)
            {

                var cur = r.sharedMaterials;
                for (int k = 0; k < cur.Length; k++)
                    if (cur[k] == outlineMaterial) return;

                var newMats = new Material[original.Length + 1];
                for (int j = 0; j < original.Length; j++) newMats[j] = original[j];
                newMats[original.Length] = outlineMaterial;
                r.sharedMaterials = newMats;
            }
            else
            {

                r.sharedMaterials = original;
            }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {

        if (!Application.isPlaying) return;
        SetHighlight(highlightOnStart);
    }
#endif
}
