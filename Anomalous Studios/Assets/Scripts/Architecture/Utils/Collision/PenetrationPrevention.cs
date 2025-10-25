using System;
using UnityEngine;

public class PenetrationPrevention : MonoBehaviour
{
    public LayerMask obstacleLayer;

    public Color mtvColor = Color.yellow;

    public bool autoResolve = true;
    public bool smoothResolve = true;

    public event Action<Vector3> OnPenetrationStart;
    public event Action<Vector3> OnPenetrationStay;
    public event Action OnPenetrationEnd;

    private Collider _collider;
    private Vector3 _lastCorrection;
    private bool _resolvingCollision;

    private void Start()
    {
        _collider = GetComponent<Collider>();

        OnPenetrationStart += correction =>
        {
            float penetrationDepth = correction.magnitude;
            Debug.Log($"Started Penetration, MTV = {penetrationDepth:F3}");
        };

        OnPenetrationEnd += () => { Debug.Log("Penetration resolved"); };


    }

    private void Update()
    {
        bool colliding = _collider.GetPenetrationsInLayer(obstacleLayer, out Vector3 correction);

        correction += correction.normalized * 0.001f;

        _lastCorrection = colliding ? correction : Vector3.zero;

        if (colliding)
        {
            if (!_resolvingCollision) OnPenetrationStart?.Invoke(correction);
            else OnPenetrationStay?.Invoke(correction);

            _resolvingCollision = true;

            if (autoResolve)
            {
                Vector3 delta = smoothResolve ? Vector3.Lerp(Vector3.zero, correction, 0.05f) : correction;
                transform.position += delta;
            }

            Debug.Log($"Colliding. MTV = {correction.magnitude:F3}");
        }
        else 
        {
            if (_resolvingCollision) OnPenetrationEnd?.Invoke();
            _resolvingCollision = false;
        }

        
    }

    void OnDrawGizmos()
    {
        if (_collider == null) _collider = GetComponent<Collider>();
        if (_collider == null) return;

        if (_lastCorrection != Vector3.zero)
        {
            Vector3 start = _collider.bounds.center;
            Vector3 end = start + _lastCorrection;

            Gizmos.color = mtvColor;
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.05f);
        }
    }
}
