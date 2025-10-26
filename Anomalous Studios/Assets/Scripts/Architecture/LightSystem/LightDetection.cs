using System.Collections;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private LayerMask lightHitMask; //What can light hit? Player, enemies, walls, objects, etc
    [Tooltip("The layers that will block the lights")]
    [SerializeField] private LayerMask blockerMask;
    [SerializeField] private bool applySmoothing = true; //Smoothly change light value or not
    [SerializeField] private float lightResponsiveness = 10f; //Time value for lerping to target light value
    public float lightTotal; //Amount of light hitting player, from 0 to 1

    [Min(0.25f)]
    public float lightDetectionCooldown;

    public void Start()
    {
        StartCoroutine(CheckLightValueLoop());
    }

    /// <summary>
    /// Periodically calculates how much light hits this object and updates lightTotal.
    /// </summary>
    private IEnumerator CheckLightValueLoop()
    {
        var wait = new WaitForSeconds(lightDetectionCooldown);
        while (true)
        {
            float target = 0f;

            var lights = LightManager.Instance?.Lights;
            //Debug.Log(lights.Length);
            if (lights != null && lights.Length > 0)
            {
                foreach (var light in lights)
                {
                    if (!light || !light.enabled || !light.gameObject.activeInHierarchy) continue;

                    switch (light.type)
                    {
                        case LightType.Point: 
                            target += CheckPointLight(light); 
                            break;
                        case LightType.Spot: 
                            target += CheckSpotLight(light); 
                            break;
                    }
                }
            }

            lightTotal = applySmoothing
                ? Mathf.Lerp(lightTotal, target, Time.deltaTime * lightResponsiveness)
                : target;

            lightTotal = Mathf.Clamp01(lightTotal);
            yield return wait;
        }
    }

    #region LightCheck

    /// <summary>
    /// Returns light contribution from a point light if in range and not blocked.
    /// </summary>
    /// <param name="pl">Point light to test.</param>
    /// <returns>Light value in [0,1].</returns>
    float CheckPointLight(Light pl)
    {
        Vector3 toPlayer = transform.position - pl.transform.position;
        float dist = toPlayer.magnitude;

        if (dist > pl.range) return 0f;
        if (IsBlocked(pl.transform.position, transform.position)) return 0f;

        return LightValueCalculation(pl.intensity, toPlayer.sqrMagnitude);
    }

    /// <summary>
    /// Returns light contribution from a spot light if in cone, range, and not blocked.
    /// </summary>
    /// <param name="_spotLight">Spot light to test.</param>
    /// <returns>Light value in [0,1].</returns>
    private float CheckSpotLight(Light _spotLight)
    {
        Vector3 direction = transform.position - _spotLight.transform.position;
        float distance = direction.magnitude;

        if (distance > _spotLight.range) return 0f;

        float angle = Vector3.Angle(_spotLight.transform.forward, direction);
        if (angle > _spotLight.spotAngle * 0.5f) return 0f;

        if (IsBlocked(_spotLight.transform.position, transform.position)) return 0f;

        return LightValueCalculation(_spotLight.intensity, direction.sqrMagnitude);
    }
    #endregion

    /// <summary>
    /// Shot and checks if a ray hits a blocker.
    /// </summary>
    /// <param name="from">Ray start.</param>
    /// <param name="to">Ray end.</param>
    /// <returns>True if blocked; otherwise false.</returns>
    bool IsBlocked(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;
        if (dist < 0.0001f) return false;
        return Physics.Raycast(from, dir / dist, dist - 0.05f, blockerMask, QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// Converts intensity and squared distance into a normalized light value.
    /// </summary>
    /// <param name="intensity">Light intensity.</param>
    /// <param name="sqrDistance">Squared distance to light.</param>
    /// <returns>Light value in [0,1].</returns>
    float LightValueCalculation(float intensity, float sqrDistance)
    {
        return Mathf.Clamp01(intensity / Mathf.Max(1f, sqrDistance));
    }
}