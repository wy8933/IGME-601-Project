using System;
using UnityEngine;

public class PenetrationPrevention : MonoBehaviour
{
    public LayerMask obstacleLayer;

    public Color mtvColor = Color.yellow;

    public bool autoResolve = true;
    public bool smoothResolve = true;

    [Header("Stability")]
    public float minCorrection = 0.0025f;
    public float separationEpsilon = 0.0005f;
    [Range(0f, 1f)]
    public float smoothFactor = 0.15f;

    public event Action<Vector3> OnPenetrationStart;
    public event Action<Vector3> OnPenetrationStay;
    public event Action OnPenetrationEnd;

    private Collider _collider;
    private Rigidbody _rb;

    private Vector3 _lastCorrection;
    private Vector3 _pendingCorrection;
    private bool _resolvingCollision;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        OnPenetrationStart += correction =>
        {
            float penetrationDepth = correction.magnitude;
            // Debug.Log($"Started Penetration, MTV = {penetrationDepth:F3}");
        };

        OnPenetrationEnd += () => { Debug.Log("Penetration resolved"); };
    }

    private void Update()
    {
        bool colliding = _collider.GetPenetrationsInLayer(obstacleLayer, out Vector3 correction);

        // Deadzone: ignore tiny corrections 
        if (!colliding || correction.sqrMagnitude < (minCorrection * minCorrection))
        {
            _pendingCorrection = Vector3.zero;
            _lastCorrection = Vector3.zero;

            if (_resolvingCollision)
                OnPenetrationEnd?.Invoke();

            _resolvingCollision = false;
            return;
        }

        // Add a tiny separation only when having a meaningful collision.
        correction += correction.normalized * separationEpsilon;

        _pendingCorrection = correction;
        _lastCorrection = correction;

        if (!_resolvingCollision) OnPenetrationStart?.Invoke(correction);
        else OnPenetrationStay?.Invoke(correction);

        _resolvingCollision = true;
    }

    private void FixedUpdate()
    {
        if (!autoResolve) return;
        if (_pendingCorrection == Vector3.zero) return;

        Vector3 correction = _pendingCorrection;

        Vector3 delta = smoothResolve
            ? Vector3.Lerp(Vector3.zero, correction, smoothFactor)
            : correction;

        if (_rb != null && !_rb.isKinematic)
        {
            _rb.MovePosition(_rb.position + delta);
        }
        else
        {
            transform.position += delta;
        }

        _pendingCorrection = Vector3.zero;
    }

    private void OnDrawGizmos()
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
