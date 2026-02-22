using UnityEngine;

public class OutlineTrigger : MonoBehaviour
{
    public enum Mode { Aim, Distance }
    public Mode mode = Mode.Aim;

    public Camera cam;
    public LayerMask interactableMask;
    public float aimRange = 10f;

    public float radius = 3f;
    public float scanInterval = 0.1f;

    OutlineHighlighter _current;
    float _t;
    readonly Collider[] _buf = new Collider[64];

    void Reset() => cam = Camera.main;

    void Update()
    {
        OutlineHighlighter target = null;

        if (mode == Mode.Aim)
        {
            if (!cam) cam = Camera.main;
            var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out var hit, aimRange, interactableMask))
                target = hit.collider.GetComponentInParent<OutlineHighlighter>();
            SetCurrent(target);
        }
        else
        {
            _t -= Time.deltaTime;
            if (_t > 0f) return;
            _t = scanInterval;

            int n = Physics.OverlapSphereNonAlloc(transform.position, radius, _buf, interactableMask);
            float best = float.MaxValue;

            for (int i = 0; i < n; i++)
            {
                var h = _buf[i] ? _buf[i].GetComponentInParent<OutlineHighlighter>() : null;
                if (!h) continue;
                float d = (_buf[i].transform.position - transform.position).sqrMagnitude;
                if (d < best) { best = d; target = h; }
            }

            SetCurrent(target);
        }
    }

    void SetCurrent(OutlineHighlighter target)
    {
        if (_current == target) return;

        if (_current) _current.SetHighlight(false);
        _current = target;
        if (_current) _current.SetHighlight(true);
    }
}
